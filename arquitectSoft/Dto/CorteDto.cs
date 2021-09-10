using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arquitectSoft.Dto
{
    class CorteDto
    {
        public DataTable GetCortes()
        {
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            con.Open(out fail);

            //Id_Corte,Descripcion,Corte_Derecho,Corte_Izquierdo
            DataTable dt = con.ExecuteDataSet("Select 0 Id_Corte, ' ' Descripcion, 0 Corte_Derecho, 0 Corte_Izquierdo  union all " + Generals.Constantes.QUERY_CORTE, out fail).Tables[0];
            con.Close();

            return dt;
        }

        public string ExistCorte(string codigo, string descripcion)
        {
            string resul = "0";
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            try
            {
                con.Open(out fail);
                MySqlDataReader row;
                string[] param = { codigo, descripcion };
                row = con.ExecuteReader(Generals.Constantes.QUERY_EXITS_CORTE, out fail, param);
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

        public string MaximoCorte()
        {
            string resul = "0";
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            try
            {
                con.Open(out fail);
                MySqlDataReader row;
                string[] param = {  };
                row = con.ExecuteReader(Generals.Constantes.QUERY_CORTE_MAX, out fail, param);
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

        public string DeleteCorte(int idComponente)
        {
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            string resul = "";
            con.Open(out fail);
            string[] param = { idComponente.ToString() };
            try
            {

                con.ExecuteNonQuery(Generals.Constantes.QUERY_DELETE_CORTE, out fail, param, 0);

                resul = "Registro Eliminado Correctamente";

            }
            catch (Exception ex)
            {
                resul = ex.Message.ToString();
                con.Close();
            }
            return resul;
        }

        public string SaveCorte(string codigo, string descripcion, string opcion,int corteizq, int corteder)
        {

            string resul = "";
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";


            try
            {
                con.Open(out fail);
                string[] param = { descripcion, corteizq.ToString(), corteder.ToString(),"0" };
                string MsgResul = "Guardado Exitosamente";
                string sqlquery = "";
                switch (opcion)
                {
                    case "Editar":
                        sqlquery = Generals.Constantes.QUERY_UPDATE_CORTE;

                        param[0] = descripcion;
                        param[1] = corteder.ToString();
                        param[2] = corteizq.ToString();
                        param[3] = codigo;

                        MsgResul = "Registro Editado Correctamente";
                        break;
                    default:
                        sqlquery = Generals.Constantes.QUERY_INSERT_CORTE;
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

        public bool ValilidationSaveCorte(string codigo, string descripcion, out string fail)
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
