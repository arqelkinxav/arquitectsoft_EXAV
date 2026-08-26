using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace arquitectSoft.View.Wpf.Panels
{
    /// <summary>
    /// Versión "panel" del Análisis MANUAL de puertas para hospedarse dentro del escritorio
    /// (MdiChild): se agregan puertas por código, se editan altura/anchura/código en la tabla,
    /// y se analiza → Perfilería y Herrajes en pestañas. Sin chrome ni liquid glass: lo aporta
    /// la ventana hija. Reutiliza AnalisisDatosDto.CalculateTab y los diálogos cristal.
    /// </summary>
    public partial class PuertasPanel : UserControl
    {
        private const int MedidaBase = 2960;          // valor por defecto (no aplica en manual)
        private const decimal Desperdicio = 1m;       // factor (0% desperdicio)

        private bool _ocultarPrimeraHerraje;
        private string _acabado = "";
        private readonly DataTable _dtAddRows = new DataTable();
        private readonly DataTable _dtPuertas = new DataTable();
        private DataTable _dtPerfil, _dtHerraje;   // resultados, para cambiar acabado

        // Acabado de perfilería vigente en las cantidades (el "01" por defecto o el último
        // aplicado con "Cambiar Acabado"). Amarra el campo "Acabado Perfilería" del export.
        private string _acabadoPerfil = "";

        // Copias intactas del análisis fresco (con placeholders MOD… y perfilería por defecto)
        // + reglas de dependencia. Al cambiar la perfilería se reconstruye desde aquí para
        // re-resolver las dependencias MOD… → acabado real.
        private DataTable _basePerfil, _baseHerraje;
        private Engine.DependenciaResolver _resolver;

        // Filas cuya nomenclatura escribió el usuario a mano: no se renumeran al quitar filas
        // ni las pisa el automático "Pn". Se vacía la celda para devolverla al automático.
        private readonly HashSet<DataRow> _nomenManual = new HashSet<DataRow>();

        // Columna oculta con la clave de orden natural de la nomenclatura (P2 < P2A < P10).
        private const string ColOrden = "_Orden";

        public PuertasPanel()
        {
            InitializeComponent();
        }

        // Ventana que hospeda el panel (para que los diálogos cristal tengan owner).
        private Window Owner { get { return Window.GetWindow(this); } }

        // ===== Buscar código de puerta =====
        private void BuscarCodigo_Click(object sender, RoutedEventArgs e)
        {
            var bsc = new BuscarDialog { Owner = Owner };
            bsc.ShowDialog();
            if (bsc.ReturnItem1 == null) return;
            string code = bsc.ReturnItem1;
            TxtCodigo.Text = code.Contains("-") ? code.Split('-')[0].Trim() : code.Trim();
            _acabado = code.Contains("-") ? code.Split('-')[1].Trim() : "";
            TxtDescripcion.Text = bsc.ReturnItem2;
        }

        // ===== Agregar puerta(s); altura/anchura quedan en blanco para editar en la tabla =====
        private void Agregar_Click(object sender, RoutedEventArgs e)
        {
            if (TxtCodigo.Text == "" || TxtDescripcion.Text == "")
            {
                GlassDialog.Informar(Owner, "Puertas", "Busca un código de puerta primero.");
                return;
            }

            if (_dtAddRows.Columns.Count == 0)
            {
                _dtAddRows.Columns.Add("Nomenclatura");
                _dtAddRows.Columns.Add("Codigo");
                _dtAddRows.Columns.Add("Apertura de Puerta");
                _dtAddRows.Columns.Add("Acabado Perfileria Puertas");
                _dtAddRows.Columns.Add("Item");
                _dtAddRows.Columns.Add("Altura");
                _dtAddRows.Columns.Add("Anchura");
                _dtAddRows.Columns.Add("Conectado/pared Tubo L1");
                _dtAddRows.Columns.Add("Conectado/pared Tubo L2");
                _dtAddRows.Columns.Add("Cantidad");
                _dtAddRows.Columns.Add("Ubicación");
                _dtAddRows.Columns.Add("Area");
                _dtAddRows.Columns.Add(ColOrden);   // siempre la última: las altas son posicionales
            }

            int n; if (!int.TryParse(TxtCantidad.Text, out n) || n < 1) n = 1;
            for (int i = 0; i < n; i++)
            {
                _dtAddRows.Rows.Add(SiguienteNomenLibre(), TxtCodigo.Text, "", _acabado, TxtDescripcion.Text, "", "", "No", "No", "1");
            }

            ReordenarPorNomenclatura();
            DgNuevas.ItemsSource = _dtAddRows.DefaultView;
            LblEstado.Text = _dtAddRows.Rows.Count + " puerta(s). Completa altura/anchura en la tabla y pulsa Analizar.";
        }

        private void DgNuevas_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Apertura de Puerta":
                case "Conectado/pared Tubo L1":
                case "Conectado/pared Tubo L2":
                case "Cantidad":
                case "Ubicación":
                case "Area":
                case ColOrden:                  // clave de orden interna: no se muestra
                    e.Cancel = true; break;
                case "Nomenclatura": e.Column.Header = "Nomen."; e.Column.Width = 100; break;   // editable
                case "Codigo": e.Column.Header = "Código"; break;               // editable
                case "Acabado Perfileria Puertas": e.Column.Header = "Acabado"; e.Column.IsReadOnly = true; break;
                case "Item": e.Column.Header = "Descripción"; e.Column.IsReadOnly = true;
                    e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star); break;
                case "Altura": e.Column.Width = 120; break;                     // editable, más ancha
                case "Anchura": e.Column.Width = 120; break;                    // editable, más ancha
            }
        }

        // Al editar el CÓDIGO en la tabla: busca la descripción de ese código; si no existe, avisa.
        // Al editar la NOMENCLATURA: se respeta tal cual (no se renumera), salvo que se deje
        // vacía —vuelve al automático Pn— o que repita la de otra puerta —se rechaza—.
        private void DgNuevas_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction != DataGridEditAction.Commit) return;
            if (e.Column == null) return;
            string cabecera = Convert.ToString(e.Column.Header);
            if (cabecera == "Nomen.") { NomenclaturaEditada(e); return; }
            if (cabecera != "Código") return;
            var drv = e.Row.Item as DataRowView; if (drv == null) return;
            var tb = e.EditingElement as TextBox;
            string nuevo = tb != null ? tb.Text : Convert.ToString(drv.Row["Codigo"]);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                string desc = DescDeCodigo(nuevo);
                if (desc == null)
                    GlassDialog.Informar(Owner, "Puertas", "No existe ningún código que coincida. Revisa el código ingresado.");
                else
                    drv.Row["Item"] = desc;
            }), DispatcherPriority.Background);
        }

        private void NomenclaturaEditada(DataGridCellEditEndingEventArgs e)
        {
            var drv = e.Row.Item as DataRowView; if (drv == null) return;
            var tb = e.EditingElement as TextBox;
            string anterior = Convert.ToString(drv.Row["Nomenclatura"]);
            string nuevo = (tb != null ? tb.Text : anterior ?? "").Trim();

            // Vacía => vuelve a la numeración automática.
            if (nuevo == "")
            {
                _nomenManual.Remove(drv.Row);
                if (tb != null) tb.Text = SiguienteNomenLibre(drv.Row);
                ReordenarTrasCommit();
                return;
            }

            // La coma separa las nomenclaturas del mismo grupo en el análisis, y las comillas
            // rompen los filtros DataTable.Select del DTO: no se admiten.
            if (nuevo.IndexOfAny(new[] { ',', '\'', '"' }) >= 0)
            {
                RechazarNomen("La nomenclatura no puede llevar comas ni comillas.");
                e.Cancel = true; return;
            }

            // Larga de más: acaba concatenada con las del grupo en el parámetro del análisis.
            if (nuevo.Length > 20)
            {
                RechazarNomen("La nomenclatura no puede pasar de 20 caracteres.");
                e.Cancel = true; return;
            }

            // Repetida => se rechaza; la nomenclatura identifica la puerta en el análisis.
            if (NomenOcupada(nuevo, drv.Row))
            {
                RechazarNomen("Ya hay otra puerta con la nomenclatura \"" + nuevo + "\". Usa una distinta.");
                e.Cancel = true; return;
            }

            _nomenManual.Add(drv.Row);
            if (tb != null) tb.Text = nuevo;   // guarda sin espacios sobrantes
            ReordenarTrasCommit();
        }

        // Deshace la edición de la celda y explica por qué (fuera del CellEditEnding).
        private void RechazarNomen(string motivo)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                DgNuevas.CancelEdit(DataGridEditingUnit.Cell);
                GlassDialog.Informar(Owner, "Puertas", motivo);
            }), DispatcherPriority.Background);
        }

        // La celda aún no está confirmada dentro de CellEditEnding: se reordena después.
        private void ReordenarTrasCommit()
        {
            Dispatcher.BeginInvoke(new Action(ReordenarPorNomenclatura), DispatcherPriority.Background);
        }

        // Recalcula la clave oculta y deja la tabla ordenada por nomenclatura.
        private void ReordenarPorNomenclatura()
        {
            if (_dtAddRows == null || !_dtAddRows.Columns.Contains(ColOrden)) return;
            foreach (DataRow r in _dtAddRows.Rows)
            {
                if (r.RowState == DataRowState.Deleted) continue;
                r[ColOrden] = ClaveOrden(Convert.ToString(r["Nomenclatura"]));
            }
            _dtAddRows.AcceptChanges();
            if (_dtAddRows.DefaultView.Sort != ColOrden) _dtAddRows.DefaultView.Sort = ColOrden;
        }

        // Orden natural: los tramos de dígitos se comparan como números, no como texto,
        // así P-2 va antes que P-2A y P-2A antes que P-10.
        private static string ClaveOrden(string nomen)
        {
            string s = (nomen ?? "").Trim().ToUpperInvariant();
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < s.Length; )
            {
                if (char.IsDigit(s[i]))
                {
                    int j = i;
                    while (j < s.Length && char.IsDigit(s[j])) j++;
                    sb.Append(s.Substring(i, j - i).TrimStart('0').PadLeft(8, '0'));
                    i = j;
                }
                else { sb.Append(s[i]); i++; }
            }
            return sb.ToString();
        }

        // ¿La usa ya otra fila? (comparación sin distinguir mayúsculas ni espacios)
        private bool NomenOcupada(string nomen, DataRow excepto)
        {
            if (_dtAddRows == null || !_dtAddRows.Columns.Contains("Nomenclatura")) return false;
            foreach (DataRow r in _dtAddRows.Rows)
            {
                if (r == excepto || r.RowState == DataRowState.Deleted) continue;
                if (string.Equals(Convert.ToString(r["Nomenclatura"]).Trim(), nomen,
                                  StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }

        // Nomenclatura automática: P1, P2, P3… (sin guion; así sale tal cual en el análisis).
        private static string NomenAuto(int n) { return "P" + n; }

        // Primer "Pn" que no esté ocupado (las manuales pueden haberse llevado algún número).
        private string SiguienteNomenLibre(DataRow excepto = null)
        {
            int n = 1;
            while (NomenOcupada(NomenAuto(n), excepto)) n++;
            return NomenAuto(n);
        }

        private string DescDeCodigo(string codigo)
        {
            string cod = (codigo ?? "").Trim();
            if (cod.Contains("-")) cod = cod.Split('-')[0].Trim();
            if (cod == "") return null;
            try
            {
                var con = new Generals.Conexion();
                string fail = "";
                if (!con.Open(out fail)) return null;
                string safe = cod.Replace("'", "");
                DataTable dt = con.ExecuteDataSet(
                    "SELECT Descripcion FROM componentes WHERE Codigo = '" + safe + "' LIMIT 1", out fail).Tables[0];
                con.Close();
                return dt.Rows.Count > 0 ? Convert.ToString(dt.Rows[0][0]) : null;
            }
            catch { return null; }
        }

        // ===== Analizar =====
        private void Analizar_Click(object sender, RoutedEventArgs e)
        {
            if (_dtAddRows.Rows.Count == 0)
            {
                GlassDialog.Informar(Owner, "Puertas", "Agrega al menos una puerta antes de analizar.");
                return;
            }
            try
            {
                LblEstado.Text = "Analizando…";
                var dto = new Dto.AnalisisDatosDto();
                DataTable perfil = dto.CalculateTab(3, _dtAddRows, _dtPuertas, false, MedidaBase, Desperdicio, true, 1);
                DataTable herraje = dto.CalculateTab(7, _dtAddRows, _dtPuertas, true, MedidaBase, Desperdicio, true, 1);

                // Guarda la base intacta (placeholders MOD… + perfilería 01), carga reglas
                // y muestra ya resuelto para la perfilería por defecto.
                _basePerfil = perfil != null ? perfil.Copy() : null;
                _baseHerraje = herraje != null ? herraje.Copy() : null;
                _resolver = Engine.DependenciaResolver.Cargar();
                _acabadoPerfil = AcabadoDePuertas();   // el acabado con que se agregaron las puertas (ej. "01")
                RefrescarDesdeBase();
                LblEstado.Text = "Análisis aplicado correctamente.";
            }
            catch (Exception ex)
            {
                LblEstado.Text = "Error al analizar.";
                GlassDialog.Informar(Owner, "Puertas", "No se pudo analizar:\n" + ex.Message);
            }
        }

        private void DgHerraje_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (_ocultarPrimeraHerraje)
            {
                e.Column.Visibility = Visibility.Collapsed;
                _ocultarPrimeraHerraje = false;
            }
        }

        // ===== Cambiar acabado en los resultados (port de FnChangeInfo) =====
        private void CambiarAcabado_Click(object sender, RoutedEventArgs e)
        {
            if ((_dtPerfil == null || _dtPerfil.Rows.Count == 0) && (_dtHerraje == null || _dtHerraje.Rows.Count == 0))
            {
                GlassDialog.Informar(Owner, "Puertas", "Primero pulsa Analizar para tener resultados.");
                return;
            }
            string a1, a2;
            if (!GlassDialog.PedirAcabado(Owner, out a1, out a2)) return;
            if (string.IsNullOrEmpty(a1) || string.IsNullOrEmpty(a2)) return;

            // ¿Se está cambiando la PERFILERÍA (el acabado del slot por defecto vigente)?
            if (CodigoAcabado(a1) == CodigoAcabado(_acabadoPerfil) && !string.IsNullOrWhiteSpace(a2))
            {
                // Pipeline base: re-aplica perfilería + re-resuelve dependencias MOD…
                _acabadoPerfil = a2;
                RefrescarDesdeBase();
            }
            else
            {
                // Cambio de un acabado cualquiera (no perfilería): edición directa, como antes.
                FnChangeInfo(a1, a2);
                DgPerfil.Items.Refresh();
                DgHerraje.Items.Refresh();
            }
            LblEstado.Text = "Acabado reemplazado en los resultados.";
        }

        /// <summary>
        /// Reconstruye los resultados desde las copias base intactas: parte de los placeholders
        /// MOD… y la perfilería por defecto, aplica el cambio a la perfilería vigente y resuelve
        /// las dependencias para ese valor. Al partir SIEMPRE de la base, es re-resolvible.
        /// </summary>
        private void RefrescarDesdeBase()
        {
            if (_basePerfil == null && _baseHerraje == null) return;

            _dtPerfil = _basePerfil != null ? _basePerfil.Copy() : null;
            _dtHerraje = _baseHerraje != null ? _baseHerraje.Copy() : null;

            // 1) Perfilería del valor por defecto (con el que se agregaron las puertas) al vigente.
            string defecto = AcabadoDePuertas();
            if (!string.IsNullOrWhiteSpace(_acabadoPerfil) &&
                CodigoAcabado(_acabadoPerfil) != CodigoAcabado(defecto))
                FnChangeInfo(defecto, _acabadoPerfil);

            // 2) Resuelve dependencias MOD… según la perfilería vigente (mismo motor que Mamparas).
            //    Recarga las reglas cada vez para tomar cambios sin re-analizar.
            _resolver = Engine.DependenciaResolver.Cargar();
            if (_resolver != null && _resolver.HayReglas)
            {
                var res = new Engine.ResultadoAnalisis { Puertas = _dtPerfil, PuertasHerraje = _dtHerraje };
                var sinRegla = _resolver.Resolver(res, CodigoAcabado(_acabadoPerfil));
                if (sinRegla.Count > 0)
                    LblEstado.Text = "Aviso: sin regla de dependencia para " + string.Join(", ", sinRegla)
                                   + " con esta perfilería.";
            }

            DgPerfil.ItemsSource = _dtPerfil != null ? _dtPerfil.DefaultView : null;
            _ocultarPrimeraHerraje = true;
            DgHerraje.ItemsSource = _dtHerraje != null ? _dtHerraje.DefaultView : null;
        }

        /// <summary>
        /// Acabado con el que se AGREGARON las puertas (columna "Acabado Perfileria Puertas"
        /// de la lista de entrada, ej. "01"), en formato "CÓDIGO - DESCRIPCIÓN". Es la fuente
        /// FIABLE de la perfilería en puertas (la tabla de perfil calculada trae la col 0 vacía).
        /// </summary>
        private string AcabadoDePuertas()
        {
            string code = "";
            if (_dtAddRows != null && _dtAddRows.Columns.Count > 0)
            {
                int idx = _dtAddRows.Columns.IndexOf("Acabado Perfileria Puertas");
                if (idx < 0) idx = 3;
                foreach (DataRow r in _dtAddRows.Rows)
                {
                    string c = idx < _dtAddRows.Columns.Count ? Convert.ToString(r[idx]).Trim() : "";
                    if (c != "") { code = c; break; }
                }
            }
            if (code == "") return "";
            string desc = DescAcabado(code);
            return desc != "" ? code + " - " + desc : code;
        }

        /// <summary>Descripción de un acabado por su Codigo_Homologacion (o "" si no se halla).</summary>
        private static string DescAcabado(string code)
        {
            try
            {
                var con = new Generals.Conexion();
                string fail = "";
                if (!con.Open(out fail)) return "";
                string safe = (code ?? "").Replace("'", "");
                DataTable dt = con.ExecuteDataSet(
                    "SELECT Descripcion FROM acabados WHERE Codigo_Homologacion = '" + safe + "' LIMIT 1", out fail).Tables[0];
                con.Close();
                return dt.Rows.Count > 0 ? Convert.ToString(dt.Rows[0][0]) : "";
            }
            catch { return ""; }
        }

        /// <summary>
        /// Acabado POR DEFECTO de perfilería = el del CÓDIGO "01" (sufijo "-01" del código),
        /// NO por descripción/homologación. Formato "CÓDIGO - DESCRIPCIÓN" como el buscador:
        /// código en col 1 ("BASE-ACAB"), descripción del acabado en col 3. Recorre las filas
        /// de perfil (no separador ni cabecera "Puerta"); si no hay código "01", cae a la 1ª.
        /// </summary>
        private static string AcabadoPorDefecto(DataTable perfil)
        {
            if (perfil == null || perfil.Columns.Count < 2) return "";
            string primera = "";
            foreach (DataRow row in perfil.Rows)
            {
                // En puertas la col 0 (Nomenclatura) viene VACÍA en las filas de perfil, así que
                // NO se filtra por col 0 vacía: solo se saltan las cabeceras "Puerta" y las filas
                // sin código-acabado (separadores). El acabado se toma del sufijo del código.
                string c0 = Convert.ToString(row[0]);
                if (c0.Contains("Puerta")) continue;
                string cod = Convert.ToString(row[1]);
                int dash = cod.LastIndexOf('-');
                if (dash < 0) continue;
                string codAcab = cod.Substring(dash + 1).Trim();
                if (codAcab == "" || codAcab.StartsWith("MOD", StringComparison.OrdinalIgnoreCase)) continue;
                string desc = perfil.Columns.Count > 3 ? Convert.ToString(row[3]) : "";
                string full = desc != "" ? codAcab + " - " + desc : codAcab;
                if (primera == "") primera = full;
                if (codAcab == "01") return full;
            }
            return primera;
        }

        /// <summary>Código de acabado de una cadena "CÓDIGO - DESCRIPCIÓN".</summary>
        private static string CodigoAcabado(string acabado)
        {
            if (string.IsNullOrEmpty(acabado)) return "";
            return acabado.Contains("-") ? acabado.Split('-')[0].Trim() : acabado.Trim();
        }

        // ===== Exportar a Excel (mismo motor que Mamparas) =====
        private void Exportar_Click(object sender, RoutedEventArgs e)
        {
            if ((_dtPerfil == null || _dtPerfil.Rows.Count == 0) &&
                (_dtHerraje == null || _dtHerraje.Rows.Count == 0))
            {
                GlassDialog.Informar(Owner, "Exportar", "No existen datos analizados para exportar. Pulsa Analizar primero.");
                return;
            }

            var bsc = new ExportDialog
            {
                Owner = Owner,
                PrefillAcabado1 = _acabadoPerfil   // Sentido A: precarga el acabado ya aplicado
            };
            bsc.ShowDialog();
            if (bsc.Numero == null) return;   // canceló

            // Sentido B: si se eligió un acabado de perfilería distinto al vigente, aplícalo
            // antes de exportar por el pipeline base (re-resuelve también las dependencias MOD…).
            if (!string.IsNullOrWhiteSpace(bsc.Acabado1) &&
                !string.IsNullOrWhiteSpace(_acabadoPerfil) &&
                bsc.Acabado1.Trim() != _acabadoPerfil.Trim())
            {
                _acabadoPerfil = bsc.Acabado1;   // el vigente pasa a ser el elegido
                RefrescarDesdeBase();
            }

            // El exportador de Mamparas trabaja sobre un ResultadoAnalisis: la perfilería de
            // puertas es res.Puertas (dg2) y los herrajes res.PuertasHerraje (dg4); el resto
            // queda vacío y el motor omite esas hojas.
            var res = new Engine.ResultadoAnalisis
            {
                Puertas = _dtPerfil,
                PuertasHerraje = _dtHerraje
            };

            string[] param = { bsc.Numero, bsc.Nombre, bsc.Tecnico, bsc.Fecha,
                               bsc.Acabado1, bsc.Acabado2, bsc.Albaran, bsc.Referencia };

            string folder;
            using (var fb = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (fb.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
                folder = fb.SelectedPath;
            }
            if (string.IsNullOrEmpty(folder)) return;

            try
            {
                LblEstado.Text = "Exportando…";
                string archivo = new Engine.ExcelExporter().Exportar(res, param, folder, "0");
                LblEstado.Text = "Exportado: " + archivo;
                if (GlassDialog.Pregunta(Owner, "Exportar",
                        "Se exportó correctamente. ¿Deseas abrirlo ahora?", si: "Abrir", no: "Ahora no"))
                    System.Diagnostics.Process.Start(archivo);
            }
            catch (Exception ex)
            {
                LblEstado.Text = "Error al exportar.";
                GlassDialog.Informar(Owner, "Exportar", "No se pudo exportar:\n" + ex.Message);
            }
        }

        private void FnChangeInfo(string a1, string a2)
        {
            for (int dg = 1; dg <= 2; dg++)
            {
                DataTable t = dg == 1 ? _dtHerraje : _dtPerfil;
                if (t == null || t.Rows.Count == 0 || t.Columns.Count < 4) continue;

                foreach (DataRow row in t.Rows)
                {
                    string valuezero = Convert.ToString(row[0]);
                    string acabado = Convert.ToString(row[3]);

                    // Perfiles (no-puerta): cambia solo el sufijo de acabado del código si coincide.
                    if (dg == 2 && !valuezero.Contains("Puerta"))
                    {
                        string[] codeParts = Convert.ToString(row[1]).Split('-');
                        if (codeParts.Length > 1)
                        {
                            string codAcabado = codeParts[1].Trim();
                            string codAcabadoOrigen = a1.Split('-')[0].Trim();
                            if (codAcabado == codAcabadoOrigen)
                            {
                                string acNew = a2.Contains("-") ? a2.Split('-')[0].Trim() : "XX";
                                row[1] = codeParts[0].Trim() + "-" + acNew;
                            }
                        }
                        continue;
                    }

                    int posini = 0, posfin = 0; string acabadoDesc = "";
                    if (dg == 2)
                    {
                        acabadoDesc = Convert.ToString(row[2]);
                        posini = acabadoDesc.IndexOf("(");
                        posfin = acabadoDesc.IndexOf(")");
                        if (posini >= 0 && posfin > posini)
                            acabado = acabadoDesc.Substring(posini + 1, posfin - (posini + 1));
                    }

                    if (!string.IsNullOrEmpty(acabado) && a1.Contains(acabado))
                    {
                        if (dg == 2 && valuezero.Contains("Puerta") && posini >= 0 && posfin > posini)
                        {
                            string ini = acabadoDesc.Substring(0, posini + 1);
                            string fin = acabadoDesc.Substring(posfin, acabadoDesc.Length - posfin);
                            string destDesc = a2.Contains("-") ? a2.Split('-')[1].Trim() : a2;
                            string acNew = a2.Contains("-") ? a2.Split('-')[0].Trim() : "XX";
                            row[1] = Convert.ToString(row[1]).Split('-')[0].Trim() + "-" + acNew;
                            row[2] = ini + destDesc + fin;
                        }
                        else
                        {
                            row[3] = a2.Contains("-") ? a2.Split('-')[1].Trim() : a2;
                        }
                    }
                }
            }
        }

        private void DgPerfil_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            var drv = e.Row.Item as DataRowView;
            if (drv == null || drv.Row.Table.Columns.Count == 0) { e.Row.ClearValue(Control.BackgroundProperty); return; }
            string c0 = Convert.ToString(drv.Row[0]);
            if (string.IsNullOrEmpty(c0))
                e.Row.Background = new SolidColorBrush(Color.FromArgb(0x55, 0x44, 0x44, 0x44));
            else if (c0.Contains("Puerta"))
                e.Row.Background = new SolidColorBrush(Color.FromArgb(0x4D, 0xE0, 0x7B, 0x5B));
            else
                e.Row.ClearValue(Control.BackgroundProperty);
        }

        private void Limpiar_Click(object sender, RoutedEventArgs e)
        {
            _dtAddRows.Rows.Clear();
            _dtAddRows.Columns.Clear();
            _nomenManual.Clear();
            DgNuevas.ItemsSource = null;
            DgPerfil.ItemsSource = null;
            DgHerraje.ItemsSource = null;
            TxtCodigo.Text = ""; TxtDescripcion.Text = ""; TxtCantidad.Text = "1"; _acabado = "";
            _dtPerfil = null; _dtHerraje = null; _acabadoPerfil = "";
            _basePerfil = null; _baseHerraje = null;
            LblEstado.Text = "Lista vaciada.";
        }

        // ===== Quitar una o varias puertas de la lista =====
        private void Quitar_Click(object sender, RoutedEventArgs e) => QuitarSeleccionadas();

        private void DgNuevas_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Supr borra las filas seleccionadas, salvo que se esté editando una celda (texto).
            if (e.Key == System.Windows.Input.Key.Delete &&
                !(System.Windows.Input.Keyboard.FocusedElement is TextBox))
            {
                QuitarSeleccionadas();
                e.Handled = true;
            }
        }

        private void QuitarSeleccionadas()
        {
            if (_dtAddRows == null || _dtAddRows.Columns.Count == 0) return;
            if (DgNuevas.SelectedItems == null || DgNuevas.SelectedItems.Count == 0)
            {
                LblEstado.Text = "Selecciona una o varias filas para quitar (Ctrl/Shift para varias).";
                return;
            }

            var filas = new System.Collections.Generic.List<DataRow>();
            foreach (var item in DgNuevas.SelectedItems)
            {
                var drv = item as DataRowView;
                if (drv != null) filas.Add(drv.Row);
            }

            int n = 0;
            foreach (var row in filas)
                if (row.RowState != DataRowState.Detached && row.RowState != DataRowState.Deleted)
                { row.Delete(); n++; }
            _dtAddRows.AcceptChanges();

            foreach (var row in filas) _nomenManual.Remove(row);

            // Renumera la nomenclatura automática (P-1, P-2, …) para que quede consecutiva,
            // saltando las filas que el usuario renombró a mano y los números que ya ocupan.
            if (_dtAddRows.Columns.Contains("Nomenclatura"))
            {
                // En el orden en que se ven en la tabla, no en el de inserción.
                var enOrden = new List<DataRow>();
                foreach (DataRowView v in _dtAddRows.DefaultView) enOrden.Add(v.Row);

                int auto = 1;
                foreach (DataRow r in enOrden)
                {
                    if (_nomenManual.Contains(r)) continue;
                    while (NomenOcupada(NomenAuto(auto), r)) auto++;
                    r["Nomenclatura"] = NomenAuto(auto);
                    auto++;
                }
                _dtAddRows.AcceptChanges();
                ReordenarPorNomenclatura();
            }

            DgNuevas.ItemsSource = _dtAddRows.DefaultView;
            LblEstado.Text = n + " puerta(s) quitada(s). Pulsa Analizar para recalcular.";
        }
    }
}
