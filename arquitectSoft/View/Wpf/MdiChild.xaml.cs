using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace arquitectSoft.View.Wpf
{
    /// <summary>
    /// Ventana "hija" dentro del canvas del escritorio (MDI propio): se mueve por la
    /// barra de tÃ­tulo, se redimensiona por la esquina, se trae al frente al tocarla,
    /// y maximiza DENTRO del canvas (no a toda la pantalla). Queda contenida en el canvas.
    /// </summary>
    public partial class MdiChild : UserControl
    {
        private Canvas _canvas;
        private bool _maximizada;
        private Rect _restaurar;

        public MdiChild()
        {
            InitializeComponent();
            DragThumb.DragDelta += DragThumb_DragDelta;
            ResizeThumb.DragDelta += ResizeThumb_DragDelta;
            PreviewMouseLeftButtonDown += (s, e) => TraerAlFrente();
        }

        public string Titulo { get { return LblTitulo.Text; } set { LblTitulo.Text = value; } }

        public void SetContenido(UIElement contenido) { Host.Content = contenido; }

        private Canvas Lienzo
        {
            get { return _canvas ?? (_canvas = ParentOfType<Canvas>(this)); }
        }

        public void TraerAlFrente()
        {
            var c = Lienzo;
            if (c == null) return;
            int max = 0;
            foreach (UIElement e in c.Children)
            {
                int z = Panel.GetZIndex(e);
                if (z > max) max = z;
            }
            Panel.SetZIndex(this, max + 1);
        }

        private void DragThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (_maximizada) return;
            var c = Lienzo; if (c == null) return;
            double x = Canvas.GetLeft(this); if (double.IsNaN(x)) x = 0;
            double y = Canvas.GetTop(this); if (double.IsNaN(y)) y = 0;
            x = Clamp(x + e.HorizontalChange, 0, Math.Max(0, c.ActualWidth - ActualWidth));
            y = Clamp(y + e.VerticalChange, 0, Math.Max(0, c.ActualHeight - ActualHeight));
            Canvas.SetLeft(this, x);
            Canvas.SetTop(this, y);
        }

        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (_maximizada) return;
            var c = Lienzo; if (c == null) return;
            double x = Canvas.GetLeft(this); if (double.IsNaN(x)) x = 0;
            double y = Canvas.GetTop(this); if (double.IsNaN(y)) y = 0;
            double w = Clamp(Width + e.HorizontalChange, MinWidth, c.ActualWidth - x);
            double h = Clamp(Height + e.VerticalChange, MinHeight, c.ActualHeight - y);
            Width = w; Height = h;
        }

        private void Maximizar_Click(object sender, RoutedEventArgs e)
        {
            var c = Lienzo; if (c == null) return;
            if (!_maximizada)
            {
                double x = Canvas.GetLeft(this); if (double.IsNaN(x)) x = 0;
                double y = Canvas.GetTop(this); if (double.IsNaN(y)) y = 0;
                _restaurar = new Rect(x, y, ActualWidth, ActualHeight);
                Canvas.SetLeft(this, 0); Canvas.SetTop(this, 0);
                Width = c.ActualWidth; Height = c.ActualHeight;
                _maximizada = true;
                BtnMax.Content = "";   // restaurar
            }
            else
            {
                Canvas.SetLeft(this, _restaurar.X); Canvas.SetTop(this, _restaurar.Y);
                Width = _restaurar.Width; Height = _restaurar.Height;
                _maximizada = false;
                BtnMax.Content = "";   // maximizar
            }
        }

        private void Cerrar_Click(object sender, RoutedEventArgs e)
        {
            var c = Lienzo;
            if (c != null) c.Children.Remove(this);
        }

        // Re-clampa al tamaÃ±o del canvas (p.ej. cuando el escritorio cambia de tamaÃ±o).
        public void AjustarAlCanvas()
        {
            var c = Lienzo; if (c == null) return;
            if (_maximizada) { Width = c.ActualWidth; Height = c.ActualHeight; return; }
            double x = Clamp(double.IsNaN(Canvas.GetLeft(this)) ? 0 : Canvas.GetLeft(this), 0, Math.Max(0, c.ActualWidth - ActualWidth));
            double y = Clamp(double.IsNaN(Canvas.GetTop(this)) ? 0 : Canvas.GetTop(this), 0, Math.Max(0, c.ActualHeight - ActualHeight));
            Canvas.SetLeft(this, x); Canvas.SetTop(this, y);
        }

        private static double Clamp(double v, double min, double max)
        {
            if (max < min) max = min;
            return v < min ? min : (v > max ? max : v);
        }

        private static T ParentOfType<T>(DependencyObject d) where T : DependencyObject
        {
            while (d != null && !(d is T)) d = System.Windows.Media.VisualTreeHelper.GetParent(d);
            return d as T;
        }
    }
}

