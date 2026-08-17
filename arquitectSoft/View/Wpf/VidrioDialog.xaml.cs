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
    /// <summary>Un tipo de vidrio del catálogo, para los desplegables del diálogo.</summary>
    public class TipoVidrioItem
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    /// <summary>Fila del diálogo: un sistema del proyecto y el vidrio elegido para él.</summary>
    public class FilaSistemaVidrio
    {
        public string Prefijo { get; set; }
        public string Titulo { get; set; }
        public string Detalle { get; set; }
        public bool Configurado { get; set; }
        public List<TipoVidrioItem> Tipos { get; set; }
        public int IdTipo { get; set; }

        public Visibility VisibleCombo { get { return Configurado ? Visibility.Visible : Visibility.Collapsed; } }
        public Visibility VisibleAviso { get { return Configurado ? Visibility.Collapsed : Visibility.Visible; } }
    }

    /// <summary>
    /// Diálogo del análisis para elegir el TIPO DE VIDRIO de cada sistema del proyecto. Lista los
    /// sistemas encontrados en los TXT cargados; los que no estén dados de alta en "Vidrios"
    /// aparecen igual pero marcados SIN DEPENDENCIA y sin poder cambiarse, para que se vea el
    /// olvido en vez de pasar en silencio.
    /// </summary>
    public partial class VidrioDialog : Window
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

        private readonly List<FilaSistemaVidrio> _filas = new List<FilaSistemaVidrio>();

        /// <summary>Tipo elegido para cada sistema (prefijo → id de tipo). Léelo si el diálogo se aceptó.</summary>
        public Dictionary<string, int> Seleccion { get; private set; }

        public VidrioDialog()
        {
            InitializeComponent();
            Seleccion = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            SourceInitialized += OnSourceInitialized;
            LiquidGlass.PrepararOculto(FrameRim, WinScale);
            Loaded += (s, e) => LiquidGlass.Apertura(FrameRim, WinScale);
        }

        /// <summary>
        /// Arma la lista a partir de los sistemas del proyecto y del resolver (catálogo de tipos),
        /// dejando marcado lo que ya estuviera elegido en una corrida anterior.
        /// </summary>
        public void Cargar(IList<SistemaVidrio> sistemas, VidrioResolver resolver,
                           IDictionary<string, int> seleccionActual)
        {
            var tipos = new List<TipoVidrioItem>();
            foreach (var par in resolver.Tipos) tipos.Add(new TipoVidrioItem { Id = par.Key, Nombre = par.Value });

            int sinConfigurar = 0;
            foreach (SistemaVidrio s in sistemas)
            {
                var fila = new FilaSistemaVidrio
                {
                    Prefijo = s.Prefijo,
                    Titulo = string.IsNullOrEmpty(s.Descripcion) ? s.Prefijo : s.Prefijo + " · " + s.Descripcion,
                    Configurado = s.Configurado,
                    Tipos = tipos
                };

                if (s.Configurado)
                {
                    fila.Detalle = string.IsNullOrEmpty(s.TipoEstandar)
                        ? "Sin tipo estándar definido."
                        : "En la base está cargado como " + s.TipoEstandar + ".";

                    int elegido;
                    if (seleccionActual != null && seleccionActual.TryGetValue(s.Prefijo, out elegido) && elegido > 0)
                        fila.IdTipo = elegido;
                    else
                        fila.IdTipo = s.IdTipoEstandar;
                }
                else
                {
                    sinConfigurar++;
                    fila.Detalle = "Este sistema no está dado de alta en Vidrios: se calculará tal como está en la base.";
                }

                _filas.Add(fila);
            }

            ListaSistemas.ItemsSource = _filas;

            if (_filas.Count == 0)
                LblNota.Text = "No se encontraron sistemas. Carga primero los archivos del proyecto.";
            else if (sinConfigurar > 0)
                LblNota.Text = sinConfigurar == 1
                    ? "Hay 1 sistema sin dependencia configurada. Si debería tenerla, créala en la pantalla Vidrios."
                    : "Hay " + sinConfigurar + " sistemas sin dependencia configurada. Si deberían tenerla, créalas en la pantalla Vidrios.";
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

        private void Aceptar_Click(object sender, RoutedEventArgs e)
        {
            foreach (FilaSistemaVidrio f in _filas)
                if (f.Configurado && f.IdTipo > 0) Seleccion[f.Prefijo] = f.IdTipo;

            DialogResult = true;
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
