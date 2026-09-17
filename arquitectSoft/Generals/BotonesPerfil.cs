using System;
using System.Collections.Generic;
using System.Data;

namespace arquitectSoft.Generals
{
    /// <summary>
    /// Perfiles de permiso y qué botones de la barra lateral ve cada uno. El perfil de un
    /// usuario es su columna <c>usuario.rol</c>: 0 = Administrador (fijo, lo ve todo), el
    /// resto apunta a <c>beta_perfil</c> (1 y 2 son los técnicos de siempre).
    ///
    /// <see cref="Catalogo"/> es el registro ÚNICO de botones configurables: para añadir uno
    /// basta con sumarlo aquí y al mapa de EscritorioWindow. Fuera de la lista, y por tanto
    /// fijos: Mi cuenta y Acerca (todos). Quien entra a Usuarios sin ser administrador no
    /// puede eliminar ni dar/tocar el perfil Administrador (ver UsuariosPanel).
    ///
    /// Tablas en db/migrations/007_botones_por_perfil.sql. Si faltan (base sin la 007) todo
    /// cae en los perfiles y botones de antes, así que el programa funciona igual.
    /// </summary>
    static class BotonesPerfil
    {
        public class Boton
        {
            public string Clave;
            public string Texto;
            public Boton(string clave, string texto) { Clave = clave; Texto = texto; }
        }

        public class Perfil
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
            public override string ToString() { return Nombre; }
        }

        public static readonly Boton[] Catalogo =
        {
            new Boton("Analisis", "Análisis"),
            new Boton("Puertas", "Puertas"),
            new Boton("Componentes", "Componentes"),
            new Boton("Subcomponentes", "Subcomponentes"),
            new Boton("Acabados", "Acabados"),
            new Boton("Mecanizados", "Mecanizados"),
            new Boton("Cortes", "Cortes"),
            new Boton("Unidad", "Unidades de medida"),
            new Boton("Dependencias", "Dependencias"),
            new Boton("Vidrios", "Vidrios"),
            new Boton("Respaldo", "Respaldo"),
            new Boton("Importar", "Importar"),
            new Boton("Usuarios", "Usuarios (sin eliminar ni dar Administrador)"),
            new Boton("CodRevit", "Cód. Revit"),
        };

        /// <summary>Los que antes eran solo del administrador: por defecto nadie más los ve.</summary>
        private static readonly string[] SoloAdminPorDefecto = { "Respaldo", "Importar", "Usuarios", "CodRevit" };

        /// <summary>Lo que veía cada rol antes de existir los perfiles editables.</summary>
        public static bool PorDefecto(int rol, string clave)
        {
            if (rol == Global.ROL_ADMIN) return true;
            if (Array.IndexOf(SoloAdminPorDefecto, clave) >= 0) return false;
            if (rol == Global.ROL_TECNICO_EDICION) return true;
            return clave == "Analisis" || clave == "Puertas";
        }

        // ===== Perfiles =====

        /// <summary>Perfiles editables (sin el administrador), por Id.</summary>
        public static List<Perfil> GetPerfiles()
        {
            var r = new List<Perfil>();
            DataTable dt = Consultar("SELECT Id, Nombre FROM beta_perfil ORDER BY Id");
            if (dt != null)
                foreach (DataRow f in dt.Rows)
                    r.Add(new Perfil { Id = Convert.ToInt32(f["Id"]), Nombre = Convert.ToString(f["Nombre"]) });
            if (r.Count == 0)
            {
                // Base sin la 007: los dos de siempre, para que se pueda seguir asignando.
                r.Add(new Perfil { Id = Global.ROL_TECNICO_EDICION, Nombre = "Técnico (edición)" });
                r.Add(new Perfil { Id = Global.ROL_TECNICO_BASICO, Nombre = "Técnico (básico)" });
            }
            return r;
        }

        /// <summary>Nombre para pantalla de un rol/perfil.</summary>
        public static string Nombre(int rol)
        {
            if (rol == Global.ROL_ADMIN) return "Administrador";
            foreach (var p in GetPerfiles())
                if (p.Id == rol) return p.Nombre;
            return "(perfil " + rol + " no existe)";
        }

        /// <summary>
        /// Alta (id 0) o cambio de nombre. Al crear, copia los botones de
        /// <paramref name="copiarDe"/> para que el perfil nuevo no nazca vacío.
        /// Devuelve el Id, o -1 con el motivo en <paramref name="fail"/>.
        /// </summary>
        public static int GuardarPerfil(int id, string nombre, int copiarDe, out string fail)
        {
            var con = new Conexion();
            if (!con.Open(out fail)) return -1;
            try
            {
                if (id > 0)
                {
                    con.ExecuteNonQuery("UPDATE beta_perfil SET Nombre = ? WHERE Id = ?", out fail,
                        new[] { nombre, id.ToString() }, 1);
                    return fail == "" ? id : -1;
                }
                int nuevo = con.ExecuteNonQuery("INSERT INTO beta_perfil (Nombre) VALUES (?)", out fail,
                    new[] { nombre }, 1);
                if (fail != "" || nuevo <= 0) return -1;
                con.ExecuteNonQuery(
                    "INSERT INTO beta_rol_boton (Rol, Boton, Visible) SELECT ?, Boton, Visible FROM beta_rol_boton WHERE Rol = ?",
                    out fail, new[] { nuevo.ToString(), copiarDe.ToString() }, 1);
                return nuevo;
            }
            finally { con.Close(); Traducir(ref fail); }
        }

