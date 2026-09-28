using System;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace arquitectSoft.View.Wpf
{
    /// <summary>
    /// Modo diurno / nocturno de toda la app WPF.
    ///
    /// Los colores viven en Temas/TemaOscuro.xaml y Temas/TemaClaro.xaml (mismas claves,
    /// valores distintos) y las pantallas los piden con {DynamicResource ...}. Aplicar()
    /// cambia el diccionario de Application.Current.Resources, así que TODO lo abierto se
    /// repinta al momento. Lo que se pinta por código (fondo, barra de título de Windows,
    /// filas coloreadas) escucha el evento Cambiado.
    ///
    /// La elección se guarda por usuario de Windows en %AppData%\arquitectSoft\tema.txt
    /// (no en Z: ni en la base: cada técnico tiene su gusto).
    /// </summary>
    public static class Tema
    {
        public static bool EsOscuro { get; private set; } = true;

        /// <summary>Se dispara tras cambiar de modo (ya con los recursos nuevos puestos).</summary>
        public static event EventHandler Cambiado;

        private static string RutaPreferencia =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "arquitectSoft", "tema.txt");

        /// <summary>
        /// Crea la Application de WPF (el programa arranca desde WinForms y no la tiene)
        /// y aplica el modo guardado. Llamar una vez al inicio, antes de abrir ventanas.
        /// </summary>
        public static void Iniciar()
        {
            if (Application.Current == null)
                new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };

            // El tema va primero: Comun.xaml usa sus colores. Aplicar() lo reemplaza siempre en el índice 0.
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/View/Wpf/Temas/TemaOscuro.xaml")
            });
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/View/Wpf/Temas/Comun.xaml")
            });

            bool oscuro = true;
            try
            {
                if (File.Exists(RutaPreferencia))
                    oscuro = File.ReadAllText(RutaPreferencia).Trim() != "claro";
            }
            catch { /* sin preferencia: nocturno, como siempre */ }
            Aplicar(oscuro, guardar: false);
        }

        public static void Alternar() => Aplicar(!EsOscuro, guardar: true);

        public static void Aplicar(bool oscuro, bool guardar)
        {
            var nuevo = new ResourceDictionary
            {
                Source = new Uri(oscuro ? "pack://application:,,,/View/Wpf/Temas/TemaOscuro.xaml"
                                        : "pack://application:,,,/View/Wpf/Temas/TemaClaro.xaml")
            };
            Application.Current.Resources.MergedDictionaries[0] = nuevo;
            EsOscuro = oscuro;

            if (guardar)
            {
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(RutaPreferencia));
                    File.WriteAllText(RutaPreferencia, oscuro ? "oscuro" : "claro");
                }
                catch { /* no poder guardar el gusto nunca impide trabajar */ }
            }
            Cambiado?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>Pincel del tema actual por clave (para lo que se pinta por código).</summary>
        public static Brush Pincel(string clave) =>
            Application.Current?.TryFindResource(clave) as Brush ?? Brushes.Transparent;

        /// <summary>
        /// Imagen de fondo del modo actual, junto al .exe: FondoApp.png (noche) o
        /// FondoApp_Dia.png (día). Si falta la de día, se usa la de noche.
        /// </summary>
        public static string RutaFondo()
        {
            string dir = Directory.GetCurrentDirectory();
            if (!EsOscuro)
            {
                string dia = Path.Combine(dir, "FondoApp_Dia.png");
                if (File.Exists(dia)) return dia;
            }
            return Path.Combine(dir, "FondoApp.png");
        }

        /// <summary>Valor para DWMWA_USE_IMMERSIVE_DARK_MODE (barra/acrílico de Windows).</summary>
        public static int DwmOscuro => EsOscuro ? 1 : 0;
    }
}
