using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arquitectSoft.Class
{


    public class Sub_Component
    {

        public Sub_Component(string codigo, string descripcion, int cxdefecto, int cadicional, string unidadcalculada, bool adecremento,int IdSubcomponente, string elevado, int corte, bool extra, int medida, int mecanizado)

        {

            this.Codigo = codigo;
            this.Descripcion = descripcion;
            this.Cxdefecto = cxdefecto;
            this.CAdicional = cadicional;
            this.UnidadCalculada = unidadcalculada;
            this.ADecremento = adecremento;
            this.IdSubcomponente = IdSubcomponente;
            this.Elevado = elevado;
            this.Cortes = corte;
            this.Extra = extra;
            this.Medida = medida;
            this.Mecanizado = mecanizado;

        }

        public string Codigo { get; set; }

        public string Descripcion { get; set; }

        public int Cxdefecto { get; set; }

        public int CAdicional { get; set; }

        public string UnidadCalculada { get; set; }

        public bool ADecremento { get; set; }

        public int IdSubcomponente { get; set; }

        public string Elevado { get; set; }

        public int Cortes { get; set; }

        public bool Extra { get; set; }

        public int Medida { get; set; }

        public int Mecanizado { get; set; }
    }
}
