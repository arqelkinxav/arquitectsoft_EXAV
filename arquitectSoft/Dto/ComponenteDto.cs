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
    class ComponenteDto
    {
        public string SaveComponent(string codigo, string descripcion, bool checkSubComponente,string opcion, Sub_Component[] Sbarray,string IdComponenteExist)
        {
            
            string resul = "";
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            try
            {

                con.Open(out fail);
                int check = checkSubComponente == true ? 1 : 0;
                string[] param = { codigo, descripcion, check.ToString() };

                string MsgResul = "Guardado Exitosamente";
                string sqlquery = "";
                switch (opcion)
                {
                    case "Editar":
                        sqlquery = Generals.Constantes.QUERY_UPDATE_COMPONENTES;
                        param[0] = descripcion;
                        param[1] = check.ToString();
                        param[2] = codigo;
                        MsgResul = "Registro Editado Correctamente";
                        break;
                    default:
                        sqlquery = Generals.Constantes.QUERY_INSERT_COMPONENTES;                        
                        break;
                }

             
               int idComponente = con.ExecuteNonQuery(sqlquery, out fail, param,0);

               idComponente = opcion == "Editar" ? Int32.Parse(IdComponenteExist) : idComponente;

               string[] paramdelete = { idComponente.ToString() };
               con.ExecuteNonQuery(Generals.Constantes.QUERY_DELETE_COMPONENTE_DETALLE, out fail, paramdelete,0);


                con.Close();

               SaveSubComponentDetalle(idComponente, Sbarray);

                resul = fail == "" ? MsgResul : fail;
               
            }
            catch (Exception ex)
            {
                resul = ex.Message.ToString();
                con.Close();
            }

            return resul;
        }

        private void SaveSubComponentDetalle(int idComponente, Sub_Component[] Sbarray)
        {
            Generals.Conexion con = new Generals.Conexion();
            
            string fail = "";
            con.Open(out fail);


            foreach (Sub_Component row in Sbarray)
            {

                int adecre = (row.ADecremento)? 1:0;

                string[] param = { idComponente.ToString(), row.IdSubcomponente.ToString(), row.UnidadCalculada
                        ,row.Cxdefecto.ToString(),row.CAdicional.ToString(),adecre.ToString() };

                int var = con.ExecuteNonQuery(Generals.Constantes.QUERY_INSERT_COMPONENTE_DETALLE, out fail, param,0);
            }
            con.Close();

        }

        public string ExistComponent(string codigo, string descripcion)
        {
            string resul = "0";
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            try
            {
                con.Open(out fail);
                MySqlDataReader row;
                string[] param = { codigo, descripcion };
                row = con.ExecuteReader(Generals.Constantes.QUERY_EXITS_COMPONENTES, out fail, param);
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

        public DataTable GetComponentDetalle(string IdComponente)
        {
           
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";

            con.Open(out fail);
            DataTable row;
            string[] param = {  };
            row = con.ExecuteDataSetSP(Generals.Constantes.QUERY_GET_COMPONENTE_DETALLE, out fail, IdComponente);
            con.Close();
            return row;
        }

        public bool ValilidationSaveComponenet(string codigo,string descripcion,bool chkNoSubcomp,int RowCountGrid,out string fail)
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
            else if (chkNoSubcomp == true && RowCountGrid == 0)
            {
                fail = "Debe Agregar un Sub Componente al menos!!!";
                SwSave = false;
            }

            
            return SwSave;
        }

        public string DeleteComponent(int idComponente)
        {
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            string resul = "";
            con.Open(out fail);
            string[] param = { idComponente.ToString() };
            try { 

                con.ExecuteNonQuery(Generals.Constantes.QUERY_DELETE_COMPONENTE, out fail, param,0);
                con.ExecuteNonQuery(Generals.Constantes.QUERY_DELETE_COMPONENTE_DETALLE, out fail, param,0);

                resul = "Registro Eliminado Correctamente";
            
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