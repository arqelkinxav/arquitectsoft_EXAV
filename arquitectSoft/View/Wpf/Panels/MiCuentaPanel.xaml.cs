using System;
using System.Windows;
using System.Windows.Controls;

namespace arquitectSoft.View.Wpf.Panels
{
    /// <summary>
    /// Panel disponible para CUALQUIER usuario: cambiar su propia contraseña.
    /// Verifica la clave actual contra la BD antes de aplicar la nueva.
    /// </summary>
    public partial class MiCuentaPanel : UserControl
    {
        public MiCuentaPanel()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                string nombre = "";
                var partes = (Generals.Global.NameConnect ?? "").Split('-');
                if (partes.Length > 1) nombre = partes[1];
                LblUsuario.Text = "Usuario: " + Generals.Global.Usuario +
                                  (nombre != "" ? "  ·  " + nombre : "") +
                                  "   ·   " + Generals.Global.NombreRol(Generals.Global.Rol);
                TxtActual.Focus();
            };
        }

        private Window Owner { get { return Window.GetWindow(this); } }

        private void Mostrar(string msg)
        {
            LblEstado.Text = msg;
            LblEstado.Visibility = Visibility.Visible;
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            string actual = TxtActual.Password;
            string nueva = TxtNueva.Password;
            string confirmar = TxtConfirmar.Password;

            if (actual.Length == 0 || nueva.Length == 0)
            {
                Mostrar("Escribe tu contraseña actual y la nueva.");
                return;
            }
            if (nueva != confirmar)
            {
                Mostrar("La nueva contraseña y su repetición no coinciden.");
                return;
            }
            if (nueva == actual)
            {
                Mostrar("La nueva contraseña debe ser distinta de la actual.");
                return;
            }

            var dto = new Dto.UsuarioDto();
            if (!dto.VerificarClave(Generals.Global.UsuarioId, actual))
            {
                Mostrar("La contraseña actual no es correcta.");
                return;
            }

            string resul = dto.CambiarClave(Generals.Global.UsuarioId, nueva);
            GlassDialog.Informar(Owner, "Mi cuenta", resul);

            TxtActual.Clear();
            TxtNueva.Clear();
            TxtConfirmar.Clear();
            LblEstado.Visibility = Visibility.Collapsed;
            TxtActual.Focus();
        }
    }
}
