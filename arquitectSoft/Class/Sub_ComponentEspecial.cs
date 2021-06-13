using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arquitectSoft.Class
{
    public class Sub_ComponentEspecial
    {

        public Sub_ComponentEspecial(string codigo, string descripcion,string columna,int cxdefecto,int cadicional, int idsubcomponente)

        {

            this.Codigo = codigo;
            this.Descripcion = descripcion;
            this.Columna = columna;
            this.IdSubcomponente = idsubcomponente;
            this.Cxdefecto = cxdefecto;
            this.CAdicional = cadicional;

        }


        public string Codigo { get; set; }

        public string Descripcion { get; set; }

        public string Columna { get; set; }

        public int Cxdefecto { get; set; }

        public int CAdicional { get; set; }

        public int IdSubcomponente { get; set; }
    }
}
