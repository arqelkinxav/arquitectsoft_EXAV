using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace arquitectSoft.View.Wpf.Panels
{
    /// <summary>
    /// Panel SOLO para administradores: crear, editar y eliminar usuarios, fijar su
    /// contraseña y su nivel de permiso (rol). Lista a la izquierda + formulario a la
    /// derecha. Reutiliza Dto.UsuarioDto.
    /// </summary>
    public partial class UsuariosPanel : UserControl
    {
        private DataTable _tabla;
        private int _id;            // 0 = formulario en modo "nuevo"

        public UsuariosPanel()
        {
            InitializeComponent();
            Loaded += (s, e) => { if (_tabla == null) { CargarLista(); Nuevo(); } };
        }

        private Window Owner { get { return Window.GetWindow(this); } }

        // ===== Carga de la lista =====
        private void CargarLista()
        {
            try
            {
                DataTable dt = new Dto.UsuarioDto().GetUsuarios();
                if (!dt.Columns.Contains("RolTexto")) dt.Columns.Add("RolTexto", typeof(string));
                foreach (DataRow r in dt.Rows)
                {
                    int rol;
                    int.TryParse(Convert.ToString(r["rol"]), out rol);
                    r["RolTexto"] = Generals.Global.NombreRol(rol);
                }
                dt.AcceptChanges();
                _tabla = dt;
                GridUsuarios.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                Mostrar("No se pudo cargar la lista: " + ex.Message);
            }
        }

        private void Mostrar(string msg)
        {
            LblEstado.Text = msg;
            LblEstado.Visibility = string.IsNullOrEmpty(msg) ? Visibility.Collapsed : Visibility.Visible;
        }

        // ===== Selección de la lista → carga el formulario =====
        private void Grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var drv = GridUsuarios.SelectedItem as DataRowView;
            if (drv == null) return;

            int.TryParse(Convert.ToString(drv.Row["id"]), out _id);
            TxtUsuario.Text = Convert.ToString(drv.Row["usuario"]);
            TxtNombre.Text = Convert.ToString(drv.Row["Nombre"]);
            TxtClave.Text = Convert.ToString(drv.Row["contrasena"]);
            int rol;
            int.TryParse(Convert.ToString(drv.Row["rol"]), out rol);
            CmbRol.SelectedIndex = (rol >= 0 && rol <= 2) ? rol : 2;

            LblTitulo.Text = "Editar usuario";
            Mostrar("");
        }

        // ===== Nuevo =====
        private void Nuevo_Click(object sender, RoutedEventArgs e) => Nuevo();

        private void Nuevo()
        {
            _id = 0;
            GridUsuarios.SelectedItem = null;
            TxtUsuario.Text = "";
            TxtNombre.Text = "";
            TxtClave.Text = "";
            CmbRol.SelectedIndex = 2;   // técnico básico por defecto
            LblTitulo.Text = "Nuevo usuario";
            Mostrar("");
            TxtUsuario.Focus();
        }

        // ===== Guardar (alta o edición) =====
        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            string login = TxtUsuario.Text.Trim();
            string nombre = TxtNombre.Text.Trim();
            string clave = TxtClave.Text;
            int rol = CmbRol.SelectedIndex < 0 ? Generals.Global.ROL_TECNICO_BASICO : CmbRol.SelectedIndex;

            if (login == "") { Mostrar("Escribe el usuario para iniciar sesión."); return; }
            if (clave == "") { Mostrar("Escribe una contraseña."); return; }
            if (nombre == "") nombre = login;

            var dto = new Dto.UsuarioDto();
            if (dto.LoginEnUso(login, _id))
            {
                Mostrar("Ya existe otro usuario con el login \"" + login + "\".");
                return;
            }

            // Evita que un admin se quite a sí mismo el rol de administrador y se bloquee.
            if (_id != 0 && _id == Generals.Global.UsuarioId && rol != Generals.Global.ROL_ADMIN)
            {
                if (!GlassDialog.Pregunta(Owner, "Usuarios",
                    "Estás quitándote a ti mismo el permiso de Administrador. Perderás el acceso a esta pantalla al volver a entrar. ¿Continuar?"))
                    return;
            }

            string resul = dto.GuardarUsuario(_id, login, clave, nombre, rol);
            CargarLista();
            Nuevo();
            Mostrar(resul);
        }

        // ===== Eliminar =====
        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            if (_id == 0) { Mostrar("Selecciona un usuario de la lista para eliminar."); return; }

            if (_id == Generals.Global.UsuarioId)
            {
                Mostrar("No puedes eliminar el usuario con el que iniciaste sesión.");
                return;
            }

            // No dejar la base sin ningún administrador.
            int rolSel = CmbRol.SelectedIndex;
            if (rolSel == Generals.Global.ROL_ADMIN && ContarAdmins() <= 1)
            {
                Mostrar("No puedes eliminar el único administrador que queda.");
                return;
            }

            if (!GlassDialog.Pregunta(Owner, "Usuarios",
                "¿Seguro que quieres eliminar el usuario \"" + TxtUsuario.Text + "\"?")) return;

            string resul = new Dto.UsuarioDto().EliminarUsuario(_id);
            CargarLista();
            Nuevo();
            Mostrar(resul);
        }

        private int ContarAdmins()
        {
            int n = 0;
            if (_tabla == null) return 0;
            foreach (DataRow r in _tabla.Rows)
            {
                int rol;
                int.TryParse(Convert.ToString(r["rol"]), out rol);
                if (rol == Generals.Global.ROL_ADMIN) n++;
            }
            return n;
        }
    }
}
