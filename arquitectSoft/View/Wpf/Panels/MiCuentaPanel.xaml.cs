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
                RefrescarCabecera();
                TxtNombre.Text = Generals.Global.Nombre ?? "";
                TxtNombre.Focus();
                TxtNombre.SelectAll();
            };
        }

        private Window Owner { get { return Window.GetWindow(this); } }

        /// <summary>Rótulo "Usuario · Nombre · Rol" con los datos actuales de la sesión.</summary>
        private void RefrescarCabecera()
        {
            string nombre = Generals.Global.Nombre ?? "";
            LblUsuario.Text = "Usuario: " + Generals.Global.Usuario +
                              (nombre != "" ? "  ·  " + nombre : "") +
                              "   ·   " + Generals.Global.NombreRol(Generals.Global.Rol);
        }

        private void Mostrar(string msg)
        {
            LblEstado.Text = msg;
            LblEstado.Visibility = Visibility.Visible;
        }

        private void MostrarNombre(string msg)
        {
            LblEstadoNombre.Text = msg;
            LblEstadoNombre.Visibility = Visibility.Visible;
        }

        private void GuardarNombre_Click(object sender, RoutedEventArgs e)
        {
            string nuevo = (TxtNombre.Text ?? "").Trim();

            if (nuevo.Length == 0)
            {
                MostrarNombre("Escribe un nombre.");
                return;
            }
            if (nuevo == (Generals.Global.Nombre ?? "").Trim())
            {
                MostrarNombre("El nombre no ha cambiado.");
                return;
            }

            var dto = new Dto.UsuarioDto();
            string resul = dto.CambiarNombre(Generals.Global.UsuarioId, nuevo);

            // Refleja el cambio en la sesión en curso (rótulos, saludo, export…).
            Generals.Global.Nombre = nuevo;
            Generals.Global.NameConnect = Generals.Global.Usuario + "-" + nuevo;
            RefrescarCabecera();

            GlassDialog.Informar(Owner, "Mi cuenta", resul);
            LblEstadoNombre.Visibility = Visibility.Collapsed;
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
