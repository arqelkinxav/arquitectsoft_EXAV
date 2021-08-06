using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arquitectSoft.Class
{
    public class MultiAcabado
    {

        public MultiAcabado(string codigo, string descripcion)

        {

            this.Codigo = codigo;
            this.Descripcion = descripcion;           

        }


        public string Codigo { get; set; }

        public string Descripcion { get; set; }
    }
}
