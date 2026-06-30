using arquitectSoft.View.Wpf.Shaders;
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
    /// Versión WPF (tema cristal) de FrmAcabados, rediseñada en torno a la tabla:
    /// se ve el listado completo, "Nuevo" añade una línea editable en la misma
    /// tabla, y se edita/elimina inline. El filtro va por letras en cualquier orden.
    /// Reutiliza Dto.AcabadoDto (capa de datos intacta).
    /// </summary>
    public partial class AcabadosWindow : Window
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
        private bool _recargando;   // evita reentradas durante el refresco
        private bool _cerrando;     // evita reentrar al cerrar mientras corre la animación

        public AcabadosWindow()
        {
            // Foto del escritorio ANTES de mostrar la ventana (si no, saldría dentro de
            // su propio fondo). Una sola captura; al mover/redimensionar solo se recorta.
            ScreenCaptureHelper.CaptureFullScreen();

            InitializeComponent();
            // La ventana se abre modeless desde el bucle de mensajes de WinForms;
            // sin esto, los TextBox de WPF no reciben las pulsaciones de teclado.
            System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(this);
            SourceInitialized += OnSourceInitialized;
            LiquidGlass.PrepararOculto(FrameRim, WinScale);
            LiquidGlass.MontarGlass(this, GlassBackdrop);   // fondo cristal real (captura + refracción)
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
            catch { /* Windows 10 o anterior */ }

            HwndSource src = HwndSource.FromHwnd(hwnd);
            if (src != null && src.CompositionTarget != null)
                src.CompositionTarget.BackgroundColor = Colors.Transparent;
        }

        private void Minimizar_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        private void Cerrar_Click(object sender, RoutedEventArgs e) => Close();

        // ===== Carga / refresco de la tabla =====
        private void CargarLista()
        {
            _recargando = true;
            try
            {
                DataTable dt = new Dto.AcabadoDto().GetAcabado();   // trae fila sintética "(Seleccione)"
                for (int i = dt.Rows.Count - 1; i >= 0; i--)
                {
                    int id;
                    int.TryParse(Convert.ToString(dt.Rows[i]["Id_Acabado"]), out id);
                    if (id == 0) { dt.Rows[i].Delete(); continue; }   // quita la fila "(Seleccione)"

                    // GetAcabado() devuelve Descripcion = CONCAT(Codigo,' - ',Descripcion).
                    // Quitamos el prefijo "código - " para mostrar/editar SOLO la descripción
                    // real; si no, al guardar se volvería a concatenar y se duplicaría el código.
                    string codigo = Convert.ToString(dt.Rows[i]["Codigo_Homologacion"]);
                    string desc = Convert.ToString(dt.Rows[i]["Descripcion"]);
                    string prefijo = codigo + " - ";
                    if (desc.StartsWith(prefijo)) desc = desc.Substring(prefijo.Length);
                    dt.Rows[i]["Descripcion"] = desc;
                }
                dt.AcceptChanges();

                _tabla = dt;
                _vista = dt.DefaultView;
                AplicarFiltro();
                GridAcabados.ItemsSource = _vista;
            }
            catch (Exception ex)
            {
                LblEstado.Text = "No se pudo cargar la lista: " + ex.Message;
            }
            finally { _recargando = false; }
        }

        // Filtro por LETRAS en cualquier orden: cada palabra debe aparecer (en código
        // o descripción), sin importar el orden en que se escriban.
        private void AplicarFiltro()
        {
            if (_vista == null) return;
            string txt = (TxtFiltro.Text ?? "").Replace("'", "").Trim();
            if (txt == "")
            {
                _vista.RowFilter = "";
                return;
            }

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

        private void GridAcabados_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Id_Acabado":
                    e.Cancel = true;   // columna interna oculta
                    break;
                case "Codigo_Homologacion":
                    e.Column.Header = "Código";
                    e.Column.Width = 130;
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

        // ===== Nuevo: añade una línea editable en la propia tabla =====
        private void Nuevo_Click(object sender, RoutedEventArgs e)
        {
            if (_tabla == null) return;
            TxtFiltro.Text = "";   // limpia el filtro para que la fila nueva sea visible

            DataRow fila = _tabla.NewRow();
            fila["Id_Acabado"] = 0;            // 0 = aún no guardada
            fila["Codigo_Homologacion"] = "";
            fila["Descripcion"] = "";
            _tabla.Rows.Add(fila);

            var drv = BuscarDrv(fila);
            if (drv != null)
            {
                GridAcabados.SelectedItem = drv;
                GridAcabados.ScrollIntoView(drv);
                if (GridAcabados.Columns.Count > 0)
                {
                    GridAcabados.CurrentCell = new DataGridCellInfo(drv, GridAcabados.Columns[0]);
                    GridAcabados.BeginEdit();
                }
            }
            LblEstado.Text = "Escribe el código y la descripción; al salir de la fila se guarda.";
        }

        private DataRowView BuscarDrv(DataRow fila)
        {
            foreach (DataRowView drv in _vista)
                if (drv.Row == fila) return drv;
            return null;
        }

        // ===== Eliminar la fila seleccionada =====
        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            var drv = GridAcabados.SelectedItem as DataRowView;
            if (drv == null)
            {
                LblEstado.Text = "Selecciona primero una fila para eliminar.";
                return;
            }

            int id;
            int.TryParse(Convert.ToString(drv.Row["Id_Acabado"]), out id);
            string codigo = Convert.ToString(drv.Row["Codigo_Homologacion"]);

            if (id == 0)
            {
                // Fila nueva sin guardar: basta con quitarla.
                drv.Row.Delete();
                _tabla.AcceptChanges();
                return;
            }

            if (!GlassDialog.Pregunta(this, "Acabados",
                "¿Seguro que quieres eliminar el acabado \"" + codigo + "\"?")) return;

            string resul = new Dto.AcabadoDto().DeleteAcabado(id);
            CargarLista();
            LblEstado.Text = resul;
        }

        // ===== Persistencia al terminar de editar una fila =====
        private void GridAcabados_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (_recargando || e.EditAction != DataGridEditAction.Commit) return;
            var drv = e.Row.Item as DataRowView;
            if (drv == null) return;

            // Diferido para que el commit de la fila llegue a la DataRow antes de leerla.
            Dispatcher.BeginInvoke(new Action(() => Persistir(drv)), DispatcherPriority.Background);
        }

        private void Persistir(DataRowView drv)
        {
            if (_recargando || drv == null || drv.Row.RowState == DataRowState.Detached) return;

            DataRow row = drv.Row;
            string codigo = Convert.ToString(row["Codigo_Homologacion"]).Trim();
            string desc = Convert.ToString(row["Descripcion"]).Trim();
            int id;
            int.TryParse(Convert.ToString(row["Id_Acabado"]), out id);

            // Fila nueva totalmente vacía: ignorar.
            if (id == 0 && codigo == "" && desc == "") return;

            if (codigo == "" || desc == "")
            {
                LblEstado.Text = "Código y descripción son obligatorios.";
                return;
            }

            var dto = new Dto.AcabadoDto();
            string resul;
            if (id == 0)
            {
                // Alta: comprobar que no exista ya.
                string existe = dto.ExistAcabado(codigo, desc);
                if (existe != "0")
                {
                    GlassDialog.Informar(this, "Acabados", "Ese acabado ya existe.");
                    CargarLista();
                    return;
                }
                resul = dto.SaveAcabado(codigo, desc, "Nuevo", "0");
            }
            else
            {
                // Edición: el DTO actualiza la descripción del registro por Id.
                resul = dto.SaveAcabado(codigo, desc, "Editar", id.ToString());
            }

            CargarLista();
            LblEstado.Text = resul;
        }
    }
}
