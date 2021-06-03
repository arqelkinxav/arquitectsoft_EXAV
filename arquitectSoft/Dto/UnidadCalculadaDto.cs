using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arquitectSoft.Dto
{
    class UnidadCalculadaDto
    {

        public DataTable GetUnidadCalculada()
        {
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            con.Open(out fail);


            DataTable dt = con.ExecuteDataSet(Generals.Constantes.QUERY_UNIDADCALCULADA, out fail).Tables[0];
            con.Close();

            return dt;
        }

    }
}
