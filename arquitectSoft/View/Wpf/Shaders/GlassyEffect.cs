using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace arquitectSoft.View.Wpf.Shaders
{
    /// <summary>
    /// Efecto de refracción "liquid glass" (Pixel Shader 2.0) que distorsiona la
    /// imagen sobre la que se aplica (el fondo capturado del escritorio). Doblez de
    /// borde + leve aberración cromática + realce de filo. El sampler s0 es la
    /// entrada implícita (lo que renderiza el propio elemento).
    /// </summary>
    public sealed class GlassyEffect : ShaderEffect
    {
        private static readonly PixelShader _shader = new PixelShader
        {
            UriSource = new Uri("pack://application:,,,/arquitectSoft;component/View/Wpf/Shaders/GlassyEffect.ps", UriKind.Absolute)
        };

        public static readonly DependencyProperty InputProperty =
            RegisterPixelShaderSamplerProperty("Input", typeof(GlassyEffect), 0);

        public static readonly DependencyProperty EdgeStartProperty =
            DependencyProperty.Register("EdgeStart", typeof(double), typeof(GlassyEffect),
                new UIPropertyMetadata(0.55, PixelShaderConstantCallback(0)));

        public static readonly DependencyProperty StrengthProperty =
            DependencyProperty.Register("Strength", typeof(double), typeof(GlassyEffect),
                new UIPropertyMetadata(0.06, PixelShaderConstantCallback(1)));

        public static readonly DependencyProperty RimProperty =
            DependencyProperty.Register("Rim", typeof(double), typeof(GlassyEffect),
                new UIPropertyMetadata(0.20, PixelShaderConstantCallback(2)));

        public GlassyEffect()
        {
            PixelShader = _shader;
            UpdateShaderValue(InputProperty);
            UpdateShaderValue(EdgeStartProperty);
            UpdateShaderValue(StrengthProperty);
            UpdateShaderValue(RimProperty);
        }

        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }
        public double EdgeStart
        {
            get { return (double)GetValue(EdgeStartProperty); }
            set { SetValue(EdgeStartProperty, value); }
        }
        public double Strength
        {
            get { return (double)GetValue(StrengthProperty); }
            set { SetValue(StrengthProperty, value); }
        }
        public double Rim
        {
            get { return (double)GetValue(RimProperty); }
            set { SetValue(RimProperty, value); }
        }
    }
}
