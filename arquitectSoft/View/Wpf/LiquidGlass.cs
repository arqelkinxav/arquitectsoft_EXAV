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
        public static void MontarGlassMdi(FrameworkElement ventana, Rectangle backdrop, Visual fondo)
        {
            if (backdrop == null || fondo == null || ventana == null) return;

            var brush = new VisualBrush(fondo)
            {
                Stretch = Stretch.Fill,
                ViewboxUnits = BrushMappingMode.Absolute,
                ViewportUnits = BrushMappingMode.RelativeToBoundingBox
            };
            backdrop.Fill = brush;
            backdrop.Effect = new GlassyEffect();

            EventHandler actualizar = (s, e) =>
            {
                try
                {
                    if (ventana.ActualWidth <= 0 || ventana.ActualHeight <= 0) return;
                    GeneralTransform t = ventana.TransformToVisual(fondo);
                    Rect r = t.TransformBounds(new Rect(0, 0, ventana.ActualWidth, ventana.ActualHeight));
                    if (r.Width > 0 && r.Height > 0) brush.Viewbox = r;
                }
                catch { /* aún sin layout */ }
            };

            // El MdiChild se mueve por Canvas.Left/Top: LayoutUpdated capta cada cambio.
            ventana.LayoutUpdated += actualizar;
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
