using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using arquitectSoft.Engine;

namespace arquitectSoft.View.Wpf
{
    /// <summary>Un bloque del informe ya listo para pintar.</summary>
    public class BloqueVista
    {
        public string Titulo { get; set; }
        public string Texto { get; set; }
        public Brush Color { get; set; }
    }

    /// <summary>
    /// Enseña, ANTES de importar, que es lo que no cuadra entre el archivo de respaldo y la
    /// base a la que esta conectado el programa: usuarios y contraseñas que se van, componentes
    /// que se quedan por el camino, reglas de vidrio que cambian, y el conteo de filas por tabla.
    ///
    /// El import es un reemplazo tabla a tabla y no perdona: lo que hay aqui y no viene en el
    /// archivo desaparece. Esta ventana existe para poder decidirlo con los datos delante, en
    /// vez de enterarse despues. Por eso el boton por defecto es Cancelar.
    /// </summary>
    public partial class RevisionImportDialog : Window
    {
        // ---- DWM: esquinas redondeadas + fondo acrílico (cristal) ----
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int val, int size);
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        private const int DWMWA_BORDER_COLOR = 34;
        private const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;
        private const int DWMWA_SYSTEMBACKDROP_TYPE = 38;
        private const int DWMWCP_ROUND = 2;
        private const int DWMSBT_TRANSIENTWINDOW = 3;
        private const uint DWMWA_COLOR_NONE = 0xFFFFFFFE;

        private static readonly Brush ROJO = new SolidColorBrush(Color.FromRgb(0xE0, 0x6C, 0x6C));
        private static readonly Brush AMBAR = new SolidColorBrush(Color.FromRgb(0xE0, 0xA9, 0x5B));
        private static readonly Brush VERDE = new SolidColorBrush(Color.FromRgb(0x7F, 0xB8, 0x8A));
        private static readonly Brush GRIS = new SolidColorBrush(Color.FromRgb(0x8C, 0x8C, 0x8C));

        public RevisionImportDialog()
        {
            InitializeComponent();
            SourceInitialized += OnSourceInitialized;
            LiquidGlass.PrepararOculto(FrameRim, WinScale);
            Loaded += (s, e) => LiquidGlass.Apertura(FrameRim, WinScale);
        }

        /// <summary>Vuelca el informe en la ventana. <paramref name="archivo"/> es solo para la cabecera.</summary>
        public void Cargar(InformeImport inf, string archivo)
        {
            LblOrigen.Text = "Archivo: " + archivo + "\nBase de destino: " + Generals.Conexion.Destino;

            if (!string.IsNullOrEmpty(inf.Error))
            {
                Pintar(ROJO, inf.Error);
                BtnImportar.IsEnabled = false;
                return;
            }

            Pintar(inf.HayPerdidas ? ROJO : (inf.Cambios > 0 || inf.Altas > 0 ? AMBAR : VERDE), inf.Titular);

            if (inf.HayPerdidas) BtnImportar.Content = "Importar y perder esas filas";

            var vistas = new List<BloqueVista>();
            foreach (BloqueInforme b in inf.Bloques)
                vistas.Add(new BloqueVista
                {
                    Titulo = b.Titulo,
                    Texto = string.Join("\n", b.Lineas),
                    Color = ColorDe(b.Nivel)
                });

            ListaBloques.ItemsSource = vistas;
        }

        private void Pintar(Brush color, string texto)
        {
            LblTitular.Text = texto;
            LblTitular.Foreground = color;
            CajaTitular.BorderBrush = color;
        }

        private static Brush ColorDe(NivelDif n)
        {
            switch (n)
            {
                case NivelDif.Perdida: return ROJO;
                case NivelDif.Cambio: return AMBAR;
                case NivelDif.Alta: return VERDE;
                default: return GRIS;
            }
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

        private void Importar_Click(object sender, RoutedEventArgs e) { DialogResult = true; }

        private void Cancelar_Click(object sender, RoutedEventArgs e) { DialogResult = false; }
    }
}
