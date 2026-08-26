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
    /// Análisis MANUAL de puertas (lo que no cubre la ventana principal): se agregan
    /// puertas por código, se editan altura/anchura/código directamente en la tabla, y
    /// se analiza → Perfilería y Herrajes en pestañas. Medida base y desperdicio NO
    /// aplican en manual (se usan valores por defecto). Reutiliza AnalisisDatosDto.CalculateTab.
    /// </summary>
    public partial class PuertasWindow : Window
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

        private const int MedidaBase = 2960;          // valor por defecto (no aplica en manual)
        private const decimal Desperdicio = 1m;       // factor (0% desperdicio)

        private bool _cerrando;
        private bool _ocultarPrimeraHerraje;
        private string _acabado = "";
        private readonly DataTable _dtAddRows = new DataTable();
        private readonly DataTable _dtPuertas = new DataTable();
        private DataTable _dtPerfil, _dtHerraje;   // resultados, para cambiar acabado

        public PuertasWindow()
        {
            ScreenCaptureHelper.CaptureFullScreen();
            InitializeComponent();
            System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(this);
            SourceInitialized += OnSourceInitialized;
            LiquidGlass.PrepararOculto(FrameRim, WinScale);
            LiquidGlass.MontarGlass(this, GlassBackdrop);
            Loaded += (s, e) => LiquidGlass.Apertura(FrameRim, WinScale);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!_cerrando)
            {
                e.Cancel = true; _cerrando = true;
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
                int round = DWMWCP_ROUND; DwmSetWindowAttribute(hwnd, DWMWA_WINDOW_CORNER_PREFERENCE, ref round, sizeof(int));
                int dark = 1; DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref dark, sizeof(int));
                int backdrop = DWMSBT_TRANSIENTWINDOW; DwmSetWindowAttribute(hwnd, DWMWA_SYSTEMBACKDROP_TYPE, ref backdrop, sizeof(int));
                int sinBorde = unchecked((int)DWMWA_COLOR_NONE); DwmSetWindowAttribute(hwnd, DWMWA_BORDER_COLOR, ref sinBorde, sizeof(int));
            }
            catch { }
            HwndSource src = HwndSource.FromHwnd(hwnd);
            if (src != null && src.CompositionTarget != null) src.CompositionTarget.BackgroundColor = Colors.Transparent;
        }

        private void Minimizar_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        private void Maximizar_Click(object sender, RoutedEventArgs e) =>
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        private void Cerrar_Click(object sender, RoutedEventArgs e) => Close();

        // ===== Buscar código de puerta =====
        private void BuscarCodigo_Click(object sender, RoutedEventArgs e)
        {
            var bsc = new BuscarDialog { Owner = this };
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
                GlassDialog.Informar(this, "Puertas", "Busca un código de puerta primero.");
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
            }

            int n; if (!int.TryParse(TxtCantidad.Text, out n) || n < 1) n = 1;
            for (int i = 0; i < n; i++)
            {
                string nomen = "P" + (_dtAddRows.Rows.Count + 1);
                _dtAddRows.Rows.Add(nomen, TxtCodigo.Text, "", _acabado, TxtDescripcion.Text, "", "", "No", "No", "1");
            }

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
                    e.Cancel = true; break;
                case "Nomenclatura": e.Column.Header = "Nomen."; e.Column.IsReadOnly = true; break;
                case "Codigo": e.Column.Header = "Código"; break;               // editable
                case "Acabado Perfileria Puertas": e.Column.Header = "Acabado"; e.Column.IsReadOnly = true; break;
                case "Item": e.Column.Header = "Descripción"; e.Column.IsReadOnly = true;
                    e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star); break;
                case "Altura": e.Column.Width = 120; break;                     // editable, más ancha
                case "Anchura": e.Column.Width = 120; break;                    // editable, más ancha
            }
        }

        // Al editar el CÓDIGO en la tabla: busca la descripción de ese código; si no existe, avisa.
        private void DgNuevas_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction != DataGridEditAction.Commit) return;
            if (e.Column == null || Convert.ToString(e.Column.Header) != "Código") return;
            var drv = e.Row.Item as DataRowView; if (drv == null) return;
            var tb = e.EditingElement as TextBox;
            string nuevo = tb != null ? tb.Text : Convert.ToString(drv.Row["Codigo"]);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                string desc = DescDeCodigo(nuevo);
                if (desc == null)
                    GlassDialog.Informar(this, "Puertas", "No existe ningún código que coincida. Revisa el código ingresado.");
                else
                    drv.Row["Item"] = desc;
            }), DispatcherPriority.Background);
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
                GlassDialog.Informar(this, "Puertas", "Agrega al menos una puerta antes de analizar.");
                return;
            }
            try
            {
                LblEstado.Text = "Analizando…";
                var dto = new Dto.AnalisisDatosDto();
                DataTable perfil = dto.CalculateTab(3, _dtAddRows, _dtPuertas, false, MedidaBase, Desperdicio, true, 1);
                DataTable herraje = dto.CalculateTab(7, _dtAddRows, _dtPuertas, true, MedidaBase, Desperdicio, true, 1);

                _dtPerfil = perfil; _dtHerraje = herraje;
                DgPerfil.ItemsSource = perfil != null ? perfil.DefaultView : null;
                _ocultarPrimeraHerraje = true;
                DgHerraje.ItemsSource = herraje != null ? herraje.DefaultView : null;
                LblEstado.Text = "Análisis aplicado correctamente.";
            }
            catch (Exception ex)
            {
                LblEstado.Text = "Error al analizar.";
                GlassDialog.Informar(this, "Puertas", "No se pudo analizar:\n" + ex.Message);
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
                GlassDialog.Informar(this, "Puertas", "Primero pulsa Analizar para tener resultados.");
                return;
            }
            string a1, a2;
            if (!GlassDialog.PedirAcabado(this, out a1, out a2)) return;
            if (string.IsNullOrEmpty(a1) || string.IsNullOrEmpty(a2)) return;

            FnChangeInfo(a1, a2);
            DgPerfil.Items.Refresh();
            DgHerraje.Items.Refresh();
            LblEstado.Text = "Acabado reemplazado en los resultados.";
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
            DgNuevas.ItemsSource = null;
            DgPerfil.ItemsSource = null;
            DgHerraje.ItemsSource = null;
            TxtCodigo.Text = ""; TxtDescripcion.Text = ""; TxtCantidad.Text = "1"; _acabado = "";
            LblEstado.Text = "Lista vaciada.";
        }
    }
}
