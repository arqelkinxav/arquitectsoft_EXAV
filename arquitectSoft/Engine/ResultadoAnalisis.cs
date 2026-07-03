using System.Data;

namespace arquitectSoft.Engine
{
    /// <summary>
    /// Resultado de un análisis: una tabla calculada por cada pestaña de la
    /// ventana "Análisis de Mamparas". Es el equivalente, sin UI, de los
    /// DataGridView "...Calculate" del formulario WinForms.
    /// El que un campo quede null significa "esa pestaña no tiene datos"
    /// (igual que un grid vacío en WinForms).
    /// </summary>
    public class ResultadoAnalisis
    {
        public DataTable PerfilMetalico { get; set; }          // dataGridViewPMCalculate
        public DataTable PerfilMetalicoHerraje { get; set; }   // dataGridViewPMHerrajeCalculate
        public DataTable VidrioPaneles { get; set; }           // dataGridViewVPCalculate
        public DataTable Puertas { get; set; }                 // dataGridViewPCalculate
        public DataTable PuertasHerraje { get; set; }          // dataGridViewPHerrajeCalculate
        public DataTable PuertasCantidad { get; set; }         // dataGridViewP2Calculate
        public DataTable Tubos { get; set; }                   // dataGridViewTMCalculate (en WinForms queda vacío)
        public DataTable Mamparas { get; set; }                // dataGridViewMCalculate
        public DataTable PMVHerraje { get; set; }              // dataGridViewPMVHerrajeCalculate

        // --- Datos CRUDOS importados del TXT (para el previsualizador) ---
        public DataTable PerfilMetalicoRaw { get; set; }
        public DataTable VidrioRaw { get; set; }
        public DataTable PuertasRaw { get; set; }
        public DataTable TubosRaw { get; set; }
        public DataTable MamparasRaw { get; set; }

        /// <summary>Índice de pestaña que el análisis sugiere mostrar (orden de la ventana WPF).</summary>
        public int PestanaSugerida { get; set; }

        /// <summary>Valor de Global.SwSegmentadoUbi al terminar el análisis (lo usa el export para agrupar Perfil Metálico, igual que WinForms).</summary>
        public string SwSegmentadoUbiFinal { get; set; }

        /// <summary>True si alguna tabla calculada trae filas (equivale a FnValidateData()).</summary>
        public bool TieneDatos
        {
            get
            {
                return ConFilas(PerfilMetalico) || ConFilas(PerfilMetalicoHerraje)
                    || ConFilas(VidrioPaneles) || ConFilas(Puertas)
                    || ConFilas(PuertasHerraje) || ConFilas(PuertasCantidad)
                    || ConFilas(Tubos) || ConFilas(Mamparas)
                    || ConFilas(PMVHerraje);
            }
        }

        private static bool ConFilas(DataTable t)
        {
            return t != null && t.Rows.Count > 0;
        }

        /// <summary>
        /// Copia profunda del resultado (cada DataTable se clona con sus filas). Se usa como
        /// "base intacta": las tablas calculadas traen los acabados placeholder (MOD…) y la
        /// perfilería por defecto; al cambiar la perfilería se reconstruye desde esta copia
        /// para poder re-resolver las dependencias tantas veces como haga falta.
        /// </summary>
        public ResultadoAnalisis Copiar()
        {
            return new ResultadoAnalisis
            {
                PerfilMetalico = Dup(PerfilMetalico),
                PerfilMetalicoHerraje = Dup(PerfilMetalicoHerraje),
                VidrioPaneles = Dup(VidrioPaneles),
                Puertas = Dup(Puertas),
                PuertasHerraje = Dup(PuertasHerraje),
                PuertasCantidad = Dup(PuertasCantidad),
                Tubos = Dup(Tubos),
                Mamparas = Dup(Mamparas),
                PMVHerraje = Dup(PMVHerraje),
                PerfilMetalicoRaw = Dup(PerfilMetalicoRaw),
                VidrioRaw = Dup(VidrioRaw),
                PuertasRaw = Dup(PuertasRaw),
                TubosRaw = Dup(TubosRaw),
                MamparasRaw = Dup(MamparasRaw),
                PestanaSugerida = PestanaSugerida,
                SwSegmentadoUbiFinal = SwSegmentadoUbiFinal
            };
        }

        private static DataTable Dup(DataTable t)
        {
            return t == null ? null : t.Copy();
        }
    }
}
