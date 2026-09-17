using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arquitectSoft.Generals
{
    static class Global
    {
        // Perfil de permiso (columna `rol` de la tabla usuario).
        //   0 = Administrador -> todo, fijo
        //   1, 2, 3... = perfiles de beta_perfil (1 y 2 son los técnicos de siempre);
        //   qué botones ve cada uno lo decide el administrador (ver BotonesPerfil).
        public const int ROL_ADMIN = 0;
        public const int ROL_TECNICO_EDICION = 1;
        public const int ROL_TECNICO_BASICO = 2;

        private static string _nameconnect = "";
        private static string _AnalisisType = "";
        private static string _SwSegmentadoUbi = "";
        private static int _rol = ROL_TECNICO_BASICO;
        private static string _usuario = "";
        private static string _nombre = "";
        private static int _usuarioId = 0;

        public static string NameConnect
        {
            get { return _nameconnect; }
            set { _nameconnect = value; }
        }

        /// <summary>Rol del usuario que inició sesión (ver constantes ROL_*).</summary>
        public static int Rol
        {
            get { return _rol; }
            set { _rol = value; }
        }

        /// <summary>Login del usuario que inició sesión.</summary>
        public static string Usuario
        {
            get { return _usuario; }
            set { _usuario = value; }
        }

        /// <summary>Nombre legible (columna `Nombre`) del usuario que inició sesión.</summary>
        public static string Nombre
        {
            get { return _nombre; }
            set { _nombre = value; }
        }

        /// <summary>Id (PK) del usuario que inició sesión.</summary>
        public static int UsuarioId
        {
            get { return _usuarioId; }
            set { _usuarioId = value; }
        }

        public static bool EsAdmin { get { return _rol == ROL_ADMIN; } }
        public static bool PuedeEditar { get { return _rol == ROL_ADMIN || _rol == ROL_TECNICO_EDICION; } }

        /// <summary>Nombre legible del rol (para mostrar en pantallas).</summary>
        public static string NombreRol(int rol)
        {
            return BotonesPerfil.Nombre(rol);
        }

        public static string AnalisisType
        {
            get { return _AnalisisType; }
            set { _AnalisisType = value; }
        }

        public static string SwSegmentadoUbi
        {
            get { return _SwSegmentadoUbi; }
            set { _SwSegmentadoUbi = value; }
        }
    }
}
