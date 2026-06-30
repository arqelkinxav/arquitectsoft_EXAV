using System;
using System.ComponentModel;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace arquitectSoft.View.Wpf
{
    /// <summary>
    /// Versión WPF (tema cristal) de FrmUnidadMedida, patrón CRUD-tabla. Campos:
    /// Código (= Id_Unidad_Medida, auto-incremental, solo lectura), Descripción y
    /// Convención (ambas editables). Reutiliza Dto.UnidadMedidaDto.
    /// </summary>
    public partial class UnidadMedidaWindow : Window
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

        private DataTable _tabla;
        private DataView _vista;
        private bool _recargando;
        private bool _cerrando;

        public UnidadMedidaWindow()
        {
            ScreenCaptureHelper.CaptureFullScreen();   // foto del escritorio antes de mostrarse
            InitializeComponent();
            System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(this);
            SourceInitialized += OnSourceInitialized;
            LiquidGlass.PrepararOculto(FrameRim, WinScale);
            LiquidGlass.MontarGlass(this, GlassBackdrop);
            Loaded += (s, e) => { CargarLista(); LiquidGlass.Apertura(FrameRim, WinScale); };
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
        private void Cerrar_Click(object sender, RoutedEventArgs e) => Close();

        // ===== Carga / refresco =====
        private void CargarLista()
        {
            _recargando = true;
            try
            {
                DataTable dt = new Dto.UnidadMedidaDto().GetUnidadMedida();   // trae fila sintética (Id 0)
                for (int i = dt.Rows.Count - 1; i >= 0; i--)
                {
                    int id;
                    int.TryParse(Convert.ToString(dt.Rows[i]["Id_Unidad_Medida"]), out id);
                    if (id == 0) dt.Rows[i].Delete();
                }
                dt.AcceptChanges();

                _tabla = dt;
                _vista = dt.DefaultView;
                AplicarFiltro();
                GridDatos.ItemsSource = _vista;
            }
            catch (Exception ex)
            {
                LblEstado.Text = "No se pudo cargar la lista: " + ex.Message;
            }
            finally { _recargando = false; }
        }

        private void AplicarFiltro()
        {
            if (_vista == null) return;
            string txt = (TxtFiltro.Text ?? "").Replace("'", "").Trim();
            if (txt == "") { _vista.RowFilter = ""; return; }

            var partes = txt.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string filtro = "";
            for (int i = 0; i < partes.Length; i++)
            {
                if (i > 0) filtro += " AND ";
                filtro += "(Descripcion LIKE '%" + partes[i] + "%'"
                        + " OR Convencion LIKE '%" + partes[i] + "%'"
                        + " OR Convert(Id_Unidad_Medida, 'System.String') LIKE '%" + partes[i] + "%')";
            }
            _vista.RowFilter = filtro;
        }

        private void Filtro_Changed(object sender, TextChangedEventArgs e)
        {
            if (_recargando) return;
            AplicarFiltro();
        }

        private void GridDatos_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Id_Unidad_Medida":
                    e.Column.Header = "Código";
                    e.Column.Width = 110;
                    e.Column.IsReadOnly = true;   // auto-incremental, no se edita
                    break;
                case "Descripcion":
                    e.Column.Header = "Descripción";
                    e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                    break;
                case "Convencion":
                    e.Column.Header = "Convención";
                    e.Column.Width = 150;
                    break;
                default:
                    e.Cancel = true;
                    break;
            }
        }

        // ===== Nuevo: línea editable; el código real lo asigna la BD al guardar =====
        private void Nuevo_Click(object sender, RoutedEventArgs e)
        {
            if (_tabla == null) return;
            TxtFiltro.Text = "";

            string codigo;
            try { codigo = new Dto.UnidadMedidaDto().MaximaUnidadMedida(); }
            catch (Exception ex) { LblEstado.Text = "No se pudo generar el código: " + ex.Message; return; }

            DataRow fila = _tabla.NewRow();
            fila["Id_Unidad_Medida"] = 0;
            fila["Descripcion"] = "";
            fila["Convencion"] = "";
            _tabla.Rows.Add(fila);

            var drv = BuscarDrv(fila);
            if (drv != null)
            {
                GridDatos.SelectedItem = drv;
                GridDatos.ScrollIntoView(drv);
                if (GridDatos.Columns.Count > 1)
                {
                    GridDatos.CurrentCell = new DataGridCellInfo(drv, GridDatos.Columns[1]);
                    GridDatos.BeginEdit();
                }
            }
            LblEstado.Text = "Código " + codigo + " (estimado). Escribe la descripción y la convención; al salir de la fila se guarda.";
        }

        private DataRowView BuscarDrv(DataRow fila)
        {
            foreach (DataRowView drv in _vista)
                if (drv.Row == fila) return drv;
            return null;
        }

        // ===== Eliminar =====
        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            var drv = GridDatos.SelectedItem as DataRowView;
            if (drv == null) { LblEstado.Text = "Selecciona primero una fila para eliminar."; return; }

            int id;
            int.TryParse(Convert.ToString(drv.Row["Id_Unidad_Medida"]), out id);
            string desc = Convert.ToString(drv.Row["Descripcion"]);

            if (id == 0)
            {
                drv.Row.Delete();
                _tabla.AcceptChanges();
                return;
            }

            if (!GlassDialog.Pregunta(this, "Unidades de Medida",
                "¿Seguro que quieres eliminar la unidad \"" + desc + "\"?")) return;

            string resul = new Dto.UnidadMedidaDto().DeleteUnidadMedida(id);
            CargarLista();
            LblEstado.Text = resul;
        }

        // ===== Persistencia al terminar de editar =====
        private void GridDatos_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (_recargando || e.EditAction != DataGridEditAction.Commit) return;
            var drv = e.Row.Item as DataRowView;
            if (drv == null) return;
            Dispatcher.BeginInvoke(new Action(() => Persistir(drv)), DispatcherPriority.Background);
        }

        private void Persistir(DataRowView drv)
        {
            if (_recargando || drv == null || drv.Row.RowState == DataRowState.Detached) return;

            DataRow row = drv.Row;
            string desc = Convert.ToString(row["Descripcion"]).Trim();
            string conv = Convert.ToString(row["Convencion"]).Trim();
            int id;
            int.TryParse(Convert.ToString(row["Id_Unidad_Medida"]), out id);

            if (id == 0 && desc == "" && conv == "") return;   // fila nueva sin tocar

            if (desc == "" || conv == "")
            {
                LblEstado.Text = "La descripción y la convención son obligatorias.";
                return;
            }

            var dto = new Dto.UnidadMedidaDto();
            string resul;
            if (id == 0)
                resul = dto.SaveUnidadMedida("0", desc, conv, "Nuevo", "0");   // la BD asigna el Id
            else
                resul = dto.SaveUnidadMedida(id.ToString(), desc, conv, "Editar", id.ToString());

            CargarLista();
            LblEstado.Text = resul;
        }
    }
}
