using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using O = arquitectSoft.Engine.Beta.OptimizadorPerfileria;

namespace arquitectSoft.Engine.Beta
{
    /// <summary>
    /// Segundo Excel del ANÁLISIS BETA ("… - OPTIMIZADO (beta).xlsx"). El Excel de siempre no se toca:
    /// este va al lado, con el plan optimizado de la perfilería (ver <see cref="OptimizadorPerfileria"/>).
    /// </summary>
    public class ExcelBetaExporter
    {
        private static readonly CultureInfo Es = CultureInfo.GetCultureInfo("es-ES");
        private static readonly XLColor Cabecera = XLColor.FromArgb(0x1B, 0x4D, 0x6B);
        private static readonly XLColor Verde = XLColor.FromArgb(0xE2, 0xF3, 0xF0);
        private static readonly XLColor Naranja = XLColor.FromArgb(0xF8, 0xEC, 0xDC);

        private class Grupo
        {
            public string Clave, Codigo, Descripcion, Acabado;
            public bool SinCodigo;
            public List<O.Corte> Cortes = new List<O.Corte>();
            public double TubosHoyRec, MetrosHoyRec;
            public List<int> MedidaHoy = new List<int>();
            public List<O.Tubo> Tubos;
        }

        /// <param name="res">Resultado ya calculado (de él sale el acabado real tras "Cambiar Acabado").</param>
        /// <param name="archivos">TXT cargados.</param>
        /// <param name="rutaExcelNormal">Excel de siempre recién exportado: el beta va a su lado.</param>
        /// <returns>Rutas creadas: el optimizado (fábrica) y la guía de montaje (obra).</returns>
        public List<string> Exportar(ResultadoAnalisis res, IEnumerable<string> archivos, int medidaBase, int desperdicioPct,
                                     string rutaExcelNormal, string proyecto, string[] param = null)
        {
            var filas = O.LeerTxt(archivos);
            var cat = O.CargarCatalogo();
            var subs = O.Recopilatorias(cat, filas);
            var a = O.Analizar(subs, medidaBase > 0 ? medidaBase : 2960, Math.Max(0, desperdicioPct));
            // Piezas a medida: las de la tabla de Perfilería ya calculada (las mismas del Excel de
            // siempre, con su acabado final y de TODOS los TXT, también paneles). Sin tabla, de los TXT.
            var medida = MedidaDesdeResultado(res);
            if (medida == null) medida = O.ACorte(cat, filas);
            else UbicacionesDesdeTxt(medida, O.ACorte(cat, filas));
            var acabados = AcabadosFinales(res);
            string ruta;
            var almacen = O.LeerAlmacen(out ruta);
            int frentes = O.ContarFrentes(archivos);
            bool conRepuestos = frentes >= O.MinFrentesRepuesto;

            // ---- grupos por tubo en bruto + acabado real ----
            var grupos = new Dictionary<string, Grupo>(StringComparer.OrdinalIgnoreCase);
            Func<string, string, string, Grupo> grupo = (sub, desc, acab) =>
            {
                string cod, acabReal;
                Final(sub, acab, acabados, out cod, out acabReal);
                string clave = O.TuboBruto(cod);
                Grupo g;
                if (!grupos.TryGetValue(clave, out g))
                {
                    grupos[clave] = g = new Grupo
                    {
                        Clave = clave, Codigo = clave, Acabado = acabReal,
                        Descripcion = clave != cod ? "PERFIL ACERO DE 20 (todas las variantes, mismo tubo en bruto)" : desc,
                        SinCodigo = cod.EndsWith("-XX", StringComparison.OrdinalIgnoreCase)
                    };
                }
                return g;
            };

            var hojas = new List<O.HojaPerfil>();
            foreach (string k in subs.Keys.OrderBy(x => x))
            {
                var h = O.Hoja(a, k, conRepuestos);
                hojas.Add(h);
                var g = grupo(k, h.Descripcion, h.Acabado);
                g.TubosHoyRec += h.PiezasHoy / O.PiezasPorBarra(a.BaseHoy);
                g.MetrosHoyRec += h.PiezasHoy * O.ConsumoPieza(a.BaseHoy);
                string codF, acabF;
                Final(k, h.Acabado, acabados, out codF, out acabF);
                string tc = a.Perfiles[k].TipoCorte;
                // Tramos recopilatorios que salen solo de perfiles verticales.
                bool vert = a.Perfiles[k].Tramos.Count > 0 && a.Perfiles[k].Tramos.All(x => x.Vertical);
                foreach (int p in h.Enviar)
                    g.Cortes.Add(new O.Corte { Largo = p, Tipo = "recopilatoria", Codigo = codF, Descripcion = h.Descripcion, Acabado = acabF, TipoCorte = tc, Vertical = vert });
                foreach (int p in h.Repuesto)
                    g.Cortes.Add(new O.Corte { Largo = p, Tipo = "REPUESTO", Codigo = codF, Descripcion = h.Descripcion, Acabado = acabF, TipoCorte = tc });
            }
            foreach (var p in medida)
            {
                var g = grupo(p.Sub, p.Descripcion, p.Acabado);
                string codM, acabM;
                Final(p.Sub, p.Acabado, acabados, out codM, out acabM);
                g.Cortes.Add(new O.Corte { Largo = p.Largo, Tipo = "a medida", Codigo = codM, Ubicacion = p.Ubicacion,
                                           Descripcion = p.Descripcion, Acabado = acabM, TipoCorte = p.TipoCorte,
                                           Vertical = p.Vertical });
                g.MedidaHoy.Add(p.Largo);
            }
            foreach (var g in grupos.Values)
            {
                // Colores sin código (-XX): no se pueden buscar en el almacén.
                var disponibles = g.SinCodigo ? new List<O.Retal>() : almacen;
                g.Tubos = O.PlanCorte(g.Clave, g.Acabado, g.Cortes, disponibles);
            }

            string carpeta = Path.GetDirectoryName(rutaExcelNormal);
            string nombre = Path.GetFileNameWithoutExtension(rutaExcelNormal);
            string destino = Path.Combine(carpeta, nombre + " - OPTIMIZADO (beta).xlsx");
            string guia = Path.Combine(carpeta, nombre + " - GUIA MONTAJE (beta).xlsx");

            var lista = grupos.Values.OrderBy(x => x.Clave).ToList();
            if (param != null && res != null && res.PerfilMetalico != null && res.PerfilMetalico.Columns.Count > 0)
            {
                // El optimizado ES el Excel de fabricación de siempre (mismo exportador, mismas hojas),
                // con la Perfilería cambiada por el plan de corte; las hojas beta van al final.
                string libro = ExportarEstandar(res, param, lista);
                using (var wb = new XLWorkbook(libro))
                {
                    var perfileria = wb.Worksheet("PERFIL METALICO");
                    OrdenarPorTubo(perfileria, lista);
                    HojaSierra(wb, lista, proyecto, perfileria.Position + 1);
                    HojaResumen(wb, a, lista, proyecto, medidaBase, desperdicioPct, ruta, almacen.Count, frentes);
                    HojaSobrantes(wb, lista, proyecto);
                    if (almacen.Count > 0) HojaDelAlmacen(wb, lista, proyecto);
                    HojaRecopilatorias(wb, a, hojas, acabados);
                    Guardar(wb, destino);
                }
                try { File.Delete(libro); } catch { }
            }
            else
            {
                using (var wb = new XLWorkbook())
                {
                    HojaResumen(wb, a, lista, proyecto, medidaBase, desperdicioPct, ruta, almacen.Count, frentes);
                    HojaFabrica(wb, lista);
                    HojaPlanCorte(wb, lista, proyecto);
                    HojaSierra(wb, lista, proyecto, 0);
                    HojaSobrantes(wb, lista, proyecto);
                    if (almacen.Count > 0) HojaDelAlmacen(wb, lista, proyecto);
                    HojaRecopilatorias(wb, a, hojas, acabados);
                    Guardar(wb, destino);
                }
            }
            var creados = new List<string> { destino };
            if (hojas.Count > 0 || medida.Count > 0)
            {
                // La obra recibe su propio documento: sin plan de corte ni almacén (eso es de fábrica).
                using (var wb = new XLWorkbook())
                {
                    HojaGuia(wb, hojas, acabados, proyecto, frentes);
                    HojaPlanMontaje(wb, hojas, medida, acabados);
                    HojaPiezas(wb, hojas, acabados);
                    Guardar(wb, guia);
                }
                creados.Add(guia);
            }
            return creados;
        }

        // ================= hoja de sierra (para el operario) =================

