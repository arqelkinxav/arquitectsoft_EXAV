using System;
using System.Collections.Generic;
using System.Data;

namespace arquitectSoft.Engine
{
    /// <summary>
    /// Resuelve los acabados "placeholder" (código MOD…) de un resultado ya calculado según
    /// el acabado de perfilería vigente. Un MOD… es un acabado variable: para cada valor de
    /// perfilería el usuario define, en la tabla <c>beta_dependencias_acabado</c>, a qué
    /// acabado REAL debe convertirse (regla = placeholder + perfilería → resultado).
    ///
    /// La sustitución se hace reutilizando <see cref="AcabadoChanger"/> (ya sabe recorrer las
    /// distintas disposiciones de columnas de cada pestaña), aplicando por cada regla que
    /// corresponda a la perfilería actual el cambio "MOD… → acabado real".
    /// </summary>
    public class DependenciaResolver
    {
        private readonly Dictionary<string, string> _desc =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);   // código → descripción
        private readonly List<Regla> _reglas = new List<Regla>();
        private readonly HashSet<string> _placeholders =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);              // códigos placeholder (de las reglas)

        private struct Regla { public string Ph, Perf, Res; }

        /// <summary>True si hay al menos una regla configurada.</summary>
        public bool HayReglas { get { return _reglas.Count > 0; } }

        /// <summary>Carga las reglas y el mapa de descripciones desde la base. Si la tabla no
        /// existe o falla la conexión, queda vacío y actúa como no-op (no rompe el análisis).</summary>
        public static DependenciaResolver Cargar()
        {
            var r = new DependenciaResolver();
            try
            {
                var dto = new Dto.DependenciaDto();
                foreach (DataRow row in dto.GetMapaDescripciones().Rows)
                    r._desc[Convert.ToString(row["Codigo_Homologacion"]).Trim()] =
                        Convert.ToString(row["Descripcion"]).Trim();
                foreach (DataRow row in dto.GetReglas().Rows)
                {
                    var regla = new Regla
                    {
                        Ph = Convert.ToString(row["Cod_Placeholder"]).Trim(),
                        Perf = Convert.ToString(row["Cod_Perfileria"]).Trim(),
                        Res = Convert.ToString(row["Cod_Resultado"]).Trim()
                    };
                    r._reglas.Add(regla);
                    r._placeholders.Add(regla.Ph);
                }
            }
            catch { /* sin tabla/BD → resolver vacío */ }
            return r;
        }

        /// <summary>
        /// Resuelve en <paramref name="res"/> los placeholders según el código de perfilería
        /// vigente. Devuelve los placeholders que quedaron SIN resolver (presentes pero sin
        /// regla para esa perfilería), para poder avisar al usuario.
        /// </summary>
        public List<string> Resolver(ResultadoAnalisis res, string codPerfileria)
        {
            var sinResolver = new List<string>();
            if (res == null || _placeholders.Count == 0) return sinResolver;
            codPerfileria = (codPerfileria ?? "").Trim();

            foreach (DataTable t in Tablas(res))
            {
                if (t == null || t.Columns.Count < 2) continue;
                foreach (DataRow row in t.Rows)
                {
                    // El código con sufijo de acabado va en col 1 (perfil/herrajes/puertas) o
                    // en col 0 (mamparas). Buscamos el primero cuyo sufijo sea un placeholder.
                    // OJO: NO se salta por col 0 vacía (los herrajes de puertas la traen vacía).
                    int cCod = -1; string suf = null;
                    foreach (int c in new[] { 1, 0 })
                    {
                        if (c >= t.Columns.Count) continue;
                        string cod = Convert.ToString(row[c]);
                        int dash = cod.LastIndexOf('-');
                        if (dash < 0) continue;
                        string s = cod.Substring(dash + 1).Trim();
                        if (_placeholders.Contains(s)) { cCod = c; suf = s; break; }
                    }
                    if (cCod < 0) continue;

                    string resultado;
                    if (!TryResultado(suf, codPerfileria, out resultado))
                    {
                        if (!sinResolver.Contains(suf)) sinResolver.Add(suf);
                        continue;
                    }

                    // Reemplaza el sufijo del código y la descripción del acabado.
                    string codActual = Convert.ToString(row[cCod]);
                    int dash2 = codActual.LastIndexOf('-');
                    row[cCod] = codActual.Substring(0, dash2) + "-" + resultado;

                    int cAcab = (cCod == 1) ? 3 : 2;   // estándar: acabado en col 3; mamparas: col 2
                    if (cAcab < t.Columns.Count) row[cAcab] = Desc(resultado);
                }
            }
            return sinResolver;
        }

        private bool TryResultado(string placeholder, string perfileria, out string resultado)
        {
            foreach (var r in _reglas)
                if (string.Equals(r.Ph, placeholder, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(r.Perf, perfileria, StringComparison.OrdinalIgnoreCase))
                { resultado = r.Res; return true; }
            resultado = null;
            return false;
        }

        private string Desc(string cod)
        {
            string d;
            return _desc.TryGetValue((cod ?? "").Trim(), out d) ? d : "";
        }

        private static IEnumerable<DataTable> Tablas(ResultadoAnalisis r)
        {
            return new[]
            {
                r.PerfilMetalico, r.Puertas, r.PerfilMetalicoHerraje, r.PuertasHerraje,
                r.VidrioPaneles, r.PuertasCantidad, r.Tubos, r.Mamparas
            };
        }
    }
}
