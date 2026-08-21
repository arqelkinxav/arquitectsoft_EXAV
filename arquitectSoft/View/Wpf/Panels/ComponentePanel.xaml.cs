using arquitectSoft.Class;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace arquitectSoft.View.Wpf.Panels
{
    /// <summary>
    /// Versión "panel" de Componente para hospedarse dentro del escritorio (MdiChild):
    /// editor con encabezado (Código/Descripción/Especial/Acabado) + dos grillas editables
    /// (genérica y especial) con columnas ComboBox de catálogo, máquina de estados, Buscar,
    /// Agregar subcomponente, Guardar, Eliminar, Nuevo, Cancelar, Verificar y Duplicar, más
    /// el menú contextual (Asignación Puertas / Mecanizado / anchura "Ambas"). Sin chrome ni
    /// liquid glass: lo aporta la ventana hija. Reutiliza Dto.ComponenteDto / AcabadoDto y los
    /// catálogos, y el converter arquitectSoft.View.Wpf.PuertaABrushConverter.
    /// </summary>
    public partial class ComponentePanel : UserControl
    {
        private string _opc;
        private string _idComponente = "";
        private string _condicionAcabado = "";

        private readonly ObservableCollection<Sub_Component> _items = new ObservableCollection<Sub_Component>();
        private readonly ObservableCollection<Sub_ComponentEspecial> _itemsEsp = new ObservableCollection<Sub_ComponentEspecial>();

        private DataView _dtUnidad, _dtCorte, _dtMedida, _dtColumna;

        // Catálogo de mecanizados: la lista completa (celdas sin editar) y una vista APARTE
        // sobre una copia, que es la que se filtra mientras se teclea en la celda en edición.
        private List<MecItem> _mecTodos = new List<MecItem>();
        private ListCollectionView _mecVista;
        private bool _mecFiltrando;   // evita reentrar al restaurar el texto de la caja
        private ComboBox _mecCombo;   // combo buscador de la celda que se está editando
        private DataGridColumn _colMecanizado;

        /// <summary>Catálogo completo de mecanizados (enlace de la celda sin editar).</summary>
        public IList<MecItem> MecanizadosTodos { get { return _mecTodos; } }

        /// <summary>Vista filtrable del catálogo (enlace del combo buscador en edición).</summary>
        public ICollectionView MecanizadosFiltrados { get { return _mecVista; } }

        public ComponentePanel()
        {
            InitializeComponent();

            CargarCatalogos();
            ConstruirColumnasGenerico();
            ConstruirColumnasEspecial();
            GridComponente.ItemsSource = _items;
            GridComponenteEsp.ItemsSource = _itemsEsp;

            KeyDown += Panel_KeyDown;
            Loaded += (s, e) =>
            {
                if (CmbAcabado.ItemsSource == null)
                {
                    CargarAcabados("");
                    EstadoInicial();
                }
            };
        }

        // Ventana que hospeda el panel (para que los diálogos cristal tengan owner).
        private Window Owner { get { return Window.GetWindow(this); } }

        // Atajos: Ctrl+Z = cancelar, Supr = quitar fila seleccionada.
        private void Panel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Z && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (BtnCancelar.IsEnabled) { Cancelar_Click(null, null); e.Handled = true; }
            }
            else if (e.Key == Key.Delete)
            {
                if (!GridComponente.IsReadOnly &&
                    (GridComponente.SelectedItems.Count > 0 || GridComponenteEsp.SelectedItems.Count > 0))
                { Borrar_Click(null, null); e.Handled = true; }
            }
        }

        // ===== Catálogos para los combos de las grillas =====
        private void CargarCatalogos()
        {
            // Unidad Calculada: agrego columna string para enlazar con Sub_Component.UnidadCalculada (string)
            DataTable uni = new Dto.UnidadCalculadaDto().GetUnidadCalculada();
            if (!uni.Columns.Contains("IdStr")) uni.Columns.Add("IdStr", typeof(string));
            foreach (DataRow r in uni.Rows) r["IdStr"] = Convert.ToString(r["Id_Unidad_Calculada"]);
            _dtUnidad = uni.DefaultView;

            // Cortes: agrego columna int garantizada para enlazar con Cortes (int)
            DataTable cor = new Dto.CorteDto().GetCortes();
            if (!cor.Columns.Contains("IdInt")) cor.Columns.Add("IdInt", typeof(int));
            foreach (DataRow r in cor.Rows)
            {
                int n; int.TryParse(Convert.ToString(r["Id_Corte"]), out n); r["IdInt"] = n;
            }
            _dtCorte = cor.DefaultView;

            // Selección Medida (estático): 0 / Altura / Anchura
            DataTable med = new DataTable();
            med.Columns.Add("Codigo", typeof(int));
            med.Columns.Add("Descripcion", typeof(string));
            med.Rows.Add(0, ""); med.Rows.Add(1, "Altura"); med.Rows.Add(2, "Anchura");
            _dtMedida = med.DefaultView;

            // Mecanizados: catálogo de la celda editable. La BD guarda el id (componentes_detalle.Mecanizado
            // es int), así que necesito un id int garantizado para enlazar con Sub_Component.Cod_Mecanizado;
            // el union de MecanizadoDto puede devolverlo como Int64. La fila 0 ("sin mecanizado") viene con
            // un espacio por descripción: lo dejo vacío para que la celda se vea limpia.
            DataTable mec = new Dto.MecanizadoDto().GetMecanizado();
            _mecTodos = new List<MecItem>();
            foreach (DataRow r in mec.Rows)
            {
                int n; int.TryParse(Convert.ToString(r["Id_mecanizado"]), out n);
                _mecTodos.Add(new MecItem(n, Convert.ToString(r["Descripcion"]).Trim()));
            }
            // Orden alfanumérico natural (M2 antes que M10) dejando arriba el "sin mecanizado".
            _mecTodos.Sort(MecItem.PorDescripcion);
            _mecVista = new ListCollectionView(new List<MecItem>(_mecTodos));

            // Columnas (estático): 1..5 + columna string para Sub_ComponentEspecial.Columna (string)
            DataTable col = new DataTable();
            col.Columns.Add("IdStr", typeof(string));
            col.Columns.Add("Descripcion", typeof(string));
            for (int i = 1; i <= 5; i++) col.Rows.Add(i.ToString(), "Columna #" + i);
            _dtColumna = col.DefaultView;
        }

        private void ConstruirColumnasGenerico()
        {
            var g = GridComponente;
            g.Columns.Add(Texto("Código", "Codigo", 120, true));
            g.Columns.Add(Texto("Descripción", "Descripcion", 280, true));
            g.Columns.Add(Combo("Unidad Calculada", "UnidadCalculada", _dtUnidad, "IdStr", 150));
            g.Columns.Add(Texto("Cx. defecto", "Cxdefecto", 80, false));
            g.Columns.Add(Texto("C. Adicional", "CAdicional", 80, false));
            g.Columns.Add(Chk("A. Decremento", "ADecremento"));
            g.Columns.Add(Combo("Cortes", "Cortes", _dtCorte, "IdInt", 120));
            g.Columns.Add(Chk("Extra", "Extra"));
            g.Columns.Add(Combo("Sel. Medida", "Medida", _dtMedida, "Codigo", 120));
            g.Columns.Add(ComboMecanizado());
        }

        private void ConstruirColumnasEspecial()
        {
            var g = GridComponenteEsp;
            g.Columns.Add(Texto("Código", "Codigo", 120, true));
            g.Columns.Add(Texto("Descripción", "Descripcion", 300, true));
            g.Columns.Add(Combo("Sel. Columna", "Columna", _dtColumna, "IdStr", 130));
            g.Columns.Add(Texto("Cx. defecto", "Cxdefecto", 90, false));
            g.Columns.Add(Texto("C. Adicional", "CAdicional", 90, false));
        }

        private static DataGridTextColumn Texto(string header, string prop, double width, bool readOnly)
        {
            return new DataGridTextColumn
            {
                Header = header,
                Binding = new Binding(prop),
                Width = width,
                IsReadOnly = readOnly
            };
        }

        private DataGridCheckBoxColumn Chk(string header, string prop)
        {
            var st = (Style)FindResource("GridCheck");
            return new DataGridCheckBoxColumn
            {
                Header = header,
                Binding = new Binding(prop),
                ElementStyle = st,
                EditingElementStyle = st
            };
        }

        private DataGridComboBoxColumn Combo(string header, string prop, DataView source, string valuePath, double width)
        {
            var st = (Style)FindResource("GridCombo");
            return new DataGridComboBoxColumn
            {
                Header = header,
                Width = width,
                ItemsSource = source,
                SelectedValuePath = valuePath,
                // PropertyChanged: el valor elegido se vuelca al objeto en el acto, sin
                // depender de que la celda/fila confirme la edición (CommitEdit). Así el
                // combo "Cortes" (y los demás) nunca se pierde al pulsar Guardar directo.
                SelectedValueBinding = new Binding(prop) { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged },
                ElementStyle = st,
                EditingElementStyle = st
            };
        }

        // Mecanizado: columna de plantilla (celda = combo de solo lectura, edición = combo
        // buscador). El enlace va a Cod_Mecanizado (el id que espera la BD), no al texto:
        // componentes_detalle.Mecanizado es int con FK a mecanizados. El texto de
        // Sub_Component.Mecanizado se mantiene sincronizado al confirmar (SincronizarMecanizado).
        private DataGridTemplateColumn ComboMecanizado()
        {
            var col = new DataGridTemplateColumn
            {
                Header = "Mecanizado",
                Width = 180,
                SortMemberPath = "Cod_Mecanizado",
                CellTemplate = (DataTemplate)FindResource("MecCelda"),
                CellEditingTemplate = (DataTemplate)FindResource("MecEdicion")
            };
            _colMecanizado = col;
            return col;
        }

        // El combo buscador se engancha desde su propio Loaded y NO desde PreparingCellForEdit:
        // en una columna de plantilla, DataGrid entrega en EditingElement el ContentPresenter que
        // envuelve la plantilla, no el ComboBox, así que "EditingElement as ComboBox" daba null y
        // se perdían filtro, foco y la restauración del valor al salir de la celda.
        private void MecanizadoCombo_Loaded(object sender, RoutedEventArgs e)
        {
            var cb = sender as ComboBox;
            if (cb == null) return;

            _mecCombo = cb;
            QuitarFiltroMecanizado();

            // TextChanged burbujea desde la caja de texto interna hasta el ComboBox.
            cb.RemoveHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(Mecanizado_TextChanged));
            cb.AddHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(Mecanizado_TextChanged), true);

            cb.ApplyTemplate();   // la caja de texto no existe hasta que se aplica el template
            cb.Dispatcher.BeginInvoke(new Action(delegate
            {
                var caja = CajaDe(cb);
                if (caja != null) { caja.Focus(); caja.SelectAll(); } else cb.Focus();
                cb.IsDropDownOpen = true;
            }), System.Windows.Threading.DispatcherPriority.Input);
        }

        private void MecanizadoCombo_Unloaded(object sender, RoutedEventArgs e)
        {
            var cb = sender as ComboBox;
            if (cb != null) cb.RemoveHandler(TextBoxBase.TextChangedEvent, new TextChangedEventHandler(Mecanizado_TextChanged));
            if (ReferenceEquals(_mecCombo, cb)) _mecCombo = null;
            QuitarFiltroMecanizado();
        }

        // Caja de texto del combo editable (no existe hasta que se aplica el template).
        private static TextBox CajaDe(ComboBox cb)
        {
            return cb.Template == null ? null : cb.Template.FindName("PART_EditableTextBox", cb) as TextBox;
        }

        // Solo se filtra con lo que ESCRIBE el usuario: al recorrer la lista con las flechas el
        // combo también reescribe la caja, y refiltrar ahí dejaría un único elemento y bloquearía
        // el recorrido. Se distingue porque en ese caso el texto es exactamente el del elegido.
        private void Mecanizado_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_mecFiltrando) return;

            var cb = sender as ComboBox;
            if (cb == null) return;

            var elegido = cb.SelectedItem as MecItem;
            if (elegido != null && string.Equals(elegido.Descripcion, cb.Text ?? "", StringComparison.Ordinal)) return;

            // Al filtrar, el elemento seleccionado puede quedar fuera: entonces el combo se queda
            // sin selección y vacía la caja, así que hay que devolver texto y cursor a su sitio.
            cb.Dispatcher.BeginInvoke(new Action(delegate
            {
                var caja = CajaDe(cb);
                string t = caja != null ? caja.Text : (cb.Text ?? "");
                int cursor = caja != null ? caja.CaretIndex : t.Length;

                _mecFiltrando = true;
                try
                {
                    AplicarFiltroMecanizado(t);
                    if (caja != null && caja.Text != t)
                    {
                        caja.Text = t;
                        caja.CaretIndex = Math.Min(cursor, t.Length);
                    }
                }
                finally { _mecFiltrando = false; }

                cb.IsDropDownOpen = true;
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        // Deja en la vista de edición solo los mecanizados que contienen todos los trozos escritos.
        private void AplicarFiltroMecanizado(string texto)
        {
            if (_mecVista == null) return;

            string[] trozos = Normalizar(texto).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (trozos.Length == 0) { QuitarFiltroMecanizado(); return; }

            _mecVista.Filter = delegate(object o)
            {
                var m = o as MecItem;
                if (m == null) return false;
                string d = Normalizar(m.Descripcion);
                foreach (string t in trozos)
                    if (d.IndexOf(t, StringComparison.Ordinal) < 0) return false;
                return true;
            };
        }

        private void QuitarFiltroMecanizado()
        {
            if (_mecVista != null && _mecVista.Filter != null) _mecVista.Filter = null;
        }

        // Texto comparable: sin tildes, sin mayúsculas y sin espacios de sobra.
        private static string Normalizar(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            string d = s.Trim().ToUpperInvariant().Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(d.Length);
            foreach (char c in d)
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark) sb.Append(c);
            return sb.ToString();
        }

        // Qué mecanizado quiso decir el texto tecleado: coincidencia exacta, o el único que quedó
        // en la lista filtrada. Vacío = sin mecanizado (id 0). null = no se pudo resolver.
        private MecItem ResolverMecanizado(string texto)
        {
            if (_mecVista == null) return null;
            if (string.IsNullOrEmpty((texto ?? "").Trim())) return new MecItem(0, "");

            string buscado = Normalizar(texto);
            foreach (MecItem m in _mecTodos)
                if (Normalizar(m.Descripcion) == buscado) return m;

            var visibles = _mecVista.Cast<MecItem>().ToList();
            return visibles.Count == 1 ? visibles[0] : null;
        }

        // Al cerrar la edición se quita el filtro (si no, la lista quedaría recortada para la
        // siguiente celda). Si se escribió sin llegar a elegir de la lista, se intenta resolver el
        // texto; si no corresponde a ningún mecanizado, el combo quedaría sin selección y el enlace
        // escribiría null en Cod_Mecanizado (int), dejando la celda en error de validación: en ese
        // caso se restaura lo que ya tenía la fila.
        private void GridComponente_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column != _colMecanizado) return;

            // EditingElement es el ContentPresenter de la plantilla, no el combo: se usa el que
            // se guardó al cargarse la celda en edición.
            ComboBox cb = e.EditingElement as ComboBox;
            if (cb == null) cb = _mecCombo;
            if (cb == null) { QuitarFiltroMecanizado(); return; }

            if (e.EditAction == DataGridEditAction.Commit && cb.SelectedItem == null)
            {
                var fila = e.Row.Item as Sub_Component;
                MecItem m = ResolverMecanizado(cb.Text);   // mira la lista ya filtrada

                QuitarFiltroMecanizado();                  // ...y se asigna sin filtro
                if (m != null) cb.SelectedValue = m.Id;
                else if (fila != null) cb.SelectedValue = fila.Cod_Mecanizado;
            }

            QuitarFiltroMecanizado();
        }

        // Descripción de catálogo para un id de mecanizado (vacío si no tiene).
        private string DescripcionMecanizado(int cod)
        {
            if (cod <= 0) return "";
            foreach (MecItem m in _mecTodos)
                if (m.Id == cod) return m.Descripcion;
            return "";
        }

        // Deja el texto de cada fila acorde al id elegido en el combo.
        private void SincronizarMecanizado()
        {
            foreach (var s in _items) s.Mecanizado = DescripcionMecanizado(s.Cod_Mecanizado);
        }

        // ===== Acabado (combo del encabezado), filtrado por los acabados de los subcomponentes =====
        private void CargarAcabados(string condicion)
        {
            DataTable dt = new Dto.AcabadoDto().GetAcabadoParam(condicion);
            CmbAcabado.ItemsSource = dt.DefaultView;
            if (dt.Rows.Count > 0) CmbAcabado.SelectedIndex = 0;
        }

        private string AcabadoSeleccionado()
        {
            return CmbAcabado.SelectedValue != null ? CmbAcabado.SelectedValue.ToString() : "0";
        }

        // ===== Máquina de estados =====
        private void EstadoInicial()
        {
            // Habilitado pero de solo-lectura: así el código se puede seleccionar y copiar
            // aunque no se pueda editar (un TextBox deshabilitado no deja copiar).
            TxtCodigo.IsEnabled = true;
            TxtCodigo.IsReadOnly = true;
            TxtDescripcion.IsEnabled = false;
            ChkEspecial.IsEnabled = false;
            CmbAcabado.IsEnabled = false;
            GridComponente.IsReadOnly = true;
            GridComponenteEsp.IsReadOnly = true;

            BtnGuardar.IsEnabled = false;
            BtnCancelar.IsEnabled = false;
            BtnCheck.IsEnabled = false;
            BtnEliminar.IsEnabled = false;
            BtnDuplicar.IsEnabled = false;
            BtnAgregar.IsEnabled = false;
            BtnBorrar.IsEnabled = false;
            BtnNuevo.IsEnabled = true;
            BtnBuscar.IsEnabled = true;
        }

        private void HabilitarNuevo(string opcion)
        {
            BtnGuardar.IsEnabled = true;
            BtnCancelar.IsEnabled = true;
            BtnCheck.IsEnabled = true;
            BtnAgregar.IsEnabled = true;
            BtnBorrar.IsEnabled = true;

            TxtCodigo.IsEnabled = true;                   // siempre seleccionable/copiable
            TxtCodigo.IsReadOnly = opcion == "Editar";    // en edición el código no se toca (pero se copia)
            TxtDescripcion.IsEnabled = true;
            ChkEspecial.IsEnabled = true;
            CmbAcabado.IsEnabled = true;
            GridComponente.IsReadOnly = false;
            GridComponenteEsp.IsReadOnly = false;

            BtnNuevo.IsEnabled = false;
            BtnEliminar.IsEnabled = false;
            BtnBuscar.IsEnabled = false;
            BtnDuplicar.IsEnabled = false;
        }

        private void BloquearCancelar()
        {
            BtnCancelar.IsEnabled = false;
            BtnGuardar.IsEnabled = false;
            BtnCheck.IsEnabled = false;
            BtnAgregar.IsEnabled = false;
            BtnBorrar.IsEnabled = false;
            TxtCodigo.IsEnabled = true;     // se mantiene copiable
            TxtCodigo.IsReadOnly = true;
            TxtDescripcion.IsEnabled = false;
            ChkEspecial.IsEnabled = false;
            CmbAcabado.IsEnabled = false;
            GridComponente.IsReadOnly = true;
            GridComponenteEsp.IsReadOnly = true;

            BtnNuevo.IsEnabled = true;
            BtnEliminar.IsEnabled = false;
            BtnDuplicar.IsEnabled = false;
            BtnBuscar.IsEnabled = true;

            _condicionAcabado = "";
            CargarAcabados("");
        }

        private void ClearComponent()
        {
            TxtCodigo.Text = "";
            TxtDescripcion.Text = "";
            ChkEspecial.IsChecked = false;
            _items.Clear();
            _itemsEsp.Clear();
            _idComponente = "";
        }

        private void HabilitarEspecial(bool sw)
        {
            EspPanel.Visibility = sw ? Visibility.Visible : Visibility.Collapsed;
            if (!sw) _itemsEsp.Clear();
        }

        private void ChkEspecial_Changed(object sender, RoutedEventArgs e)
        {
            HabilitarEspecial(ChkEspecial.IsChecked == true);
        }

        // ===== Botones =====
        private void Nuevo_Click(object sender, RoutedEventArgs e)
        {
            _opc = "Nuevo";
            ClearComponent();
            _condicionAcabado = "";
            CargarAcabados("");
            HabilitarNuevo(null);
            LblEstado.Text = "Nuevo componente. Agrega subcomponentes y guarda.";
        }

        private void Duplicar_Click(object sender, RoutedEventArgs e)
        {
            _opc = "Duplicar";
            TxtCodigo.Text = "";
            TxtDescripcion.Text = "";
            _idComponente = "";
            HabilitarNuevo("Duplicar");   // conserva el detalle cargado para guardarlo con otro código
            LblEstado.Text = "Duplicando: cambia el código y guarda.";
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            ClearComponent();
            BloquearCancelar();
            HabilitarEspecial(false);
            LblEstado.Text = "";
        }

        private void Buscar_Click(object sender, RoutedEventArgs e)
        {
            var bsc = new BuscarDialog { Owner = Owner };   // sin Consulta = componentes
            bsc.ShowDialog();
            if (bsc.ReturnItem1 == null) return;

            ClearComponent();
            _idComponente = bsc.ReturnItem0;
            TxtCodigo.Text = bsc.ReturnItem1;
            TxtDescripcion.Text = bsc.ReturnItem2;
            ChkEspecial.IsChecked = bsc.ReturnItem3 == "1";

            CargarDetalle(new Dto.ComponenteDto().GetComponentDetalle(bsc.ReturnItem0));

            DataTable esp = new Dto.ComponenteDto().GetComponentEspecialDetalle(bsc.ReturnItem0);
            bool swEsp = esp != null && esp.Rows.Count > 0;
            HabilitarEspecial(swEsp);
            if (swEsp) CargarDetalleEspecial(esp);

            CargarAcabados(_condicionAcabado);
            int idAc; int.TryParse(bsc.ReturnItem4, out idAc);
            CmbAcabado.SelectedValue = idAc;

            // Cargado = editable de inmediato (ya no hay botón "Editar").
            _opc = "Editar";
            HabilitarNuevo("Editar");
            BtnEliminar.IsEnabled = true;
            BtnDuplicar.IsEnabled = true;
            LblEstado.Text = "Componente cargado. Edítalo y guarda.";
        }

        private void GridComponente_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            // Sin edición no se puede usar el menú de acciones sobre los subcomponentes.
            if (GridComponente.IsReadOnly) e.Handled = true;
        }

        private void Check_Click(object sender, RoutedEventArgs e)
        {
            ConfirmarEdiciones();
            string resul = new Dto.ComponenteDto().ExistComponent(TxtCodigo.Text, TxtDescripcion.Text, AcabadoSeleccionado());
            GlassDialog.Informar(Owner, "Componente",
                resul != "0" ? "El componente ya existe." : "Componente disponible para guardar.");
        }

        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            if (!GlassDialog.Pregunta(Owner, "Componente", "¿Seguro que quieres eliminar el registro?")) return;
            var dto = new Dto.ComponenteDto();
            string resul = dto.ExistComponent(TxtCodigo.Text, TxtDescripcion.Text, AcabadoSeleccionado());
            int id;
            if (!int.TryParse(resul, out id))
            {
                GlassDialog.Informar(Owner, "Componente", "No se pudo identificar el registro: " + resul);
                return;
            }
            resul = dto.DeleteComponent(id);
            BloquearCancelar();
            ClearComponent();
            HabilitarEspecial(false);
            GlassDialog.Informar(Owner, "Componente", resul);
        }

        private void Agregar_Click(object sender, RoutedEventArgs e)
        {
            var bsc = new BuscarDialog { Consulta = "SubComp", Owner = Owner };
            bsc.ShowDialog();
            if (bsc.ReturnItem0 == null && bsc.ReturnItem4 == null) return;

            if (bsc.ReturnItem0 != null && bsc.ReturnItem4 == "0")
            {
                int idSub; int.TryParse(bsc.ReturnItem0, out idSub);
                _items.Add(new Sub_Component(bsc.ReturnItem1, bsc.ReturnItem2, 1, 30, "", false, idSub, "", 0, false, 1, "", 0, 0));
                AcumularAcabado(bsc.ReturnItem1);
                CargarAcabados(_condicionAcabado);
            }
            else if (bsc.ReturnItem0 != null && ChkEspecial.IsChecked == true)
            {
                int idSub; int.TryParse(bsc.ReturnItem0, out idSub);
                _itemsEsp.Add(new Sub_ComponentEspecial(bsc.ReturnItem1, bsc.ReturnItem2, "", 1, 1, idSub));
                AcumularAcabado(bsc.ReturnItem1);
                CargarAcabados(_condicionAcabado);
            }
            else
            {
                GlassDialog.Informar(Owner, "Componente", "Primero marca que es un componente Especial.");
            }
        }

        private void Borrar_Click(object sender, RoutedEventArgs e)
        {
            // Soporta selección múltiple: borra todas las filas marcadas a la vez.
            var gen = GridComponente.SelectedItems.OfType<Sub_Component>().ToList();
            var esp = GridComponenteEsp.SelectedItems.OfType<Sub_ComponentEspecial>().ToList();
            if (gen.Count == 0 && esp.Count == 0)
            {
                GlassDialog.Informar(Owner, "Componente", "Selecciona una o varias filas para quitar.");
                return;
            }
            if (gen.Count > 0)
            {
                string msg = gen.Count == 1
                    ? "¿Quitar el subcomponente \"" + gen[0].Descripcion + "\"?"
                    : "¿Quitar los " + gen.Count + " subcomponentes seleccionados?";
                if (GlassDialog.Pregunta(Owner, "Componente", msg))
                    foreach (var s in gen) _items.Remove(s);
            }
            if (esp.Count > 0)
            {
                string msg = esp.Count == 1
                    ? "¿Quitar el subcomponente especial \"" + esp[0].Descripcion + "\"?"
                    : "¿Quitar los " + esp.Count + " subcomponentes especiales seleccionados?";
                if (GlassDialog.Pregunta(Owner, "Componente", msg))
                    foreach (var s in esp) _itemsEsp.Remove(s);
            }
        }

        // Confirma la edición pendiente de las grillas: si el usuario cambia una celda
        // (p.ej. el combo "Cortes") y pulsa Guardar sin salir de la celda/fila, WPF aún
        // no ha volcado el valor al objeto enlazado. Sin esto se guardaría el valor viejo.
        private void ConfirmarEdiciones()
        {
            GridComponente.CommitEdit(DataGridEditingUnit.Cell, true);
            GridComponente.CommitEdit(DataGridEditingUnit.Row, true);
            GridComponenteEsp.CommitEdit(DataGridEditingUnit.Cell, true);
            GridComponenteEsp.CommitEdit(DataGridEditingUnit.Row, true);
            SincronizarMecanizado();
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            ConfirmarEdiciones();
            var dto = new Dto.ComponenteDto();
            int acabado; int.TryParse(AcabadoSeleccionado(), out acabado);
            string fail;

            bool ok = dto.ValilidationSaveComponenet(TxtCodigo.Text, TxtDescripcion.Text,
                ChkEspecial.IsChecked == true, acabado, _items.Count, _itemsEsp.Count, out fail);

            // Validaciones por fila (igual que WinForms)
            if (ok)
            {
                foreach (var r in _items)
                {
                    if (string.IsNullOrEmpty(r.UnidadCalculada))
                    { fail = "La unidad calculada en uno de los subcomponentes está vacía."; ok = false; break; }
                    if (r.Medida == 0 && r.UnidadCalculada != "6")
                    { fail = "La selección de medida en uno de los subcomponentes está vacía."; ok = false; break; }
                    if ((r.Elevado == "0" || r.Elevado == "") && r.UnidadCalculada == "6")
                    { fail = "No se han seleccionado datos para la anchura (unidad 'Ambas')."; ok = false; break; }
                }
            }

            if (!ok)
            {
                GlassDialog.Informar(Owner, "Componente", fail);
                return;
            }

            string resul = dto.ExistComponent(TxtCodigo.Text, TxtDescripcion.Text, AcabadoSeleccionado());
            if (resul == "0" || _opc == "Editar")
            {
                Sub_ComponentEspecial[] esp = ChkEspecial.IsChecked == true ? _itemsEsp.ToArray() : new Sub_ComponentEspecial[0];
                resul = dto.SaveComponent(TxtCodigo.Text, TxtDescripcion.Text, ChkEspecial.IsChecked == true,
                    AcabadoSeleccionado(), _opc, _items.ToArray(), esp, resul);
                ClearComponent();
                BloquearCancelar();
                HabilitarEspecial(false);
                GlassDialog.Informar(Owner, "Componente", resul);
            }
            else
            {
                GlassDialog.Informar(Owner, "Componente", "El componente ya existe.");
            }
        }

        // ===== Carga del detalle (port de CargarDataDetalle / Especial) =====
        private void CargarDetalle(DataTable dt)
        {
            _condicionAcabado = "";
            if (dt == null) return;
            foreach (DataRow row in dt.Rows)
            {
                bool adecre = IntOf(row, "Aplica_Decremento") == 1;
                bool extra = IntOf(row, "extra") == 1;
                int cort = IntOf(row, "corte");
                int codMec = IntOf(row, "Cod_Mecanizado");
                _items.Add(new Sub_Component(
                    StrOf(row, "Codigo"), StrOf(row, "Descripcion"),
                    IntOf(row, "Cantidad_Default"), IntOf(row, "Cantidad_Adicional"),
                    StrOf(row, "Id_Unidad_Calculada"), adecre, IntOf(row, "Id_Subcomponente"),
                    StrOf(row, "elevado"), cort, extra, IntOf(row, "Medida"),
                    StrOf(row, "mecanizado"), IntOf(row, "Asignacion_puertas"), codMec));
                AcumularAcabado(StrOf(row, "Codigo"));
            }
        }

        private void CargarDetalleEspecial(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                _itemsEsp.Add(new Sub_ComponentEspecial(
                    StrOf(row, "Codigo"), StrOf(row, "Descripcion"), StrOf(row, "Id_Columna"),
                    IntOf(row, "Cantidad_Default"), IntOf(row, "Cantidad_Adicional"), IntOf(row, "Id_Subcomponente")));
                AcumularAcabado(StrOf(row, "Codigo"));
            }
        }

        // Acumula el código de acabado (parte tras '-') para filtrar el combo Acabado.
        private void AcumularAcabado(string codigoSub)
        {
            if (string.IsNullOrEmpty(codigoSub) || !codigoSub.Contains("-")) return;
            string ac = codigoSub.Split('-')[1].Trim();
            string sep = _condicionAcabado == "" ? "'" : ",'";
            _condicionAcabado += sep + ac + "'";
        }

        // ===== Acciones por fila (menú contextual de la grilla genérica) =====
        private Sub_Component FilaSel()
        {
            var s = GridComponente.SelectedItem as Sub_Component;
            if (s == null) GlassDialog.Informar(Owner, "Componente", "Selecciona primero una fila.");
            return s;
        }

        private void AsignarPuertas_Click(object sender, RoutedEventArgs e)
        {
            var s = FilaSel(); if (s == null) return;
            s.Asignacion_puertas = s.Asignacion_puertas > 0 ? 0 : 1;   // alterna verde
            GridComponente.Items.Refresh();
        }

        private void QuitarMecanizado_Click(object sender, RoutedEventArgs e)
        {
            var s = FilaSel(); if (s == null) return;
            s.Cod_Mecanizado = 0;
            s.Mecanizado = "";
            GridComponente.Items.Refresh();
        }

        private void AsignarMecanizado_Click(object sender, RoutedEventArgs e)
        {
            var s = FilaSel(); if (s == null) return;
            var bsc = new BuscarDialog { Consulta = "Mecan", Owner = Owner };
            bsc.ShowDialog();
            if (bsc.ReturnItem1 == null) return;
            int cod; int.TryParse(bsc.ReturnItem0, out cod);
            s.Cod_Mecanizado = cod;
            s.Mecanizado = bsc.ReturnItem2;
            GridComponente.Items.Refresh();
        }

        private void Anchura_Click(object sender, RoutedEventArgs e)
        {
            var s = FilaSel(); if (s == null) return;
            if (s.UnidadCalculada != "6")
            {
                GlassDialog.Informar(Owner, "Componente",
                    "Los datos de anchura solo aplican cuando la Unidad Calculada es 'Ambas' (6).");
                return;
            }

            var dlg = new View.FrmDataAmbas();
            // Carga valores actuales desde Elevado ("Cant-Adi|X;Apli-Decr|Y")
            try
            {
                if (!string.IsNullOrEmpty(s.Elevado) && s.Elevado != "0")
                {
                    dlg.ReturnItem0 = decimal.Parse(s.Elevado.Split(';')[0].Split('|')[1]);
                    dlg.ReturnItem1 = bool.Parse(s.Elevado.Split(';')[1].Split('|')[1]);
                }
                else { dlg.ReturnItem0 = 30m; dlg.ReturnItem1 = false; }
            }
            catch { dlg.ReturnItem0 = 30m; dlg.ReturnItem1 = false; }

            dlg.ShowDialog();
            s.Elevado = "Cant-Adi|" + dlg.ReturnItem0 + ";Apli-Decr|" + dlg.ReturnItem1;
            LblEstado.Text = "Anchura asignada al subcomponente: " + s.Descripcion;
        }

        private static int IntOf(DataRow r, string col)
        {
            if (!r.Table.Columns.Contains(col)) return 0;
            object v = r[col];
            if (v == null || v == DBNull.Value) return 0;
            int n; return int.TryParse(v.ToString().Trim(), out n) ? n : 0;
        }
        private static string StrOf(DataRow r, string col)
        {
            if (!r.Table.Columns.Contains(col)) return "";
            object v = r[col];
            return v == null || v == DBNull.Value ? "" : v.ToString();
        }
    }

    /// <summary>
    /// Entrada del catálogo de mecanizados para los combos de la grilla. Es un objeto simple (no
    /// un DataRowView) porque la vista de edición necesita filtrarse y la vista por defecto de un
    /// DataView (BindingListCollectionView) no admite Filter.
    /// </summary>
    public sealed class MecItem
    {
        public MecItem(int id, string descripcion)
        {
            Id = id;
            Descripcion = descripcion ?? "";
        }

        public int Id { get; private set; }
        public string Descripcion { get; private set; }

        public override string ToString() { return Descripcion; }

        /// <summary>
        /// Orden alfanumérico natural: los tramos de dígitos se comparan como números, así que
        /// "M2" va antes que "M10". El vacío ("sin mecanizado") queda siempre el primero.
        /// </summary>
        public static int PorDescripcion(MecItem a, MecItem b)
        {
            bool va = string.IsNullOrEmpty(a.Descripcion), vb = string.IsNullOrEmpty(b.Descripcion);
            if (va || vb) return va && vb ? 0 : (va ? -1 : 1);
            return ComparaNatural(a.Descripcion, b.Descripcion);
        }

        private static int ComparaNatural(string x, string y)
        {
            int i = 0, j = 0;
            while (i < x.Length && j < y.Length)
            {
                if (char.IsDigit(x[i]) && char.IsDigit(y[j]))
                {
                    int i0 = i, j0 = j;
                    while (i < x.Length && char.IsDigit(x[i])) i++;
                    while (j < y.Length && char.IsDigit(y[j])) j++;

                    string nx = x.Substring(i0, i - i0).TrimStart('0');
                    string ny = y.Substring(j0, j - j0).TrimStart('0');
                    if (nx.Length != ny.Length) return nx.Length - ny.Length;   // más dígitos = mayor
                    int cn = string.CompareOrdinal(nx, ny);
                    if (cn != 0) return cn;
                }
                else
                {
                    int c = char.ToUpperInvariant(x[i]).CompareTo(char.ToUpperInvariant(y[j]));
                    if (c != 0) return c;
                    i++; j++;
                }
            }
            return (x.Length - i) - (y.Length - j);
        }
    }
}
