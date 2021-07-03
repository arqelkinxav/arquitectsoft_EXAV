using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arquitectSoft.Dto
{
    class UnidadMedidaDto
    {
       
        public string ExistUnidadMedida(string codigo)
        {
            string resul = "0";
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            try
            {
                con.Open(out fail);
                MySqlDataReader row;
                string[] param = { codigo };
                row = con.ExecuteReader(Generals.Constantes.QUERY_EXITS_UNIDADMEDIDA, out fail, param);
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

        public string DeleteUnidadMedida(int idComponente)
        {
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            string resul = "";
            con.Open(out fail);
            string[] param = { idComponente.ToString() };
            try
            {

                con.ExecuteNonQuery(Generals.Constantes.QUERY_DELETE_UNIDADMEDIDA, out fail, param, 0);

                resul = "Registro Eliminado Correctamente";

            }
            catch (Exception ex)
            {
                resul = ex.Message.ToString();
                con.Close();
            }
            return resul;
        }

        public string SaveUnidadMedida(string codigo, string descripcion,string convencion, string opcion, string IdUnidadMExist)
        {

            string resul = "";
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";


            try
            {
                con.Open(out fail);
                string[] param = {  descripcion, convencion, codigo };
                string MsgResul = "Guardado Exitosamente";
                string sqlquery = "";
                switch (opcion)
                {
                    case "Editar":
                        sqlquery = Generals.Constantes.QUERY_UPDATE_UNIDADMEDIDA;

                        param[0] = descripcion;
                        param[1] = convencion;
                        param[2] = IdUnidadMExist;

                        MsgResul = "Registro Editado Correctamente";
                        break;
                    default:
                        sqlquery = Generals.Constantes.QUERY_INSERT_UNIDADMEDIDA;
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

        public bool ValilidationSaveUnidadMedida(string codigo, string descripcion,string convencion, out string fail)
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
            else if (convencion == "")
            {
                fail = "Debe Digitar una convencion";
                SwSave = false;
            }

            return SwSave;
        }

        public string MaximaUnidadMedida()
        {
            string resul = "0";
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            try
            {
                con.Open(out fail);
                MySqlDataReader row;
                string[] param = { };
                row = con.ExecuteReader(Generals.Constantes.QUERY_UNIDADMEDIDA_MAX, out fail, param);
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
