using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arquitectSoft.Generals
{
    static class Global
    {
        // Niveles de permiso (columna `rol` de la tabla usuario).
        //   0 = Administrador   -> todos los botones + gestión de usuarios
        //   1 = Técnico edición -> todos MENOS Respaldo, Importar y Usuarios
        //   2 = Técnico básico  -> solo Análisis, Puertas y Acerca
        // Todos los roles tienen además "Mi cuenta" para cambiar su clave.
        public const int ROL_ADMIN = 0;
        public const int ROL_TECNICO_EDICION = 1;
        public const int ROL_TECNICO_BASICO = 2;

        private static string _nameconnect = "";
        private static string _AnalisisType = "";
        private static string _SwSegmentadoUbi = "";
        private static int _rol = ROL_TECNICO_BASICO;
        private static string _usuario = "";
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
            switch (rol)
            {
                case ROL_ADMIN: return "Administrador";
                case ROL_TECNICO_EDICION: return "Técnico (edición)";
                default: return "Técnico (básico)";
            }
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
