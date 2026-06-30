using arquitectSoft.Class;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace arquitectSoft.View.Wpf
{
    /// <summary>
    /// Versión WPF (cristal) de FrmComponente — FASE 1: editor con encabezado
    /// (Código/Descripción/Especial/Acabado) + dos grillas editables (genérica y
    /// especial) con columnas ComboBox de catálogo, máquina de estados, Buscar
    /// (carga detalle), Agregar subcomponente, Guardar, Eliminar, Nuevo, Cancelar,
    /// Verificar y Duplicar. Reutiliza Dto.ComponenteDto / AcabadoDto y los catálogos.
    /// PENDIENTE Fase 2: diálogos in-cell (anchura "Ambas" / Mecanizado), menú
    /// contextual (Asignación Puertas / Quitar Mecanizado), coloreado de filas.
    /// </summary>
    public partial class ComponenteWindow : Window
    {
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int val, int size);
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        private const int DWMWA_BORDER_COLOR = 34;
        private const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;
        private const int DWMWA_SYSTEMBACKDROP_TYPE = 38;
        private const int DWMWCP_ROUND = 2;
        private const int DWMSBT_TRANSIENTWINDOW = 3;
        private const uint DWMWA_COLOR_NONE = 0xFFFFFFFE;

        private string _opc;
        private string _idComponente = "";
        private string _condicionAcabado = "";
        private bool _cerrando;

        private readonly ObservableCollection<Sub_Component> _items = new ObservableCollection<Sub_Component>();
        private readonly ObservableCollection<Sub_ComponentEspecial> _itemsEsp = new ObservableCollection<Sub_ComponentEspecial>();

        private DataView _dtUnidad, _dtCorte, _dtMedida, _dtColumna;

        public ComponenteWindow()
        {
            ScreenCaptureHelper.CaptureFullScreen();   // foto del escritorio antes de mostrarse
            InitializeComponent();
            System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(this);
            SourceInitialized += OnSourceInitialized;
            StateChanged += (s, e) => BtnMaxGlifo();

            CargarCatalogos();
            ConstruirColumnasGenerico();
            ConstruirColumnasEspecial();
            GridComponente.ItemsSource = _items;
            GridComponenteEsp.ItemsSource = _itemsEsp;

            LiquidGlass.PrepararOculto(FrameRim, WinScale);
            LiquidGlass.MontarGlass(this, GlassBackdrop);
            KeyDown += Window_KeyDown;
            Loaded += (s, e) =>
            {
                CargarAcabados("");
                EstadoInicial();
                LiquidGlass.Apertura(FrameRim, WinScale);
            };
        }

        // Atajos: Esc = cerrar, Ctrl+Z = cancelar, Supr = quitar fila seleccionada.
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) { Close(); e.Handled = true; }
            else if (e.Key == Key.Z && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (BtnCancelar.IsEnabled) { Cancelar_Click(null, null); e.Handled = true; }
            }
            else if (e.Key == Key.Delete)
            {
                if (!GridComponente.IsReadOnly &&
                    (GridComponente.SelectedItem != null || GridComponenteEsp.SelectedItem != null))
                { Borrar_Click(null, null); e.Handled = true; }
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!_cerrando)
            {
                e.Cancel = true;
                _cerrando = true;
                LiquidGlass.Cierre(FrameRim, WinScale, Close);
                return;
            }
            base.OnClosing(e);
        }

        private void OnSourceInitialized(object sender, EventArgs e)
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            try
            {
                int round = DWMWCP_ROUND;
                DwmSetWindowAttribute(hwnd, DWMWA_WINDOW_CORNER_PREFERENCE, ref round, sizeof(int));
                int dark = 1;
                DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref dark, sizeof(int));
                int backdrop = DWMSBT_TRANSIENTWINDOW;
                DwmSetWindowAttribute(hwnd, DWMWA_SYSTEMBACKDROP_TYPE, ref backdrop, sizeof(int));
                int sinBorde = unchecked((int)DWMWA_COLOR_NONE);
                DwmSetWindowAttribute(hwnd, DWMWA_BORDER_COLOR, ref sinBorde, sizeof(int));
            }
            catch { }
            HwndSource src = HwndSource.FromHwnd(hwnd);
            if (src != null && src.CompositionTarget != null)
                src.CompositionTarget.BackgroundColor = Colors.Transparent;
        }

        private void Minimizar_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        private void Maximizar_Click(object sender, RoutedEventArgs e) =>
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        private void Cerrar_Click(object sender, RoutedEventArgs e) => Close();
        private void BtnMaxGlifo() { /* glifo fijo; el filo lo maneja el DWM */ }

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
            g.Columns.Add(Texto("Mecanizado", "Mecanizado", 150, true));
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
            // Sin DisplayMemberPath: el display lo da el ItemTemplate de GridCombo
            // (DisplayMemberPath dejaba "System.Data.DataRowView" en el cuadro cerrado).
            return new DataGridComboBoxColumn
            {
                Header = header,
                Width = width,
                ItemsSource = source,
                SelectedValuePath = valuePath,
                SelectedValueBinding = new Binding(prop),
                ElementStyle = st,
                EditingElementStyle = st
            };
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
            TxtCodigo.IsEnabled = false;
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

            TxtCodigo.IsEnabled = opcion != "Editar";   // en edición el código no se toca
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
            TxtCodigo.IsEnabled = false;
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
            var bsc = new BuscarDialog { Owner = this };   // sin Consulta = componentes
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
            string resul = new Dto.ComponenteDto().ExistComponent(TxtCodigo.Text, TxtDescripcion.Text, AcabadoSeleccionado());
            GlassDialog.Informar(this, "Componente",
                resul != "0" ? "El componente ya existe." : "Componente disponible para guardar.");
        }

        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            if (!GlassDialog.Pregunta(this, "Componente", "¿Seguro que quieres eliminar el registro?")) return;
            var dto = new Dto.ComponenteDto();
            string resul = dto.ExistComponent(TxtCodigo.Text, TxtDescripcion.Text, AcabadoSeleccionado());
            int id;
            if (!int.TryParse(resul, out id))
            {
                GlassDialog.Informar(this, "Componente", "No se pudo identificar el registro: " + resul);
                return;
            }
            resul = dto.DeleteComponent(id);
            BloquearCancelar();
            ClearComponent();
            HabilitarEspecial(false);
            GlassDialog.Informar(this, "Componente", resul);
        }

        private void Agregar_Click(object sender, RoutedEventArgs e)
        {
            var bsc = new BuscarDialog { Consulta = "SubComp", Owner = this };
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
                GlassDialog.Informar(this, "Componente", "Primero marca que es un componente Especial.");
            }
        }

        private void Borrar_Click(object sender, RoutedEventArgs e)
        {
            var sg = GridComponente.SelectedItem as Sub_Component;
            var se = GridComponenteEsp.SelectedItem as Sub_ComponentEspecial;
            if (sg == null && se == null)
            {
                GlassDialog.Informar(this, "Componente", "Selecciona una fila para quitar.");
                return;
            }
            if (sg != null && GlassDialog.Pregunta(this, "Componente", "¿Quitar el subcomponente \"" + sg.Descripcion + "\"?"))
                _items.Remove(sg);
            if (se != null && GlassDialog.Pregunta(this, "Componente", "¿Quitar el subcomponente especial \"" + se.Descripcion + "\"?"))
                _itemsEsp.Remove(se);
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
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
                    { fail = "No se han seleccionado datos para la anchura (unidad 'Ambas'). [Disponible en Fase 2]"; ok = false; break; }
                }
            }

            if (!ok)
            {
                GlassDialog.Informar(this, "Componente", fail);
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
                GlassDialog.Informar(this, "Componente", resul);
            }
            else
            {
                GlassDialog.Informar(this, "Componente", "El componente ya existe.");
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

        // ===== FASE 2: acciones por fila (menú contextual de la grilla genérica) =====
        private Sub_Component FilaSel()
        {
            var s = GridComponente.SelectedItem as Sub_Component;
            if (s == null) GlassDialog.Informar(this, "Componente", "Selecciona primero una fila.");
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
            var bsc = new BuscarDialog { Consulta = "Mecan", Owner = this };
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
                GlassDialog.Informar(this, "Componente",
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

    /// <summary>Pinta de verde translúcido la fila cuyo subcomponente tiene Asignación de Puertas (>0).</summary>
    public sealed class PuertaABrushConverter : System.Windows.Data.IValueConverter
    {
        private static readonly Brush Verde = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3D4CAF50"));
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int n;
            if (value != null && int.TryParse(value.ToString(), out n) && n > 0) return Verde;
            return Brushes.Transparent;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return System.Windows.Data.Binding.DoNothing;
        }
    }
}
