using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using arquitectSoft.Engine;

namespace arquitectSoft.View.Wpf
{
    /// <summary>
    /// Versión WPF de la ventana "Análisis de Mamparas".
    /// Tema oscuro "soft UI" + cálculo en segundo plano (el motor sin UI corre en
    /// Task.Run para que la interfaz no se congele, a diferencia de WinForms).
    /// </summary>
    public partial class AnalisisWindow : Window
    {
        private const int MedidaMin = 0;
        private const int MedidaMax = 99999;
        private const int DesperdicioMin = 0;
        private const int DesperdicioMax = 100;

        private static readonly string GlifoMaximizar = ((char)0xE922).ToString();
        private static readonly string GlifoRestaurar = ((char)0xE923).ToString();

        // ---- DWM: esquinas redondeadas nativas de Windows 11 ----
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int size);
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        private const int DWMWA_BORDER_COLOR = 34;
        private const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;
        private const int DWMWA_SYSTEMBACKDROP_TYPE = 38;
        private const int DWMWCP_ROUND = 2;
        private const int DWMSBT_TRANSIENTWINDOW = 3;   // material acrílico (cristal)
        private const uint DWMWA_COLOR_NONE = 0xFFFFFFFE;   // sin borde

        // ---- Motor de cálculo (sin UI) + control de recálculo ----
        private readonly AnalisisEngine _engine = new AnalisisEngine();
        private DispatcherTimer _recalcTimer;
        private int _generacion = 0;        // descarta resultados de cálculos obsoletos
        private bool _cerrando;             // evita reentrar al cerrar mientras corre la animación

        public AnalisisWindow()
        {
            ScreenCaptureHelper.CaptureFullScreen();   // foto del escritorio antes de mostrarse
            InitializeComponent();
            // Ventana modeless lanzada desde WinForms: habilita el enrutado de teclado
            // hacia los controles WPF (TextBox, edición de grillas, etc.).
            System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(this);
            this.SourceInitialized += AnalisisWindow_SourceInitialized;
            this.StateChanged += AnalisisWindow_StateChanged;

            // Debounce: recalcula 0,25 s después del último cambio de Medida/Desperdicio.
            _recalcTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(250) };
            _recalcTimer.Tick += RecalcTimer_Tick;
            TxtMedidaBase.TextChanged += Valor_Changed;
            TxtDesperdicio.TextChanged += Valor_Changed;

            // Ajuste de columnas (centrado + wrap de descripción) en todas las grillas.
            foreach (var dg in TodasLasGrillas())
                dg.AutoGeneratingColumn += Dg_AutoGeneratingColumn;
            DgPreview.AutoGeneratingColumn += Dg_AutoGeneratingColumn;

            // Animación de apertura "Jelly Pop" (cristal que rebota al aparecer).
            LiquidGlass.PrepararOculto(FrameRim, WinScale);
            LiquidGlass.MontarGlass(this, GlassBackdrop);
            this.Loaded += (s, e) => LiquidGlass.Apertura(FrameRim, WinScale);
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

        // Último resultado, para el previsualizador (acceso a tablas crudas por pestaña).
        private ResultadoAnalisis _ultimo;

        private DataGrid[] TodasLasGrillas() => new[]
        {
            DgPerfilMetalico, DgPerfilMetalicoHerraje, DgVidrioPaneles, DgPuertas,
            DgPuertasHerrajes, DgPuertasCantidad, DgTubos, DgMamparas
        };

