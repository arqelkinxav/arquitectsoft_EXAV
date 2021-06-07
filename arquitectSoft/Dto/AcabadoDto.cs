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


            DataTable dt = con.ExecuteDataSet("Select 0 Id_Acabado, '(Seleccione)' Descripcion union all " + Generals.Constantes.QUERY_ACABADO, out fail).Tables[0];
            con.Close();

            return dt;
        }
    }
}
