using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace arquitectSoft.View.Wpf
{
    /// <summary>
    /// Versión WPF (cristal) de FrmAcercade: muestra los datos del ensamblado
    /// (producto, versión, empresa, copyright, descripción).
    /// </summary>
    public partial class AcercaWindow : Window
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

        public AcercaWindow()
        {
            ScreenCaptureHelper.CaptureFullScreen();
            InitializeComponent();
            System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(this);
            SourceInitialized += OnSourceInitialized;
            LiquidGlass.PrepararOculto(FrameRim, WinScale);
            LiquidGlass.MontarGlass(this, GlassBackdrop);

            LblProducto.Text = AssemblyProduct != "" ? AssemblyProduct : AssemblyTitle;
            LblVersion.Text = "Versión " + AssemblyVersion;
            LblEmpresa.Text = AssemblyCompany;
            LblCopyright.Text = AssemblyCopyright;
            LblDescripcion.Text = AssemblyDescription;

            Loaded += (s, e) => LiquidGlass.Apertura(FrameRim, WinScale);
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

        private void Cerrar_Click(object sender, RoutedEventArgs e) => Close();

        // ===== Atributos del ensamblado (igual que FrmAcercade) =====
        private static string Attr<T>(Func<T, string> pick) where T : Attribute
        {
            object[] at = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(T), false);
            return at.Length == 0 ? "" : pick((T)at[0]);
        }
        public string AssemblyTitle
        {
            get
            {
                string t = Attr<AssemblyTitleAttribute>(a => a.Title);
                return t != "" ? t : System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }
        public string AssemblyVersion { get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }
        public string AssemblyDescription { get { return Attr<AssemblyDescriptionAttribute>(a => a.Description); } }
        public string AssemblyProduct { get { return Attr<AssemblyProductAttribute>(a => a.Product); } }
        public string AssemblyCopyright { get { return Attr<AssemblyCopyrightAttribute>(a => a.Copyright); } }
        public string AssemblyCompany { get { return Attr<AssemblyCompanyAttribute>(a => a.Company); } }
    }
}
