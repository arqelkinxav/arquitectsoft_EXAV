using arquitectSoft.View.Wpf.Panels;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace arquitectSoft.View.Wpf
{
    /// <summary>
    /// Shell principal en WPF: barra lateral (dock, botones circulares liquid glass) +
    /// canvas "Escritorio" donde se irán hospedando las pantallas como ventanas contenidas.
    /// FASE 1: piloto con el panel "Acerca de"; el resto abre por ahora como ventana flotante.
    /// </summary>
    public partial class EscritorioWindow : Window
    {
        public EscritorioWindow()
        {
            InitializeComponent();
            SourceInitialized += OnSourceInitialized;
            Loaded += (s, e) => CargarFondo();

            // Arrancar centrada en el MONITOR PRINCIPAL (área de trabajo).
            Rect wa = SystemParameters.WorkArea;
            Left = wa.Left + Math.Max(0, (wa.Width - Width) / 2);
            Top = wa.Top + Math.Max(0, (wa.Height - Height) / 2);
        }

        // ===== Barra de título: arrastrar entre monitores / doble clic = maximizar =====
        private void TitleBar_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton != System.Windows.Input.MouseButton.Left) return;
            if (e.ClickCount == 2) { MaximizarApp_Click(null, null); return; }
            try { DragMove(); } catch { }
        }

        private void MaximizarApp_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void OnSourceInitialized(object sender, EventArgs e)
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            HwndSource src = HwndSource.FromHwnd(hwnd);
            if (src != null) src.AddHook(WndProc);
        }

        private void CargarFondo()
        {
            try
            {
                string ruta = Path.Combine(Directory.GetCurrentDirectory(), "FondoApp.png");
                if (File.Exists(ruta))
                {
                    var bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.CacheOption = BitmapCacheOption.OnLoad;
                    bmp.UriSource = new Uri(ruta, UriKind.Absolute);
                    bmp.EndInit();
                    bmp.Freeze();
                    ImgFondo.Source = bmp;
                }
            }
            catch { /* sin fondo: queda el degradado */ }
        }

        // Botón de barra de tareas por cada ventana contenida.
        private readonly System.Collections.Generic.Dictionary<MdiChild, Button> _botonesBarra
            = new System.Collections.Generic.Dictionary<MdiChild, Button>();
        private static readonly System.Windows.Media.Brush AcentoBarra =
            new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0xE0, 0x7B, 0x5B));

        // ===== Abrir una pantalla como ventana contenida en el escritorio =====
        private void AbrirPanel(string titulo, string icono, UIElement contenido, double w, double h)
        {
            var child = new MdiChild { Titulo = titulo, Width = w, Height = h };
            child.SetContenido(contenido);

            // Snap (vista previa) + barra de tareas.
            child.MostrarPreviewSnap = MostrarPreview;
            child.Cerrada += (s, e) => QuitarDeBarra(child);
            child.EstadoCambiado += (s, e) => RefrescarBarra();

            double off = (Lienzo.Children.Count % 6) * 28;
            Canvas.SetLeft(child, 40 + off);
            Canvas.SetTop(child, 28 + off);
            Lienzo.Children.Add(child);

            AgregarABarra(child, icono);
            child.TraerAlFrente();
            child.UpdateLayout();
            child.AjustarAlCanvas();
            RefrescarBarra();
        }

        private void Lienzo_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (UIElement el in Lienzo.Children)
            {
                var c = el as MdiChild;
                if (c != null) c.AjustarAlCanvas();
            }
        }

        // ===== Vista previa de snap =====
        private void MostrarPreview(Rect? zona)
        {
            if (!zona.HasValue) { PreviewSnap.Visibility = Visibility.Collapsed; return; }
            Rect r = zona.Value;
            PreviewSnap.Margin = new Thickness(r.X, r.Y, 0, 0);
            PreviewSnap.Width = r.Width;
            PreviewSnap.Height = r.Height;
            PreviewSnap.Visibility = Visibility.Visible;
        }

        // ===== Barra de tareas =====
        private void AgregarABarra(MdiChild child, string icono)
        {
            var btn = new Button
            {
                Style = (Style)FindResource("TaskButton"),
                Content = child.Titulo,
                Tag = icono,
                BorderBrush = System.Windows.Media.Brushes.Transparent
            };
            btn.Click += (s, e) => child.AlternarDesdeBarra();
            _botonesBarra[child] = btn;
            BarraTareas.Children.Add(btn);
            BarraTareasHost.Visibility = Visibility.Visible;
        }

        private void QuitarDeBarra(MdiChild child)
        {
            Button btn;
            if (_botonesBarra.TryGetValue(child, out btn))
            {
                BarraTareas.Children.Remove(btn);
                _botonesBarra.Remove(child);
            }
            if (_botonesBarra.Count == 0) BarraTareasHost.Visibility = Visibility.Collapsed;
            else RefrescarBarra();
        }

        // Resalta el botón de la ventana frontal y atenúa el de las minimizadas.
        private void RefrescarBarra()
        {
            foreach (var kv in _botonesBarra)
            {
                MdiChild c = kv.Key; Button b = kv.Value;
                bool activa = !c.EstaMinimizada && c.EsFrontal();
                b.BorderBrush = activa ? AcentoBarra : System.Windows.Media.Brushes.Transparent;
                b.Opacity = c.EstaMinimizada ? 0.55 : 1.0;
            }
        }

        // ===== Dock: piloto contenido =====
        private void Acerca_Click(object sender, RoutedEventArgs e) =>
            AbrirPanel("Acerca de", "", new AcercaPanel(), 500, 380);

        // ===== Dock: resto (por ahora como ventana flotante; Fase 2 las contiene) =====
        private void Analisis_Click(object sender, RoutedEventArgs e) => new AnalisisWindow().Show();
        private void Puertas_Click(object sender, RoutedEventArgs e) => new PuertasWindow().Show();
        private void Componentes_Click(object sender, RoutedEventArgs e) => new ComponenteWindow().Show();
        private void Subcomponentes_Click(object sender, RoutedEventArgs e) => new SubComponenteWindow().Show();
        private void Acabados_Click(object sender, RoutedEventArgs e) => new AcabadosWindow().Show();
        private void Mecanizados_Click(object sender, RoutedEventArgs e) => new MecanizadoWindow().Show();
        private void Cortes_Click(object sender, RoutedEventArgs e) => new CorteWindow().Show();
        private void Unidad_Click(object sender, RoutedEventArgs e) => new UnidadMedidaWindow().Show();
        private void Respaldo_Click(object sender, RoutedEventArgs e) => new DbaBackupWindow().Show();
        private void Importar_Click(object sender, RoutedEventArgs e) => new DbaImportWindow().Show();

        // ===== Controles de la app =====
        private void MinimizarApp_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        private void CerrarApp_Click(object sender, RoutedEventArgs e)
        {
            if (GlassDialog.Pregunta(this, "arquitectSoft", "¿Cerrar el programa?")) Close();
        }

        // ===== Maximizar sin tapar la barra de tareas =====
        private const int WM_GETMINMAXINFO = 0x0024;
        private const int MONITOR_DEFAULTTONEAREST = 2;

        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_GETMINMAXINFO)
            {
                MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
                IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
                if (monitor != IntPtr.Zero)
                {
                    MONITORINFO mi = new MONITORINFO();
                    GetMonitorInfo(monitor, mi);
                    RECT work = mi.rcWork; RECT mon = mi.rcMonitor;
                    mmi.ptMaxPosition.X = work.Left - mon.Left;
                    mmi.ptMaxPosition.Y = work.Top - mon.Top;
                    mmi.ptMaxSize.X = work.Right - work.Left;
                    mmi.ptMaxSize.Y = work.Bottom - work.Top;
                    Marshal.StructureToPtr(mmi, lParam, true);
                }
                handled = true;
            }
            return IntPtr.Zero;
        }

        [DllImport("user32.dll")] private static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);
        [DllImport("user32.dll")] private static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        [StructLayout(LayoutKind.Sequential)] private struct POINT { public int X; public int Y; }
        [StructLayout(LayoutKind.Sequential)] private struct RECT { public int Left; public int Top; public int Right; public int Bottom; }
        [StructLayout(LayoutKind.Sequential)]
        private struct MINMAXINFO
        {
            public POINT ptReserved; public POINT ptMaxSize; public POINT ptMaxPosition;
            public POINT ptMinTrackSize; public POINT ptMaxTrackSize;
        }
        [StructLayout(LayoutKind.Sequential)]
        private class MONITORINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            public RECT rcMonitor = new RECT(); public RECT rcWork = new RECT(); public int dwFlags = 0;
        }
    }
}