        /// <summary>Usuarios que tienen asignado el perfil.</summary>
        public static int UsuariosConPerfil(int id)
        {
            DataTable dt = Consultar("SELECT COUNT(*) n FROM usuario WHERE rol = " + id);
            return dt == null || dt.Rows.Count == 0 ? 0 : Convert.ToInt32(dt.Rows[0]["n"]);
        }

        /// <summary>Borra el perfil y sus botones. Quien llama comprueba antes que nadie lo use.</summary>
        public static string EliminarPerfil(int id)
        {
            var con = new Conexion();
            string fail;
            if (!con.Open(out fail)) return fail;
            try
            {
                con.ExecuteNonQuery("DELETE FROM beta_rol_boton WHERE Rol = ?", out fail, new[] { id.ToString() }, 1);
                if (fail == "")
                    con.ExecuteNonQuery("DELETE FROM beta_perfil WHERE Id = ?", out fail, new[] { id.ToString() }, 1);
                return Traducido(fail);
            }
            finally { con.Close(); }
        }

        // ===== Botones =====

        private static string Llave(int rol, string clave) { return rol + "|" + clave; }

        /// <summary>
        /// Lo guardado en la base. Vacío si la tabla no existe todavía o falla la conexión:
        /// entonces todo cae en <see cref="PorDefecto"/>.
        /// </summary>
        public static Dictionary<string, bool> Cargar()
        {
            var r = new Dictionary<string, bool>();
            DataTable dt = Consultar("SELECT Rol, Boton, Visible FROM beta_rol_boton");
            if (dt != null)
                foreach (DataRow f in dt.Rows)
                    r[Llave(Convert.ToInt32(f["Rol"]), Convert.ToString(f["Boton"]))] = Convert.ToInt32(f["Visible"]) != 0;
            return r;
        }

        private static Dictionary<string, bool> _sesion;

        /// <summary>Si el usuario que inició sesión ve ese botón (se lee una vez por sesión).</summary>
        public static bool VisibleEnSesion(string clave)
        {
            if (Global.EsAdmin) return true;
            if (_sesion == null) _sesion = Cargar();
            return Visible(_sesion, Global.Rol, clave);
        }

        /// <summary>Olvida lo leído (al abrir el escritorio de una sesión nueva).</summary>
        public static void OlvidarSesion() { _sesion = null; }

        public static bool Visible(Dictionary<string, bool> guardado, int rol, string clave)
        {
            if (rol == Global.ROL_ADMIN) return true;
            bool v;
            return guardado != null && guardado.TryGetValue(Llave(rol, clave), out v) ? v : PorDefecto(rol, clave);
        }

        /// <summary>Guarda el estado de todos los botones de un perfil. Devuelve "" o el error.</summary>
        public static string Guardar(int rol, IDictionary<string, bool> visibles)
        {
            var con = new Conexion();
            string fail;
            if (!con.Open(out fail)) return "No se pudo conectar: " + fail;
            try
            {
                foreach (var kv in visibles)
                {
                    string[] param = { rol.ToString(), kv.Key, kv.Value ? "1" : "0" };
                    con.ExecuteNonQuery("REPLACE INTO beta_rol_boton (Rol, Boton, Visible) VALUES (?, ?, ?)", out fail, param, 1);
                    if (fail != "") return Traducido(fail);
                }
                return "";
            }
            finally { con.Close(); }
        }

        // ===== Utilidades =====

        private static DataTable Consultar(string sql)
        {
            try
            {
                var con = new Conexion();
                string fail;
                if (!con.Open(out fail)) return null;
                DataSet ds = con.ExecuteDataSet(sql, out fail);
                con.Close();
                return ds == null || ds.Tables.Count == 0 ? null : ds.Tables[0];
            }
            catch { return null; }
        }

        private static string Traducido(string fail) { Traducir(ref fail); return fail; }

        private static void Traducir(ref string fail)
        {
            if (string.IsNullOrEmpty(fail)) return;
            if (fail.Contains("doesn't exist"))
                fail = "Faltan las tablas de perfiles en esta base: aplica db/migrations/007_botones_por_perfil.sql.";
            else if (fail.Contains("Duplicate entry"))
                fail = "Ya existe un perfil con ese nombre.";
        }
    }
}
