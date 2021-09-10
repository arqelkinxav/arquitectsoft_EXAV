using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arquitectSoft.Dto
{
    class MecanizadoDto
    {
        public DataTable GetMecanizado()
        {
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            con.Open(out fail);


            DataTable dt = con.ExecuteDataSet("Select 0 Id_mecanizado,00 Codigo_Homologacion, ' ' Descripcion union all " + Generals.Constantes.QUERY_MECANIZADO, out fail).Tables[0];
            con.Close();

            return dt;
        }

        public DataTable GetMecanizadoParam(string id)
        {
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            con.Open(out fail);


            string condicion = id == "" ? " Where 1=2" : " where Codigo_Homologacion in (" + id + ")";


            DataTable dt = con.ExecuteDataSet("Select 0 Id_mecanizado,00 Codigo_Homologacion, '(Seleccione)' Descripcion union all "
                                                + Generals.Constantes.QUERY_MECANIZADO
                                                + condicion
                                                , out fail).Tables[0];
            con.Close();

            return dt;
        }

        public string ExistMecanizado(string codigo)
        {
            string resul = "0";
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            try
            {
                con.Open(out fail);
                MySqlDataReader row;
                string[] param = { codigo };
                row = con.ExecuteReader(Generals.Constantes.QUERY_EXITS_MECANIZADO, out fail, param);
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

        public string DeleteMecanizado(int idComponente)
        {
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            string resul = "";
            con.Open(out fail);
            string[] param = { idComponente.ToString() };
            try
            {

                con.ExecuteNonQuery(Generals.Constantes.QUERY_DELETE_MECANIZADO, out fail, param, 0);

                resul = "Registro Eliminado Correctamente";

            }
            catch (Exception ex)
            {
                resul = ex.Message.ToString();
                con.Close();
            }
            return resul;
        }

        public string SaveMecanizado(string codigo, string descripcion, string opcion, string IdAcabadoExist)
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
                        sqlquery = Generals.Constantes.QUERY_UPDATE_MECANIZADO;

                        param[0] = descripcion;
                        param[1] = IdAcabadoExist;

                        MsgResul = "Registro Editado Correctamente";
                        break;
                    default:
                        sqlquery = Generals.Constantes.QUERY_INSERT_MECANIZADO;
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

        public bool ValilidationSaveMecanizado(string codigo, string descripcion, out string fail)
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

        public string MaximoMecanizado()
        {
            string resul = "0";
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            try
            {
                con.Open(out fail);
                MySqlDataReader row;
                string[] param = { };
                row = con.ExecuteReader(Generals.Constantes.QUERY_MECANIZADO_MAX, out fail, param);
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
    }
}