        private void AnalisisWindow_SourceInitialized(object sender, EventArgs e)
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            try
            {
                int pref = DWMWCP_ROUND;
                DwmSetWindowAttribute(hwnd, DWMWA_WINDOW_CORNER_PREFERENCE, ref pref, sizeof(int));

                // Tema oscuro en la decoración nativa + material acrílico (cristal)
                // de fondo, estilo "vidrio esmerilado" de Windows 11.
                int dark = 1;
                DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref dark, sizeof(int));
                int backdrop = DWMSBT_TRANSIENTWINDOW;
                DwmSetWindowAttribute(hwnd, DWMWA_SYSTEMBACKDROP_TYPE, ref backdrop, sizeof(int));
                // Quita el borde claro que Windows 11 dibuja alrededor de la ventana.
                int sinBorde = unchecked((int)DWMWA_COLOR_NONE);
                DwmSetWindowAttribute(hwnd, DWMWA_BORDER_COLOR, ref sinBorde, sizeof(int));
            }
            catch { /* Windows 10 o anterior: esquinas rectas, sin acrílico */ }

            // Hook para que al maximizar la ventana sin bordes NO tape la barra de tareas.
            HwndSource src = HwndSource.FromHwnd(hwnd);
            if (src != null)
            {
                src.AddHook(WndProc);
                // Para que el material acrílico se vea a través del fondo translúcido,
                // el lienzo de composición de WPF debe ser transparente.
                if (src.CompositionTarget != null)
                    src.CompositionTarget.BackgroundColor = Colors.Transparent;
            }
        }

        // ---- Maximizar respetando el área de trabajo del monitor ----
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
                    RECT work = mi.rcWork;
                    RECT mon = mi.rcMonitor;
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

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT { public int X; public int Y; }
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT { public int Left; public int Top; public int Right; public int Bottom; }
        [StructLayout(LayoutKind.Sequential)]
        private struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        }
        [StructLayout(LayoutKind.Sequential)]
        private class MONITORINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            public RECT rcMonitor;
            public RECT rcWork;
            public int dwFlags;
        }

        private void AnalisisWindow_StateChanged(object sender, EventArgs e)
        {
            bool max = this.WindowState == WindowState.Maximized;
            BtnMax.Content = max ? GlifoRestaurar : GlifoMaximizar;
            // Sin esquinas redondeadas ni filo cuando está maximizada (si no, asoma el
            // acrílico en las 4 esquinas de pantalla).
            FrameRim.CornerRadius = new CornerRadius(max ? 0 : 8);
            FrameRim.BorderThickness = new Thickness(max ? 0 : 1.1);
        }

        // ===== Barra de título =====
        private void Minimizar_Click(object sender, RoutedEventArgs e) => this.WindowState = WindowState.Minimized;

        private void Maximizar_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = (this.WindowState == WindowState.Maximized)
                ? WindowState.Normal : WindowState.Maximized;
        }

        private void Cerrar_Click(object sender, RoutedEventArgs e) => this.Close();

        // ===== Validación: solo dígitos =====
        private void SoloEnteros(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }

        // ===== Spinners =====
        private void MedidaUp_Click(object sender, RoutedEventArgs e) =>
            TxtMedidaBase.Text = Clamp(LeerEntero(TxtMedidaBase.Text) + 1, MedidaMin, MedidaMax).ToString();
        private void MedidaDown_Click(object sender, RoutedEventArgs e) =>
            TxtMedidaBase.Text = Clamp(LeerEntero(TxtMedidaBase.Text) - 1, MedidaMin, MedidaMax).ToString();
        private void DesperdicioUp_Click(object sender, RoutedEventArgs e) =>
            TxtDesperdicio.Text = Clamp(LeerEntero(TxtDesperdicio.Text) + 1, DesperdicioMin, DesperdicioMax).ToString();
        private void DesperdicioDown_Click(object sender, RoutedEventArgs e) =>
            TxtDesperdicio.Text = Clamp(LeerEntero(TxtDesperdicio.Text) - 1, DesperdicioMin, DesperdicioMax).ToString();

        private static int LeerEntero(string texto)
        {
            int v;
            return int.TryParse(texto, out v) ? v : 0;
        }
        private static int Clamp(int v, int min, int max) => v < min ? min : (v > max ? max : v);

        // ===== Cargar TXT → motor → mostrar =====
        private async void Cargar_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true,
                Filter = "Archivos TXT (*.txt)|*.txt",
                Title = "Seleccionar despieces (.txt)"
            };
            if (dlg.ShowDialog(this) != true) return;
            await CargarArchivos(dlg.FileNames);
        }

        /// <summary>
        /// Lógica común de carga (la usan el botón Cargar y el arrastrar-soltar):
        /// filtra los .txt, pregunta por la segmentación y dispara el análisis.
        /// </summary>
        private async Task CargarArchivos(string[] rutas)
        {
            string[] txts = (rutas ?? new string[0])
                .Where(r => !string.IsNullOrEmpty(r) &&
                            r.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (txts.Length == 0)
            {
                LblEstado.Text = "No se encontraron archivos .txt para cargar.";
                return;
            }

            bool segmentar = GlassDialog.Pregunta(this, "Análisis de Mamparas",
                "¿Quieres segmentar el análisis de los Perfiles Metálicos por Ubicación?",
                si: "Sí, por ubicación", no: "No");

            try
            {
                _engine.Cargar(txts, segmentar);
            }
            catch (Exception ex)
            {
                LblEstado.Text = "Error al leer los archivos: " + ex.Message;
                return;
            }

            LblRuta.Text = _engine.DirectorioActual ?? "";
            await RecalcularAsync(seleccionarPestana: true);
        }

        // ===== Arrastrar y soltar archivos sobre la ventana =====
        private static bool TraeArchivos(DragEventArgs e) =>
            e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop);

        private void Ventana_DragOver(object sender, DragEventArgs e)
        {
            bool ok = TraeArchivos(e);
            e.Effects = ok ? DragDropEffects.Copy : DragDropEffects.None;
            if (ok) DropOverlay.Visibility = Visibility.Visible;
            e.Handled = true;
        }

        private void Ventana_DragLeave(object sender, DragEventArgs e)
        {
            // Ocultar sólo cuando el cursor sale de verdad de la ventana
            // (DragLeave también burbujea desde los controles internos).
            var p = e.GetPosition(this);
            if (p.X <= 0 || p.Y <= 0 || p.X >= ActualWidth || p.Y >= ActualHeight)
                DropOverlay.Visibility = Visibility.Collapsed;
        }

        private async void Ventana_Drop(object sender, DragEventArgs e)
        {
            DropOverlay.Visibility = Visibility.Collapsed;
            if (!TraeArchivos(e)) return;

            var rutas = e.Data.GetData(DataFormats.FileDrop) as string[];
            e.Handled = true;
            await CargarArchivos(rutas);
        }

        // ===== Recálculo en tiempo real (debounce) =====
        private void Valor_Changed(object sender, TextChangedEventArgs e)
        {
            if (!_engine.DatosCargados) return;
            _recalcTimer.Stop();
            _recalcTimer.Start();
        }

        private async void RecalcTimer_Tick(object sender, EventArgs e)
        {
            _recalcTimer.Stop();
            await RecalcularAsync(seleccionarPestana: false);
        }

        /// <summary>
        /// Lee los parámetros, corre el motor en segundo plano y muestra el resultado.
        /// Si llega un recálculo más nuevo mientras este corre, descarta el resultado viejo.
        /// </summary>
        private async Task RecalcularAsync(bool seleccionarPestana)
        {
            if (!_engine.DatosCargados) return;

            int medida = LeerEntero(TxtMedidaBase.Text);
            int pct = LeerEntero(TxtDesperdicio.Text);
            int gen = ++_generacion;

            LblEstado.Text = "Calculando…";
            SpinnerCargando();
            var sw = Stopwatch.StartNew();

            ResultadoAnalisis res = null;
            Exception err = null;
            try
            {
                // El motor NO toca UI → seguro en hilo de fondo.
                res = await Task.Run(() => _engine.Ejecutar(medida, pct));
            }
            catch (Exception ex) { err = ex; }

            sw.Stop();
            if (gen != _generacion) return;   // ya hay un cálculo más reciente: ignorar este

            if (err != null)
            {
                SpinnerOcultar();
                LblEstado.Text = "Error en el cálculo: " + err.Message;
                return;
            }

            MostrarResultado(res, seleccionarPestana);
            LblEstado.Text = res.TieneDatos
                ? string.Format("Listo · {0:0.0} s", sw.ElapsedMilliseconds / 1000.0)
                : "No se encontraron datos para analizar.";

            SpinnerListo();
            await Task.Delay(750);
            if (gen == _generacion) SpinnerOcultar();
        }

        // ===== Animación spinner → check (XAML puro, sin dependencias) =====
        private void IniciarGiro()
        {
            var giro = new DoubleAnimation(0, 360, new Duration(TimeSpan.FromSeconds(0.9)))
            { RepeatBehavior = RepeatBehavior.Forever };
            SpinRotate.BeginAnimation(RotateTransform.AngleProperty, giro);
        }

        private void DetenerGiro() => SpinRotate.BeginAnimation(RotateTransform.AngleProperty, null);

        private void SpinnerCargando()
        {
            SpinnerOverlay.BeginAnimation(OpacityProperty, null);
            SpinnerOverlay.Opacity = 1;
            SpinnerOverlay.Visibility = Visibility.Visible;

            SpinArc.BeginAnimation(OpacityProperty, null);
            SpinArc.Opacity = 1;
            CheckPath.BeginAnimation(OpacityProperty, null);
            CheckPath.Opacity = 0;
            CheckScale.ScaleX = CheckScale.ScaleY = 0.6;
            LblSpinner.Text = "Calculando…";
            IniciarGiro();
        }

        private void SpinnerListo()
        {
            DetenerGiro();
            LblSpinner.Text = "Listo";

            SpinArc.BeginAnimation(OpacityProperty,
                new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.15))));
            CheckPath.BeginAnimation(OpacityProperty,
                new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.15))));

            var pop = new DoubleAnimation(0.6, 1, new Duration(TimeSpan.FromSeconds(0.35)))
            { EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.7 } };
            CheckScale.BeginAnimation(ScaleTransform.ScaleXProperty, pop);
            CheckScale.BeginAnimation(ScaleTransform.ScaleYProperty, pop);
        }

        private void SpinnerOcultar()
        {
            DetenerGiro();
            var fade = new DoubleAnimation(SpinnerOverlay.Opacity, 0,
                new Duration(TimeSpan.FromSeconds(0.2)));
            fade.Completed += (s, e) => SpinnerOverlay.Visibility = Visibility.Collapsed;
            SpinnerOverlay.BeginAnimation(OpacityProperty, fade);
        }

        private void MostrarResultado(ResultadoAnalisis r, bool seleccionarPestana)
        {
            _ultimo = r;
            DgPerfilMetalico.ItemsSource = Vista(r.PerfilMetalico);
            DgPerfilMetalicoHerraje.ItemsSource = Vista(r.PerfilMetalicoHerraje);
            DgVidrioPaneles.ItemsSource = Vista(r.VidrioPaneles);
            DgPuertas.ItemsSource = Vista(r.Puertas);
            DgPuertasHerrajes.ItemsSource = Vista(r.PuertasHerraje);
            DgPuertasCantidad.ItemsSource = Vista(r.PuertasCantidad);
            DgTubos.ItemsSource = Vista(r.Tubos);
            DgMamparas.ItemsSource = Vista(r.Mamparas);

            BtnChange.Visibility = r.TieneDatos ? Visibility.Visible : Visibility.Collapsed;
            HintVacio.Visibility = r.TieneDatos ? Visibility.Collapsed : Visibility.Visible;

            if (seleccionarPestana && r.PestanaSugerida >= 0 && r.PestanaSugerida < Tabs.Items.Count)
                Tabs.SelectedIndex = r.PestanaSugerida;

            if (PreviewPanel.Visibility == Visibility.Visible)
                RefreshPreview();
        }

        // ===== Previsualizador de datos cargados (crudos) =====
        private void Preview_Click(object sender, RoutedEventArgs e)
        {
            bool mostrar = PreviewPanel.Visibility != Visibility.Visible;
            if (mostrar)
            {
                FilaSplitter.Height = new GridLength(8);
                // Conserva el alto previo si el usuario ya lo había ajustado.
                if (FilaPreview.Height.Value < 60)
                    FilaPreview.Height = new GridLength(240);
                FilaPreview.MinHeight = 90;
                PreviewSplitter.Visibility = Visibility.Visible;
                PreviewPanel.Visibility = Visibility.Visible;
                RefreshPreview();
            }
            else
            {
                FilaSplitter.Height = new GridLength(0);
                FilaPreview.Height = new GridLength(0);
                FilaPreview.MinHeight = 0;
                PreviewSplitter.Visibility = Visibility.Collapsed;
                PreviewPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Ignora eventos de selección que burbujean desde grillas internas.
            if (!ReferenceEquals(e.OriginalSource, Tabs)) return;
            if (PreviewPanel == null || PreviewPanel.Visibility != Visibility.Visible) return;
            RefreshPreview();
        }

        private void RefreshPreview()
        {
            string nombre = (Tabs.SelectedItem as TabItem)?.Header?.ToString() ?? "";
            DataTable raw = _ultimo == null ? null : RawPorPestana(_ultimo, Tabs.SelectedIndex);
            DgPreview.ItemsSource = raw == null ? null : raw.DefaultView;
            LblPreview.Text = "Datos cargados — " + nombre + (raw == null ? "  (sin datos)" : "");
        }

        private static DataTable RawPorPestana(ResultadoAnalisis r, int idx)
        {
            switch (idx)
            {
                case 0:
                case 1: return r.PerfilMetalicoRaw;   // Perfil Metálico (+ Herraje)
                case 2: return r.VidrioRaw;
                case 3:
                case 4:
                case 5: return r.PuertasRaw;          // Puertas (+ Herrajes, Cantidad)
                case 6: return r.TubosRaw;
                case 7: return r.MamparasRaw;
                default: return null;
            }
        }

        private static DataView Vista(DataTable t) => t == null ? null : t.DefaultView;

        // ===== Ajuste de columnas: todo centrado; la "descripción" se ajusta al
        //       ancho disponible y hace wrap (la fila crece) en vez de desbordar. =====
        private void Dg_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            bool esDescripcion = e.PropertyName != null
                && e.PropertyName.ToLowerInvariant().Contains("descrip");

            var estiloCelda = new Style(typeof(TextBlock));
            estiloCelda.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center));
            estiloCelda.Setters.Add(new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center));
            estiloCelda.Setters.Add(new Setter(TextBlock.PaddingProperty, new Thickness(6, 3, 6, 3)));
            if (esDescripcion)
                estiloCelda.Setters.Add(new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap));

            var bound = e.Column as DataGridBoundColumn;
            if (bound != null) bound.ElementStyle = estiloCelda;

            if (esDescripcion)
            {
                // Ancho inicial acotado; el texto largo hace wrap (la fila crece).
                // Redimensionable a mano si se quiere ver completo.
                e.Column.Width = new DataGridLength(250);
                e.Column.MinWidth = 140;
            }
            else
            {
                e.Column.Width = DataGridLength.Auto;
            }
        }

        // ===== Coloreado de filas del grid de Puertas (equivale al CellFormatting
        //       de WinForms: separador / título de puerta / variante "~"). =====
        private static readonly Brush PuertaSeparador = Congelar(Color.FromRgb(0x2A, 0x2A, 0x2A));
        private static readonly Brush PuertaTitulo    = Congelar(Color.FromArgb(0x4D, 0xE0, 0x7B, 0x5B)); // peach
        private static readonly Brush PuertaVariante  = Congelar(Color.FromArgb(0x4D, 0x53, 0xC5, 0x6E)); // verde

        private static Brush Congelar(Color c)
        {
            var b = new SolidColorBrush(c);
            b.Freeze();
            return b;
        }

        private void DgPuertas_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            var drv = e.Row.Item as DataRowView;
            if (drv == null || drv.Row.ItemArray.Length == 0)
            {
                e.Row.ClearValue(Control.BackgroundProperty);
                return;
            }

            string v = Convert.ToString(drv.Row[0]);
            if (string.IsNullOrEmpty(v))
                e.Row.Background = PuertaSeparador;
            else if (v.Contains("Puerta"))
                e.Row.Background = PuertaTitulo;
            else if (v.Contains("~"))
                e.Row.Background = PuertaVariante;
            else
                e.Row.ClearValue(Control.BackgroundProperty);   // fila normal: estilo por defecto
        }

        // ===== Cancelar: limpia las tablas mostradas =====
        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            _generacion++;   // descarta cualquier cálculo en vuelo
            DgPerfilMetalico.ItemsSource = null;
            DgPerfilMetalicoHerraje.ItemsSource = null;
            DgVidrioPaneles.ItemsSource = null;
            DgPuertas.ItemsSource = null;
            DgPuertasHerrajes.ItemsSource = null;
            DgPuertasCantidad.ItemsSource = null;
            DgTubos.ItemsSource = null;
            DgMamparas.ItemsSource = null;
            _ultimo = null;
            DgPreview.ItemsSource = null;
            BtnChange.Visibility = Visibility.Collapsed;
            HintVacio.Visibility = Visibility.Visible;
            LblEstado.Text = "Listo. Carga uno o varios archivos TXT para analizar.";
            LblRuta.Text = "";
        }

        // ===== Exportar a Excel (mismo formato que WinForms) =====
        private void Exportar_Click(object sender, RoutedEventArgs e)
        {
            if (_ultimo == null || !_ultimo.TieneDatos)
            {
                GlassDialog.Informar(this, "Exportar", "No existen datos analizados para exportar.");
                return;
            }

            // Ventana WPF (tema cristal) que recoge nº proyecto, nombre, técnico, etc.
            // Si se cargó un TXT de información del proyecto, prerellenamos nº y nombre.
            var bsc = new ExportDialog
            {
                Owner = this,
                PrefillNumero = _engine.ProyectoCodigo,
                PrefillNombre = _engine.ProyectoNombre,
                PrefillReferencia = _engine.ProyectoReferencia
            };
            bsc.ShowDialog();
            if (bsc.Numero == null) return;   // canceló

            string[] param = { bsc.Numero, bsc.Nombre, bsc.Tecnico, bsc.Fecha,
                               bsc.Acabado1, bsc.Acabado2, bsc.Albaran, bsc.Referencia };

            string folder;
            using (var fb = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (!string.IsNullOrEmpty(_engine.DirectorioActual))
                    fb.SelectedPath = _engine.DirectorioActual;
                if (fb.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
                folder = fb.SelectedPath;
            }
            if (string.IsNullOrEmpty(folder)) return;

            try
            {
                LblEstado.Text = "Exportando…";
                string archivo = new ExcelExporter()
                    .Exportar(_ultimo, param, folder, _ultimo.SwSegmentadoUbiFinal);

                LblEstado.Text = "Exportado: " + archivo;
                if (GlassDialog.Pregunta(this, "Exportar",
                        "Se exportó correctamente. ¿Deseas abrirlo ahora?", si: "Abrir", no: "Ahora no"))
                    Process.Start(archivo);
                else
                    GlassDialog.Informar(this, "Exportar", "El archivo está en:\n" + archivo);
            }
            catch (IOException)
            {
                GlassDialog.Informar(this, "Exportar",
                    "Un archivo se encontraba abierto. Ciérralo e inténtalo nuevamente.");
                LblEstado.Text = "Exportación cancelada (archivo abierto).";
            }
            catch (Exception ex)
            {
                GlassDialog.Informar(this, "Exportar", "Error al exportar: " + ex.Message);
                LblEstado.Text = "Error al exportar.";
            }
        }

        // ===== Cambiar Acabado (global): busca un acabado y lo reemplaza en
        //       TODAS las tablas calculadas (equivale a BtnChange_Click + FnChangeInfo). =====
        private void CambiarAcabado_Click(object sender, RoutedEventArgs e)
        {
            if (_ultimo == null || !_ultimo.TieneDatos)
            {
                GlassDialog.Informar(this, "Cambiar Acabado", "Primero carga y analiza unos archivos.");
                return;
            }

            string a1, a2;
            if (!GlassDialog.PedirAcabado(this, out a1, out a2)) return;
            if (string.IsNullOrWhiteSpace(a1))
            {
                GlassDialog.Informar(this, "Cambiar Acabado", "Indica el acabado a buscar (origen).");
                return;
            }

            AcabadoChanger.Aplicar(_ultimo, a1, a2 ?? "");
            MostrarResultado(_ultimo, false);   // re-enlaza los grids con las tablas ya modificadas
            LblEstado.Text = "Acabado actualizado: \"" + a1 + "\" → \"" + a2 + "\".";
        }

        // ===== Menú clic-derecho sobre Perfil Metálico =====
        private DataRow FilaPerfilSeleccionada()
        {
            var drv = DgPerfilMetalico.SelectedItem as DataRowView;
            return drv == null ? null : drv.Row;
        }

        // "Remplazar SubComponente" → catálogo (reutiliza el buscador WinForms).
        private void RemplazarSub_Click(object sender, RoutedEventArgs e)
        {
            var row = FilaPerfilSeleccionada();
            if (row == null)
            {
                GlassDialog.Informar(this, "Remplazar SubComponente", "Selecciona primero una fila de Perfil Metálico.");
                return;
            }

            var bsc = new BuscarDialog { Consulta = "SubComp", Owner = this };
            if (bsc.ShowDialog() != true) return;

            string codigo = bsc.ReturnItem1.Trim();
            string descripcion = bsc.ReturnItem2.Split('(')[0].Trim();
            string acabado = bsc.ReturnItem5;

            row[1] = codigo;
            row[2] = descripcion;
            row[3] = acabado;
            MostrarResultado(_ultimo, false);
            LblEstado.Text = "SubComponente remplazado por " + codigo + ".";
        }

        // "Cambiar Acabado Temporal" → sólo la fila seleccionada.
        private void AcabadoTemporal_Click(object sender, RoutedEventArgs e)
        {
            var row = FilaPerfilSeleccionada();
            if (row == null)
            {
                GlassDialog.Informar(this, "Cambiar Acabado", "Selecciona primero una fila de Perfil Metálico.");
                return;
            }

            string acabadoActual = row.ItemArray.Length > 3 ? Convert.ToString(row[3]) : "";
            string a1, a2;
            if (!GlassDialog.PedirAcabado(this, out a1, out a2, acabadoActual)) return;
            if (string.IsNullOrWhiteSpace(a2)) return;

            AcabadoChanger.CambiarFilaPerfil(row, a2);
            MostrarResultado(_ultimo, false);
            LblEstado.Text = "Acabado de la fila actualizado.";
        }
    }
}
