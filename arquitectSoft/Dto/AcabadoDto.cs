using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arquitectSoft.Dto
{
    class AcabadoDto
    {
        public DataTable GetAcabado()
        {
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            con.Open(out fail);


            DataTable dt = con.ExecuteDataSet("Select 0 Id_Acabado,00 Codigo_Homologacion, '(Seleccione)' Descripcion union all " + Generals.Constantes.QUERY_ACABADO, out fail).Tables[0];
            con.Close();

            return dt;
        }

        public DataTable GetAcabadoParam(string id)
        {
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            con.Open(out fail);

       
            string condicion = id == "" ? " Where 1=2" : " where Codigo_Homologacion in (" + id + ")";


            DataTable dt = con.ExecuteDataSet("Select 0 Id_Acabado,00 Codigo_Homologacion, '(Seleccione)' Descripcion union all "
                                                + Generals.Constantes.QUERY_ACABADO
                                                + condicion
                                                , out fail).Tables[0];
            con.Close();

            return dt;
        }

        public string ExistAcabado(string codigo, string descripcion)
        {
            string resul = "0";
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            try
            {
                con.Open(out fail);
                MySqlDataReader row;
                string[] param = { codigo, descripcion };
                row = con.ExecuteReader(Generals.Constantes.QUERY_EXITS_ACABADO, out fail, param);
                while (row.Read())
                {
                    resul = row.GetString(0);
                }

            }
            catch (Exception ex)
            {
                resul = ex.Message.ToString();
                con.Close();
            }

            return resul;
        }

        /// <summary>True si el código está libre (ningún OTRO acabado distinto de idActual lo usa).</summary>
        public bool CodigoLibre(string codigo, string idActual)
        {
            bool libre = true;
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            try
            {
                con.Open(out fail);
                string[] param = { codigo, idActual };
                MySqlDataReader row = con.ExecuteReader(Generals.Constantes.QUERY_ACABADO_COD_OTRO, out fail, param);
                if (row != null && row.Read()) libre = false;
                con.Close();
            }
            catch { con.Close(); }
            return libre;
        }

        public string DeleteAcabado(int idComponente)
        {
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            string resul = "";
            con.Open(out fail);
            string[] param = { idComponente.ToString() };
            try
            {

                con.ExecuteNonQuery(Generals.Constantes.QUERY_DELETE_ACABADO, out fail, param, 0);

                resul = "Registro Eliminado Correctamente";

            }
            catch (Exception ex)
            {
                resul = ex.Message.ToString();
                con.Close();
            }
            return resul;
        }

        public string SaveAcabado(string codigo, string descripcion, string opcion, string IdAcabadoExist)
        {

            string resul = "";
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";


            try
            {
                con.Open(out fail);
                string[] param = { codigo, descripcion };
                string MsgResul = "Guardado Exitosamente";
                string sqlquery = "";
                switch (opcion)
                {
                    case "Editar":
                        // Actualiza código + descripción del MISMO registro (por Id_Acabado):
                        // el Id no cambia, así que las relaciones (subcomponentes por Id_Acabado)
                        // no se rompen; solo cambia el texto del código/descripción.
                        sqlquery = Generals.Constantes.QUERY_UPDATE_ACABADO;
                        param = new string[] { codigo, descripcion, IdAcabadoExist };
                        MsgResul = "Registro Editado Correctamente";
                        break;
                    default:
                        sqlquery = Generals.Constantes.QUERY_INSERT_ACABADO;
                        break;
                }

                int idComponente = con.ExecuteNonQuery(sqlquery, out fail, param, 1);
                con.Close();

                resul = fail == "" ? MsgResul : fail;
            }
            catch (Exception ex)
            {
                resul = ex.Message.ToString();
                con.Close();
            }


            return resul;
        }

        public bool ValilidationSaveAcabado(string codigo, string descripcion, out string fail)
        {
            bool SwSave = true;
            fail = null;
            if (codigo == "")
            {
                fail = "Debe Digitar un Codigo";
                SwSave = false;
            }
            else if (descripcion == "")
            {
                fail = "Debe Digitar un Descripcion";
                SwSave = false;
            }

            return SwSave;
        }
    }
}
