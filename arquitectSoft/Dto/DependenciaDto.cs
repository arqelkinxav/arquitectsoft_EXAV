using System.Data;

namespace arquitectSoft.Dto
{
    /// <summary>
    /// Acceso a datos de las "dependencias de acabado" (BETA): reglas que dicen, para un
    /// acabado placeholder MOD… y un valor de perfilería, qué acabado REAL debe tomar el
    /// material dependiente. Las reglas viven en la tabla nueva <c>beta_dependencias_acabado</c>
    /// (sobrevive a los imports de la base de la empresa porque su dump no la referencia).
    /// </summary>
    class DependenciaDto
    {
        /// <summary>Acabados placeholder (código MOD…), para la pantalla de configuración.</summary>
        public DataTable GetPlaceholders()
        {
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            con.Open(out fail);
            DataTable dt = con.ExecuteDataSet(Generals.Constantes.QUERY_ACABADOS_MOD, out fail).Tables[0];
            con.Close();
            return dt;
        }

        /// <summary>Acabados REALES (sin los MOD…), para elegir el resultado y los selectores.</summary>
        public DataTable GetAcabadosReales()
        {
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            con.Open(out fail);
            DataTable dt = con.ExecuteDataSet(
                "Select 0 Id_Acabado,'' Codigo_Homologacion, '(Seleccione)' Descripcion union all "
                + Generals.Constantes.QUERY_ACABADOS_REALES, out fail).Tables[0];
            con.Close();
            return dt;
        }

        /// <summary>Todas las reglas (Cod_Placeholder, Cod_Perfileria, Cod_Resultado).</summary>
        public DataTable GetReglas()
        {
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            con.Open(out fail);
            DataTable dt = con.ExecuteDataSet(Generals.Constantes.QUERY_DEPENDENCIAS, out fail).Tables[0];
            con.Close();
            return dt;
        }

        /// <summary>Reglas con las descripciones de cada acabado (para la pantalla de configuración).</summary>
        public DataTable GetReglasDetalle()
        {
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            con.Open(out fail);
            DataTable dt = con.ExecuteDataSet(Generals.Constantes.QUERY_DEPENDENCIAS_DETALLE, out fail).Tables[0];
            con.Close();
            return dt;
        }

        /// <summary>Mapa código→descripción de TODOS los acabados (para resolver el resultado).</summary>
        public DataTable GetMapaDescripciones()
        {
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            con.Open(out fail);
            DataTable dt = con.ExecuteDataSet(Generals.Constantes.QUERY_ACABADOS_DESC, out fail).Tables[0];
            con.Close();
            return dt;
        }

        /// <summary>Inserta o actualiza una regla (upsert por placeholder+perfilería).</summary>
        public string GuardarRegla(string placeholder, string perfileria, string resultado)
        {
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            con.Open(out fail);
            string[] param = { placeholder, perfileria, resultado };
            con.ExecuteNonQuery(Generals.Constantes.QUERY_UPSERT_DEPENDENCIA, out fail, param, 1);
            con.Close();
            return fail;
        }

        /// <summary>
        /// Propaga el renombrado del código de un acabado a las reglas que lo referencien
        /// (como placeholder, valor de perfilería o resultado), para que la dependencia siga
        /// funcionando tras editar el código en el catálogo.
        /// </summary>
        public void RenombrarCodigo(string codigoViejo, string codigoNuevo)
        {
            if (string.IsNullOrEmpty(codigoViejo) || codigoViejo == codigoNuevo) return;
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            if (!con.Open(out fail)) return;
            string[] param = { codigoNuevo, codigoViejo };
            con.ExecuteNonQuery(Generals.Constantes.QUERY_REN_DEP_PLACEHOLDER, out fail, param, 1);
            con.ExecuteNonQuery(Generals.Constantes.QUERY_REN_DEP_PERFILERIA, out fail, param, 1);
            con.ExecuteNonQuery(Generals.Constantes.QUERY_REN_DEP_RESULTADO, out fail, param, 1);
            con.Close();
        }

        /// <summary>Elimina la regla de un placeholder para un valor de perfilería.</summary>
        public string EliminarRegla(string placeholder, string perfileria)
        {
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            con.Open(out fail);
            string[] param = { placeholder, perfileria };
            con.ExecuteNonQuery(Generals.Constantes.QUERY_DELETE_DEPENDENCIA, out fail, param, 1);
            con.Close();
            return fail;
        }
    }
}
