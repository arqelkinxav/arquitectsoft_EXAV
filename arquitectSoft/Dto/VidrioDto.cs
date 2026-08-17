using System.Data;

namespace arquitectSoft.Dto
{
    /// <summary>
    /// Acceso a datos de las "dependencias de vidrio" (BETA). En la base, los componentes
    /// tienen sus subcomponentes cargados como si todo fuese el tipo de vidrio ESTÁNDAR de
    /// su sistema; estas reglas dicen, para un sistema (prefijo del código: DV, IT, AV…) y
    /// un tipo destino, qué subcomponente pasa a ser cuál — el vidrio y todo lo que arrastra
    /// (calces, gomas, cintas, perfiles).
    ///
    /// Tablas <c>beta_vidrio_tipo</c> / <c>beta_vidrio_sistema</c> / <c>beta_vidrio_regla</c>
    /// (ver db/migrations/005_dependencias_vidrio.sql). Como las de acabado, sobreviven a los
    /// imports de la base de la empresa porque su dump no las referencia.
    /// </summary>
    class VidrioDto
    {
        // ===== Tipos de vidrio =====

        /// <summary>Catálogo de tipos (3+3, 5+5, 6+6, 5+5/c/6+6…).</summary>
        public DataTable GetTipos()
        {
            return Consultar(Generals.Constantes.QUERY_VIDRIO_TIPOS);
        }

        /// <summary>Tipos con una fila inicial vacía, para los combos que admiten "sin elegir".</summary>
        public DataTable GetTiposConVacio()
        {
            DataTable dt = GetTipos();
            if (dt == null) return null;
            DataRow vacia = dt.NewRow();
            vacia["Id"] = 0;
            vacia["Nombre"] = "(sin definir)";
            dt.Rows.InsertAt(vacia, 0);
            return dt;
        }

        // ===== Sistemas =====

        /// <summary>Sistemas configurados, con el nombre de su tipo estándar.</summary>
        public DataTable GetSistemas()
        {
            return Consultar(Generals.Constantes.QUERY_VIDRIO_SISTEMAS);
        }

        /// <summary>
        /// Alta o modificación de un sistema. Con <paramref name="id"/> 0 inserta (o
        /// actualiza el que ya tuviera ese prefijo); con id &gt; 0 actualiza ese registro,
        /// que es lo que permite corregir el prefijo sin perder sus reglas.
        /// </summary>
        public string GuardarSistema(int id, string prefijo, string descripcion, int idTipoEstandar)
        {
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            if (!con.Open(out fail)) return fail;

            // 0 = sin definir (no se manda NULL: el proveedor no admite parámetros nulos
            // en este helper, y las consultas ya tratan el 0 como "sin estándar").
            string estandar = (idTipoEstandar > 0 ? idTipoEstandar : 0).ToString();
            if (id > 0)
            {
                string[] param = { prefijo, descripcion, estandar, id.ToString() };
                con.ExecuteNonQuery(Generals.Constantes.QUERY_UPDATE_VIDRIO_SISTEMA, out fail, param, 1);
            }
            else
            {
                string[] param = { prefijo, descripcion, estandar };
                con.ExecuteNonQuery(Generals.Constantes.QUERY_INSERT_VIDRIO_SISTEMA, out fail, param, 1);
            }
            con.Close();
            return fail;
        }

        /// <summary>Borra un sistema y, con él, todas sus reglas (no dejar reglas huérfanas).</summary>
        public string EliminarSistema(int id)
        {
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            if (!con.Open(out fail)) return fail;
            string[] param = { id.ToString() };
            con.ExecuteNonQuery(Generals.Constantes.QUERY_DELETE_VIDRIO_REGLAS_SISTEMA, out fail, param, 1);
            con.ExecuteNonQuery(Generals.Constantes.QUERY_DELETE_VIDRIO_SISTEMA, out fail, param, 1);
            con.Close();
            return fail;
        }

        // ===== Reglas de sustitución =====

        /// <summary>Sustituciones de un sistema para un tipo destino, con códigos y descripciones.</summary>
        public DataTable GetReglas(int idSistema, int idTipo)
        {
            return Consultar(Generals.Constantes.QUERY_VIDRIO_REGLAS_DETALLE
                             + " WHERE r.Id_Sistema = " + idSistema + " AND r.Id_Tipo = " + idTipo
                             + " ORDER BY CodOrigen");
        }

        /// <summary>Alta o cambio de una sustitución (upsert por sistema+tipo+origen).</summary>
        public string GuardarRegla(int idSistema, int idTipo, int idSubOrigen, int idSubDestino)
        {
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            if (!con.Open(out fail)) return fail;
            string[] param = { idSistema.ToString(), idTipo.ToString(),
                               idSubOrigen.ToString(), idSubDestino.ToString() };
            con.ExecuteNonQuery(Generals.Constantes.QUERY_UPSERT_VIDRIO_REGLA, out fail, param, 1);
            con.Close();
            return fail;
        }

        /// <summary>Borra una sustitución por su Id.</summary>
        public string EliminarRegla(int id)
        {
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            if (!con.Open(out fail)) return fail;
            string[] param = { id.ToString() };
            con.ExecuteNonQuery(Generals.Constantes.QUERY_DELETE_VIDRIO_REGLA, out fail, param, 1);
            con.Close();
            return fail;
        }

        /// <summary>
        /// Todas las reglas en plano (prefijo, tipo estándar del sistema, tipo destino,
        /// subcomponente origen y destino). Lo consume el motor al resolver un análisis.
        /// </summary>
        public DataTable GetReglasMotor()
        {
            return Consultar(Generals.Constantes.QUERY_VIDRIO_REGLAS_MOTOR);
        }

        /// <summary>Sistemas en plano (prefijo + tipo estándar), tengan reglas o no.</summary>
        public DataTable GetSistemasMotor()
        {
            return Consultar(Generals.Constantes.QUERY_VIDRIO_SISTEMAS_MOTOR);
        }

        /// <summary>Código y descripción de cada subcomponente, para reescribir la pieza sustituida.</summary>
        public DataTable GetMapaSubcomponentes()
        {
            return Consultar(Generals.Constantes.QUERY_VIDRIO_SUBCOMPONENTES_MAPA);
        }

        // Consulta suelta; devuelve null si la tabla no existe todavía o falla la conexión,
        // para que la pantalla avise en vez de reventar.
        private static DataTable Consultar(string sql)
        {
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            if (!con.Open(out fail)) return null;
            DataSet ds = con.ExecuteDataSet(sql, out fail);
            con.Close();
            return (ds == null || ds.Tables.Count == 0) ? null : ds.Tables[0];
        }
    }
}
