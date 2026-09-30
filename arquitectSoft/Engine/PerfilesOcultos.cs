using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace arquitectSoft.Engine
{
    /// <summary>
    /// Perfiles OCULTOS: no se ven (estructura interna, soporte), p. ej. el zócalo OX.
    /// Sus tramos recopilatorios no usan la Medida Base de 2960 sino la "Medida base
    /// ocultos", que se propone para enviar lo MÍNIMO a obra: como no se ven, da igual
    /// dónde caigan los empalmes. Se marcan por código base (sin acabado) en la tabla
    /// beta_perfil_oculto (db/migrations/009_perfiles_ocultos.sql), desde Subcomponentes.
    ///
    /// No toca el SP: se aplica sobre la Perfilería ya calculada, a partir de la
    /// "Medidida Calculada" (la suma de tramos, con su C.adicional y el % de desperdicio).
    /// </summary>
    public static class PerfilesOcultos
    {
        /// <summary>Límites de la fábrica: mínimo que envía y máximo que cabe en el transporte.</summary>
        public const int BaseMin = 1000, BaseMax = 3000;
        /// <summary>La base propuesta va en múltiplos de 100 (medida redonda para cortar).</summary>
        public const int Paso = 100;

        // ===== Catálogo =====

        /// <summary>Códigos base marcados como ocultos. Sin tabla o sin conexión = ninguno.</summary>
        public static HashSet<string> Cargar()
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                var con = new Generals.Conexion();
                string fail;
                con.Open(out fail);
                DataSet ds = con.ExecuteDataSet("SELECT Codigo FROM beta_perfil_oculto", out fail);
                con.Close();
                if (ds != null && ds.Tables.Count > 0)
                    foreach (DataRow r in ds.Tables[0].Rows)
                        set.Add(Convert.ToString(r[0]).Trim());
            }
            catch { /* sin tabla/BD → ningún perfil oculto, el análisis sigue como siempre */ }
            return set;
        }

        /// <summary>¿Está marcado este código? Acepta "IMC0004" o "IMC0004-72".</summary>
        public static bool EstaMarcado(string codigo)
        {
            return Cargar().Contains(CodigoBase(codigo));
        }

        /// <summary>Marca o desmarca un código base. Devuelve "" si fue bien, o el error.</summary>
        public static string Marcar(string codigo, bool oculto)
        {
            string cod = CodigoBase(codigo);
            if (cod == "") return "";
            var con = new Generals.Conexion();
            string fail;
            con.Open(out fail);
            if (fail != "") return fail;
            con.ExecuteNonQuery(oculto
                    ? "INSERT IGNORE INTO beta_perfil_oculto (Codigo) VALUES (?)"
                    : "DELETE FROM beta_perfil_oculto WHERE Codigo = ?",
                out fail, new[] { cod }, 0);
            con.Close();
            // Desmarcar sin la tabla (base sin la migración 009) no es un error: ya no está.
            return oculto ? fail : "";
        }

        public static string CodigoBase(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo)) return "";
            return codigo.Split('-')[0].Trim();
        }

        // ===== Cálculo =====

        /// <summary>Filas recopilatorias de perfiles ocultos, con su total en mm.</summary>
        private static List<Tuple<DataRow, decimal>> Filas(DataTable pm, HashSet<string> ocultos)
        {
            var res = new List<Tuple<DataRow, decimal>>();
            if (pm == null || ocultos == null || ocultos.Count == 0) return res;
            if (!pm.Columns.Contains("Se_Calcula_Por") || !pm.Columns.Contains("codigo")
                || !pm.Columns.Contains("Medidida Calculada")) return res;

            foreach (DataRow r in pm.Rows)
            {
                if (!EsRecopilatoria(Convert.ToString(r["Se_Calcula_Por"]))) continue;
                if (!ocultos.Contains(CodigoBase(Convert.ToString(r["codigo"])))) continue;
                decimal total = Numero(r["Medidida Calculada"]);
                if (total > 0) res.Add(Tuple.Create(r, total));
            }
            return res;
        }

        /// <summary>
        /// Unidad 1. La tabla final ya no trae Id_Unidad_Medida (GetDataFinal la quita), así
        /// que se reconoce por el texto de fnUnidadCalculada: "Longitud Recompilacón"
        /// (la 5 es "Longitud sin Recompilacón").
        /// </summary>
        private static bool EsRecopilatoria(string calculaPor)
        {
            string t = (calculaPor ?? "").ToLowerInvariant();
            return (t.Contains("recomp") || t.Contains("recop")) && !t.Contains(" sin ");
        }

        /// <summary>¿Hay algún perfil oculto recopilatorio en esta perfilería?</summary>
        public static bool Hay(DataTable pm, HashSet<string> ocultos)
        {
            return Filas(pm, ocultos).Count > 0;
        }

        /// <summary>
        /// Medida base que envía lo mínimo a obra para TODOS los ocultos del proyecto
        /// (una sola base, entre 1000 y 3000, de 100 en 100). A igualdad de material, la de
        /// menos piezas. 0 si no hay ocultos recopilatorios.
        /// Ej.: 3180 mm → 2 × 1600 = 3200 (con 2960: 1 × 2960, faltaban 220).
        /// </summary>
        public static int Proponer(DataTable pm, HashSet<string> ocultos)
        {
            var totales = Filas(pm, ocultos).Select(f => f.Item2).ToList();
            if (totales.Count == 0) return 0;

            int mejor = 0;
            decimal mejorEnvio = decimal.MaxValue;
            int mejorPiezas = int.MaxValue;
            for (int b = BaseMin; b <= BaseMax; b += Paso)
            {
                int piezas = totales.Sum(t => Piezas(t, b));
                decimal envio = (decimal)piezas * b;
                if (envio < mejorEnvio || (envio == mejorEnvio && piezas < mejorPiezas))
                {
                    mejor = b;
                    mejorEnvio = envio;
                    mejorPiezas = piezas;
                }
            }
            return mejor;
        }

        /// <summary>
        /// Pone la base de ocultos en sus filas: medida = base, cantidad = piezas enteras
        /// que cubren el total (siempre hacia arriba: aquí no hay regla del 0,1).
        /// Devuelve cuántas filas cambió.
        /// </summary>
        public static int Aplicar(DataTable pm, HashSet<string> ocultos, int medidaBase)
        {
            if (medidaBase <= 0) return 0;
            var filas = Filas(pm, ocultos);
            foreach (var f in filas)
            {
                f.Item1["cantidad"] = Piezas(f.Item2, medidaBase).ToString(CultureInfo.InvariantCulture);
                f.Item1["medida"] = medidaBase.ToString(CultureInfo.InvariantCulture);
            }
            return filas.Count;
        }

        private static int Piezas(decimal total, int medidaBase)
        {
            return (int)Math.Ceiling(total / medidaBase);
        }

        // La tabla trae texto; según la máquina, con punto o coma decimal.
        private static decimal Numero(object v)
        {
            // Son milímetros, sin separador de miles: la coma solo puede ser el decimal.
            string s = Convert.ToString(v).Trim().Replace(',', '.');
            decimal d;
            return decimal.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out d) ? d : 0;
        }
    }
}
