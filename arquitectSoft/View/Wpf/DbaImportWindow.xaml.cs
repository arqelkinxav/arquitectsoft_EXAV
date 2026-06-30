using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace arquitectSoft.View.Wpf
{
    /// <summary>
    /// Versión WPF (cristal) de FrmDBA_Import: importa un .sql a la base y registra
    /// la importación en dbmanagments. Reutiliza Generals.Conexion.ImportBackupMysql.
    /// </summary>
    public partial class DbaImportWindow : Window
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

        private bool _cerrando;
        private string _fileName = "";

        public DbaImportWindow()
        {
            ScreenCaptureHelper.CaptureFullScreen();
            InitializeComponent();
            System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(this);
            SourceInitialized += OnSourceInitialized;
            LiquidGlass.PrepararOculto(FrameRim, WinScale);
            LiquidGlass.MontarGlass(this, GlassBackdrop);
            Loaded += (s, e) => { CargarUltima(); LiquidGlass.Apertura(FrameRim, WinScale); };
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

        private void Cerrar_Click(object sender, RoutedEventArgs e) => Close();

        private void Examinar_Click(object sender, RoutedEventArgs e)
        {
            using (var dlg = new System.Windows.Forms.OpenFileDialog())
            {
                dlg.Filter = "Respaldos SQL (*.sql)|*.sql|Todos los archivos (*.*)|*.*";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var fi = new FileInfo(dlg.FileName);
                    TxtPath.Text = fi.FullName;
                    _fileName = fi.Name.Replace(fi.Extension, "");
                }
            }
        }

        private void Importar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtPath.Text))
            {
                GlassDialog.Informar(this, "Importar", "Debes seleccionar un archivo .sql.");
                return;
            }

            if (!GlassDialog.Pregunta(this, "Importar",
                "Esto REEMPLAZA el contenido actual de la base con el del archivo.\n¿Continuar?")) return;

            try
            {
                LblFecha.Text = "Importando…";
                var con = new Generals.Conexion();
                con.ImportBackupMysql(TxtPath.Text);

                // Registra la importación en dbmanagments.
                string fail = "";
                string[] param = { _fileName };
                con.Open(out fail);
                con.ExecuteReader(Generals.Constantes.QUERY_INSERT_dbmanagmet, out fail, param);
                con.Close();

                CargarUltima();
                GlassDialog.Informar(this, "Importar", "Archivo cargado correctamente.");
            }
            catch (Exception ex)
            {
                GlassDialog.Informar(this, "Importar", "No se pudo importar:\n" + ex.Message);
            }
        }

        private void CargarUltima()
        {
            try
            {
                var con = new Generals.Conexion();
                string fail = "";
                con.Open(out fail);
                DataTable dt = con.ExecuteDataSet(
                    "SELECT filename,created_at FROM `dbmanagments` ORDER BY `created_at` DESC LIMIT 1;", out fail).Tables[0];
                con.Close();
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    LblUltimo.Text = "Archivo cargado: " + Convert.ToString(row["filename"]);
                    LblFecha.Text = "Última actualización local: " + Convert.ToString(row["created_at"]);
                }
            }
            catch { /* tabla aún sin datos */ }
        }
    }
}
