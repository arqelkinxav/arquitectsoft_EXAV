using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace arquitectSoft.View.Wpf
{
    /// <summary>
    /// Logo de arquitectSoft. Quieto es el de la barra de título; PrepararEntrada + Armar
    /// lo construyen como en la presentación: las barras entran enteras, se cortan, la
    /// pieza melocotón se enciende, el retal cae a gris y aparece el nombre.
    /// </summary>
    public partial class LogoMarca : UserControl
    {
        public LogoMarca()
        {
            InitializeComponent();
        }

        /// <summary>Deja el logo "sin armar" (invisible) para que Armar lo construya.</summary>
        public void PrepararEntrada()
        {
            EscFila1.ScaleX = EscFila2.ScaleX = EscFila3.ScaleX = 0;
            Corte1.X = Corte2.X = Corte3.X = -4;          // los tramos, pegados: barra entera
            Pieza.Opacity = 0;
            Retal.Opacity = 0;
            Palabra.Opacity = 0;
            PalabraMov.X = -6;
        }

        /// <summary>Arma el logo empezando en el segundo <paramref name="t"/>. Dura unos 1,9 s.</summary>
        public void Armar(double t)
        {
            var salida = new CubicEase { EasingMode = EasingMode.EaseOut };
            Animar(EscFila1, ScaleTransform.ScaleXProperty, 0, 1, t, .7, salida);
            Animar(EscFila2, ScaleTransform.ScaleXProperty, 0, 1, t + .12, .7, salida);
            Animar(EscFila3, ScaleTransform.ScaleXProperty, 0, 1, t + .24, .7, salida);
            var corte = new QuadraticEase { EasingMode = EasingMode.EaseOut };
            Animar(Corte1, TranslateTransform.XProperty, -4, 0, t + .9, .45, corte);
            Animar(Corte2, TranslateTransform.XProperty, -4, 0, t + .96, .45, corte);
            Animar(Corte3, TranslateTransform.XProperty, -4, 0, t + 1.02, .45, corte);
            Animar(Pieza, OpacityProperty, 0, 1, t + 1.2, .45, null);
            Animar(Retal, OpacityProperty, 0, 1, t + 1.3, .45, null);
            Animar(Palabra, OpacityProperty, 0, 1, t + 1.1, .7, null);
            Animar(PalabraMov, TranslateTransform.XProperty, -6, 0, t + 1.1, .7, salida);
        }

        internal static void Animar(IAnimatable objetivo, DependencyProperty prop,
                                    double desde, double hasta, double inicio, double dur,
                                    IEasingFunction curva, EventHandler alTerminar = null)
        {
            var a = new DoubleAnimation(desde, hasta, TimeSpan.FromSeconds(dur))
            {
                BeginTime = TimeSpan.FromSeconds(inicio),
                EasingFunction = curva
            };
            if (alTerminar != null) a.Completed += alTerminar;
            objetivo.BeginAnimation(prop, a);
        }
    }
}
