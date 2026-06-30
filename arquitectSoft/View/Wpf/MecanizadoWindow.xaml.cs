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
    /// Versión WPF (tema cristal) de FrmMecanizado, centrada en la tabla. Igual que
    /// Acabados, pero el CÓDIGO se auto-genera (MaximoMecanizado) y es de solo lectura;
    /// solo se edita la descripción. Reutiliza Dto.MecanizadoDto.
    /// </summary>
    public partial class MecanizadoWindow : Window
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

        public MecanizadoWindow()
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
                DataTable dt = new Dto.MecanizadoDto().GetMecanizado();   // trae fila sintética (Id 0)
                for (int i = dt.Rows.Count - 1; i >= 0; i--)
                {
                    int id;
                    int.TryParse(Convert.ToString(dt.Rows[i]["Id_mecanizado"]), out id);
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
                filtro += "(Codigo_Homologacion LIKE '%" + partes[i] + "%' OR Descripcion LIKE '%" + partes[i] + "%')";
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
                case "Id_mecanizado":
                    e.Cancel = true;
                    break;
                case "Codigo_Homologacion":
                    e.Column.Header = "Código";
                    e.Column.Width = 130;
                    e.Column.IsReadOnly = true;   // el código se auto-genera, no se edita
                    break;
                case "Descripcion":
                    e.Column.Header = "Descripción";
                    e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                    break;
                default:
                    e.Cancel = true;
                    break;
            }
        }

        // ===== Nuevo: línea editable con código auto-generado =====
        private void Nuevo_Click(object sender, RoutedEventArgs e)
        {
            if (_tabla == null) return;
            TxtFiltro.Text = "";

            string codigo;
            try { codigo = new Dto.MecanizadoDto().MaximoMecanizado(); }
            catch (Exception ex) { LblEstado.Text = "No se pudo generar el código: " + ex.Message; return; }

            DataRow fila = _tabla.NewRow();
            fila["Id_mecanizado"] = 0;
            fila["Codigo_Homologacion"] = codigo;
            fila["Descripcion"] = "";
            _tabla.Rows.Add(fila);

            var drv = BuscarDrv(fila);
            if (drv != null)
            {
                GridDatos.SelectedItem = drv;
                GridDatos.ScrollIntoView(drv);
                // Edita directamente la columna Descripción (índice 1: Código está oculto el Id).
                if (GridDatos.Columns.Count > 1)
                {
                    GridDatos.CurrentCell = new DataGridCellInfo(drv, GridDatos.Columns[1]);
                    GridDatos.BeginEdit();
                }
            }
            LblEstado.Text = "Código " + codigo + " asignado. Escribe la descripción; al salir de la fila se guarda.";
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
            int.TryParse(Convert.ToString(drv.Row["Id_mecanizado"]), out id);
            string codigo = Convert.ToString(drv.Row["Codigo_Homologacion"]);

            if (id == 0)
            {
                drv.Row.Delete();
                _tabla.AcceptChanges();
                return;
            }

            if (!GlassDialog.Pregunta(this, "Mecanizados",
                "¿Seguro que quieres eliminar el mecanizado \"" + codigo + "\"?")) return;

            string resul = new Dto.MecanizadoDto().DeleteMecanizado(id);
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
            string codigo = Convert.ToString(row["Codigo_Homologacion"]).Trim();
            string desc = Convert.ToString(row["Descripcion"]).Trim();
            int id;
            int.TryParse(Convert.ToString(row["Id_mecanizado"]), out id);

            if (id == 0 && desc == "") return;   // fila nueva sin tocar

            if (desc == "")
            {
                LblEstado.Text = "La descripción es obligatoria.";
                return;
            }

            var dto = new Dto.MecanizadoDto();
            string resul;
            if (id == 0)
            {
                string existe = dto.ExistMecanizado(codigo);
                if (existe != "0")
                {
                    GlassDialog.Informar(this, "Mecanizados", "Ese código ya existe.");
                    CargarLista();
                    return;
                }
                resul = dto.SaveMecanizado(codigo, desc, "Nuevo", "0");
            }
            else
            {
                resul = dto.SaveMecanizado(codigo, desc, "Editar", id.ToString());
            }

            CargarLista();
            LblEstado.Text = resul;
        }
    }
}
