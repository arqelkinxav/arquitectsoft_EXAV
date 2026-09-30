using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace arquitectSoft.View.Wpf
{
    /// <summary>
    /// Entrada tras el login, sin fondo: una ventana transparente sobre el escritorio de
    /// Windows donde el logo se arma en grande. Cuando el escritorio del programa ya está
    /// pintado, VolarA lleva el logo hasta su sitio en la barra de título y la ventana se
    /// cierra. Va aparte porque la ventana principal no puede ser transparente (con
    /// AllowsTransparency se rompía el maximizado).
    /// </summary>
    public class EntradaWindow : Window
    {
        /// <summary>Lo que tarda el logo en armarse; Program espera esto antes de abrir el escritorio.</summary>
        public const double Armado = 2.3;
        private const double Escala = 6;

        private readonly Canvas _lienzo = new Canvas();
        private readonly Border _marco = new Border();
        private readonly LogoMarca _logo = new LogoMarca();
        private readonly Ellipse _halo = new Ellipse();
        private readonly ScaleTransform _escala = new ScaleTransform(Escala, Escala);
        private readonly TranslateTransform _vuelo = new TranslateTransform();
        private double _izq, _arr;
        private bool _terminada;

        public EntradaWindow()
        {
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;
            Background = Brushes.Transparent;
            ResizeMode = ResizeMode.NoResize;
            ShowInTaskbar = false;
            ShowActivated = false;
            Topmost = true;
            UseLayoutRounding = true;         // como el escritorio: al aterrizar, mismos píxeles
            // Cubre la zona de trabajo del monitor principal, donde se abre maximizado el escritorio.
            Rect wa = SystemParameters.WorkArea;
            Left = wa.Left; Top = wa.Top; Width = wa.Width; Height = wa.Height;

            // La entrada va SIEMPRE en colores de noche, sea cual sea el modo: los del tema oscuro
            // en esta ventana tapan los de la aplicación. Al aterrizar se funde con el logo de la
            // barra, que lleva los del modo actual (en modo día, se ve pasar de blanco a negro).
            var noche = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/View/Wpf/Temas/TemaOscuro.xaml")
            };
            Resources.MergedDictionaries.Add(noche);
            Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/View/Wpf/Estilos.xaml")
            });
            TextElement.SetFontFamily(_lienzo, (FontFamily)FindResource("Jakarta"));
            TextOptions.SetTextFormattingMode(_logo, TextFormattingMode.Ideal);
            TextOptions.SetTextHintingMode(_logo, TextHintingMode.Animated);

            // Halo muy suave y oscuro detrás del logo, para que se lea sobre cualquier fondo
            // de pantalla (iconos, fotos...). No tiene bordes.
            Color c = (noche["T.0A0A0C"] as SolidColorBrush)?.Color ?? Colors.Black;
            _halo.Width = 1500; _halo.Height = 640;
            _halo.IsHitTestVisible = false;
            _halo.Fill = new RadialGradientBrush(Color.FromArgb(150, c.R, c.G, c.B), Color.FromArgb(0, c.R, c.G, c.B));
            _halo.Opacity = 0;

            var tg = new TransformGroup();
            tg.Children.Add(_escala);
            tg.Children.Add(_vuelo);
            _marco.RenderTransform = tg;
            _marco.Child = _logo;
            _lienzo.Children.Add(_halo);
            _lienzo.Children.Add(_marco);
            Content = _lienzo;

            _logo.PrepararEntrada();          // el primer fotograma sale vacío
            ContentRendered += (s, e) => Armar();
        }

        private void Armar()
        {
            _izq = (ActualWidth - _marco.ActualWidth * Escala) / 2;
            _arr = (ActualHeight - _marco.ActualHeight * Escala) / 2;
            Canvas.SetLeft(_marco, _izq);
            Canvas.SetTop(_marco, _arr);
            Canvas.SetLeft(_halo, (ActualWidth - _halo.Width) / 2);
            Canvas.SetTop(_halo, (ActualHeight - _halo.Height) / 2);
            LogoMarca.Animar(_halo, OpacityProperty, 0, 1, 0, .6, null);
            _logo.Armar(.15);
        }

        /// <summary>
        /// Lleva el logo hasta <paramref name="destino"/> (el logo de la barra de título, que a
        /// escala 1 es idéntico). Al llegar llama <paramref name="alTerminar"/>, que funde el de
        /// verdad, mientras este se desvanece; luego la ventana se cierra.
        /// </summary>
        public void VolarA(UIElement destino, Action alTerminar)
        {
            if (_terminada) { alTerminar?.Invoke(); return; }
            try
            {
                Point p = PointFromScreen(destino.PointToScreen(new Point(0, 0)));
                const double dur = .8;
                var curva = new CubicEase { EasingMode = EasingMode.EaseInOut };
                LogoMarca.Animar(_escala, ScaleTransform.ScaleXProperty, Escala, 1, 0, dur, curva);
                LogoMarca.Animar(_escala, ScaleTransform.ScaleYProperty, Escala, 1, 0, dur, curva);
                LogoMarca.Animar(_vuelo, TranslateTransform.XProperty, 0, p.X - _izq, 0, dur, curva);
                LogoMarca.Animar(_vuelo, TranslateTransform.YProperty, 0, p.Y - _arr, 0, dur, curva,
                                 (s, e) => Aterrizar(destino, alTerminar));
                LogoMarca.Animar(_halo, OpacityProperty, 1, 0, 0, .45, null);
            }
            catch { Terminar(alTerminar); }   // una animación nunca puede impedir trabajar
        }

        // Ya encima del logo de la barra (que alTerminar enseña debajo, con los colores del modo):
        // un barrido de izquierda a derecha retira el de noche y descubre el de la barra. En modo
        // día es la transición de blanco a negro; en modo noche no se nota.
        private void Aterrizar(UIElement destino, Action alTerminar)
        {
            if (_terminada) return;
            _terminada = true;
            try { Encajar(destino); } catch { }
            try { alTerminar?.Invoke(); } catch { }
            var borde = new GradientStop(Colors.Transparent, -.35);
            var opaco = new GradientStop(Colors.Black, 0);
            _marco.OpacityMask = new LinearGradientBrush(new GradientStopCollection { borde, opaco },
                                                         new Point(0, .5), new Point(1, .5));
            var curva = new SineEase { EasingMode = EasingMode.EaseInOut };
            LogoMarca.Animar(borde, GradientStop.OffsetProperty, -.35, 1, .12, Barrido, curva);
            LogoMarca.Animar(opaco, GradientStop.OffsetProperty, 0, 1.35, .12, Barrido, curva, (s, e) => Close());
        }

        private const double Barrido = .75;

        /// <summary>
        /// Durante el barrido los dos logos están uno encima del otro: tienen que coincidir
        /// píxel a píxel o se ve doble. El de la barra pinta el texto ajustado a píxeles
        /// (Display + UseLayoutRounding) y este volaba en Ideal y con decimales. Así que al
        /// llegar: se quitan las animaciones, se vuelve a medir dónde está el destino (la
        /// ventana pudo acabar de maximizarse durante el vuelo), se redondea a píxel de
        /// pantalla y se pasa a la misma forma de pintar el texto.
        /// </summary>
        private void Encajar(UIElement destino)
        {
            _escala.BeginAnimation(ScaleTransform.ScaleXProperty, null);
            _escala.BeginAnimation(ScaleTransform.ScaleYProperty, null);
            _vuelo.BeginAnimation(TranslateTransform.XProperty, null);
            _vuelo.BeginAnimation(TranslateTransform.YProperty, null);
            _escala.ScaleX = _escala.ScaleY = 1;
            _vuelo.X = _vuelo.Y = 0;

            Point p = PointFromScreen(destino.PointToScreen(new Point(0, 0)));
            var fuente = PresentationSource.FromVisual(this);
            Matrix m = fuente != null && fuente.CompositionTarget != null
                ? fuente.CompositionTarget.TransformToDevice : Matrix.Identity;
            Canvas.SetLeft(_marco, Math.Round(p.X * m.M11) / m.M11);
            Canvas.SetTop(_marco, Math.Round(p.Y * m.M22) / m.M22);

            TextOptions.SetTextFormattingMode(_logo, TextFormattingMode.Display);
            _logo.ClearValue(TextOptions.TextHintingModeProperty);
        }

        /// <summary>Corta la entrada donde esté (también sirve para saltarla).</summary>
        public void Terminar(Action alTerminar)
        {
            if (_terminada) return;
            _terminada = true;
            try { alTerminar?.Invoke(); } finally { Close(); }
        }

        /// <summary>Deja correr las animaciones <paramref name="segundos"/> sin abrir aún nada más.</summary>
        public static void Esperar(double segundos)
        {
            var marco = new DispatcherFrame();
            var t = new DispatcherTimer { Interval = TimeSpan.FromSeconds(segundos) };
            t.Tick += (s, e) => { t.Stop(); marco.Continue = false; };
            t.Start();
            Dispatcher.PushFrame(marco);
        }
    }
}
