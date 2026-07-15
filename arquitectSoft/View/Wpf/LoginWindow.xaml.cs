using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.IO;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using arquitectSoft.View.Wpf.Shaders;
using MySql.Data.MySqlClient;

namespace arquitectSoft.View.Wpf
{
    /// <summary>
    /// Versión WPF (cristal) de FrmLogin: valida usuario/contraseña contra la tabla
    /// usuario (QUERY_EXITS_USUARIO) y, si son correctos, abre el escritorio WPF.
    /// </summary>
    public partial class LoginWindow : Window
    {
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int val, int size);
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        private const int DWMWA_BORDER_COLOR = 34;
        private const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;
        private const int DWMWA_SYSTEMBACKDROP_TYPE = 38;
        private const int DWMWCP_ROUND = 2;
        private const int DWMSBT_TRANSIENTWINDOW = 3;
        private const uint DWMWA_COLOR_NONE = 0xFFFFFFFE;

        private bool _cerrando;

        /// <summary>true si el usuario se autenticó (el escritorio debe abrirse). Lo lee Program.Main.</summary>
        public bool LoginOk { get; private set; }

        public LoginWindow()
        {
            InitializeComponent();
            SourceInitialized += OnSourceInitialized;
            LiquidGlass.PrepararOculto(FrameRim, WinScale);
            MontarFondoEscritorio();   // el cristal refracta el wallpaper del PROGRAMA, no la pantalla

            Loaded += (s, e) =>
            {
                LiquidGlass.Apertura(FrameRim, WinScale);
                PrecargarRecordado();
            };
        }

        // ===== "Recordar mis datos" =====
        // El usuario se guarda en claro; la contraseña se cifra con DPAPI (atada a la
        // cuenta de Windows), de modo que el user.config no la expone en texto plano.

        private void PrecargarRecordado()
        {
            try
            {
                var cfg = Properties.Settings.Default;
                if (cfg.RecordarLogin && !string.IsNullOrEmpty(cfg.LoginUsuario))
                {
                    TxtUser.Text = cfg.LoginUsuario;
                    TxtPass.Password = Desproteger(cfg.LoginClaveProt);
                    ChkRecordar.IsChecked = true;
                    // Datos ya puestos: el foco va al botón para entrar de una.
                    BtnIngresar.Focus();
                    return;
                }
            }
            catch { /* si el settings está corrupto, arranca en limpio */ }
            TxtUser.Focus();
        }

        private void GuardarRecordado(string usuario, string clave)
        {
            var cfg = Properties.Settings.Default;
            if (ChkRecordar.IsChecked == true)
            {
                cfg.RecordarLogin = true;
                cfg.LoginUsuario = usuario;
                cfg.LoginClaveProt = Proteger(clave);
            }
            else
            {
                cfg.RecordarLogin = false;
                cfg.LoginUsuario = "";
                cfg.LoginClaveProt = "";
            }
            cfg.Save();
        }

        private static string Proteger(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return "";
            byte[] datos = System.Text.Encoding.UTF8.GetBytes(texto);
            byte[] cif = System.Security.Cryptography.ProtectedData.Protect(
                datos, null, System.Security.Cryptography.DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(cif);
        }

        private static string Desproteger(string protegido)
        {
            if (string.IsNullOrEmpty(protegido)) return "";
            try
            {
                byte[] cif = Convert.FromBase64String(protegido);
                byte[] datos = System.Security.Cryptography.ProtectedData.Unprotect(
                    cif, null, System.Security.Cryptography.DataProtectionScope.CurrentUser);
                return System.Text.Encoding.UTF8.GetString(datos);
            }
            catch { return ""; }
        }

        private void Ingresar_Click(object sender, RoutedEventArgs e)
        {
            string usuario = TxtUser.Text.Trim();
            string clave = TxtPass.Password;

            if (usuario.Length == 0 || clave.Length == 0)
            {
                MostrarError("Escribe tu usuario y tu contraseña.");
                return;
            }

            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            string nombreDesc = "";
            string nombre = "";
            int rol = Generals.Global.ROL_TECNICO_BASICO;
            int idUsuario = 0;

            try
            {
                con.Open(out fail);
                string[] param = { usuario, clave };
                MySqlDataReader row = con.ExecuteReader(Generals.Constantes.QUERY_EXITS_USUARIO, out fail, param);
                if (row != null && row.HasRows)
                {
                    while (row.Read())
                    {
                        nombre = row["Nombre"].ToString();
                        nombreDesc = row["usuario"].ToString() + "-" + nombre;
                        int.TryParse(Convert.ToString(row["rol"]), out rol);
                        int.TryParse(Convert.ToString(row["id"]), out idUsuario);
                    }
                }
                else
                {
                    fail = "No se encontró un usuario con los datos ingresados.";
                }
                con.Close();
            }
            catch (Exception ex)
            {
                fail = ex.Message;
                try { con.Close(); } catch { }
            }

            if (fail == "" && nombreDesc != "")
            {
                Generals.Global.NameConnect = nombreDesc;
                Generals.Global.Rol = rol;
                Generals.Global.Usuario = usuario;
                Generals.Global.Nombre = nombre;
                Generals.Global.UsuarioId = idUsuario;
                GuardarRecordado(usuario, clave);
                // Autenticado: cierra el login; Program.Main abre el escritorio. Si desde
                // allí se "Cierra sesión", el bucle de Main vuelve a crear un login nuevo.
                LoginOk = true;
                Close();
            }
            else
            {
                MostrarError(fail);
            }
        }

        // Pinta el GlassBackdrop con el MISMO wallpaper del escritorio del programa
        // (FondoApp.png, junto al .exe) y le aplica el shader de cristal suave. Así el
        // login parece flotar sobre el escritorio de la app antes de entrar.
        private void MontarFondoEscritorio()
        {
            try
            {
                string ruta = Path.Combine(Directory.GetCurrentDirectory(), "FondoApp.png");
                if (!File.Exists(ruta)) return;

                // Decodifica la imagen al ancho de la pantalla (no a full-res 14MB): así el
                // paneo al mover la ventana es fluido, sin re-muestrear una imagen gigante.
                ScreenCaptureHelper.EnsureMetrics();
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                if (ScreenCaptureHelper.PrimaryScreenWidth > 0)
                    bmp.DecodePixelWidth = ScreenCaptureHelper.PrimaryScreenWidth;
                bmp.UriSource = new Uri(ruta);
                bmp.EndInit();
                bmp.Freeze();

                // El fondo se comporta como wallpaper de la pantalla principal: la ventana es
                // una "lente" que revela la zona bajo su posición (al moverla, panea sobre la
                // imagen). Deformación de cristal MUY sutil, solo insinuada en el filo.
                LiquidGlass.MontarGlassImagen(this, GlassBackdrop, bmp);
                GlassBackdrop.Effect = new GlassyEffect { Strength = 0.008, EdgeStart = 0.90, Rim = 0.05 };
            }
            catch { /* si no está la imagen, el login queda con el tinte oscuro sin fondo */ }
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e) => Close();

        private void MostrarError(string msg)
        {
            LblError.Text = msg;
            LblError.Visibility = Visibility.Visible;
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
            catch { }
            HwndSource src = HwndSource.FromHwnd(hwnd);
            if (src != null && src.CompositionTarget != null)
                src.CompositionTarget.BackgroundColor = Colors.Transparent;
        }
    }
}
