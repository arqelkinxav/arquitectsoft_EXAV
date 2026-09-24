using System;
using System.Collections.Generic;
using System.Data;

namespace arquitectSoft.Generals
{
    /// <summary>Un cambio apuntado en beta_ultimo_cambio.</summary>
    public class CambioBase
    {
        public string Tabla;
        public string Accion;
        public DateTime Fecha;
        public string Usuario;

        /// <summary>"Reglas de vidrio" en vez de "beta_vidrio_regla".</summary>
        public string QueLegible { get { return RegistroCambios.NombreTabla(Tabla); } }

        public string AccionLegible
        {
            get
            {
                switch ((Accion ?? "").ToUpperInvariant())
                {
                    case "INSERT": return "añadido";
                    case "UPDATE": return "modificado";
                    case "DELETE": return "borrado";
                    default: return Accion ?? "";
                }
            }
        }
    }

    /// <summary>
    /// Lee cuándo se tocó la base por última vez. Lo apuntan triggers en cada tabla que se
    /// guarda de verdad (db/migrations/008_registro_cambios.sql), con el usuario que
    /// Conexion.Open deja en @arq_usuario. Si la base no tiene la 008 devuelve null y la
    /// barra simplemente no lo enseña.
    /// </summary>
    public static class RegistroCambios
    {
        /// <summary>Los últimos cambios, del más reciente al más viejo (uno por tabla).</summary>
        public static List<CambioBase> Ultimos(int cuantos)
        {
            try
            {
                var con = new Conexion();
                string fail;
                if (!con.Open(out fail)) return null;
                DataSet ds = con.ExecuteDataSet(
                    "SELECT Tabla, Accion, Fecha, Usuario FROM beta_ultimo_cambio ORDER BY Fecha DESC LIMIT " + cuantos, out fail);
                con.Close();
                if (ds == null || ds.Tables.Count == 0) return null;

                var lista = new List<CambioBase>();
                foreach (DataRow r in ds.Tables[0].Rows)
                    lista.Add(new CambioBase
                    {
                        Tabla = Convert.ToString(r["Tabla"]),
                        Accion = Convert.ToString(r["Accion"]),
                        Fecha = Convert.ToDateTime(r["Fecha"]),
                        Usuario = r["Usuario"] == DBNull.Value ? "" : Convert.ToString(r["Usuario"])
                    });
                return lista;
            }
            catch { return null; }
        }

        private static readonly Dictionary<string, string> Nombres = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "acabados", "Acabados" },
            { "beta_dependencias_acabado", "Dependencias de acabado" },
            { "beta_perfil", "Perfiles de permiso" },
            { "beta_rol_boton", "Botones por perfil" },
            { "beta_vidrio_regla", "Reglas de vidrio" },
            { "beta_vidrio_sistema", "Sistemas de vidrio" },
            { "beta_vidrio_tipo", "Tipos de vidrio" },
            { "categorias", "Categorías" },
            { "componentes", "Componentes" },
            { "componentes_detalle", "Detalle de componentes" },
            { "componentes_especial", "Componentes especiales" },
            { "componentes_especial_detalle", "Detalle de especiales" },
            { "cortes", "Cortes" },
            { "dbmanagments", "Importación de base" },
            { "mecanizados", "Mecanizados" },
            { "subcomponentes", "Subcomponentes" },
            { "unidades_calculadas", "Unidades calculadas" },
            { "unidades_medidas", "Unidades de medida" },
            { "usuario", "Usuarios" },
        };

        public static string NombreTabla(string tabla)
        {
            string n;
            return tabla != null && Nombres.TryGetValue(tabla, out n) ? n : tabla;
        }
    }
}
