using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace arquitectSoft.View.Wpf
{
    /// <summary>
    /// Ventana "hija" dentro del canvas del escritorio (MDI propio). Se comporta como una
    /// ventana de Windows pero contenida en el lienzo: se mueve por la barra de título,
    /// se redimensiona por los 4 lados y las 4 esquinas, hace snap a los bordes
    /// (mitad/mitad, cuartos, maximizar), se minimiza a la barra de tareas, se maximiza
    /// dentro del canvas, se trae al frente al tocarla y se cierra.
    /// </summary>
    public partial class MdiChild : UserControl
    {
        private enum Estado { Normal, Maximizada, Snap, Minimizada }

        private Canvas _canvas;
        private Estado _estado = Estado.Normal;
        private Rect _restaurar;            // geometría a la que vuelve al restaurar
        private bool _arrastrando;
        private bool _cerrando;             // evita re-disparar el cierre durante su animación

        // Escala para las animaciones de abrir/cerrar (pop elástico, como el login).
        private readonly ScaleTransform _winScale = new ScaleTransform(1, 1);

        // Vista previa de snap: la pinta el escritorio (callback) durante el arrastre.
        public Action<Rect?> MostrarPreviewSnap;
        // Avisos para que el escritorio mantenga la barra de tareas.
        public event EventHandler Cerrada;
        public event EventHandler EstadoCambiado;

        public MdiChild()
        {
            InitializeComponent();

            // La ventana escala desde su centro; arranca oculta y la anima AnimarApertura().
            RenderTransformOrigin = new Point(0.5, 0.5);
            RenderTransform = _winScale;
            Opacity = 0;

            DragThumb.DragStarted += DragThumb_DragStarted;
            DragThumb.DragDelta += DragThumb_DragDelta;
            DragThumb.DragCompleted += DragThumb_DragCompleted;

            // Doble clic en la barra de título = maximizar / restaurar (como Windows).
            DragThumb.MouseDoubleClick += (s, e) => Maximizar_Click(null, null);

            // Agarres de redimensión.
            RzL.DragDelta += (s, e) => Redimensionar(e, true, false, false, false);
            RzR.DragDelta += (s, e) => Redimensionar(e, false, false, true, false);
            RzT.DragDelta += (s, e) => Redimensionar(e, false, true, false, false);
            RzB.DragDelta += (s, e) => Redimensionar(e, false, false, false, true);
            RzTL.DragDelta += (s, e) => Redimensionar(e, true, true, false, false);
            RzTR.DragDelta += (s, e) => Redimensionar(e, false, true, true, false);
            RzBL.DragDelta += (s, e) => Redimensionar(e, true, false, false, true);
            RzBR.DragDelta += (s, e) => Redimensionar(e, false, false, true, true);

            PreviewMouseLeftButtonDown += (s, e) => TraerAlFrente();
        }

        // ===================== Animaciones abrir / cerrar =====================
        // Mismo lenguaje que el login: entra con un "pop" elástico (gelatina) y sale
        // encogiendo con leve anticipación + desvanecido. Sólo toca escala y opacidad,
        // así se conserva la sombra de flotado del marco.

        /// <summary>Anima la aparición de la ventana. La llama el escritorio tras montarla.</summary>
        public void AnimarApertura()
        {
            var jelly = new ElasticEase { EasingMode = EasingMode.EaseOut, Oscillations = 2, Springiness = 5 };
            var fade = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(160)));
            var sx = new DoubleAnimation(0.7, 1, new Duration(TimeSpan.FromMilliseconds(480))) { EasingFunction = jelly };
            var sy = new DoubleAnimation(0.7, 1, new Duration(TimeSpan.FromMilliseconds(480))) { EasingFunction = jelly };

            BeginAnimation(OpacityProperty, fade);
            _winScale.BeginAnimation(ScaleTransform.ScaleXProperty, sx);
            _winScale.BeginAnimation(ScaleTransform.ScaleYProperty, sy);
        }

        /// <summary>Anima el cierre y ejecuta <paramref name="alTerminar"/> al acabar.</summary>
        private void AnimarCierre(Action alTerminar)
        {
            var easeIn = new BackEase { EasingMode = EasingMode.EaseIn, Amplitude = 0.5 };
            var dur = new Duration(TimeSpan.FromMilliseconds(180));
            var fade = new DoubleAnimation(0, new Duration(TimeSpan.FromMilliseconds(170)));
            var sx = new DoubleAnimation(0.82, dur) { EasingFunction = easeIn };
            var sy = new DoubleAnimation(0.82, dur) { EasingFunction = easeIn };

            fade.Completed += (s, e) => { if (alTerminar != null) alTerminar(); };

            BeginAnimation(OpacityProperty, fade);
            _winScale.BeginAnimation(ScaleTransform.ScaleXProperty, sx);
            _winScale.BeginAnimation(ScaleTransform.ScaleYProperty, sy);
        }

        public string Titulo { get { return LblTitulo.Text; } set { LblTitulo.Text = value; } }
        public bool EstaMinimizada { get { return _estado == Estado.Minimizada; } }

        /// <summary>Cambia el título en caliente y refresca la barra de tareas.</summary>
        public void CambiarTitulo(string t)
        {
            LblTitulo.Text = t ?? "";
            OnEstadoCambiado();
        }

        public void SetContenido(UIElement contenido) { Host.Content = contenido; }

        private System.Windows.Media.Visual _fondo;
        private Action _desmontarGlass;

        /// <summary>Activa el liquid glass refractando el fondo del escritorio (wallpaper).</summary>
        public void MontarGlass(System.Windows.Media.Visual fondo)
        {
            _fondo = fondo;
            AplicarGlass();
        }

        /// <summary>
        /// (Re)aplica el cristal según LiquidGlass.ModoRendimiento. Permite alternar
        /// cristal↔rendimiento en caliente sobre ventanas ya abiertas.
        /// </summary>
        public void AplicarGlass()
        {
            if (_desmontarGlass != null) { _desmontarGlass(); _desmontarGlass = null; }
            if (_fondo != null)
                _desmontarGlass = LiquidGlass.MontarGlassMdi(this, GlassBackdrop, _fondo);
        }

        // Recorta el contenido (cristal incluido) a esquinas redondeadas, para que el shader
        // no rellene las esquinas y se salga del marco.
        private void Recorte_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Recorte.Clip = new System.Windows.Media.RectangleGeometry(
                new Rect(0, 0, Recorte.ActualWidth, Recorte.ActualHeight), 11, 11);
        }

        private Canvas Lienzo
        {
            get { return _canvas ?? (_canvas = ParentOfType<Canvas>(this)); }
        }

        // ===================== Z-order =====================
        public void TraerAlFrente()
        {
            var c = Lienzo;
            if (c == null) return;
            int max = 0;
            foreach (UIElement el in c.Children)
            {
                int z = Panel.GetZIndex(el);
                if (z > max) max = z;
            }
            Panel.SetZIndex(this, max + 1);
            OnEstadoCambiado();
        }

        public bool EsFrontal()
        {
            var c = Lienzo;
            if (c == null) return false;
            int z = Panel.GetZIndex(this);
            foreach (UIElement el in c.Children)
            {
                var o = el as MdiChild;
                if (o == null || o == this || o._estado == Estado.Minimizada) continue;
                if (Panel.GetZIndex(el) > z) return false;
            }
            return true;
        }

        // ===================== Arrastre + snap =====================
        private void DragThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            _arrastrando = true;
            // Si está maximizada/snap, "salta" a tamaño normal bajo el cursor (como Windows).
            if (_estado == Estado.Maximizada || _estado == Estado.Snap)
            {
                var c = Lienzo;
                double nw = _restaurar.Width > 0 ? _restaurar.Width : 480;
                double nh = _restaurar.Height > 0 ? _restaurar.Height : 360;
                Point p = c != null ? Mouse.GetPosition(c) : new Point(40, 40);
                Width = nw; Height = nh;
                Canvas.SetLeft(this, p.X - nw / 2);
                Canvas.SetTop(this, Math.Max(0, p.Y - 17)); // 17 ≈ media barra de título
                _estado = Estado.Normal;
                OnEstadoCambiado();
            }
        }

        private void DragThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var c = Lienzo; if (c == null) return;
            double x = GetLeft(); double y = GetTop();
            x = Clamp(x + e.HorizontalChange, 0, Math.Max(0, c.ActualWidth - ActualWidth));
            y = Clamp(y + e.VerticalChange, 0, Math.Max(0, c.ActualHeight - ActualHeight));
            Canvas.SetLeft(this, x);
            Canvas.SetTop(this, y);

            // Vista previa de la zona de snap según dónde esté el cursor.
            Rect? zona = ZonaSnap(Mouse.GetPosition(c), c);
            if (MostrarPreviewSnap != null) MostrarPreviewSnap(zona);
        }

        private void DragThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            _arrastrando = false;
            var c = Lienzo; if (c == null) return;
            Rect? zona = ZonaSnap(Mouse.GetPosition(c), c);
            if (MostrarPreviewSnap != null) MostrarPreviewSnap(null);
            if (zona.HasValue) AplicarSnap(zona.Value);
        }

        /// <summary>Devuelve el rectángulo destino si el cursor está en una zona de snap, o null.</summary>
        private Rect? ZonaSnap(Point p, Canvas c)
        {
            double W = c.ActualWidth, H = c.ActualHeight;
            const double borde = 16;     // franja sensible junto al borde
            const double esquina = 80;   // alto/ancho de las esquinas (cuartos)

            bool izq = p.X <= borde, der = p.X >= W - borde;
            bool arr = p.Y <= borde, aba = p.Y >= H - borde;

            // Esquinas → cuartos
            if (izq && p.Y <= esquina) return new Rect(0, 0, W / 2, H / 2);
            if (der && p.Y <= esquina) return new Rect(W / 2, 0, W / 2, H / 2);
            if (izq && p.Y >= H - esquina) return new Rect(0, H / 2, W / 2, H / 2);
            if (der && p.Y >= H - esquina) return new Rect(W / 2, H / 2, W / 2, H / 2);
            // Lados → mitades
            if (izq) return new Rect(0, 0, W / 2, H);
            if (der) return new Rect(W / 2, 0, W / 2, H);
            // Borde superior → maximizar
            if (arr) return new Rect(0, 0, W, H);
            // (no usamos el borde inferior para snap: ahí va la barra de tareas)
            _ = aba;
            return null;
        }

        private void AplicarSnap(Rect r)
        {
            if (_estado == Estado.Normal) GuardarRestaurar();
            Canvas.SetLeft(this, r.X); Canvas.SetTop(this, r.Y);
            Width = r.Width; Height = r.Height;
            // Maximizar (ocupa todo) se marca como Maximizada; el resto como Snap.
            var c = Lienzo;
            bool full = c != null && r.Width >= c.ActualWidth - 1 && r.Height >= c.ActualHeight - 1;
            _estado = full ? Estado.Maximizada : Estado.Snap;
            ActualizarIconoMax();
            OnEstadoCambiado();
        }

        // ===================== Redimensión =====================
        private void Redimensionar(DragDeltaEventArgs e, bool izq, bool arr, bool der, bool aba)
        {
            if (_estado == Estado.Maximizada) return;
            var c = Lienzo; if (c == null) return;
            // Cualquier redimensión rompe el estado de snap.
            if (_estado == Estado.Snap) { _estado = Estado.Normal; ActualizarIconoMax(); OnEstadoCambiado(); }

            double x = GetLeft(), y = GetTop();
            double w = double.IsNaN(Width) ? ActualWidth : Width;
            double h = double.IsNaN(Height) ? ActualHeight : Height;
            double dx = e.HorizontalChange, dy = e.VerticalChange;

            if (der) w = w + dx;
            if (aba) h = h + dy;
            if (izq) { double nw = w - dx; if (nw >= MinWidth && x + dx >= 0) { w = nw; x += dx; } }
            if (arr) { double nh = h - dy; if (nh >= MinHeight && y + dy >= 0) { h = nh; y += dy; } }

            w = Clamp(w, MinWidth, c.ActualWidth - x);
            h = Clamp(h, MinHeight, c.ActualHeight - y);
            Canvas.SetLeft(this, x); Canvas.SetTop(this, y);
            Width = w; Height = h;
        }

        // ===================== Botones de ventana =====================
        private void Maximizar_Click(object sender, RoutedEventArgs e)
        {
            var c = Lienzo; if (c == null) return;
            if (_estado != Estado.Maximizada)
            {
                if (_estado == Estado.Normal) GuardarRestaurar();
                Canvas.SetLeft(this, 0); Canvas.SetTop(this, 0);
                Width = c.ActualWidth; Height = c.ActualHeight;
                _estado = Estado.Maximizada;
            }
            else
            {
                RestaurarGeometria();
                _estado = Estado.Normal;
            }
            ActualizarIconoMax();
            OnEstadoCambiado();
        }

        private void Minimizar_Click(object sender, RoutedEventArgs e) { Minimizar(); }

        public void Minimizar()
        {
            if (_estado != Estado.Minimizada && _estado != Estado.Maximizada && _estado != Estado.Snap)
                GuardarRestaurar();
            else if (_estado != Estado.Minimizada)
            {
                // venía de max/snap: guarda su rect actual para volver a él
                _restaurar = RectActual();
            }
            Visibility = Visibility.Collapsed;
            _estado = Estado.Minimizada;
            OnEstadoCambiado();
        }

        /// <summary>Restaura desde la barra de tareas y la trae al frente.</summary>
        public void RestaurarDesdeBarra()
        {
            if (_estado == Estado.Minimizada)
            {
                Visibility = Visibility.Visible;
                Canvas.SetLeft(this, _restaurar.X); Canvas.SetTop(this, _restaurar.Y);
                Width = _restaurar.Width; Height = _restaurar.Height;
                _estado = Estado.Normal;
                ActualizarIconoMax();
                OnEstadoCambiado();
            }
            TraerAlFrente();
        }

        /// <summary>Clic en el botón de la barra de tareas: minimiza/restaura/trae al frente.</summary>
        public void AlternarDesdeBarra()
        {
            if (_estado == Estado.Minimizada) RestaurarDesdeBarra();
            else if (EsFrontal()) Minimizar();
            else TraerAlFrente();
        }

        private void Cerrar_Click(object sender, RoutedEventArgs e)
        {
            if (_cerrando) return;
            _cerrando = true;
            AnimarCierre(() =>
            {
                var c = Lienzo;
                if (c != null) c.Children.Remove(this);
                if (Cerrada != null) Cerrada(this, EventArgs.Empty);
            });
        }

        // ===================== Ajuste al cambiar el canvas =====================
        public void AjustarAlCanvas()
        {
            var c = Lienzo; if (c == null) return;
            if (_estado == Estado.Maximizada) { Canvas.SetLeft(this, 0); Canvas.SetTop(this, 0); Width = c.ActualWidth; Height = c.ActualHeight; return; }
            if (_estado == Estado.Minimizada) return;
            double x = Clamp(GetLeft(), 0, Math.Max(0, c.ActualWidth - ActualWidth));
            double y = Clamp(GetTop(), 0, Math.Max(0, c.ActualHeight - ActualHeight));
            Canvas.SetLeft(this, x); Canvas.SetTop(this, y);
        }

        // ===================== Helpers =====================
        private void GuardarRestaurar() { _restaurar = RectActual(); }

        private void RestaurarGeometria()
        {
            Canvas.SetLeft(this, _restaurar.X); Canvas.SetTop(this, _restaurar.Y);
            Width = _restaurar.Width; Height = _restaurar.Height;
        }

        private Rect RectActual()
        {
            double w = double.IsNaN(Width) ? ActualWidth : Width;
            double h = double.IsNaN(Height) ? ActualHeight : Height;
            return new Rect(GetLeft(), GetTop(), w, h);
        }

        private double GetLeft() { double v = Canvas.GetLeft(this); return double.IsNaN(v) ? 0 : v; }
        private double GetTop() { double v = Canvas.GetTop(this); return double.IsNaN(v) ? 0 : v; }

        private void ActualizarIconoMax()
        {
            // E922 = maximizar, E923 = restaurar (Segoe MDL2 Assets)
            BtnMax.Content = (_estado == Estado.Maximizada || _estado == Estado.Snap) ? "" : "";
        }

        private void OnEstadoCambiado() { if (EstadoCambiado != null) EstadoCambiado(this, EventArgs.Empty); }

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
