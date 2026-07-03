using arquitectSoft.View.Wpf.Shaders;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace arquitectSoft.View.Wpf
{
    /// <summary>
    /// Animaciones "Jelly Pop" compartidas por las ventanas WPF de cristal.
    /// Cada ventana envuelve su contenido en un Border (FrameRim) con un ScaleTransform
    /// (WinScale) centrado; estos helpers animan ese par para abrir/cerrar.
    /// </summary>
    internal static class LiquidGlass
    {
        // Deja el marco listo para "saltar": invisible, encogido y con leve desenfoque.
        public static void PrepararOculto(UIElement frame, ScaleTransform scale)
        {
            frame.Opacity = 0;
            scale.ScaleX = scale.ScaleY = 0.7;
            frame.Effect = new BlurEffect { Radius = 8, RenderingBias = RenderingBias.Performance };
        }

        // Apertura: entra rebotando como gelatina (escala elástica) + opacidad + desenfoque→nítido.
        public static void Apertura(UIElement frame, ScaleTransform scale)
        {
            var jelly = new ElasticEase { EasingMode = EasingMode.EaseOut, Oscillations = 2, Springiness = 5 };

            var fade = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(160)));
            var sx = new DoubleAnimation(0.7, 1, new Duration(TimeSpan.FromMilliseconds(480))) { EasingFunction = jelly };
            var sy = new DoubleAnimation(0.7, 1, new Duration(TimeSpan.FromMilliseconds(480))) { EasingFunction = jelly };

            var blur = frame.Effect as BlurEffect ?? new BlurEffect { RenderingBias = RenderingBias.Performance };
            frame.Effect = blur;
            var bl = new DoubleAnimation(8, 0, new Duration(TimeSpan.FromMilliseconds(200)));
            bl.Completed += (s, e) => frame.Effect = null;   // sin efecto en reposo = texto nítido

            frame.BeginAnimation(UIElement.OpacityProperty, fade);
            scale.BeginAnimation(ScaleTransform.ScaleXProperty, sx);
            scale.BeginAnimation(ScaleTransform.ScaleYProperty, sy);
            blur.BeginAnimation(BlurEffect.RadiusProperty, bl);
        }

        // ===== Fondo "liquid glass real" (captura del escritorio + refracción por shader) =====
        // El llamador debe haber hecho ScreenCaptureHelper.CaptureFullScreen() en el ctor
        // ANTES de mostrar la ventana. Aquí se pinta esa foto en 'backdrop', se le aplica el
        // shader de refracción y se recorta al área de la ventana al mover/redimensionar.
        public static void MontarGlass(Window w, Rectangle backdrop)
        {
            if (backdrop == null || ScreenCaptureHelper.FullScreenSnapshot == null) return;

            var brush = new ImageBrush(ScreenCaptureHelper.FullScreenSnapshot)
            {
                Stretch = Stretch.Fill,
                ViewboxUnits = BrushMappingMode.Absolute,
                ViewportUnits = BrushMappingMode.RelativeToBoundingBox
            };
            backdrop.Fill = brush;
            backdrop.Effect = new GlassyEffect();

            // Usa el origen/medida de la VENTANA (no del backdrop), así el recorte no se ve
            // afectado por la animación de escala Jelly del marco.
            Action actualizar = () =>
            {
                try
                {
                    Point tl = w.PointToScreen(new Point(0, 0));
                    Point br = w.PointToScreen(new Point(w.ActualWidth, w.ActualHeight));
                    double ww = br.X - tl.X;
                    double hh = br.Y - tl.Y;
                    if (ww <= 0 || hh <= 0) return;
                    brush.Viewbox = new Rect(
                        tl.X - ScreenCaptureHelper.VirtualScreenX,
                        tl.Y - ScreenCaptureHelper.VirtualScreenY,
                        ww, hh);
                }
                catch { /* sin HWND todavía */ }
            };

            w.LocationChanged += (s, e) => actualizar();
            w.SizeChanged += (s, e) => actualizar();
            w.Loaded += (s, e) => actualizar();
        }

        // ===== Liquid glass para ventanas CONTENIDAS (MdiChild) =====
        // A diferencia de MontarGlass (que usa una foto del escritorio de Windows), aquí el
        // fondo a refractar es el wallpaper del propio escritorio de la app (un Visual dentro
        // de la ventana). Se recorta a la posición de la ventana hija dentro de ese fondo y
        // se le aplica el mismo shader de refracción. Así toda ventana contenida tiene cristal.
        // MODO RENDIMIENTO: cuando está activo, las ventanas contenidas NO usan el shader ni
        // el VisualBrush del wallpaper (que es lo caro con varias ventanas): se pintan con un
        // panel oscuro plano. Se puede alternar en caliente reaplicando el cristal.
        public static bool ModoRendimiento { get; set; }

        // Panel oscuro plano y congelado para el modo rendimiento (opaco, tema negro).
        private static readonly Brush FondoPlano = CrearFondoPlano();
        private static Brush CrearFondoPlano()
        {
            var b = new SolidColorBrush(Color.FromRgb(0x1B, 0x1B, 0x1E));
            b.Freeze();
            return b;
        }

        /// <summary>
        /// Monta el liquid glass (o el panel plano si ModoRendimiento) sobre el backdrop y
        /// devuelve una acción para DESMONTARLO (quita el handler y limpia fill/effect), de
        /// modo que se pueda alternar cristal↔rendimiento en caliente.
        /// </summary>
        public static Action MontarGlassMdi(FrameworkElement ventana, Rectangle backdrop, Visual fondo)
        {
            if (backdrop == null || fondo == null || ventana == null) return delegate { };

            if (ModoRendimiento)
            {
                // Sin refracción: panel opaco plano, barato de renderizar.
                backdrop.Effect = null;
                backdrop.Fill = FondoPlano;
                return delegate { backdrop.Fill = null; };
            }

            var brush = new VisualBrush(fondo)
            {
                Stretch = Stretch.Fill,
                ViewboxUnits = BrushMappingMode.Absolute,
                ViewportUnits = BrushMappingMode.RelativeToBoundingBox
            };
            // RENDIMIENTO (1): cachear la rasterización del wallpaper. El fondo no cambia, así
            // que se rasteriza una vez y se reutiliza en vez de re-renderizar en cada frame.
            // Sin esto, con varias ventanas abiertas cada VisualBrush "vivo" repinta el
            // wallpaper por frame y el PC se arrastra.
            RenderOptions.SetCachingHint(brush, CachingHint.Cache);
            RenderOptions.SetCacheInvalidationThresholdMinimum(brush, 0.5);
            RenderOptions.SetCacheInvalidationThresholdMaximum(brush, 2.0);

            backdrop.Fill = brush;
            backdrop.Effect = new GlassyEffect();

            // RENDIMIENTO (2): recalcular el recorte SOLO cuando la ventana se movió o cambió
            // de tamaño. LayoutUpdated se dispara en cada pasada de layout (hover, spinner,
            // scroll de grillas…); reasignar el Viewbox invalida el brush y fuerza repintado.
            // Guardamos el último rect y salimos si no cambió de forma apreciable.
            Rect ultimo = Rect.Empty;
            EventHandler actualizar = (s, e) =>
            {
                try
                {
                    if (ventana.ActualWidth <= 0 || ventana.ActualHeight <= 0) return;
                    GeneralTransform t = ventana.TransformToVisual(fondo);
                    Rect r = t.TransformBounds(new Rect(0, 0, ventana.ActualWidth, ventana.ActualHeight));
                    if (r.Width <= 0 || r.Height <= 0) return;

                    // Umbral de medio pixel: ignora reflows internos que no mueven la ventana.
                    if (ultimo != Rect.Empty
                        && Math.Abs(r.X - ultimo.X) < 0.5 && Math.Abs(r.Y - ultimo.Y) < 0.5
                        && Math.Abs(r.Width - ultimo.Width) < 0.5 && Math.Abs(r.Height - ultimo.Height) < 0.5)
                        return;

                    ultimo = r;
                    brush.Viewbox = r;
                }
                catch { /* aún sin layout */ }
            };

            // El MdiChild se mueve por Canvas.Left/Top: LayoutUpdated capta cada cambio.
            ventana.LayoutUpdated += actualizar;
            actualizar(null, EventArgs.Empty);   // recorte inicial (importante al re-montar)

            return delegate
            {
                ventana.LayoutUpdated -= actualizar;
                backdrop.Effect = null;
                backdrop.Fill = null;
            };
        }

        // Cierre: encoge rápido con leve anticipación y se desvanece; llama alTerminar() al final.
        public static void Cierre(UIElement frame, ScaleTransform scale, Action alTerminar)
        {
            var easeIn = new BackEase { EasingMode = EasingMode.EaseIn, Amplitude = 0.5 };
            var dur = new Duration(TimeSpan.FromMilliseconds(180));

            var blur = new BlurEffect { Radius = 0, RenderingBias = RenderingBias.Performance };
            frame.Effect = blur;

            var fade = new DoubleAnimation(0, new Duration(TimeSpan.FromMilliseconds(170)));
            var sx = new DoubleAnimation(0.82, dur) { EasingFunction = easeIn };
            var sy = new DoubleAnimation(0.82, dur) { EasingFunction = easeIn };
            var bl = new DoubleAnimation(8, dur);

            fade.Completed += (s, e) => alTerminar();

            frame.BeginAnimation(UIElement.OpacityProperty, fade);
            scale.BeginAnimation(ScaleTransform.ScaleXProperty, sx);
            scale.BeginAnimation(ScaleTransform.ScaleYProperty, sy);
            blur.BeginAnimation(BlurEffect.RadiusProperty, bl);
        }
    }
}
