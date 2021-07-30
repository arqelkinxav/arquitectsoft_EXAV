using arquitectSoft.Class;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arquitectSoft.Dto
{
    class SubComponenteDto
    {

        public string SaveSubComponent(string codigo, string descripcion, string acabado, bool checkVidriosPaneles, string opcion, string IdComponenteExist)
        {

            string resul = "";
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";


            try
            {
                con.Open(out fail);
                int check = checkVidriosPaneles == true ? 1 : 0;
                string[] param = { acabado, codigo, descripcion, check.ToString(), "0" };
                string MsgResul = "Guardado Exitosamente";
                string sqlquery = "";
                switch (opcion)
                {
                    case "Editar":
                        sqlquery = Generals.Constantes.QUERY_UPDATE_SUBCOMPONENTES;

                        param[0] = acabado;
                        param[1] = codigo.Split('-')[0].Trim();
                        param[2] = descripcion;
                        param[3] = IdComponenteExist;
                        param[4] = check.ToString();

                        MsgResul = "Registro Editado Correctamente";
                        break;
                    default:
                        sqlquery = Generals.Constantes.QUERY_INSERT_SUBCOMPONENTES;
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

        public bool ValilidationSaveSubComponenet(string codigo, string descripcion, out string fail)
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

        public string ExistSubComponent(string codigo, string descripcion, string acabado, string opcion)
        {
            string resul = "0";
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            try
            {
                con.Open(out fail);
                MySqlDataReader row;
                string[] param = { codigo, descripcion, acabado };

                string query = opcion != "Editar" ? " and Id_Acabado = ?;" : ";";


                row = con.ExecuteReader(Generals.Constantes.QUERY_EXITS_SUBCOMPONENTES + query, out fail, param);
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

        public string DeleteComponent(int idComponente)
        {
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            string resul = "";
            con.Open(out fail);
            string[] param = { idComponente.ToString() };
            try
            {

                con.ExecuteNonQuery(Generals.Constantes.QUERY_DELETE_SUBCOMPONENTES, out fail, param, 0);

                resul = "Registro Eliminado Correctamente";

            }
            catch (Exception ex)
            {
                resul = ex.Message.ToString();
                con.Close();
            }
            return resul;
        }


        public DataTable GetComponentRelation(string IdComponente)
        {

            Generals.Conexion con = new Generals.Conexion();
            string fail = "";

            con.Open(out fail);
            DataTable row;
            row = con.ExecuteDataSetSP(Generals.Constantes.QUERY_GET_SUBCOMPONENTE_RELACION, out fail, IdComponente);
            con.Close();
            return row;
        }
    }
}
