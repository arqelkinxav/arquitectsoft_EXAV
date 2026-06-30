using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace arquitectSoft.View.Wpf
{
    /// <summary>
    /// Diálogo modal con el mismo tema "cristal oscuro" que la ventana de análisis.
    /// Reemplaza los MessageBox de WinForms (pregunta/aviso) y sirve además para
    /// pedir el acabado origen/destino al "Cambiar Acabado".
    /// </summary>
    public partial class GlassDialog : Window
    {
        // ---- DWM: esquinas redondeadas + fondo acrílico (cristal) ----
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int val, int size);
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        private const int DWMWA_BORDER_COLOR = 34;
        private const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;
        private const int DWMWA_SYSTEMBACKDROP_TYPE = 38;
        private const int DWMWCP_ROUND = 2;
        private const int DWMSBT_TRANSIENTWINDOW = 3; // acrílico
        private const uint DWMWA_COLOR_NONE = 0xFFFFFFFE; // sin borde

        public bool Aceptado { get; private set; }
        public string Acabado1 { get; private set; }
        public string Acabado2 { get; private set; }

        public GlassDialog()
        {
            InitializeComponent();
            SourceInitialized += OnSourceInitialized;
            LiquidGlass.PrepararOculto(FrameRim, WinScale);
            Loaded += (s, e) => LiquidGlass.Apertura(FrameRim, WinScale);
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
            catch { /* Windows 10 o anterior: sin acrílico, queda el tinte negro */ }

            HwndSource src = HwndSource.FromHwnd(hwnd);
            if (src != null && src.CompositionTarget != null)
                src.CompositionTarget.BackgroundColor = Colors.Transparent;
        }

        private void Cabecera_Drag(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed) DragMove();
        }

        private void Primario_Click(object sender, RoutedEventArgs e)
        {
            Aceptado = true;
            Acabado1 = TxtAcabado1.Text;
            Acabado2 = TxtAcabado2.Text;
            DialogResult = true;
        }

        private void Secundario_Click(object sender, RoutedEventArgs e)
        {
            Aceptado = false;
            DialogResult = false;
        }

        // ===== Selección de acabado desde el catálogo (igual que FrmChange legacy) =====
        private void Buscar1_Click(object sender, RoutedEventArgs e) => ElegirAcabado(TxtAcabado1);
        private void Buscar2_Click(object sender, RoutedEventArgs e) => ElegirAcabado(TxtAcabado2);

        private void ElegirAcabado(System.Windows.Controls.TextBox destino)
        {
            var bsc = new BuscarDialog { Consulta = "Acaba", Owner = this };
            if (bsc.ShowDialog() != true) return;   // canceló
            destino.Text = bsc.ReturnItem2;         // "CÓDIGO - DESCRIPCIÓN"
            destino.Focus();
            destino.CaretIndex = destino.Text.Length;
        }

        // ===== Helpers estáticos =====

        /// <summary>Pregunta Sí/No. Devuelve true si el usuario elige "Sí".</summary>
        public static bool Pregunta(Window owner, string titulo, string mensaje,
                                    string si = "Sí", string no = "No")
        {
            var d = Crear(owner, titulo, mensaje);
            d.BtnPrimario.Content = si;
            d.BtnSecundario.Content = no;
            return d.ShowDialog() == true;
        }

        /// <summary>Aviso simple con un solo botón "Entendido".</summary>
        public static void Informar(Window owner, string titulo, string mensaje)
        {
            var d = Crear(owner, titulo, mensaje);
            d.BtnPrimario.Content = "Entendido";
            d.BtnSecundario.Visibility = Visibility.Collapsed;
            d.ShowDialog();
        }

        /// <summary>
        /// Pide acabado origen/destino. Devuelve true si se aceptó; los valores
        /// quedan en Acabado1/Acabado2 del diálogo (out por parámetros).
        /// </summary>
        public static bool PedirAcabado(Window owner, out string acabado1, out string acabado2,
                                        string acabadoActual = "")
        {
            var d = Crear(owner, "Cambiar Acabado",
                "Indica el acabado a buscar y el acabado por el que se reemplazará en todos los resultados.");
            d.PanelInput.Visibility = Visibility.Visible;
            d.TxtAcabado1.Text = acabadoActual ?? "";
            d.BtnPrimario.Content = "Aplicar";
            d.Loaded += (s, e) =>
            {
                if (string.IsNullOrEmpty(d.TxtAcabado1.Text)) d.TxtAcabado1.Focus();
                else d.TxtAcabado2.Focus();
            };
            bool ok = d.ShowDialog() == true;
            acabado1 = ok ? d.Acabado1 : null;
            acabado2 = ok ? d.Acabado2 : null;
            return ok;
        }

        private static GlassDialog Crear(Window owner, string titulo, string mensaje)
        {
            var d = new GlassDialog();
            if (owner != null && owner.IsLoaded) d.Owner = owner;
            d.LblTitulo.Text = titulo;
            d.LblMensaje.Text = mensaje;
            return d;
        }
    }
}
