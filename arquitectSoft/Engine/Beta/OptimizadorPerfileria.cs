using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace arquitectSoft.Engine.Beta
{
    /// <summary>
    /// ANÁLISIS BETA (solo Olimpo): optimizador de perfilería. Va APARTE del cálculo de siempre:
    /// relee los mismos TXT y el catálogo y no toca ni el motor ni los procedimientos.
    ///
    /// Recopilatorias (unidad 1): en vez de sumar todo y dividir por la Medida Base, reparte tramo
    /// a tramo, propone la medida base de la obra, manda media pieza cuando alcanza, sólo añade un
    /// empalme si ahorra al menos 1 m y ningún trozo baja de 300. Los restos de 1000 o más no viajan:
    /// se cortan en fábrica y se quedan. Repuestos: si la holgura del perfil no llega al 2 % se
    /// completa, con piezas del tamaño del trozo más largo.
    ///
    /// Piezas a medida (unidad 5): se cortan en los mismos tubos que las recopilatorias, por tubo en
    /// bruto y acabado, usando primero los retazos del almacén si hay archivo de almacén.
    ///
    /// Es el port de D:\CLAUDE\optimizador_recopilatorias (Python), donde están las simulaciones.
    /// </summary>
    public static class OptimizadorPerfileria
    {
        public const int Barra = 6000;
        public const int Margen = 40;          // inglete + disco por pieza en fábrica (6000 → 2 × 2960)
        public const int MaxPieza = 3000;      // transporte
        public const int MinEnvio = 1000;      // pieza más corta que se manda / retazo que se guarda
        public const int MinVisible = 300;     // trozo más corto que se admite en obra
        public const int CosteEmpalme = 1000;  // un empalme de más tiene que ahorrar 1 m de tubo
        public const double Errores = 0.02;    // el 2 % decide si hace falta repuesto
        public const int MinFrentesRepuesto = 5; // obras de menos frentes (M1, M2…) no llevan repuestos

        // Variantes que salen del mismo tubo en bruto (extremos y troqueles se hacen después de cortar).
        private static readonly Regex AceroDe20 = new Regex(@"^IMC0003[A-Z]?-", RegexOptions.IgnoreCase);

        public static string TuboBruto(string codigo)
        {
            if (AceroDe20.IsMatch(codigo)) return "IMC0003*" + codigo.Substring(codigo.IndexOf('-'));
            return codigo;
        }

        // ================= modelo =================

        public class Tramo
        {
            public int Largo;
            public double LargoTxt;      // tal cual viene en el TXT (con decimales)
            public string Ubicacion;
            public string Codigo;
            public bool Vertical;        // del TXT 0- (verticales) o con "VERTICAL" en el ITEM
        }

        public class Perfil
        {
            public string Sub;           // código-acabado del catálogo (ITC0002-01)
            public string Descripcion;
            public string Acabado;
            public string TipoCorte;
            public List<Tramo> Tramos = new List<Tramo>();
            public List<int> Sumas = new List<int>();      // lo que suma el programa hoy, fila a fila
        }

        public class Pieza
        {
            public int Usado;
            public List<KeyValuePair<int, int>> Segs = new List<KeyValuePair<int, int>>();   // (largo, índice de tramo)
        }

        public class Reparto
        {
            public Tramo Tramo;
            public List<int> Partes;
            public string Tipo;          // entera | sim | ajuste | restos
        }

        public class PlanPerfil
        {
            public List<Reparto> Lay;
            public List<Pieza> Piezas;
            public int Enteras, Medias, Media, Enviado, Empalmes, Barras;
            public double Consumo;
        }

        public class Evaluacion
        {
            public int B;
            public double Consumo;
            public int Enviado, Empalmes, Barras;
            public Dictionary<string, PlanPerfil> Detalle = new Dictionary<string, PlanPerfil>();
        }

        public class Analisis
        {
            public Dictionary<string, Perfil> Perfiles;
            public int BaseHoy;
            public double Desp;
            public Evaluacion Propuesta, Referencia, Alternativa;   // Alternativa: la 2960 cuando se propone otra
            public Dictionary<string, double> PiezasHoy = new Dictionary<string, double>();
            public double ConsumoHoy;
            public int BarrasHoy;
            public List<string> Justificacion = new List<string>();
        }

        // ================= lectura =================

        internal class Fila
        {
            public string Comp, AcabComp, Uni, Sub, SubDesc, AcabDesc, TipoCorte;
            public int Cdef, Adic;
            public bool Decr;
        }

        public class Catalogo
        {
            internal readonly Dictionary<string, List<Fila>> Por = new Dictionary<string, List<Fila>>(StringComparer.OrdinalIgnoreCase);

            internal List<Fila> Buscar(string cod, string uni)
            {
                List<Fila> l;
                if (!Por.TryGetValue(cod, out l) && !(cod.IndexOf('-') < 0 && Por.TryGetValue("#" + cod, out l)))
                    return new List<Fila>();
                return l.Where(f => f.Uni == uni).ToList();
            }
        }

        public static Catalogo CargarCatalogo()
        {
            const string sql = @"SELECT componentes.Codigo AS comp, IFNULL(ap.Codigo_Homologacion,'') AS acab_comp,
 cd.Id_Unidad_Calculada AS uni, cd.Cantidad_Default AS cdef, cd.Cantidad_Adicional AS adic, cd.Aplica_Decremento AS decr,
 CONCAT(s.codigo_homologacion,'-',a.codigo_homologacion) AS sub, s.Descripcion AS subdesc, a.Descripcion AS acabdesc,
 IFNULL(ct.descripcion,'') AS tipocorte
FROM componentes_detalle cd JOIN componentes ON cd.Id_Componente=componentes.Id_Componente
LEFT JOIN acabados ap ON componentes.AcabadoPrincipal=ap.Id_Acabado
JOIN subcomponentes s ON s.Id_Subcomponente=cd.Id_Subcomponente JOIN acabados a ON a.Id_Acabado=s.Id_Acabado
LEFT JOIN cortes ct ON ct.Id_Corte=cd.idcorte
WHERE cd.Asignacion_puertas=0 AND cd.Id_Unidad_Calculada IN (1,5)";
            var cat = new Catalogo();
            var con = new Generals.Conexion();
            string fail;
            con.Open(out fail);
            DataSet ds = con.ExecuteDataSet(sql, out fail);
            con.Close();
            if (ds == null || ds.Tables.Count == 0)
                throw new InvalidOperationException("No se pudo leer el catálogo: " + fail);
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                var f = new Fila
                {
                    Comp = Convert.ToString(r["comp"]).Trim(),
                    AcabComp = Convert.ToString(r["acab_comp"]).Trim(),
                    Uni = Convert.ToString(r["uni"]).Trim(),
                    Cdef = Convert.ToInt32(r["cdef"]),
                    Adic = Convert.ToInt32(r["adic"]),
                    Decr = Convert.ToString(r["decr"]) == "1" || Convert.ToString(r["decr"]).Equals("True", StringComparison.OrdinalIgnoreCase),
                    Sub = Convert.ToString(r["sub"]).Trim(),
                    SubDesc = Convert.ToString(r["subdesc"]).Trim(),
                    AcabDesc = Convert.ToString(r["acabdesc"]).Trim(),
                    TipoCorte = Convert.ToString(r["tipocorte"]).Trim()
                };
                string key = f.Comp + (f.AcabComp.Length > 0 ? "-" + f.AcabComp : "");
                Agregar(cat, key, f);
                Agregar(cat, "#" + f.Comp, f);
            }
            return cat;
        }

        private static void Agregar(Catalogo c, string k, Fila f)
        {
            List<Fila> l;
            if (!c.Por.TryGetValue(k, out l)) c.Por[k] = l = new List<Fila>();
            l.Add(f);
        }

        /// <summary>Filas (código, longitud, ubicación) de los TXT de despiece con columna de longitud.</summary>
        public static List<Tramo> LeerTxt(IEnumerable<string> archivos)
        {
            var salida = new List<Tramo>();
            foreach (string ruta in archivos)
            {
                string[] lineas;
                try { lineas = File.ReadAllLines(ruta); } catch { continue; }
                if (lineas.Length < 3) continue;
                var cab = Celdas(lineas[1]).Select(c => c.ToUpperInvariant()).ToList();
                int colL = cab.FindIndex(c => c == "LONGITUD" || c == "ALTURA" || c == "ALTO");
                int colU = cab.FindIndex(c => c.StartsWith("UBICA"));
                int colItem = cab.FindIndex(c => c == "ITEM");
                bool archivoVertical = Path.GetFileName(ruta).TrimStart().StartsWith("0-");
                // Modelos genéricos (TXT 4-): la ubicación (M1-M2-M3) viene en "Comentarios".
                if (colU < 0) colU = cab.FindIndex(c => c == "COMENTARIOS");
                if (colL < 0 || cab.Contains("ANCHURA")) continue;
                double ult = 0;
                for (int i = 3; i < lineas.Length; i++)
                {
                    var c = Celdas(lineas[i]);
                    if (c.Count == 0 || c[0].Length == 0) continue;
                    string v = colL < c.Count ? c[colL] : "";
                    double L;
                    if (v.Length == 0) L = ult;
                    else if (!double.TryParse(v.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out L)) continue;
                    ult = L;
                    salida.Add(new Tramo
                    {
                        Codigo = c[0],
                        LargoTxt = L,
                        Largo = (int)Math.Round(L),
                        Ubicacion = colU >= 0 && colU < c.Count ? c[colU] : "",
                        Vertical = archivoVertical
                                   || (colItem >= 0 && colItem < c.Count
                                       && c[colItem].IndexOf("VERTICAL", StringComparison.OrdinalIgnoreCase) >= 0)
                    });
                }
            }
            return salida;
        }

        private static List<string> Celdas(string linea)
        {
            return linea.TrimEnd('\r', '\n').Split('\t').Select(x => x.Trim().Trim('"').Trim().TrimStart('\uFEFF')).ToList();
        }

        /// <summary>Tramos recopilatorios por perfil (unidad 1), como los calcula spComponentePerfilesCargar.</summary>
        public static Dictionary<string, Perfil> Recopilatorias(Catalogo cat, List<Tramo> filas)
        {
            var subs = new Dictionary<string, Perfil>();
            foreach (var t in filas)
                foreach (var d in cat.Buscar(t.Codigo, "1"))
                {
                    double val = d.Decr ? t.LargoTxt - d.Adic : t.LargoTxt + d.Adic;
                    Perfil p;
                    if (!subs.TryGetValue(d.Sub, out p))
                        subs[d.Sub] = p = new Perfil { Sub = d.Sub, Descripcion = d.SubDesc, Acabado = d.AcabDesc, TipoCorte = d.TipoCorte };
                    p.Sumas.Add((int)Math.Ceiling(val * d.Cdef));
                    for (int i = 0; i < d.Cdef; i++)
                        p.Tramos.Add(new Tramo { Largo = (int)Math.Ceiling(val), Ubicacion = t.Ubicacion, Codigo = t.Codigo, Vertical = t.Vertical });
                }
            return subs.Where(kv => kv.Value.Sumas.Sum() > 0).ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        public class PiezaMedida
        {
            public string Sub, Descripcion, Acabado, Ubicacion, TipoCorte;
            public int Largo;
            public bool Vertical;
        }

        /// <summary>Piezas a medida (unidad 5): largo ± C. adicional, una por Cantidad por defecto.</summary>
        public static List<PiezaMedida> ACorte(Catalogo cat, List<Tramo> filas)
        {
            var l = new List<PiezaMedida>();
            foreach (var t in filas)
                foreach (var d in cat.Buscar(t.Codigo, "5"))
                {
                    int v = (int)Math.Ceiling(d.Decr ? t.LargoTxt - d.Adic : t.LargoTxt + d.Adic);
                    if (v <= 0) continue;
                    for (int i = 0; i < d.Cdef; i++)
                        l.Add(new PiezaMedida { Sub = d.Sub, Descripcion = d.SubDesc, Acabado = d.AcabDesc, Ubicacion = t.Ubicacion, Largo = v, TipoCorte = d.TipoCorte, Vertical = t.Vertical });
                }
            return l;
        }

        // ================= cálculo de hoy =================

        public static double PiezasHoy(double suma, int b, double desp)
        {
            double x = suma / b;
            if (x - Math.Floor(x) < 0.1) return Math.Floor(x) * desp;
            return Math.Ceiling(suma * desp / b);
        }

        public static int PiezasPorBarra(int b) { return b + Margen <= Barra ? Math.Max(1, Barra / (b + Margen)) : 1; }
        /// <summary>
        /// Tubo que gasta cada pieza de medida base <paramref name="b"/>. El resto del tubo, si es de
        /// 1000 o más, vuelve al almacén (lo guarda el plan de corte), así que no se carga a las piezas;
        /// si es más corto es merma y se reparte entre ellas.
        /// </summary>
        public static double ConsumoPieza(int b)
        {
            return Retazo(b) >= MinEnvio ? b + Margen : (double)Barra / PiezasPorBarra(b);
        }
        public static int Retazo(int b) { return Barra - PiezasPorBarra(b) * (b + Margen); }

        // ================= reparto de recopilatorias =================

        private static List<int> Repartir(int t, int b, string modo, out string tipo)
        {
            int n = (t + b - 1) / b;
            if (n <= 1) { tipo = "entera"; return new List<int> { t }; }
            var l = new List<int>();
            if (modo == "sim")
            {
                tipo = "sim";
                int p = (t + n - 1) / n;
                for (int i = 0; i < n - 1; i++) l.Add(p);
                l.Add(t - p * (n - 1));
                return l;
            }
            tipo = "ajuste";
            int r = t - (n - 1) * b;
            if (r >= MinVisible)
            {
                for (int i = 0; i < n - 1; i++) l.Add(b);
                l.Add(r);
                return l;
            }
            for (int i = 0; i < n - 2; i++) l.Add(b);
            l.Add(b - (MinVisible - r));
            l.Add(MinVisible);
            return l;
        }

        /// <summary>Primero el trozo más largo, en la pieza donde mejor quepa (lo que hace el instalador con los sobrantes).</summary>
        private static List<Pieza> Empacar(List<KeyValuePair<int, int>> segs, int b, bool flex, HashSet<int> extra, List<Pieza> piezas = null)
        {
            piezas = piezas ?? new List<Pieza>();
            foreach (var sg in segs.OrderByDescending(z => z.Key))
            {
                int s = sg.Key, i = sg.Value;
                Pieza mejor = null;
                foreach (var p in piezas)
                {
                    int libre = b - p.Usado;
                    if (s <= libre && (mejor == null || libre < b - mejor.Usado)) mejor = p;
                }
                if (mejor == null && flex && s >= 2 * MinVisible)
                {
                    var libres = piezas.Select((p, j) => new { L = b - p.Usado, J = j })
                                       .Where(x => x.L >= MinVisible).OrderBy(x => x.L).ThenBy(x => x.J).ToList();
                    bool hecho = false;
                    for (int x = 0; x < libres.Count && !hecho; x++)
                        for (int y = x + 1; y < libres.Count; y++)
                        {
                            int la = libres[x].L, lb = libres[y].L;
                            if (la + lb < s) continue;
                            int a = Math.Max(MinVisible, s - lb), bb = s - a;
                            if (a <= la && bb >= MinVisible && bb <= lb)
                            {
                                var pa = piezas[libres[x].J]; pa.Usado += a; pa.Segs.Add(new KeyValuePair<int, int>(a, i));
                                var pb = piezas[libres[y].J]; pb.Usado += bb; pb.Segs.Add(new KeyValuePair<int, int>(bb, i));
                                if (extra != null) extra.Add(i);
                                hecho = true;
                                break;
                            }
                        }
                    if (hecho) continue;
                }
                if (mejor == null)
                {
                    mejor = new Pieza();
                    piezas.Add(mejor);
                }
                mejor.Usado += s;
                mejor.Segs.Add(new KeyValuePair<int, int>(s, i));
            }
            return piezas;
        }

        private static string TipoDe(List<int> partes)
        {
            if (partes.Count == 1) return "entera";
            return partes.Max() - partes.Min() <= 10 ? "sim" : "ajuste";
        }

        /// <summary>Cortos primero; los largos arrancan con el sobrante que mejor sirva, sin añadir empalmes.</summary>
        private static void PlanResto(List<Tramo> tramos, int b, out List<Reparto> lay, out List<Pieza> piezas, out int emp)
        {
            var cortos = new List<KeyValuePair<int, int>>();
            var largos = new List<KeyValuePair<int, int>>();
            for (int i = 0; i < tramos.Count; i++)
                (tramos[i].Largo <= b ? cortos : largos).Add(new KeyValuePair<int, int>(tramos[i].Largo, i));
            largos = largos.OrderByDescending(z => z.Key).ThenByDescending(z => z.Value).ToList();
            piezas = Empacar(cortos, b, false, null);
            var partesDe = new Dictionary<int, List<int>>();
            foreach (var c in cortos) partesDe[c.Value] = new List<int> { c.Key };
            foreach (var lg in largos)
            {
                int t = lg.Key, i = lg.Value;
                int n = (t + b - 1) / b;
                int minimo = Math.Max(MinVisible, t - (n - 1) * b);
                var partes = new List<int>();
                int resto = t;
                var libre = piezas.Select((p, j) => new { L = b - p.Usado, J = j })
                                  .Where(x => x.L >= minimo).OrderBy(x => x.L).ThenBy(x => x.J).FirstOrDefault();
                if (libre != null)
                {
                    int a = n >= 2 ? Math.Min(libre.L, resto - (n - 2) * b - MinVisible) : libre.L;
                    if (a >= minimo)
                    {
                        piezas[libre.J].Usado += a;
                        piezas[libre.J].Segs.Add(new KeyValuePair<int, int>(a, i));
                        partes.Add(a); resto -= a; n -= 1;
                    }
                }
                int p1 = (resto + n - 1) / n;
                var cola = new List<int>();
                for (int k = 0; k < n - 1; k++) cola.Add(p1);
                cola.Add(resto - p1 * (n - 1));
                foreach (int c in cola)
                {
                    var hueco = piezas.Select((p, j) => new { L = b - p.Usado, J = j })
                                      .Where(x => x.L >= c).OrderBy(x => x.L).ThenBy(x => x.J).FirstOrDefault();
                    if (hueco != null)
                    {
                        piezas[hueco.J].Usado += c;
                        piezas[hueco.J].Segs.Add(new KeyValuePair<int, int>(c, i));
                    }
                    else
                    {
                        var nueva = new Pieza { Usado = c };
                        nueva.Segs.Add(new KeyValuePair<int, int>(c, i));
                        piezas.Add(nueva);
                    }
                }
                partes.AddRange(cola);
                partesDe[i] = partes;
            }
            lay = new List<Reparto>();
            emp = 0;
            for (int i = 0; i < tramos.Count; i++)
            {
                var pr = partesDe[i];
                lay.Add(new Reparto { Tramo = tramos[i], Partes = pr, Tipo = TipoDe(pr) });
                emp += pr.Count - 1;
            }
        }

        public static PlanPerfil OptimizarPerfil(List<Tramo> tramos, int b)
        {
            PlanPerfil mejor = null;
            var modos = new[] { Tuple.Create("sim", false), Tuple.Create("ajuste", false), Tuple.Create("resto", false),
                                Tuple.Create("sim", true), Tuple.Create("ajuste", true) };
            int media = b / 2;
            bool usaMedia = media >= MinEnvio;
            foreach (var m in modos)
            {
                List<Reparto> lay;
                List<Pieza> piezas;
                int emp;
                if (m.Item1 == "resto")
                {
                    PlanResto(tramos, b, out lay, out piezas, out emp);
                }
                else
                {
                    var segs = new List<KeyValuePair<int, int>>();
                    lay = new List<Reparto>();
                    emp = 0;
                    for (int i = 0; i < tramos.Count; i++)
                    {
                        string tipo;
                        var partes = Repartir(tramos[i].Largo, b, m.Item1, out tipo);
                        emp += partes.Count - 1;
                        lay.Add(new Reparto { Tramo = tramos[i], Partes = partes, Tipo = tipo });
                        foreach (int p in partes) segs.Add(new KeyValuePair<int, int>(p, i));
                    }
                    var extra = new HashSet<int>();
                    piezas = Empacar(segs, b, m.Item2, extra);
                    emp += extra.Count;
                    if (extra.Count > 0)
                    {
                        var partesDe = new Dictionary<int, List<int>>();
                        foreach (var pz in piezas)
                            foreach (var sg in pz.Segs)
                            {
                                List<int> l;
                                if (!partesDe.TryGetValue(sg.Value, out l)) partesDe[sg.Value] = l = new List<int>();
                                l.Add(sg.Key);
                            }
                        for (int i = 0; i < lay.Count; i++)
                            lay[i] = new Reparto { Tramo = lay[i].Tramo, Partes = partesDe[i], Tipo = extra.Contains(i) ? "restos" : lay[i].Tipo };
                    }
                }
                int enteras = piezas.Count(p => !(usaMedia && p.Usado <= media));
                int medias = piezas.Count - enteras;
                var cand = new PlanPerfil
                {
                    Lay = lay, Piezas = piezas, Enteras = enteras, Medias = medias, Media = media,
                    Enviado = enteras * b + medias * media, Empalmes = emp
                };
                if (mejor == null || Clave(cand) < Clave(mejor)
                    || (Clave(cand) == Clave(mejor) && cand.Empalmes < mejor.Empalmes))
                    mejor = cand;
            }
            int eq = mejor.Enteras + (mejor.Medias + 1) / 2;
            mejor.Barras = (eq + PiezasPorBarra(b) - 1) / PiezasPorBarra(b);
            mejor.Consumo = (mejor.Enteras + mejor.Medias / 2.0) * ConsumoPieza(b);
            return mejor;
        }

        private static long Clave(PlanPerfil p) { return (long)p.Enviado + CosteEmpalme * p.Empalmes; }

        public static Evaluacion Evaluar(Dictionary<string, Perfil> subs, int b)
        {
            var e = new Evaluacion { B = b };
            foreach (var kv in subs)
            {
                var r = OptimizarPerfil(kv.Value.Tramos, b);
                e.Enviado += r.Enviado; e.Consumo += r.Consumo; e.Empalmes += r.Empalmes; e.Barras += r.Barras;
                e.Detalle[kv.Key] = r;
            }
            return e;
        }

        public static Analisis Analizar(Dictionary<string, Perfil> subs, int baseHoy, int desperdicioPct)
        {
            var a = new Analisis { Perfiles = subs, BaseHoy = baseHoy, Desp = 1 + desperdicioPct / 100.0 };
            if (subs.Count == 0) return a;
            foreach (var kv in subs)
            {
                double pz = PiezasHoy(kv.Value.Sumas.Sum(), baseHoy, a.Desp);
                a.PiezasHoy[kv.Key] = pz;
                a.ConsumoHoy += pz * ConsumoPieza(baseHoy);
                a.BarrasHoy += (int)Math.Ceiling(Math.Ceiling(pz) / PiezasPorBarra(baseHoy));
            }
            var candidatas = new SortedSet<int> { baseHoy, MaxPieza - Margen };
            for (int b = MinEnvio; b <= MaxPieza - Margen; b += 10) candidatas.Add(b);
            var evals = candidatas.Where(b => b > 0).Select(b => Evaluar(subs, b)).ToList();
            a.Referencia = evals.First(e => e.B == baseHoy);
            a.Propuesta = evals.OrderBy(e => Math.Round(Puntos(e) / 100, MidpointRounding.ToEven))
                               .ThenBy(e => e.Barras).ThenByDescending(e => e.B).First();
            // La medida de siempre (2960: dos piezas limpias por tubo) solo cede si la otra ahorra
            // de verdad y no hace cortar más tubos; si no, se llenaría el almacén de restos justos.
            var estandar = evals.FirstOrDefault(e => e.B == MaxPieza - Margen);
            if (estandar != null && a.Propuesta.B != estandar.B)
            {
                double gana = Puntos(estandar) - Puntos(a.Propuesta);
                if (gana < Math.Max(1000, 0.03 * Puntos(estandar)) || a.Propuesta.Barras > estandar.Barras)
                    a.Propuesta = estandar;
                else
                    a.Alternativa = estandar;
            }
            a.Justificacion = Justificar(a, evals);
            return a;
        }

        private static double Puntos(Evaluacion e) { return e.Consumo + CosteEmpalme * e.Empalmes; }

        private static string M(double mm) { return (mm / 1000).ToString("0.0", CultureInfo.GetCultureInfo("es-ES")); }

        private static List<string> Justificar(Analisis a, List<Evaluacion> evals)
        {
            var f = new List<string>();
            var rec = a.Propuesta;
            int b = rec.B, r = Retazo(b);
            var tramos = a.Perfiles.Values.SelectMany(p => p.Tramos).ToList();
            f.Add(string.Format("Medida base propuesta: {0}. De cada tubo de 6000 salen {1} piezas{2}", b, PiezasPorBarra(b),
                r < 50 ? " y no sobra nada." : string.Format(" y quedan {0} mm de retazo ({1}).", r, r >= MinEnvio ? "se guarda para otra obra" : "se pierde")));
            f.Add(string.Format("{0} de {1} tramos caben en una sola pieza (sin empalme); en total hay {2} empalmes.",
                tramos.Count(t => t.Largo <= b), tramos.Count, rec.Empalmes));
            double d = a.ConsumoHoy - rec.Consumo;
            f.Add(string.Format("Frente a hoy (base {0}, {1} %): {2} tubos en vez de {3}, {4} m de tubo en vez de {5} m{6}",
                a.BaseHoy, Math.Round((a.Desp - 1) * 100), rec.Barras, a.BarrasHoy, M(rec.Consumo), M(a.ConsumoHoy),
                d > 1 ? string.Format(" → ahorra {0} m ({1:0} %).", M(d), 100 * d / a.ConsumoHoy)
                      : d < -1 ? string.Format(" → necesita {0} m más (hoy solo cuadra con trozos cortos o con el margen).", M(-d)) : " → igual."));
            if (a.Alternativa != null)
                f.Add(string.Format("Con la medida de siempre ({0}) harían falta {1} tubos, {2} empalmes y {3} m de tubo: {4} es mejor.",
                    a.Alternativa.B, a.Alternativa.Barras, a.Alternativa.Empalmes, M(a.Alternativa.Consumo), b));
            var otra = evals.Where(e => e.B != b).OrderBy(e => Math.Round(e.Consumo / 100, MidpointRounding.ToEven)).ThenBy(e => e.Empalmes).FirstOrDefault();
            if (otra != null && rec.Consumo - otra.Consumo >= Math.Max(1000, 0.03 * rec.Consumo))
                f.Add(string.Format("La base {0} gastaría {1} m menos, pero con {2} empalmes más y {3} tubos; por eso no se propone.",
                    otra.B, M(rec.Consumo - otra.Consumo), otra.Empalmes - rec.Empalmes, otra.Barras));
            else
                f.Add("Ninguna otra medida ahorra tubo de forma apreciable sin añadir empalmes.");
            return f;
        }

        // ================= hoja de fábrica, repuestos y plan de corte =================

        public class HojaPerfil
        {
            public string Codigo, Descripcion, Acabado;
            public int Base, Cortar;
            public List<int> Enviar = new List<int>();
            public List<int> Repuesto = new List<int>();
            public List<int> Queda = new List<int>();
            public List<int> PiezasTubo = new List<int>();   // lo que ocupa tubo (enviar + repuesto), para el plan de corte
            public PlanPerfil Plan;
            public double PiezasHoy;
        }

        /// <summary>
        /// Frentes de la obra: ubicaciones M1, M2, M3… de todos los TXT (las de los modelos genéricos,
        /// "M1-M2-M3", cuentan cada una). Los apilados (M2A) son el mismo frente que M2.
        /// </summary>
        public static int ContarFrentes(IEnumerable<string> archivos)
        {
            var frentes = new HashSet<int>();
            var rx = new Regex(@"(?<![A-Z0-9])M(\d+)", RegexOptions.IgnoreCase);
            foreach (string ruta in archivos)
            {
                string[] lineas;
                try { lineas = File.ReadAllLines(ruta); } catch { continue; }
                if (lineas.Length < 2) continue;
                var cab = Celdas(lineas[1]).Select(c => c.ToUpperInvariant()).ToList();
                int col = cab.FindIndex(c => c.StartsWith("UBICA"));
                if (col < 0) col = cab.FindIndex(c => c == "COMENTARIOS");
                if (col < 0) continue;
                for (int i = 2; i < lineas.Length; i++)
                {
                    var c = Celdas(lineas[i]);
                    if (col >= c.Count) continue;
                    foreach (Match m in rx.Matches(c[col])) frentes.Add(int.Parse(m.Groups[1].Value));
                }
            }
            return frentes.Count;
        }

        public const int RedondeoCorte = 100;   // las piezas cortadas en fábrica van a múltiplos de 100

        /// <summary>
        /// Largo con el que una pieza sale de fábrica: la medida base (o media pieza) entera, o cortada
        /// —redondeada hacia arriba a 100 mm— si así quedan 1000 o más para el almacén.
        /// </summary>
        public static int LargoEnvio(PlanPerfil r, Pieza pz, int b)
        {
            int L = r.Media >= MinEnvio && pz.Usado <= r.Media ? r.Media : b;
            int corte = (pz.Usado + RedondeoCorte - 1) / RedondeoCorte * RedondeoCorte;
            return L - corte >= MinEnvio ? corte : L;
        }

        public static HojaPerfil Hoja(Analisis a, string sub, bool conRepuestos = true)
        {
            var p = a.Perfiles[sub];
            var r = a.Propuesta.Detalle[sub];
            int b = a.Propuesta.B;
            var h = new HojaPerfil { Codigo = sub, Descripcion = p.Descripcion, Acabado = p.Acabado, Base = b, Plan = r, PiezasHoy = a.PiezasHoy[sub] };
            int inst = p.Tramos.Sum(t => t.Largo);
            foreach (var pz in r.Piezas)
            {
                int L = LargoEnvio(r, pz, b);
                h.Enviar.Add(L);
                int entera = r.Media >= MinEnvio && pz.Usado <= r.Media ? r.Media : b;
                if (L < entera) h.Queda.Add(entera - L);
            }
            h.Cortar = r.Enteras + (r.Medias + 1) / 2;
            if (r.Medias % 2 == 1) h.Queda.Add(r.Media);
            double falta = Errores * inst - Math.Max(0, h.Enviar.Sum() - inst);
            int largo = r.Lay.SelectMany(x => x.Partes).DefaultIfEmpty(0).Max();
            if (conRepuestos && falta > 0)
            {
                if (falta <= r.Media && largo <= r.Media && r.Media >= MinEnvio)
                {
                    if (r.Medias % 2 == 1) h.Queda.Remove(r.Media);
                    else { h.Cortar++; h.Queda.Add(r.Media); }
                    h.Repuesto.Add(r.Media);
                }
                else
                {
                    int nn = Math.Max(1, (int)Math.Ceiling(falta / b));
                    h.Cortar += nn;
                    for (int i = 0; i < nn; i++) h.Repuesto.Add(b);
                }
            }
            h.PiezasTubo.AddRange(h.Enviar);
            h.PiezasTubo.AddRange(h.Repuesto);
            return h;
        }

        public class Corte
        {
            public int Largo;
            public string Tipo;      // a medida · recopilatoria · REPUESTO
            public string Codigo, Ubicacion;
            public string Descripcion, Acabado, TipoCorte;   // tal como van al listado de fábrica
            public bool Vertical;
        }

        public class Tubo
        {
            public string Grupo, Acabado, Origen;   // Origen: "nuevo" o id del retazo
            public int Largo = Barra, Libre = Barra;
            public List<Corte> Cortes = new List<Corte>();
        }

        public class Retal
        {
            public string Id, Codigo;
            public int Largo;
        }

        /// <summary>Almacén opcional: almacen_retazos.csv junto al programa (id;codigo;largo_mm).</summary>
        public static List<Retal> LeerAlmacen(out string ruta)
        {
            ruta = null;
            try
            {
                string cfg = ConfigurationManager.AppSettings["AlmacenRetazos"];
                string r = !string.IsNullOrWhiteSpace(cfg) ? cfg.Trim()
                         : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "almacen_retazos.csv");
                if (!File.Exists(r)) return new List<Retal>();
                ruta = r;
                var l = new List<Retal>();
                foreach (string linea in File.ReadAllLines(r).Skip(1))
                {
                    var c = linea.Split(';', ',', '\t');
                    int largo;
                    if (c.Length < 3 || !int.TryParse(c[2].Trim(), out largo)) continue;
                    l.Add(new Retal { Id = c[0].Trim(), Codigo = c[1].Trim(), Largo = largo });
                }
                return l;
            }
            catch { return new List<Retal>(); }
        }

        /// <summary>Plan de corte por tubo en bruto: la pieza más larga primero, en el tubo o retazo donde mejor quepa.</summary>
        public static List<Tubo> PlanCorte(string grupo, string acabado, List<Corte> cortes, List<Retal> almacen)
        {
            var tubos = new List<Tubo>();
            foreach (var r in almacen.Where(x => string.Equals(TuboBruto(x.Codigo), grupo, StringComparison.OrdinalIgnoreCase)))
                tubos.Add(new Tubo { Grupo = grupo, Acabado = acabado, Origen = "retazo " + r.Id, Largo = r.Largo, Libre = r.Largo });
            foreach (var c in cortes.OrderByDescending(x => x.Largo))
            {
                int need = c.Largo + Margen;
                Tubo mejor = tubos.Where(t => t.Libre >= need).OrderBy(t => t.Libre).FirstOrDefault();
                if (mejor == null)
                {
                    mejor = new Tubo { Grupo = grupo, Acabado = acabado, Origen = "nuevo" };
                    tubos.Add(mejor);
                }
                mejor.Libre -= need;
                mejor.Cortes.Add(c);
            }
            return tubos.Where(t => t.Cortes.Count > 0).ToList();
        }

        /// <summary>Tubos que hoy se gastan en las piezas a medida (mismo encaje, sin almacén).</summary>
        public static int TubosACorteHoy(List<int> largos)
        {
            return RestosACorteHoy(largos).Count;
        }

        /// <summary>Lo que queda libre en cada tubo al cortar hoy las piezas a medida (uno por tubo).</summary>
        public static List<int> RestosACorteHoy(List<int> largos)
        {
            var libres = new List<int>();
            foreach (int p in largos.OrderByDescending(x => x))
            {
                int need = p + Margen;
                int j = -1;
                for (int i = 0; i < libres.Count; i++)
                    if (libres[i] >= need && (j < 0 || libres[i] < libres[j])) j = i;
                if (j < 0) libres.Add(Barra - need); else libres[j] -= need;
            }
            return libres;
        }

        /// <summary>Metros de tubo que se gastan de verdad: lo cortado menos los restos de 1000 o más que vuelven al almacén.</summary>
        public static double Neto(IEnumerable<Tubo> tubos, bool guardaRestos)
        {
            return tubos.Sum(t => t.Largo - (guardaRestos && t.Libre >= MinEnvio ? t.Libre : 0));
        }
    }
}
