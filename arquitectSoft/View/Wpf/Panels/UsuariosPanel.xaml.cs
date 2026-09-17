using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Perfil = arquitectSoft.Generals.BotonesPerfil.Perfil;

namespace arquitectSoft.View.Wpf.Panels
{
    /// <summary>
    /// Panel del administrador (o de un perfil al que se le dé, con límites): usuarios (lista + formulario) y perfiles de permiso
    /// (qué botones de la barra ve cada uno). El perfil de un usuario es su columna `rol`:
    /// 0 = Administrador, el resto sale de beta_perfil (ver Generals.BotonesPerfil).
    /// </summary>
    public partial class UsuariosPanel : UserControl
    {
        private DataTable _tabla;
        private int _id;            // 0 = formulario en modo "nuevo"
        private List<Perfil> _perfiles = new List<Perfil>();

        public UsuariosPanel()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                if (_tabla != null) return;
                AplicarLimites();
                CargarPerfiles(Generals.Global.ROL_TECNICO_BASICO);
                CargarLista();
                Nuevo();
            };
        }

        private Window Owner { get { return Window.GetWindow(this); } }

        // Quien entra sin ser administrador (un "asistente" al que se le dio el botón Usuarios)
        // gestiona usuarios y perfiles, pero no elimina nada ni da o toca el perfil Administrador:
        // si pudiera, se haría administrador él mismo.
        private static bool EsAdmin { get { return Generals.Global.EsAdmin; } }

        private void AplicarLimites()
        {
            if (EsAdmin) return;
            BtnEliminar.Visibility = Visibility.Collapsed;
            BtnEliminarPerfil.Visibility = Visibility.Collapsed;
        }

        private bool _editandoAdmin;   // el usuario cargado en el formulario es administrador

        private string NombrePerfil(int rol)
        {
            if (rol == Generals.Global.ROL_ADMIN) return "Administrador";
            var p = _perfiles.FirstOrDefault(x => x.Id == rol);
            return p != null ? p.Nombre : "(perfil " + rol + " no existe)";
        }

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
                    r["RolTexto"] = NombrePerfil(rol);
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

        /// <summary>Combo del usuario: Administrador + los perfiles de la base.</summary>
        private void LlenarComboUsuario()
        {
            int actual = RolElegido();
            var items = new List<Perfil>();
            if (EsAdmin) items.Add(new Perfil { Id = Generals.Global.ROL_ADMIN, Nombre = "Administrador" });
            items.AddRange(_perfiles);
            CmbRol.ItemsSource = items;
            ElegirRol(actual);
        }

        private int RolElegido()
        {
            var p = CmbRol.SelectedItem as Perfil;
            return p != null ? p.Id : Generals.Global.ROL_TECNICO_BASICO;
        }

        private void ElegirRol(int rol)
        {
            var items = CmbRol.ItemsSource as List<Perfil>;
            if (items == null) return;
            CmbRol.SelectedItem = items.FirstOrDefault(x => x.Id == rol)
                ?? items.FirstOrDefault(x => x.Id == Generals.Global.ROL_TECNICO_BASICO)
                ?? items.LastOrDefault();
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
            ElegirRol(rol);

            _editandoAdmin = rol == Generals.Global.ROL_ADMIN;
            LblTitulo.Text = "Editar usuario";
            Mostrar(_editandoAdmin && !EsAdmin ? "Los administradores solo los puede cambiar un administrador." : "");
        }

        // ===== Nuevo =====
        private void Nuevo_Click(object sender, RoutedEventArgs e) => Nuevo();

        private void Nuevo()
        {
            _id = 0;
            _editandoAdmin = false;
            GridUsuarios.SelectedItem = null;
            TxtUsuario.Text = "";
            TxtNombre.Text = "";
            TxtClave.Text = "";
            ElegirRol(Generals.Global.ROL_TECNICO_BASICO);   // técnico básico por defecto
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
            int rol = RolElegido();

            if (!EsAdmin && (_editandoAdmin || rol == Generals.Global.ROL_ADMIN))
            {
                Mostrar("Los administradores solo los puede crear o cambiar un administrador.");
                return;
            }
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
            if (EsAdmin && _id != 0 && _id == Generals.Global.UsuarioId && rol != Generals.Global.ROL_ADMIN)
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
            if (!EsAdmin) return;
            if (_id == 0) { Mostrar("Selecciona un usuario de la lista para eliminar."); return; }

            if (_id == Generals.Global.UsuarioId)
            {
                Mostrar("No puedes eliminar el usuario con el que iniciaste sesión.");
                return;
            }

            // No dejar la base sin ningún administrador.
            if (RolElegido() == Generals.Global.ROL_ADMIN && ContarAdmins() <= 1)
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

        // ===== Perfiles =====
        private readonly Dictionary<string, CheckBox> _casillas = new Dictionary<string, CheckBox>();
        private bool _perfilNuevo;

        /// <summary>Relee los perfiles y deja elegido <paramref name="elegir"/>.</summary>
        private void CargarPerfiles(int elegir)
        {
            _perfiles = Generals.BotonesPerfil.GetPerfiles();
            CmbPerfil.ItemsSource = _perfiles;
            CmbPerfil.SelectedItem = _perfiles.FirstOrDefault(x => x.Id == elegir) ?? _perfiles.FirstOrDefault();
            LlenarComboUsuario();
        }

        private void CmbPerfil_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var p = CmbPerfil.SelectedItem as Perfil;
            if (p == null) return;
            _perfilNuevo = false;
            TxtPerfil.Text = p.Nombre;
            PintarBotones(p.Id);
            BtnEliminarPerfil.IsEnabled = EsAdmin;
            MostrarPerfiles("");
        }

        private void PintarBotones(int rol)
        {
            var guardado = Generals.BotonesPerfil.Cargar();
            PanelBotones.Children.Clear();
            _casillas.Clear();
            foreach (var b in Generals.BotonesPerfil.Catalogo)
            {
                var ck = new CheckBox
                {
                    Content = b.Texto,
                    Style = (Style)FindResource("DarkCheck"),
                    IsChecked = Generals.BotonesPerfil.Visible(guardado, rol, b.Clave)
                };
                PanelBotones.Children.Add(ck);
                _casillas[b.Clave] = ck;
            }
        }

        private void NuevoPerfil_Click(object sender, RoutedEventArgs e)
        {
            // Parte de lo que tenga marcado el perfil que se está viendo.
            _perfilNuevo = true;
            CmbPerfil.SelectedItem = null;
            TxtPerfil.Text = "";
            BtnEliminarPerfil.IsEnabled = false;
            MostrarPerfiles("Perfil nuevo: ponle nombre, marca sus botones y guarda.");
            TxtPerfil.Focus();
        }

        private void GuardarPerfil_Click(object sender, RoutedEventArgs e)
        {
            string nombre = TxtPerfil.Text.Trim();
            if (nombre == "") { MostrarPerfiles("Escribe el nombre del perfil."); return; }
            if (nombre.Equals("Administrador", StringComparison.OrdinalIgnoreCase))
            {
                MostrarPerfiles("\"Administrador\" es el perfil fijo; usa otro nombre.");
                return;
            }

            var actual = CmbPerfil.SelectedItem as Perfil;
            if (!_perfilNuevo && actual == null) { MostrarPerfiles("Elige un perfil o crea uno nuevo."); return; }

            string fail;
            int id = Generals.BotonesPerfil.GuardarPerfil(_perfilNuevo ? 0 : actual.Id, nombre,
                Generals.Global.ROL_TECNICO_BASICO, out fail);
            if (id <= 0) { MostrarPerfiles(fail); return; }

            var visibles = _casillas.ToDictionary(kv => kv.Key, kv => kv.Value.IsChecked == true);
            fail = Generals.BotonesPerfil.Guardar(id, visibles);
            if (fail != "") { MostrarPerfiles(fail); return; }

            bool eraNuevo = _perfilNuevo;
            CargarPerfiles(id);
            CargarLista();
            MostrarPerfiles(eraNuevo
                ? "Perfil \"" + nombre + "\" creado. Ya se puede asignar a los usuarios."
                : "Perfil guardado. Sus usuarios lo verán al volver a iniciar sesión.");
        }

        private void EliminarPerfil_Click(object sender, RoutedEventArgs e)
        {
            var p = CmbPerfil.SelectedItem as Perfil;
            if (p == null || !EsAdmin) return;

            int n = Generals.BotonesPerfil.UsuariosConPerfil(p.Id);
            if (n > 0)
            {
                MostrarPerfiles("No se puede eliminar: " + n + (n == 1 ? " usuario lo tiene" : " usuarios lo tienen")
                    + " asignado. Cámbiales el perfil primero.");
                return;
            }
            if (_perfiles.Count <= 1)
            {
                MostrarPerfiles("Tiene que quedar al menos un perfil además del administrador.");
                return;
            }
            if (!GlassDialog.Pregunta(Owner, "Perfiles", "¿Eliminar el perfil \"" + p.Nombre + "\"?")) return;

            string fail = Generals.BotonesPerfil.EliminarPerfil(p.Id);
            if (fail != "") { MostrarPerfiles(fail); return; }
            CargarPerfiles(Generals.Global.ROL_TECNICO_BASICO);
            MostrarPerfiles("Perfil \"" + p.Nombre + "\" eliminado.");
        }

        private void MostrarPerfiles(string msg)
        {
            LblEstadoPerfiles.Text = msg;
            LblEstadoPerfiles.Visibility = string.IsNullOrEmpty(msg) ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
