using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace arquitectSoft.View.Wpf
{
    /// <summary>
    /// Versión WPF (cristal) de FrmDBA: genera un respaldo .sql de la base en la
    /// carpeta elegida. Reutiliza Generals.Conexion.ExportBackupMysql.
    /// </summary>
    public partial class DbaBackupWindow : Window
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

        public DbaBackupWindow()
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

        private void Cerrar_Click(object sender, RoutedEventArgs e) => Close();

        private void Examinar_Click(object sender, RoutedEventArgs e)
        {
            using (var dlg = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    TxtPath.Text = dlg.SelectedPath;
            }
        }

        private void Backup_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtPath.Text))
            {
                GlassDialog.Informar(this, "Respaldo", "Debes seleccionar una carpeta destino.");
                return;
            }

            try
            {
                const string database = "arquitectdb";
                string fileName = database + "_backup_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".sql";
                string backupFilePath = Path.Combine(TxtPath.Text, fileName);

                LblEstado.Text = "Generando respaldo…";
                var con = new Generals.Conexion();
                string result = con.ExportBackupMysql(backupFilePath);

                // Header necesario para reimportar (funciones/triggers).
                string header = "SET GLOBAL log_bin_trust_function_creators = 1;\n";
                string existing = File.ReadAllText(backupFilePath);
                File.WriteAllText(backupFilePath, header + existing);

                LblEstado.Text = "Respaldo creado: " + fileName;
                GlassDialog.Informar(this, "Respaldo", result);
            }
            catch (Exception ex)
            {
                LblEstado.Text = "Error al respaldar.";
                GlassDialog.Informar(this, "Respaldo", "No se pudo crear el respaldo:\n" + ex.Message);
            }
        }
    }
}
