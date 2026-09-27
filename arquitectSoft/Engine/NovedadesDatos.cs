using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace arquitectSoft.Engine
{
    /// <summary>
    /// Novedades del CATÁLOGO (no del programa): qué componentes, subcomponentes, despieces,
    /// reglas… cambiaron desde la última vez que cada usuario abrió arquitectSoft.
    ///
    /// Se apuntan al Importar: la revisión previa (RespaldoDiff) ya sabe, en claro, qué trae
    /// el respaldo respecto a esta base; si el import sale bien, esas líneas se guardan aquí.
    /// Así en la beta cada técnico ve "DVS0007D: se añade IMC0015B-01…" al abrir.
    ///
    /// Las dos tablas viven SOLO en la base donde se importa y el Importar nunca las
    /// sobrescribe (RespaldoSinUsuarios las quita siempre del archivo): si no, un respaldo de
    /// Olimpo se llevaría el historial de la beta.
    /// </summary>
    public static class NovedadesDatos
    {
        public const string TABLA = "beta_novedades_datos";
        public const string VISTAS = "beta_novedades_vistas";

        /// <summary>Tope de líneas por aviso, como el de los cambios del programa.</summary>
        private const int MAX = 100;

        private static void Crear(MySqlConnection cn)
        {
            Ejecutar(cn,
                "CREATE TABLE IF NOT EXISTS " + TABLA + " (" +
                "  Id BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY," +
                "  Lote INT NOT NULL," +
                "  Fecha DATETIME NOT NULL," +
                "  Archivo VARCHAR(255) NULL," +
                "  Texto VARCHAR(1000) NOT NULL," +
                "  KEY ix_" + TABLA + "_lote (Lote)" +
                ") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");
            Ejecutar(cn,
                "CREATE TABLE IF NOT EXISTS " + VISTAS + " (" +
                "  Usuario VARCHAR(100) NOT NULL PRIMARY KEY," +
                "  UltimoId BIGINT NOT NULL," +
                "  Fecha DATETIME NOT NULL" +
                ") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4");
        }

        /// <summary>Apunta lo que trajo un Importar (un lote). Nunca lanza.</summary>
        public static void Registrar(IList<string> textos, string archivo)
        {
            if (textos == null || textos.Count == 0) return;
            try
            {
                using (var cn = new MySqlConnection(Generals.Conexion.strProvider))
                {
                    cn.Open();
                    Crear(cn);
                    int lote = Convert.ToInt32(Escalar(cn, "SELECT COALESCE(MAX(Lote), 0) + 1 FROM " + TABLA));
                    using (var tx = cn.BeginTransaction())
                    using (var cmd = new MySqlCommand(
                        "INSERT INTO " + TABLA + " (Lote, Fecha, Archivo, Texto) VALUES (@l, NOW(), @a, @t)", cn, tx))
                    {
                        cmd.Parameters.AddWithValue("@l", lote);
                        cmd.Parameters.AddWithValue("@a", archivo ?? "");
                        var p = cmd.Parameters.Add("@t", MySqlDbType.VarChar);
                        foreach (string t in textos)
                        {
                            p.Value = t.Length > 1000 ? t.Substring(0, 997) + "…" : t;
                            cmd.ExecuteNonQuery();
                        }
                        tx.Commit();
                    }
                }
            }
            catch { /* apuntar novedades nunca puede estropear un import que ya salió bien */ }
        }

        /// <summary>
        /// Cambios del catálogo que <paramref name="usuario"/> no ha visto. La primera vez, los
        /// del último Importar. <paramref name="hasta"/> es lo que hay que pasar a MarcarVisto.
        /// </summary>
        public static List<Novedades.Cambio> Pendientes(string usuario, out long hasta)
        {
            var l = new List<Novedades.Cambio>();
            hasta = 0;
            if (string.IsNullOrWhiteSpace(usuario)) return l;
            try
            {
                using (var cn = new MySqlConnection(Generals.Conexion.strProvider))
                {
                    cn.Open();
                    if (Escalar(cn, "SELECT COUNT(*) FROM information_schema.TABLES " +
                                    "WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = '" + TABLA + "'").ToString() == "0")
                        return l;
                    Crear(cn);

                    object visto = Escalar(cn, "SELECT UltimoId FROM " + VISTAS + " WHERE Usuario = @u", usuario.Trim());
                    string filtro = visto != null
                        ? "Id > " + Convert.ToInt64(visto)
                        : "Lote = (SELECT MAX(Lote) FROM " + TABLA + ")";

                    var da = new MySqlDataAdapter(
                        "SELECT Id, Fecha, Texto FROM " + TABLA + " WHERE " + filtro + " ORDER BY Id DESC", cn);
                    var dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count == 0) return l;

                    hasta = Convert.ToInt64(dt.Rows[0]["Id"]);
                    // Se enseñan en el orden en que se apuntaron (van agrupadas por tipo).
                    for (int i = Math.Min(dt.Rows.Count, MAX) - 1; i >= 0; i--)
                        l.Add(new Novedades.Cambio
                        {
                            Titulo = Convert.ToString(dt.Rows[i]["Texto"]),
                            Fecha = Convert.ToDateTime(dt.Rows[i]["Fecha"])
                        });
                    if (dt.Rows.Count > MAX)
                        l.Add(new Novedades.Cambio
                        {
                            Titulo = "… y " + (dt.Rows.Count - MAX) + " cambios más en el catálogo.",
                            Fecha = Convert.ToDateTime(dt.Rows[MAX]["Fecha"])
                        });
                }
            }
            catch { l.Clear(); hasta = 0; }
            return l;
        }

        public static void MarcarVisto(string usuario, long hasta)
        {
            if (string.IsNullOrWhiteSpace(usuario) || hasta <= 0) return;
            try
            {
                using (var cn = new MySqlConnection(Generals.Conexion.strProvider))
                {
                    cn.Open();
                    Crear(cn);
                    using (var cmd = new MySqlCommand(
                        "INSERT INTO " + VISTAS + " (Usuario, UltimoId, Fecha) VALUES (@u, @h, NOW()) " +
                        "ON DUPLICATE KEY UPDATE UltimoId = GREATEST(UltimoId, VALUES(UltimoId)), Fecha = NOW()", cn))
                    {
                        cmd.Parameters.AddWithValue("@u", usuario.Trim());
                        cmd.Parameters.AddWithValue("@h", hasta);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch { }
        }

        private static void Ejecutar(MySqlConnection cn, string sql)
        {
            using (var cmd = new MySqlCommand(sql, cn)) cmd.ExecuteNonQuery();
        }

        private static object Escalar(MySqlConnection cn, string sql, string usuario = null)
        {
            using (var cmd = new MySqlCommand(sql, cn))
            {
                if (usuario != null) cmd.Parameters.AddWithValue("@u", usuario);
                object o = cmd.ExecuteScalar();
                return o == DBNull.Value ? null : o;
            }
        }
    }
}
