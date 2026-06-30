using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace arquitectSoft.View.Wpf
{
    /// <summary>
    /// Versión WPF (tema cristal) del FrmLoading de exportación: recoge nº de
    /// proyecto, nombre, técnico, fecha, acabados y las categorías del albarán.
    /// Expone las MISMAS propiedades que FrmLoading para no tocar el exportador.
    /// </summary>
    public partial class ExportDialog : Window
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

        public string Fecha { get; private set; }
        public string Numero { get; private set; }
        public string Nombre { get; private set; }
        public string Tecnico { get; private set; }
        public string Acabado1 { get; private set; }
        public string Acabado2 { get; private set; }
        public string Albaran { get; private set; }

        public ExportDialog()
        {
            InitializeComponent();
            TxtFecha.Text = DateTime.Now.ToString("yyyy-MM-dd");
            SourceInitialized += OnSourceInitialized;
            LiquidGlass.PrepararOculto(FrameRim, WinScale);
            Loaded += (s, e) => { LiquidGlass.Apertura(FrameRim, WinScale); TxtNumero.Focus(); };
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

        private void Cabecera_Drag(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed) DragMove();
        }

        private void BuscarPerfil_Click(object sender, RoutedEventArgs e) => ElegirAcabado(TxtAcabado1);
        private void BuscarMelamina_Click(object sender, RoutedEventArgs e) => ElegirAcabado(TxtAcabado2);

        private void ElegirAcabado(TextBox destino)
        {
            var bsc = new BuscarDialog { Consulta = "Acaba", Owner = this };
            if (bsc.ShowDialog() != true) return;
            destino.Text = bsc.ReturnItem2;
        }

        private void Exportar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNumero.Text)) { Error("Debes indicar un número de proyecto."); return; }
            if (string.IsNullOrWhiteSpace(TxtNombre.Text)) { Error("Debes indicar un nombre de proyecto."); return; }

            Numero = TxtNumero.Text.Trim();
            Nombre = TxtNombre.Text.Trim();
            Tecnico = TxtTecnico.Text;
            Acabado1 = TxtAcabado1.Text;
            Acabado2 = TxtAcabado2.Text;
            Fecha = TxtFecha.Text;

            // Albarán: índices marcados unidos por "|", igual que el CheckedListBox legacy
            // (0 = Perfiles Metálicos, 1 = Vidrios y Paneles, 2 = Puertas).
            var indices = new List<string>();
            if (ChkPerfiles.IsChecked == true) indices.Add("0");
            if (ChkVidrios.IsChecked == true) indices.Add("1");
            if (ChkPuertas.IsChecked == true) indices.Add("2");
            Albaran = string.Join("|", indices);

            DialogResult = true;
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            Numero = null;   // señal de cancelación (igual que FrmLoading)
            DialogResult = false;
        }

        private void Error(string msg)
        {
            LblError.Text = msg;
            LblError.Visibility = Visibility.Visible;
        }
    }
}