        /// <summary>
        /// El plan de corte pensado para una sierra MANUAL y para imprimir: perfil a perfil, y dentro
        /// de cada perfil los tubos agrupados por tipo de corte (se mueve poco el cabezal) y de más
        /// largo a más corto (el tope avanza en un solo sentido). Casilla para tachar cada pieza, qué
        /// escribir en ella con rotulador y qué hacer con el sobrante. "CORTE n" es el mismo número
        /// que en la hoja PERFIL METALICO.
        /// </summary>
        private void HojaSierra(XLWorkbook wb, List<Grupo> grupos, string proyecto, int posicion)
        {
            var ws = wb.Worksheets.Add("HOJA DE SIERRA");
            if (posicion > 0) ws.Position = posicion;

            ws.Cell(1, 1).Value = "HOJA DE SIERRA";
            ws.Cell(1, 1).Style.Font.Bold = true;
            ws.Cell(1, 1).Style.Font.FontSize = 16;
            ws.Cell(1, 4).Value = proyecto;
            ws.Cell(1, 4).Style.Font.Bold = true;
            ws.Cell(1, 4).Style.Font.FontSize = 13;
            ws.Cell(1, 8).Value = "Fecha: " + DateTime.Now.ToString("dd-MM-yyyy");
            ws.Cell(2, 1).Value = "Tacha cada pieza al cortarla. Escribe con rotulador lo que dice \"Marcar\" (los tramos sin marca no hace falta marcarlos). "
                                + "Los restos de 1000 o más van al estante indicado; los demás, a la chatarra.";
            ws.Cell(2, 1).Style.Font.Italic = true;

            // ---- qué coger antes de empezar ----
            int f = 4;
            ws.Cell(f, 1).Value = "ANTES DE EMPEZAR, COGER:";
            ws.Cell(f, 1).Style.Font.Bold = true;
            f++;
            foreach (var g in grupos)
            {
                int nuevos = g.Tubos.Count(x => x.Origen == "nuevo");
                var retazos = g.Tubos.Where(x => x.Origen != "nuevo").ToList();
                string txt = (nuevos > 0 ? nuevos + (nuevos == 1 ? " tubo de 6000" : " tubos de 6000") : "")
                           + (retazos.Count > 0 ? (nuevos > 0 ? "  +  " : "") + "del almacén: "
                              + string.Join(", ", retazos.Select(r => r.Largo + " (" + r.Origen.Replace("retazo ", "") + ")")) : "");
                ws.Cell(f, 1).Value = "☐";
                ws.Cell(f, 2).Value = g.Codigo;
                ws.Cell(f, 3).Value = g.Acabado;
                ws.Cell(f, 5).Value = txt;
                ws.Cell(f, 2).Style.Font.Bold = true;
                ws.Cell(f, 5).Style.Font.Bold = true;
                f++;
            }
            f++;

            string[] cols = { "✔", "Tubo", "Medida", "Cant.", "Corte", "Marcar", "Tacha al cortar", "Resto → qué hacer" };
            const int nCol = 8;
            int fCab = f;
            for (int c = 0; c < nCol; c++)
            {
                var cel = ws.Cell(f, c + 1);
                cel.Value = cols[c];
                cel.Style.Font.Bold = true;
                cel.Style.Font.FontColor = XLColor.White;
                cel.Style.Fill.BackgroundColor = Cabecera;
            }
            f++;

            var bloques = Bloques(grupos);
            foreach (var g in grupos)
            {
                var suyos = bloques.Where(b => string.Equals(b.Tubos[0].Grupo, g.Clave, StringComparison.OrdinalIgnoreCase))
                                   .Select(b => new { b, principal = b.Tubos[0].Cortes.OrderByDescending(c => c.Largo).First() })
                                   .OrderBy(x => x.principal.TipoCorte ?? "")
                                   .ThenByDescending(x => x.principal.Largo)
                                   .Select(x => x.b)
                                   .ToList();
                if (suyos.Count == 0) continue;

                // cabecera del perfil
                ws.Cell(f, 1).Value = g.Codigo + "  ·  " + g.Descripcion + "  ·  " + g.Acabado;
                var cab = ws.Range(f, 1, f, nCol);
                cab.Merge();
                cab.Style.Font.Bold = true;
                cab.Style.Font.FontSize = 12;
                cab.Style.Fill.BackgroundColor = XLColor.FromArgb(0xE3, 0xED, 0xF3);
                f++;

                string corteAnterior = null;
                foreach (var b in suyos)
                {
                    var t0 = b.Tubos[0];
                    int n = b.Tubos.Count;
                    string numero = b.Etiqueta.Split('·')[0].Trim();
                    string corteBloque = t0.Cortes.OrderByDescending(c => c.Largo).First().TipoCorte ?? "";
                    if (corteBloque != corteAnterior)
                    {
                        ws.Cell(f, 2).Value = "Corte " + (corteBloque == "" ? "sin indicar" : corteBloque);
                        ws.Range(f, 2, f, nCol).Style.Font.Italic = true;
                        ws.Range(f, 2, f, nCol).Style.Font.FontColor = XLColor.FromArgb(0x46, 0x52, 0x5E);
                        corteAnterior = corteBloque;
                        f++;
                    }

                    string tubo = numero + "\n" + (t0.Origen == "nuevo"
                        ? (n == 1 ? "1 tubo" : n + " tubos") + " de 6000"
                        : "retazo " + t0.Largo + "\n(" + string.Join(", ", b.Tubos.Select(x => x.Origen.Replace("retazo ", ""))) + ")");
                    string resto;
                    bool alAlmacen = t0.Libre >= O.MinEnvio && !g.SinCodigo;
                    if (t0.Libre < 50) resto = "sin resto";
                    else if (alAlmacen) resto = t0.Libre + " → ESTANTE " + g.Codigo + " · " + (t0.Libre / 100 * 100)
                                                + (n > 1 ? "\n(" + n + " restos)" : "");
                    else resto = t0.Libre + " → chatarra";

                    int ini = f;
                    foreach (var fl in b.Filas)
                    {
                        var c0 = fl.First();
                        int cant = fl.Count();
                        // Lo que interesa marcar son los horizontales a medida (su ubicación); los
                        // verticales van como VERTICAL y los tramos recopilatorios no se marcan.
                        int verticales = fl.Count(c => c.Vertical);
                        var ubis = fl.Where(c => !c.Vertical && c.Tipo == "a medida")
                                     .Select(c => c.Ubicacion).Where(u => !string.IsNullOrEmpty(u))
                                     .Distinct().OrderBy(u => u, new OrdenUbicacion()).ToList();
                        string marcar;
                        if (c0.Tipo == "REPUESTO") marcar = "REPUESTO";
                        else if (verticales == cant) marcar = "VERTICAL";
                        else
                        {
                            marcar = string.Join(", ", ubis);
                            if (verticales > 0)
                            {
                                int sinUbi = fl.Count(c => !c.Vertical && c.Tipo == "a medida" && string.IsNullOrEmpty(c.Ubicacion));
                                marcar = verticales + " VERTICAL"
                                       + (marcar != "" ? " · " + marcar : "")
                                       + (sinUbi > 0 ? " · " + sinUbi + " sin ubicación" : "");
                            }
                        }
                        // Variantes que salen del mismo tubo en bruto (IMC0003A, B…): que se sepa cuál es.
                        if (!string.Equals(c0.Codigo, g.Codigo, StringComparison.OrdinalIgnoreCase))
                            marcar = c0.Codigo + (marcar != "" ? " · " + marcar : "");

                        ws.Cell(f, 3).Value = c0.Largo;
                        ws.Cell(f, 4).Value = cant;
                        ws.Cell(f, 5).Value = c0.TipoCorte ?? "";
                        ws.Cell(f, 6).Value = marcar;
                        ws.Cell(f, 7).Value = cant <= 12 ? string.Join(" ", Enumerable.Repeat("☐", cant))
                                                         : string.Join(" ", Enumerable.Repeat("☐", 10)) + " … (" + cant + ")";
                        ws.Cell(f, 3).Style.Font.Bold = true;
                        ws.Cell(f, 3).Style.Font.FontSize = 14;
                        ws.Cell(f, 4).Style.Font.Bold = true;
                        ws.Cell(f, 7).Style.Font.FontSize = 13;
                        if (c0.Tipo == "REPUESTO") ws.Range(f, 3, f, 7).Style.Fill.BackgroundColor = Naranja;
                        f++;
                    }
                    int fin = f - 1;
                    Combinar(ws, ini, fin, 1, "☐");
                    Combinar(ws, ini, fin, 2, tubo);
                    Combinar(ws, ini, fin, 8, resto);
                    ws.Cell(ini, 1).Style.Font.FontSize = 16;
                    ws.Cell(ini, 2).Style.Font.Bold = true;
                    if (t0.Origen != "nuevo") ws.Cell(ini, 2).Style.Fill.BackgroundColor = Naranja;
                    if (alAlmacen)
                    {
                        ws.Cell(ini, 8).Style.Font.Bold = true;
                        ws.Cell(ini, 8).Style.Fill.BackgroundColor = Verde;
                    }
                    var r = ws.Range(ini, 1, fin, nCol);
                    r.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    r.Style.Border.InsideBorder = XLBorderStyleValues.Hair;
                }
                f++;
            }

            ws.Column(1).Width = 5; ws.Column(2).Width = 18; ws.Column(3).Width = 10; ws.Column(4).Width = 7;
            ws.Column(5).Width = 16; ws.Column(6).Width = 22; ws.Column(7).Width = 34; ws.Column(8).Width = 26;
            ws.Range(fCab, 3, f, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range(fCab, 1, f, nCol).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Column(6).Style.Alignment.WrapText = true;
            ws.SheetView.FreezeRows(fCab);
            ws.PageSetup.PageOrientation = XLPageOrientation.Portrait;
            ws.PageSetup.FitToPages(1, 0);
            ws.PageSetup.SetRowsToRepeatAtTop(fCab, fCab);
            ws.PageSetup.Margins.Left = 0.4;
            ws.PageSetup.Margins.Right = 0.4;
            ws.PageSetup.Footer.Center.AddText(proyecto + " · página ");
            ws.PageSetup.Footer.Center.AddText(XLHFPredefinedText.PageNumber);
        }

        // ================= Excel de fabricación estándar =================

        private class Bloque
        {
            public string Etiqueta;
            public List<O.Tubo> Tubos;
            public List<IGrouping<string, O.Corte>> Filas;
        }

        /// <summary>Tubos con el mismo patrón de corte, en el orden en que se listan.</summary>
        private static List<Bloque> Bloques(List<Grupo> grupos)
        {
            var l = new List<Bloque>();
            int n = 0;
            foreach (var g in grupos)
                foreach (var pat in g.Tubos
                    .GroupBy(t => (t.Origen == "nuevo" ? "N" : "R" + t.Largo) + "|" + t.Libre + "|" +
                                  string.Join(";", t.Cortes.Select(c => c.Codigo + "/" + c.Largo + "/" + c.Tipo).OrderBy(x => x)))
                    .OrderByDescending(p => p.Count()).ThenByDescending(p => p.First().Cortes.Max(c => c.Largo)))
                {
                    n++;
                    var t0 = pat.First();
                    int cuantos = pat.Count();
                    string destino = t0.Libre >= O.MinEnvio ? (g.SinCodigo ? "merma" : "ALMACÉN") : (t0.Libre < 50 ? "sin resto" : "merma");
                    string origen = t0.Origen == "nuevo" ? (cuantos == 1 ? "1 tubo" : cuantos + " tubos")
                        : "retazo " + t0.Largo + " (" + string.Join(", ", pat.Select(t => t.Origen.Replace("retazo ", ""))) + ")";
                    l.Add(new Bloque
                    {
                        Etiqueta = "CORTE " + n + " · " + origen + " · resto " + (t0.Libre < 50 ? 0 : t0.Libre) + " → " + destino,
                        Tubos = pat.ToList(),
                        Filas = pat.SelectMany(t => t.Cortes)
                                   .GroupBy(c => c.Codigo + "|" + c.Largo + "|" + c.Tipo)
                                   .OrderByDescending(x => x.First().Largo).ThenBy(x => x.First().Codigo).ToList()
                    });
                }
            return l;
        }

        /// <summary>Exporta con el exportador de siempre, cambiando solo la Perfilería por el plan de corte.</summary>
        private static string ExportarEstandar(ResultadoAnalisis res, string[] param, List<Grupo> grupos)
        {
            var orig = res.PerfilMetalico;
            var opt = orig.Clone();
            Func<string, int> col = nombre =>
            {
                foreach (DataColumn c in orig.Columns)
                    if (string.Equals(c.ColumnName, nombre, StringComparison.OrdinalIgnoreCase)) return c.Ordinal;
                return -1;
            };
            int cId = col("id_subcomponente"), cCod = col("codigo"), cDesc = col("descripcion"), cAcab = col("acabado"),
                cCant = col("cantidad"), cMed = col("medida"), cCal = col("Medidida Calculada"), cPor = col("Se_Calcula_Por"),
                cCorte = col("Corte"), cUbi = col("Ubicación"), cMec = col("Mecanizado");

            // datos de cada código (id y mecanizado) y filas que no son de perfilería lineal: se dejan tal cual
            var info = new Dictionary<string, DataRow>(StringComparer.OrdinalIgnoreCase);
            foreach (DataRow r in orig.Rows)
            {
                string cod = Convert.ToString(r[cCod]).Trim();
                if (!info.ContainsKey(cod)) info[cod] = r;
                string por = cPor >= 0 ? Convert.ToString(r[cPor]) : "";
                bool lineal = por.IndexOf("Recop", StringComparison.OrdinalIgnoreCase) >= 0
                           || por.IndexOf("Recomp", StringComparison.OrdinalIgnoreCase) >= 0;
                if (!lineal) opt.ImportRow(r);
            }

            foreach (var b in Bloques(grupos))
                foreach (var fl in b.Filas)
                {
                    var c0 = fl.First();
                    var nr = opt.NewRow();
                    DataRow ref0;
                    info.TryGetValue(c0.Codigo, out ref0);
                    if (cId >= 0) nr[cId] = ref0 != null ? ref0[cId] : "";
                    nr[cCod] = c0.Codigo;
                    if (cDesc >= 0) nr[cDesc] = c0.Descripcion;
                    if (cAcab >= 0) nr[cAcab] = c0.Acabado;
                    nr[cCant] = (float)fl.Count();
                    nr[cMed] = c0.Largo.ToString(CultureInfo.InvariantCulture);
                    if (cCal >= 0) nr[cCal] = "0";
                    if (cPor >= 0) nr[cPor] = b.Etiqueta + (c0.Tipo == "REPUESTO" ? " · REPUESTO" : "");
                    if (cCorte >= 0) nr[cCorte] = string.IsNullOrEmpty(c0.TipoCorte) && ref0 != null ? ref0[cCorte] : (object)c0.TipoCorte;
                    if (cUbi >= 0)
                        nr[cUbi] = string.Join(", ", fl.Select(c => c.Ubicacion).Where(u => !string.IsNullOrEmpty(u))
                                                       .Distinct().OrderBy(u => u, new OrdenUbicacion()));
                    if (cMec >= 0) nr[cMec] = ref0 != null ? ref0[cMec] : "";
                    opt.Rows.Add(nr);
                }

            var copia = new ResultadoAnalisis
            {
                PerfilMetalico = opt,
                PerfilMetalicoHerraje = res.PerfilMetalicoHerraje,
                VidrioPaneles = res.VidrioPaneles,
                Puertas = res.Puertas,
                PuertasHerraje = res.PuertasHerraje,
                PuertasCantidad = res.PuertasCantidad,
                Tubos = res.Tubos,
                Mamparas = res.Mamparas,
                PMVHerraje = res.PMVHerraje,
                SwSegmentadoUbiFinal = res.SwSegmentadoUbiFinal
            };
            string tmp = Path.Combine(Path.GetTempPath(), "arqsoft_beta_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tmp);
            return new ExcelExporter().Exportar(copia, param, tmp, res.SwSegmentadoUbiFinal);
        }

        /// <summary>
        /// El exportador ordena la Perfilería por descripción; aquí se vuelve a poner por tubo (CORTE 1, 2…)
        /// y cada tubo se enmarca, sin tocar el resto del formato.
        /// </summary>
        private static void OrdenarPorTubo(IXLWorksheet ws, List<Grupo> grupos)
        {
            int fCab = -1, cPor = -1, ultCol = 0;
            foreach (var row in ws.RowsUsed())
            {
                if (!string.Equals(row.Cell(1).GetString().Trim(), "id_subcomponente", StringComparison.OrdinalIgnoreCase)) continue;
                fCab = row.RowNumber();
                foreach (var c in row.CellsUsed())
                {
                    ultCol = Math.Max(ultCol, c.Address.ColumnNumber);
                    if (c.GetString().Trim() == "Se_Calcula_Por") cPor = c.Address.ColumnNumber;
                }
                break;
            }
            if (fCab < 0 || cPor < 0) return;
            ws.Cell(fCab, cPor).Value = "Plan de corte";

            // filas de datos de la perfilería: hasta la primera vacía
            int fin = fCab;
            while (!ws.Cell(fin + 1, 1).IsEmpty() || !ws.Cell(fin + 1, 2).IsEmpty()) fin++;
            if (fin <= fCab) return;

            var orden = Bloques(grupos).Select(b => b.Etiqueta).ToList();
            Func<string, int> pos = et =>
            {
                for (int i = 0; i < orden.Count; i++)
                    if (et.StartsWith(orden[i] + " ·", StringComparison.Ordinal) || et == orden[i]) return i;
                return int.MaxValue;
            };
            var filas = new List<object[]>();
            for (int f = fCab + 1; f <= fin; f++)
            {
                var v = new object[ultCol];
                for (int c = 1; c <= ultCol; c++) v[c - 1] = ws.Cell(f, c).Value;
                filas.Add(v);
            }
            var ordenadas = filas.Select((v, i) => new { v, i, p = pos(Convert.ToString(v[cPor - 1])) })
                                 .OrderBy(x => x.p).ThenBy(x => x.i).Select(x => x.v).ToList();
            for (int k = 0; k < ordenadas.Count; k++)
                for (int c = 1; c <= ultCol; c++)
                    ws.Cell(fCab + 1 + k, c).Value = ordenadas[k][c - 1];

            // marco por tubo
            int ini = fCab + 1;
            for (int f = fCab + 1; f <= fin + 1; f++)
            {
                int pa = f <= fin ? pos(ws.Cell(f, cPor).GetString()) : -1;
                int pp = pos(ws.Cell(ini, cPor).GetString());
                if (f <= fin && pa == pp) continue;
                if (pp != int.MaxValue)
                {
                    var r = ws.Range(ini, 1, f - 1, ultCol);
                    r.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    ws.Cell(ini, cPor).Style.Font.Bold = true;
                    if (ws.Cell(ini, cPor).GetString().Contains("ALMACÉN"))
                        ws.Range(ini, cPor, f - 1, cPor).Style.Fill.BackgroundColor = Verde;
                }
                for (int k = ini; k < f; k++)
                    if (ws.Cell(k, cPor).GetString().Contains("REPUESTO"))
                        ws.Range(k, 1, k, ultCol).Style.Fill.BackgroundColor = Naranja;
                ini = f;
            }
            ws.Column(cPor).Width = 46;
            ws.Column(cPor).Style.Alignment.WrapText = true;
        }

        /// <summary>Guarda en un temporal local y copia (Google Drive bloquea la escritura directa).</summary>
        private static void Guardar(XLWorkbook wb, string destino)
        {
            string tmp = Path.Combine(Path.GetTempPath(), "arqsoft_beta_" + Guid.NewGuid().ToString("N") + ".xlsx");
            wb.SaveAs(tmp);
            for (int i = 0; ; i++)
            {
                try { File.Copy(tmp, destino, true); break; }
                catch (IOException) { if (i >= 10) throw; System.Threading.Thread.Sleep(300); }
            }
            try { File.Delete(tmp); } catch { }
        }

        /// <summary>Filas "Longitud sin Recopilar" de la Perfilería calculada, una pieza por unidad.</summary>
        private static List<O.PiezaMedida> MedidaDesdeResultado(ResultadoAnalisis res)
        {
            var t = res == null ? null : res.PerfilMetalico;
            if (t == null || t.Rows.Count == 0) return null;
            Func<string, int> col = nombre =>
            {
                foreach (DataColumn c in t.Columns)
                    if (c.ColumnName.IndexOf(nombre, StringComparison.OrdinalIgnoreCase) >= 0) return c.Ordinal;
                return -1;
            };
            int cCod = col("codigo"), cDesc = col("descripcion"), cAcab = col("acabado"), cCant = col("cantidad"),
                cMed = col("medida"), cPor = col("Calcula_Por"), cUbi = col("Ubica"), cCorte = -1;
            foreach (DataColumn c in t.Columns)
                if (string.Equals(c.ColumnName, "Corte", StringComparison.OrdinalIgnoreCase)) cCorte = c.Ordinal;
            if (cCod < 0 || cCant < 0 || cMed < 0 || cPor < 0) return null;
            var l = new List<O.PiezaMedida>();
            foreach (DataRow r in t.Rows)
            {
                string por = Convert.ToString(r[cPor]);
                if (por.IndexOf("sin", StringComparison.OrdinalIgnoreCase) < 0) continue;   // solo "Longitud sin Recopilar"
                double cant, med;
                if (!double.TryParse(Convert.ToString(r[cCant]), NumberStyles.Any, CultureInfo.CurrentCulture, out cant)
                    && !double.TryParse(Convert.ToString(r[cCant]), NumberStyles.Any, CultureInfo.InvariantCulture, out cant)) continue;
                if (!double.TryParse(Convert.ToString(r[cMed]), NumberStyles.Any, CultureInfo.CurrentCulture, out med)
                    && !double.TryParse(Convert.ToString(r[cMed]), NumberStyles.Any, CultureInfo.InvariantCulture, out med)) continue;
                if (med <= 0) continue;
                for (int i = 0; i < (int)Math.Round(cant); i++)
                    l.Add(new O.PiezaMedida
                    {
                        Sub = Convert.ToString(r[cCod]).Trim(),
                        Descripcion = cDesc >= 0 ? Convert.ToString(r[cDesc]).Trim() : "",
                        Acabado = cAcab >= 0 ? Convert.ToString(r[cAcab]).Trim() : "",
                        Ubicacion = cUbi >= 0 ? Convert.ToString(r[cUbi]).Trim() : "",
                        TipoCorte = cCorte >= 0 ? Convert.ToString(r[cCorte]).Trim() : "",
                        Largo = (int)Math.Ceiling(med)
                    });
            }
            return l;
        }

        /// <summary>
        /// La tabla calculada junta en una fila las piezas iguales sin ubicación cuando no se segmenta.
        /// Se la devuelve de los TXT emparejando por código base y largo (una a una).
        /// </summary>
        private static void UbicacionesDesdeTxt(List<O.PiezaMedida> medida, List<O.PiezaMedida> txt)
        {
            Func<string, string> bas = c => { int i = (c ?? "").IndexOf('-'); return i > 0 ? c.Substring(0, i) : (c ?? ""); };
            // Por código base y largo, las piezas de los TXT (con su ubicación y si son verticales);
            // primero las que traen ubicación, que son las que sirve devolver.
            var cola = txt.GroupBy(p => bas(p.Sub) + "|" + p.Largo)
                          .ToDictionary(g => g.Key,
                                        g => g.OrderBy(p => string.IsNullOrEmpty(p.Ubicacion) ? 1 : 0).ToList(),
                                        StringComparer.OrdinalIgnoreCase);
            // primero se descuentan las que ya traen ubicación
            foreach (var p in medida.Where(x => !string.IsNullOrEmpty(x.Ubicacion)))
            {
                List<O.PiezaMedida> l;
                if (!cola.TryGetValue(bas(p.Sub) + "|" + p.Largo, out l)) continue;
                var par = l.FirstOrDefault(x => string.Equals(x.Ubicacion, p.Ubicacion, StringComparison.OrdinalIgnoreCase));
                if (par == null) continue;
                p.Vertical = par.Vertical;
                l.Remove(par);
            }
            foreach (var p in medida.Where(x => string.IsNullOrEmpty(x.Ubicacion)))
            {
                List<O.PiezaMedida> l;
                if (!cola.TryGetValue(bas(p.Sub) + "|" + p.Largo, out l) || l.Count == 0) continue;
                var par = l[0];
                l.RemoveAt(0);
                p.Ubicacion = par.Ubicacion;
                p.Vertical = par.Vertical;
            }
        }

        // ---------- acabado real ----------

        /// <summary>Código base → (código final, acabado) según las tablas ya calculadas (tras "Cambiar Acabado").</summary>
        private static Dictionary<string, KeyValuePair<string, string>> AcabadosFinales(ResultadoAnalisis res)
        {
            var d = new Dictionary<string, KeyValuePair<string, string>>(StringComparer.OrdinalIgnoreCase);
            var dudosos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var t in new[] { res.PerfilMetalico, res.PerfilMetalicoHerraje })
            {
                if (t == null) continue;
                foreach (DataRow r in t.Rows)
                {
                    if (t.Columns.Count < 4) break;
                    string cod = Convert.ToString(r[1]).Trim();
                    int i = cod.IndexOf('-');
                    if (i <= 0) continue;
                    string bas = cod.Substring(0, i);
                    var par = new KeyValuePair<string, string>(cod, Convert.ToString(r[3]).Trim());
                    KeyValuePair<string, string> ya;
                    if (d.TryGetValue(bas, out ya) && !string.Equals(ya.Key, cod, StringComparison.OrdinalIgnoreCase)) dudosos.Add(bas);
                    d[bas] = par;
                }
            }
            foreach (string b in dudosos) d.Remove(b);   // cambio "temporal" de una sola fila: se deja el del catálogo
            return d;
        }

        private static void Final(string sub, string acab, Dictionary<string, KeyValuePair<string, string>> map, out string cod, out string acabado)
        {
            cod = sub; acabado = acab;
            int i = sub.IndexOf('-');
            if (i <= 0) return;
            KeyValuePair<string, string> f;
            if (map.TryGetValue(sub.Substring(0, i), out f)) { cod = f.Key; acabado = f.Value; }
        }

        // ---------- hojas ----------

        private static IXLWorksheet Nueva(XLWorkbook wb, string nombre, string titulo, string nota)
        {
            var ws = wb.Worksheets.Add(nombre);
            ws.Cell(1, 1).Value = titulo;
            ws.Cell(1, 1).Style.Font.Bold = true;
            ws.Cell(1, 1).Style.Font.FontSize = 14;
            ws.Cell(2, 1).Value = nota;
            ws.Cell(2, 1).Style.Font.Italic = true;
            ws.Cell(2, 1).Style.Font.FontColor = XLColor.FromArgb(0x46, 0x52, 0x5E);
            return ws;
        }

        private static void Encabezado(IXLWorksheet ws, int fila, params string[] cols)
        {
            for (int i = 0; i < cols.Length; i++)
            {
                var c = ws.Cell(fila, i + 1);
                c.Value = cols[i];
                c.Style.Font.Bold = true;
                c.Style.Font.FontColor = XLColor.White;
                c.Style.Fill.BackgroundColor = Cabecera;
            }
            ws.SheetView.FreezeRows(fila);
        }

        private static void Cerrar(IXLWorksheet ws, int ancho = 60)
        {
            ws.Columns().AdjustToContents(3, 400);
            foreach (var col in ws.ColumnsUsed())
                if (col.Width > ancho) { col.Width = ancho; col.Style.Alignment.WrapText = true; }
        }

        private static string Lista(IEnumerable<int> v)
        {
            var g = v.GroupBy(x => x).OrderByDescending(x => x.Key).Select(x => x.Count() + " × " + x.Key).ToList();
            return g.Count == 0 ? "—" : string.Join(" + ", g);
        }

        private static string Pct(double nuevo, double hoy)
        {
            if (hoy <= 0) return "";
            return (100 * (nuevo - hoy) / hoy).ToString("+0.0;−0.0;0.0", Es) + " %";
        }

        private void HojaResumen(XLWorkbook wb, O.Analisis a, List<Grupo> grupos, string proyecto, int medidaBase,
                                 int pct, string rutaAlmacen, int nRetazos, int frentes)
        {
            var ws = Nueva(wb, "Resumen", "ANÁLISIS BETA · Perfilería optimizada",
                "Excel de prueba generado en Olimpo junto al de siempre. No sustituye al pedido normal.");
            int f = 4;
            Action<string, string> dato = (k, v) => { ws.Cell(f, 1).Value = k; ws.Cell(f, 1).Style.Font.Bold = true; ws.Cell(f, 2).Value = v; f++; };
            dato("Proyecto", proyecto);
            dato("Fecha", DateTime.Now.ToString("dd-MM-yyyy HH:mm"));
            dato("Cálculo de hoy", string.Format("Medida Base {0} · {1} % de desperdicio", medidaBase, pct));
            dato("Frentes de la obra", frentes + (frentes < O.MinFrentesRepuesto
                ? " → sin repuestos (menos de " + O.MinFrentesRepuesto + " frentes)"
                : " → repuestos solo donde la holgura no llega al 2 %"));
            dato("Almacén de retazos", rutaAlmacen == null ? "sin archivo (solo se aprovecha dentro de la obra)" : nRetazos + " retazos de " + rutaAlmacen);
            f++;
            if (a.Propuesta != null)
            {
                ws.Cell(f, 1).Value = "Medida base de las recopilatorias";
                ws.Cell(f, 1).Style.Font.Bold = true;
                f++;
                foreach (string j in a.Justificacion)
                {
                    ws.Cell(f, 1).Value = "• " + j;
                    ws.Range(f, 1, f, 9).Merge().Style.Alignment.WrapText = true;
                    ws.Row(f).Height = 30;
                    f++;
                }
                f++;
            }

            Encabezado(ws, f, "Tubo en bruto", "Descripción", "Acabado", "Tubos hoy", "Tubos propuesta", "Diferencia",
                       "Retazos del almacén usados", "Restos al almacén hoy", "Restos al almacén propuesta");
            f++;
            int tHoy = 0, tTub = 0, tRet = 0;
            foreach (var g in grupos.OrderBy(x => x.Clave))
            {
                // Hoy: recopilatorias en piezas de la Medida Base (N por tubo) y piezas a medida aparte.
                var restosHoy = O.RestosACorteHoy(g.MedidaHoy);
                int tubRec = (int)Math.Ceiling(g.TubosHoyRec - 0.001);
                int tubHoy = tubRec + restosHoy.Count;
                var guardaHoy = restosHoy.Where(r => r >= O.MinEnvio).ToList();
                if (O.Retazo(a.BaseHoy) >= O.MinEnvio)
                    for (int i = 0; i < tubRec; i++) guardaHoy.Add(O.Retazo(a.BaseHoy));
                int tub = g.Tubos.Count(t => t.Origen == "nuevo");
                int ret = g.Tubos.Count - tub;
                ws.Cell(f, 1).Value = g.Codigo;
                ws.Cell(f, 2).Value = g.Descripcion;
                ws.Cell(f, 3).Value = g.Acabado + (g.SinCodigo ? " (sin código: sin almacén)" : "");
                ws.Cell(f, 4).Value = tubHoy;
                ws.Cell(f, 5).Value = tub;
                ws.Cell(f, 6).Value = Pct(tub, tubHoy);
                ws.Cell(f, 7).Value = ret;
                ws.Cell(f, 8).Value = Lista(guardaHoy);
                ws.Cell(f, 9).Value = Lista(g.Tubos.Where(t => t.Libre >= O.MinEnvio).Select(t => t.Libre));
                if (tub < tubHoy) ws.Cell(f, 6).Style.Fill.BackgroundColor = Verde;
                else if (tub > tubHoy) ws.Cell(f, 6).Style.Fill.BackgroundColor = Naranja;
                tHoy += tubHoy; tTub += tub; tRet += ret;
                f++;
            }
            ws.Cell(f, 1).Value = "TOTAL";
            ws.Cell(f, 4).Value = tHoy;
            ws.Cell(f, 5).Value = tTub;
            ws.Cell(f, 6).Value = Pct(tTub, tHoy);
            ws.Cell(f, 7).Value = tRet;
            ws.Range(f, 1, f, 9).Style.Font.Bold = true;
            ws.Range(f, 1, f, 9).Style.Fill.BackgroundColor = Verde;
            f += 2;
            string[] notas =
            {
                "Tubos = tubos de 6000 nuevos que hay que cortar. Los restos de 1000 mm o más van al almacén; se listan aparte porque solo ahorran si se usan en otra obra.",
                "Hoy: recopilatorias como las calcula arquitectSoft (suma ÷ Medida Base + %, las piezas que quepan por tubo) y piezas a medida encajadas en tubos de 6000 aparte.",
                "Propuesta: recopilatorias repartidas tramo a tramo (media pieza, empalmes solo si ahorran 1 m, trozos ≥ 300; repuestos solo en obras de 5 frentes o más y donde la holgura no llega al 2 %) cortadas en los mismos tubos que las piezas a medida.",
                "El acabado es el real (después de \"Cambiar Acabado\"). Los aceros de 20 (IMC0003 y variantes) comparten tubo en bruto. Los colores sin código no tienen almacén.",
                "Con archivo de almacén (almacen_retazos.csv junto al programa: id;codigo;largo_mm) se usan primero sus retazos y la hoja \"Del almacén\" dice cuáles dar de baja."
            };
            foreach (string n in notas)
            {
                ws.Cell(f, 1).Value = n;
                ws.Range(f, 1, f, 9).Merge().Style.Alignment.WrapText = true;
                ws.Row(f).Height = 30;
                f++;
            }
            ws.Column(1).Width = 18; ws.Column(2).Width = 42; ws.Column(3).Width = 28;
            for (int c = 4; c <= 8; c++) ws.Column(c).Width = 16;
            ws.Column(9).Width = 32;
        }

        private void HojaFabrica(XLWorkbook wb, List<Grupo> grupos)
        {
            var ws = Nueva(wb, "Hoja de fábrica", "Hoja de fábrica · por tubo en bruto y acabado",
                "Lo que se corta, lo que va a obra y lo que se queda en fábrica. Los repuestos van marcados como REPUESTO.");
            Encabezado(ws, 4, "Tubo en bruto", "Descripción", "Acabado", "Tubos nuevos", "Retazos del almacén",
                       "Recopilatorias a obra", "Piezas a medida", "Repuesto (a obra, marcado)", "Se queda en fábrica");
            int f = 5;
            foreach (var g in grupos.OrderBy(x => x.Clave))
            {
                var cortes = g.Tubos.SelectMany(t => t.Cortes).ToList();
                ws.Cell(f, 1).Value = g.Codigo;
                ws.Cell(f, 2).Value = g.Descripcion;
                ws.Cell(f, 3).Value = g.Acabado;
                ws.Cell(f, 4).Value = g.Tubos.Count(t => t.Origen == "nuevo");
                ws.Cell(f, 5).Value = g.Tubos.Count(t => t.Origen != "nuevo");
                ws.Cell(f, 6).Value = Lista(cortes.Where(c => c.Tipo == "recopilatoria").Select(c => c.Largo));
                ws.Cell(f, 7).Value = cortes.Count(c => c.Tipo == "a medida") + " piezas (ver plan de corte)";
                ws.Cell(f, 8).Value = Lista(cortes.Where(c => c.Tipo == "REPUESTO").Select(c => c.Largo));
                var queda = g.Tubos.Where(t => t.Libre >= O.MinEnvio).Select(t => t.Libre).ToList();
                ws.Cell(f, 9).Value = Lista(queda);
                if (queda.Count > 0) ws.Cell(f, 9).Style.Font.Bold = true;
                f++;
            }
            Cerrar(ws);
        }

        /// <summary>
        /// Listado de cortes con el formato de la hoja de Perfilería. Cada bloque con borde = lo que sale
        /// de UN tubo; la columna "Tubos" dice cuántos se cortan igual y "Cantidad" es el total de piezas
        /// de esa medida en el bloque.
        /// </summary>
        private void HojaPlanCorte(XLWorkbook wb, List<Grupo> grupos, string proyecto)
        {
            var ws = wb.Worksheets.Add("Plan de corte");
            ws.Cell(1, 1).Value = "PLAN DE CORTE (beta)";
            ws.Cell(1, 1).Style.Font.Bold = true;
            ws.Cell(1, 1).Style.Font.FontSize = 14;
            ws.Cell(2, 1).Value = "Proyecto:"; ws.Cell(2, 2).Value = proyecto;
            ws.Cell(3, 1).Value = "Fecha:"; ws.Cell(3, 2).Value = DateTime.Now.ToString("dd-MM-yyyy");
            ws.Range(2, 1, 3, 1).Style.Font.Bold = true;
            ws.Cell(4, 1).Value = "Cada bloque es lo que sale de un tubo. \"Tubos\" = cuántos tubos se cortan igual. \"Cantidad\" = total de piezas de esa medida en el bloque.";
            ws.Cell(4, 1).Style.Font.Italic = true;

            string[] cols = { "Código", "Descripción", "Acabado", "Medida", "Cantidad", "Corte", "Tipo", "Ubicación",
                              "Tubos", "Sale de", "Resto por tubo", "Destino del resto" };
            const int nCol = 12;
            int f = 6;
            for (int c = 0; c < nCol; c++)
            {
                var cel = ws.Cell(f, c + 1);
                cel.Value = cols[c];
                cel.Style.Font.Bold = true;
                cel.Style.Font.FontColor = XLColor.White;
                cel.Style.Fill.BackgroundColor = Cabecera;
            }
            ws.SheetView.FreezeRows(f);
            f++;

            int totalTubos = 0;
            foreach (var g in grupos.OrderBy(x => x.Clave))
            {
                int nuevos = g.Tubos.Count(t => t.Origen == "nuevo");
                totalTubos += nuevos;
                // cabecera del tubo en bruto
                ws.Cell(f, 1).Value = g.Codigo + "  ·  " + g.Descripcion + "  ·  " + g.Acabado;
                ws.Cell(f, 9).Value = nuevos;
                ws.Cell(f, 10).Value = "tubos de 6000" + (g.Tubos.Count > nuevos ? " + " + (g.Tubos.Count - nuevos) + " retazos" : "");
                var cab = ws.Range(f, 1, f, nCol);
                cab.Style.Font.Bold = true;
                cab.Style.Fill.BackgroundColor = XLColor.FromArgb(0xE3, 0xED, 0xF3);
                f++;

                // tubos con el mismo patrón de corte
                var patrones = g.Tubos
                    .GroupBy(t => (t.Origen == "nuevo" ? "N" : "R" + t.Largo) + "|" + t.Libre + "|" +
                                  string.Join(";", t.Cortes.Select(c => c.Codigo + "/" + c.Largo + "/" + c.Tipo).OrderBy(x => x)))
                    .OrderByDescending(p => p.Count()).ThenByDescending(p => p.First().Cortes.Max(c => c.Largo))
                    .ToList();
                foreach (var pat in patrones)
                {
                    var tubo = pat.First();
                    int n = pat.Count();
                    var filas = pat.SelectMany(t => t.Cortes)
                        .GroupBy(c => new { c.Codigo, c.Descripcion, c.Acabado, c.Largo, c.TipoCorte, c.Tipo })
                        .OrderByDescending(x => x.Key.Largo).ThenBy(x => x.Key.Codigo)
                        .ToList();
                    int ini = f;
                    foreach (var fl in filas)
                    {
                        ws.Cell(f, 1).Value = fl.Key.Codigo;
                        ws.Cell(f, 2).Value = fl.Key.Descripcion;
                        ws.Cell(f, 3).Value = fl.Key.Acabado;
                        ws.Cell(f, 4).Value = fl.Key.Largo;
                        ws.Cell(f, 5).Value = fl.Count();
                        ws.Cell(f, 6).Value = fl.Key.TipoCorte;
                        ws.Cell(f, 7).Value = fl.Key.Tipo;
                        var ubis = fl.Select(c => c.Ubicacion).Where(u => !string.IsNullOrEmpty(u)).Distinct().ToList();
                        ws.Cell(f, 8).Value = string.Join(", ", ubis.OrderBy(u => u, new OrdenUbicacion()));
                        ws.Cell(f, 4).Style.Font.Bold = true;
                        ws.Cell(f, 5).Style.Font.Bold = true;
                        if (fl.Key.Tipo == "REPUESTO") ws.Range(f, 1, f, 8).Style.Fill.BackgroundColor = Naranja;
                        f++;
                    }
                    int fin = f - 1;
                    string saleDe = tubo.Origen == "nuevo" ? "tubo de 6000"
                        : "retazo " + tubo.Largo + " (" + string.Join(", ", pat.Select(t => t.Origen.Replace("retazo ", ""))) + ")";
                    string destino = tubo.Libre >= O.MinEnvio ? (g.SinCodigo ? "merma (color sin código)" : "ALMACÉN")
                                   : (tubo.Libre < 50 ? "—" : "merma");
                    Combinar(ws, ini, fin, 9, n);
                    Combinar(ws, ini, fin, 10, saleDe);
                    Combinar(ws, ini, fin, 11, tubo.Libre < 50 ? (object)"—" : tubo.Libre);
                    Combinar(ws, ini, fin, 12, destino);
                    ws.Cell(ini, 9).Style.Font.Bold = true;
                    ws.Cell(ini, 9).Style.Font.FontSize = 13;
                    if (destino == "ALMACÉN")
                    {
                        ws.Cell(ini, 12).Style.Font.Bold = true;
                        ws.Cell(ini, 12).Style.Fill.BackgroundColor = Verde;
                    }
                    if (tubo.Origen != "nuevo") ws.Cell(ini, 10).Style.Fill.BackgroundColor = Naranja;
                    var bloque = ws.Range(ini, 1, fin, nCol);
                    bloque.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    bloque.Style.Border.InsideBorder = XLBorderStyleValues.Hair;
                }
                f++;
            }
            ws.Cell(f, 1).Value = "TOTAL TUBOS DE 6000";
            ws.Cell(f, 9).Value = totalTubos;
            ws.Range(f, 1, f, nCol).Style.Font.Bold = true;
            ws.Range(f, 1, f, nCol).Style.Fill.BackgroundColor = Verde;

            ws.Column(1).Width = 14; ws.Column(2).Width = 44; ws.Column(3).Width = 24;
            ws.Column(4).Width = 9; ws.Column(5).Width = 9; ws.Column(6).Width = 13; ws.Column(7).Width = 14;
            ws.Column(8).Width = 14; ws.Column(9).Width = 7; ws.Column(10).Width = 16; ws.Column(11).Width = 9; ws.Column(12).Width = 14;
            ws.Column(2).Style.Alignment.WrapText = true;
            ws.Range(7, 4, f, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.PageSetup.PageOrientation = XLPageOrientation.Landscape;
            ws.PageSetup.FitToPages(1, 0);
        }

        private static void Combinar(IXLWorksheet ws, int ini, int fin, int col, object valor)
        {
            ws.Cell(ini, col).Value = valor;
            var r = ws.Range(ini, col, fin, col);
            if (fin > ini) r.Merge();
            r.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            r.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            r.Style.Alignment.WrapText = true;
        }

        private void HojaSobrantes(XLWorkbook wb, List<Grupo> grupos, string proyecto)
        {
            var ws = Nueva(wb, "Sobrantes previstos", "Sobrantes previstos · alta en el inventario",
                "Restos de 1000 mm o más que la fábrica guarda y etiqueta. Es la tabla que lee el inventario.");
            Encabezado(ws, 4, "obra", "codigo", "acabado", "largo_mm", "cantidad", "origen", "etiqueta");
            int f = 5;
            foreach (var g in grupos.Where(x => !x.SinCodigo).OrderBy(x => x.Clave))
                foreach (var grp in g.Tubos.Where(t => t.Libre >= O.MinEnvio)
                                           .GroupBy(t => new { t.Libre, Ret = t.Origen != "nuevo" }))
                {
                    ws.Cell(f, 1).Value = proyecto;
                    ws.Cell(f, 2).Value = g.Codigo;
                    ws.Cell(f, 3).Value = g.Acabado;
                    ws.Cell(f, 4).Value = grp.Key.Libre;
                    ws.Cell(f, 5).Value = grp.Count();
                    ws.Cell(f, 6).Value = grp.Key.Ret ? "resto de retazo" : "resto de tubo";
                    ws.Cell(f, 7).Value = g.Codigo + " · " + grp.Key.Libre + " · " + proyecto;
                    f++;
                }
            Cerrar(ws);
        }

        private void HojaDelAlmacen(XLWorkbook wb, List<Grupo> grupos, string proyecto)
        {
            var ws = Nueva(wb, "Del almacén", "Retazos del almacén usados en esta obra · baja en el inventario", "");
            Encabezado(ws, 4, "obra", "id_retazo", "codigo", "acabado", "largo_mm", "cortes");
            int f = 5;
            foreach (var g in grupos)
                foreach (var t in g.Tubos.Where(x => x.Origen != "nuevo"))
                {
                    ws.Cell(f, 1).Value = proyecto;
                    ws.Cell(f, 2).Value = t.Origen.Replace("retazo ", "");
                    ws.Cell(f, 3).Value = g.Codigo;
                    ws.Cell(f, 4).Value = g.Acabado;
                    ws.Cell(f, 5).Value = t.Largo;
                    ws.Cell(f, 6).Value = string.Join(" · ", t.Cortes.Select(c => c.Largo + " (" + c.Tipo + ")"));
                    f++;
                }
            Cerrar(ws);
        }

        private void HojaRecopilatorias(XLWorkbook wb, O.Analisis a, List<O.HojaPerfil> hojas,
                                        Dictionary<string, KeyValuePair<string, string>> acabados)
        {
            var ws = Nueva(wb, "Recopilatorias", "Recopilatorias · hoy frente a la propuesta",
                a.Propuesta == null ? "Este proyecto no tiene perfiles con medida recopilatoria."
                    : string.Format("Hoy: Medida Base {0}. Propuesta: base {1}, media pieza {2}.", a.BaseHoy, a.Propuesta.B, a.Propuesta.B / 2));
            Encabezado(ws, 4, "Código", "Descripción", "Acabado", "Tramos (m)", "Hoy", "Propuesta a obra", "Repuesto", "Empalmes");
            int f = 5;
            foreach (var h in hojas)
            {
                string cod, acab;
                Final(h.Codigo, h.Acabado, acabados, out cod, out acab);
                ws.Cell(f, 1).Value = cod;
                ws.Cell(f, 2).Value = h.Descripcion;
                ws.Cell(f, 3).Value = acab;
                ws.Cell(f, 4).Value = Math.Round(a.Perfiles[h.Codigo].Tramos.Sum(t => t.Largo) / 1000.0, 2);
                ws.Cell(f, 5).Value = h.PiezasHoy.ToString("0.##", Es) + " × " + a.BaseHoy;
                ws.Cell(f, 6).Value = Lista(h.Enviar);
                ws.Cell(f, 7).Value = Lista(h.Repuesto);
                ws.Cell(f, 8).Value = h.Plan.Empalmes;
                f++;
            }
            Cerrar(ws);
        }

        private void HojaGuia(XLWorkbook wb, List<O.HojaPerfil> hojas, Dictionary<string, KeyValuePair<string, string>> acabados, string proyecto, int frentes)
        {
            var ws = Nueva(wb, "Guía", "Guía de montaje · perfiles recopilatorios", proyecto);
            string[] reglas =
            {
                "• Empalmes centrados (piezas iguales), salvo lo que aparece abajo.",
                "• Ningún trozo de menos de 300 mm.",
                "• Los tramos cortos salen de los sobrantes antes de empezar una pieza nueva (ver hojas \"Plan de montaje\" y \"Piezas\").",
                "• Las piezas marcadas REPUESTO solo se usan para reponer una pieza mal cortada o dañada.",
                "• Cada tramo ya lleva su margen de corte y empalme (C. adicional): no hace falta pedir más material."
            };
            int f = 4;
            foreach (string r in reglas)
            {
                ws.Cell(f, 1).Value = r;
                ws.Range(f, 1, f, 5).Merge();
                f++;
            }
            f++;
            ws.Cell(f, 1).Value = "Repuestos incluidos";
            ws.Cell(f, 1).Style.Font.Bold = true;
            f++;
            bool hayRep = false;
            foreach (var h in hojas.Where(x => x.Repuesto.Count > 0))
            {
                string c0, a0;
                Final(h.Codigo, h.Acabado, acabados, out c0, out a0);
                ws.Cell(f, 1).Value = c0 + " " + h.Descripcion + " (" + a0 + ")";
                ws.Range(f, 1, f, 3).Merge();
                ws.Cell(f, 4).Value = Lista(h.Repuesto);
                f++;
                hayRep = true;
            }
            if (!hayRep)
            {
                ws.Cell(f, 1).Value = frentes < O.MinFrentesRepuesto
                    ? "Ninguno: obra de " + frentes + " frentes (con menos de " + O.MinFrentesRepuesto + " no se mandan repuestos)."
                    : "Ninguno: la holgura de cada perfil ya cubre los errores habituales.";
                f++;
            }
            f++;
            ws.Cell(f, 1).Value = "Solo estos tramos llevan algo especial";
            ws.Cell(f, 1).Style.Font.Bold = true;
            f++;
            Encabezado(ws, f, "Ubicación", "Perfil", "Tramo", "Montaje", "Indicación");
            ws.SheetView.FreezeRows(0);
            f++;
            int ini = f;
            foreach (var h in hojas)
            {
                string cod, acab;
                Final(h.Codigo, h.Acabado, acabados, out cod, out acab);
                foreach (var grp in h.Plan.Lay.Where(x => x.Partes.Count > 1 && x.Tipo != "sim")
                                              .GroupBy(x => x.Tramo.Ubicacion + "|" + x.Tramo.Largo + "|" + string.Join(",", x.Partes) + "|" + x.Tipo))
                {
                    var x0 = grp.First();
                    ws.Cell(f, 1).Value = x0.Tramo.Ubicacion;
                    ws.Cell(f, 2).Value = cod + " " + h.Descripcion + (grp.Count() > 1 ? " (×" + grp.Count() + ")" : "");
                    ws.Cell(f, 3).Value = x0.Tramo.Largo;
                    ws.Cell(f, 4).Value = "[ " + string.Join(" │ ", x0.Partes) + " ]";
                    ws.Cell(f, 5).Value = x0.Tipo == "restos" ? "se arma con dos sobrantes"
                                        : "el ajuste de " + x0.Partes.Last() + " va en el extremo menos visible";
                    f++;
                }
            }
            if (f == ini) ws.Cell(f, 1).Value = "Ningún tramo necesita indicaciones especiales.";
            Cerrar(ws);
            ws.Column(1).Width = 14;
        }

        /// <summary>Nombre de cada pieza que llega a obra (P1, P2…) y su largo tal como sale de fábrica.</summary>
        private static List<KeyValuePair<string, int>> Piezas(O.HojaPerfil h)
        {
            var l = new List<KeyValuePair<string, int>>();
            int j = 0;
            foreach (var pz in h.Plan.Piezas)
            {
                j++;
                l.Add(new KeyValuePair<string, int>("P" + j, O.LargoEnvio(h.Plan, pz, h.Base)));
            }
            return l;
        }

        private void HojaPlanMontaje(XLWorkbook wb, List<O.HojaPerfil> hojas, List<O.PiezaMedida> medida,
                                     Dictionary<string, KeyValuePair<string, string>> acabados)
        {
            var ws = Nueva(wb, "Plan de montaje", "Plan de montaje · por ubicación",
                "Todo lo que va en cada ubicación: las piezas a medida (verticales, genéricos…) llegan cortadas y van tal cual; los tramos recopilatorios se arman como se indica (las piezas P1, P2… están en la hoja \"Piezas\").");
            Encabezado(ws, 4, "Ubicación", "Perfil", "Descripción", "Medida", "Cantidad", "Montaje", "Sale de", "Indicación");
            var filas = new List<object[]>();
            // piezas a medida: una fila por ubicación, código y medida
            foreach (var g in medida
                .Select(p =>
                {
                    string cm, am;
                    Final(p.Sub, p.Acabado, acabados, out cm, out am);
                    return new { Ubi = string.IsNullOrEmpty(p.Ubicacion) ? "TODA LA OBRA" : p.Ubicacion, Cod = cm, p.Descripcion, p.Largo };
                })
                .GroupBy(x => new { x.Ubi, x.Cod, x.Descripcion, x.Largo }))
                filas.Add(new object[] { g.Key.Ubi, g.Key.Cod, g.Key.Descripcion, g.Key.Largo, g.Count(),
                                         "pieza a medida", "llega cortada",
                                         g.Key.Ubi == "TODA LA OBRA" ? "misma medida: repartir donde haga falta" : "va entera" });
            foreach (var h in hojas)
            {
                string cod, acab;
                Final(h.Codigo, h.Acabado, acabados, out cod, out acab);
                var nombres = Piezas(h);
                // qué piezas alimentan cada tramo
                var origen = new Dictionary<int, List<string>>();
                for (int j = 0; j < h.Plan.Piezas.Count; j++)
                    foreach (var sg in h.Plan.Piezas[j].Segs)
                    {
                        List<string> l;
                        if (!origen.TryGetValue(sg.Value, out l)) origen[sg.Value] = l = new List<string>();
                        l.Add(nombres[j].Key + " (" + sg.Key + ")");
                    }
                for (int i = 0; i < h.Plan.Lay.Count; i++)
                {
                    var x = h.Plan.Lay[i];
                    string ind = x.Partes.Count == 1 ? "sin empalme"
                               : x.Tipo == "sim" ? "empalme centrado"
                               : x.Tipo == "restos" ? "se arma con dos sobrantes"
                               : "el ajuste de " + x.Partes.Last() + " va en el extremo menos visible";
                    filas.Add(new object[] { string.IsNullOrEmpty(x.Tramo.Ubicacion) ? "—" : x.Tramo.Ubicacion, cod, h.Descripcion,
                        x.Tramo.Largo, 1, "[ " + string.Join(" │ ", x.Partes) + " ]",
                        origen.ContainsKey(i) ? string.Join(" + ", origen[i]) : "", ind });
                }
            }
            int f = 5;
            string anterior = null;
            foreach (var r in filas.OrderBy(z => Convert.ToString(z[0]) == "TODA LA OBRA" ? 0 : 1)
                                   .ThenBy(z => Convert.ToString(z[0]), new OrdenUbicacion())
                                   .ThenBy(z => Convert.ToString(z[5]) == "pieza a medida" ? 0 : 1)
                                   .ThenBy(z => Convert.ToString(z[1])).ThenByDescending(z => Convert.ToInt32(z[3])))
            {
                string ubi = Convert.ToString(r[0]);
                if (anterior != null && ubi != anterior) f++;          // una línea en blanco entre ubicaciones
                anterior = ubi;
                for (int c = 0; c < r.Length; c++) ws.Cell(f, c + 1).Value = r[c];
                ws.Cell(f, 1).Style.Font.Bold = true;
                string ind = Convert.ToString(r[7]);
                if (ind != "sin empalme" && ind != "empalme centrado" && ind != "va entera" && ubi != "TODA LA OBRA")
                    ws.Range(f, 1, f, r.Length).Style.Fill.BackgroundColor = Naranja;
                f++;
            }
            if (f == 5) ws.Cell(f, 1).Value = "Esta obra no lleva perfilería.";
            Cerrar(ws);
        }

        /// <summary>Piezas cortadas a su medida en fábrica (verticales, horizontales a medida, genéricos), por ubicación.</summary>
        private void HojaAMedida(XLWorkbook wb, List<O.PiezaMedida> medida, Dictionary<string, KeyValuePair<string, string>> acabados)
        {
            var ws = Nueva(wb, "Piezas a medida", "Piezas a medida · llegan cortadas de fábrica",
                "Cada una va a su sitio tal cual, sin cortar en obra.");
            Encabezado(ws, 4, "Ubicación", "Perfil", "Descripción", "Acabado", "Largo", "Cantidad");
            int f = 5;
            var filas = medida
                .Select(p =>
                {
                    string cod, acab;
                    Final(p.Sub, p.Acabado, acabados, out cod, out acab);
                    return new { p.Ubicacion, Cod = cod, p.Descripcion, Acab = acab, p.Largo };
                })
                .GroupBy(x => new { x.Ubicacion, x.Cod, x.Descripcion, x.Acab, x.Largo })
                .OrderBy(g => g.Key.Ubicacion ?? "", new OrdenUbicacion()).ThenBy(g => g.Key.Cod).ThenByDescending(g => g.Key.Largo);
            foreach (var g in filas)
            {
                ws.Cell(f, 1).Value = string.IsNullOrEmpty(g.Key.Ubicacion) ? "—" : g.Key.Ubicacion;
                ws.Cell(f, 2).Value = g.Key.Cod;
                ws.Cell(f, 3).Value = g.Key.Descripcion;
                ws.Cell(f, 4).Value = g.Key.Acab;
                ws.Cell(f, 5).Value = g.Key.Largo;
                ws.Cell(f, 6).Value = g.Count();
                f++;
            }
            if (f == 5) ws.Cell(f, 1).Value = "Esta obra no lleva piezas a medida.";
            Cerrar(ws);
        }

        private void HojaPiezas(XLWorkbook wb, List<O.HojaPerfil> hojas, Dictionary<string, KeyValuePair<string, string>> acabados)
        {
            var ws = Nueva(wb, "Piezas", "Piezas que llegan a obra · qué se corta de cada una",
                "Primero el trozo más largo. Lo que queda de cada pieza es sobrante de obra.");
            Encabezado(ws, 4, "Perfil", "Descripción", "Pieza", "Largo", "Trozos (ubicación · largo)", "Sobra");
            int f = 5;
            foreach (var h in hojas)
            {
                string cod, acab;
                Final(h.Codigo, h.Acabado, acabados, out cod, out acab);
                var nombres = Piezas(h);
                for (int j = 0; j < h.Plan.Piezas.Count; j++)
                {
                    var pz = h.Plan.Piezas[j];
                    ws.Cell(f, 1).Value = j == 0 ? cod : "";
                    ws.Cell(f, 2).Value = j == 0 ? h.Descripcion : "";
                    ws.Cell(f, 3).Value = nombres[j].Key;
                    ws.Cell(f, 4).Value = nombres[j].Value;
                    ws.Cell(f, 5).Value = string.Join(" + ", pz.Segs.Select(sg => h.Plan.Lay[sg.Value].Tramo.Ubicacion + " · " + sg.Key));
                    ws.Cell(f, 6).Value = nombres[j].Value - pz.Usado;
                    f++;
                }
                foreach (int r in h.Repuesto)
                {
                    ws.Cell(f, 1).Value = h.Plan.Piezas.Count == 0 ? cod : "";
                    ws.Cell(f, 3).Value = "REPUESTO";
                    ws.Cell(f, 4).Value = r;
                    ws.Cell(f, 5).Value = "solo para reponer una pieza mal cortada o dañada";
                    ws.Range(f, 1, f, 6).Style.Fill.BackgroundColor = Naranja;
                    f++;
                }
            }
            Cerrar(ws);
        }

        /// <summary>M1, M2 … M10 en orden natural.</summary>
        private class OrdenUbicacion : IComparer<string>
        {
            public int Compare(string a, string b)
            {
                a = a ?? ""; b = b ?? "";
                string pa = new string(a.TakeWhile(ch => !char.IsDigit(ch)).ToArray());
                string pb = new string(b.TakeWhile(ch => !char.IsDigit(ch)).ToArray());
                int c = string.Compare(pa, pb, StringComparison.OrdinalIgnoreCase);
                if (c != 0) return c;
                int na, nb;
                int.TryParse(new string(a.Skip(pa.Length).TakeWhile(char.IsDigit).ToArray()), out na);
                int.TryParse(new string(b.Skip(pb.Length).TakeWhile(char.IsDigit).ToArray()), out nb);
                c = na.CompareTo(nb);
                return c != 0 ? c : string.Compare(a, b, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
