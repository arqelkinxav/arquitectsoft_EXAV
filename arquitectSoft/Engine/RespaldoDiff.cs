using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace arquitectSoft.Engine
{
    /// <summary>Gravedad de un bloque del informe, para pintarlo y para decidir si hay riesgo.</summary>
    public enum NivelDif
    {
        /// <summary>Hay filas en la base de aqui que el respaldo NO trae: el import las borra.</summary>
        Perdida,
        /// <summary>La fila existe en los dos sitios pero con datos distintos.</summary>
        Cambio,
        /// <summary>Filas nuevas que entran con el respaldo.</summary>
        Alta,
        /// <summary>Informativo: conteos, tablas que no se tocan.</summary>
        Info
    }

    public class BloqueInforme
    {
        public string Titulo { get; set; }
        public NivelDif Nivel { get; set; }
        public List<string> Lineas { get; set; }

        public BloqueInforme() { Lineas = new List<string>(); }
    }

    /// <summary>Lo que pasaria si se importase el archivo elegido sobre la base conectada.</summary>
    public class InformeImport
    {
        public List<BloqueInforme> Bloques { get; set; }
        public int Perdidas { get; set; }
        public int Cambios { get; set; }
        public int Altas { get; set; }
        public string Titular { get; set; }
        public string Error { get; set; }

        public InformeImport() { Bloques = new List<BloqueInforme>(); }

        public bool HayPerdidas { get { return Perdidas > 0; } }
    }

    /// <summary>
    /// Compara un archivo .sql de respaldo contra la base a la que esta conectado el programa
    /// y cuenta lo que NO cuadra, ANTES de importar nada.
    ///
    /// Por que existe: el import reemplaza tabla por tabla (DROP + CREATE + INSERT), asi que
    /// todo lo que exista aqui y no venga en el archivo se pierde sin avisar. Este informe lo
    /// saca a la cara para poder decidir con conocimiento: usuarios y contraseñas que se van,
    /// componentes que se quedan por el camino, reglas de vidrio que cambian.
    ///
    /// Va fila a fila en las tablas que duelen y por conteo en el resto, que
    /// componentes_detalle cambia con cada retoque y no aporta nada listarla entera.
    /// </summary>
    public static class RespaldoDiff
    {
        /// <summary>Una tabla que se compara fila a fila, con su clave de negocio.</summary>
        private class Sensible
        {
            public string Tabla;
            public string Singular;
            public string[] Clave;
            public string[] Comparar;
            /// <summary>Columnas cuyo valor NO se escribe en el informe (contraseñas).</summary>
            public string[] Secretas = new string[0];
        }

        private static readonly Sensible[] SENSIBLES =
        {
            new Sensible {
                Tabla = "usuario", Singular = "usuario",
                Clave = new[] { "usuario" },
                Comparar = new[] { "contrasena", "Nombre", "rol", "estado" },
                Secretas = new[] { "contrasena" } },

            new Sensible {
                Tabla = "acabados", Singular = "acabado",
                Clave = new[] { "Codigo_Homologacion" },
                Comparar = new[] { "Descripcion" } },

            new Sensible {
                Tabla = "mecanizados", Singular = "mecanizado",
                Clave = new[] { "Codigo_Homologacion" },
                Comparar = new[] { "Descripcion" } },

            new Sensible {
                Tabla = "componentes", Singular = "componente",
                Clave = new[] { "Codigo" },
                Comparar = new[] { "Descripcion", "NoSubcomponente", "Especial", "AcabadoPrincipal" } },

            new Sensible {
                Tabla = "subcomponentes", Singular = "subcomponente",
                Clave = new[] { "Codigo_Homologacion", "Id_Acabado" },
                Comparar = new[] { "Descripcion", "Especial" } },

            new Sensible {
                Tabla = "beta_vidrio_tipo", Singular = "tipo de vidrio",
                Clave = new[] { "Nombre" },
                Comparar = new[] { "Orden" } },

            new Sensible {
                Tabla = "beta_vidrio_sistema", Singular = "sistema de vidrio",
                Clave = new[] { "Prefijo" },
                Comparar = new[] { "Descripcion" } },

            new Sensible {
                Tabla = "beta_dependencias_acabado", Singular = "dependencia de acabado",
                Clave = new[] { "Cod_Placeholder", "Cod_Perfileria" },
                Comparar = new[] { "Cod_Resultado" } }
        };

        /// <summary>Tope de lineas por bloque, para que el informe se pueda leer.</summary>
        private const int MAX_LINEAS = 40;

        // ==================================================================
        // ENTRADA
        // ==================================================================
        public static InformeImport Comparar(string rutaSql)
        {
            var inf = new InformeImport();
            var con = new Generals.Conexion();
            string fail = "";

            try
            {
                Dictionary<string, TablaDump> dump = DumpSql.Leer(rutaSql);
                if (dump.Count == 0)
                {
                    inf.Error = "El archivo no contiene ninguna tabla reconocible. ¿Seguro que es un respaldo?";
                    return inf;
                }

                if (!con.Open(out fail))
                {
                    inf.Error = "No se pudo conectar con la base para comparar:\n" + fail;
                    return inf;
                }

                foreach (Sensible s in SENSIBLES) CompararTabla(con, s, dump, inf);
                CompararReglasVidrio(con, dump, inf);
                ResumenPorTabla(con, dump, inf);

                inf.Titular = Titular(inf);
            }
            catch (Exception ex)
            {
                inf.Error = "No se pudo analizar el respaldo:\n" + ex.Message;
            }
            finally
            {
                try { con.Close(); } catch { }
            }

            return inf;
        }

        private static string Titular(InformeImport inf)
        {
            if (inf.Perdidas == 0 && inf.Cambios == 0 && inf.Altas == 0)
                return "El respaldo y esta base coinciden en todo lo que se revisa. Importar no cambia nada.";

            var partes = new List<string>();
            if (inf.Perdidas > 0) partes.Add(inf.Perdidas + (inf.Perdidas == 1 ? " fila se PIERDE" : " filas se PIERDEN"));
            if (inf.Cambios > 0) partes.Add(inf.Cambios + (inf.Cambios == 1 ? " cambia" : " cambian"));
            if (inf.Altas > 0) partes.Add(inf.Altas + (inf.Altas == 1 ? " entra nueva" : " entran nuevas"));

            string t = string.Join(", ", partes) + ".";
            return inf.Perdidas > 0
                ? t + " Lo que se pierde solo existe aqui: el respaldo no lo trae."
                : t;
        }

        // ==================================================================
        // COMPARACION FILA A FILA
        // ==================================================================
        private static void CompararTabla(Generals.Conexion con, Sensible s,
                                          Dictionary<string, TablaDump> dump, InformeImport inf)
        {
            DataTable viva = LeerTabla(con, s.Tabla);
            TablaDump t;
            bool enDump = dump.TryGetValue(s.Tabla, out t);

            if (viva == null && !enDump) return;

            if (viva == null)
            {
                Agregar(inf, NivelDif.Alta, s.Tabla,
                        new List<string> { "Tabla nueva: no existe en esta base y el respaldo la crea con " + t.Filas.Count + " filas." },
                        0, 0, t.Filas.Count);
                return;
            }

            if (!enDump)
            {
                Agregar(inf, NivelDif.Info, s.Tabla,
                        new List<string> { "El respaldo no trae esta tabla: se queda tal cual esta (" + viva.Rows.Count + " filas)." },
                        0, 0, 0);
                return;
            }

            // Clave de negocio -> fila, en los dos lados.
            var aqui = new Dictionary<string, DataRow>(StringComparer.OrdinalIgnoreCase);
            foreach (DataRow r in viva.Rows)
            {
                string k = ClaveViva(r, s.Clave);
                if (k != null) aqui[k] = r;
            }

            var alla = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
            foreach (string[] f in t.Filas)
            {
                string k = ClaveDump(t, f, s.Clave);
                if (k != null) alla[k] = f;
            }

            var perdidas = new List<string>();
            var cambios = new List<string>();
            var altas = new List<string>();

            foreach (var par in aqui)
            {
                string[] f;
                if (!alla.TryGetValue(par.Key, out f)) { perdidas.Add(par.Key); continue; }

                string detalle = Diferencias(s, par.Value, t, f);
                if (detalle != null) cambios.Add(par.Key + ": " + detalle);
            }

            foreach (var par in alla)
                if (!aqui.ContainsKey(par.Key)) altas.Add(par.Key);

            if (perdidas.Count > 0)
                Agregar(inf, NivelDif.Perdida, TituloPerdida(perdidas.Count, s.Singular),
                        perdidas, perdidas.Count, 0, 0);

            if (cambios.Count > 0)
                Agregar(inf, NivelDif.Cambio, TituloCambio(cambios.Count, s.Singular),
                        cambios, 0, cambios.Count, 0);

            if (altas.Count > 0)
                Agregar(inf, NivelDif.Alta, TituloAlta(altas.Count, s.Singular),
                        altas, 0, 0, altas.Count);
        }

        /// <summary>Devuelve la descripcion de lo que cambia, o null si la fila es igual.</summary>
        private static string Diferencias(Sensible s, DataRow viva, TablaDump t, string[] fila)
        {
            var trozos = new List<string>();

            foreach (string col in s.Comparar)
            {
                if (!viva.Table.Columns.Contains(col)) continue;
                if (t.IndiceDe(col) < 0) continue;

                string a = Texto(viva[col]);
                string b = t.Valor(fila, col);
                if (MismoValor(a, b)) continue;

                trozos.Add(s.Secretas.Contains(col, StringComparer.OrdinalIgnoreCase)
                    ? "cambia " + Bonito(col)
                    : Bonito(col) + " \"" + Corto(a) + "\" -> \"" + Corto(b) + "\"");
            }

            return trozos.Count == 0 ? null : string.Join("; ", trozos);
        }

        // ==================================================================
        // REGLAS DE VIDRIO (se leen en claro, no por id)
        //
        // Se guardan por Id_SubComponente, asi que comparar los numeros no dice
        // nada: si los catalogos han evolucionado por separado, el mismo id es
        // otra pieza. Se traduce cada regla a codigos por su propio lado y se
        // comparan los textos, que es lo que de verdad tiene que cuadrar.
        // ==================================================================
        private static void CompararReglasVidrio(Generals.Conexion con,
                                                 Dictionary<string, TablaDump> dump, InformeImport inf)
        {
            const string TABLA = "beta_vidrio_regla";
            DataTable viva = LeerTabla(con, TABLA);
            TablaDump t;
            bool enDump = dump.TryGetValue(TABLA, out t);

            if (viva == null && !enDump) return;
            if (!enDump)
            {
                Agregar(inf, NivelDif.Info, "Reglas de vidrio",
                        new List<string> { "El respaldo no trae las reglas de vidrio: se quedan como estan." }, 0, 0, 0);
                return;
            }

            Dictionary<string, string> aqui = viva == null
                ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                : ReglasVivas(con, viva);
            Dictionary<string, string> alla = ReglasDump(dump, t);

            var perdidas = new List<string>();
            var cambios = new List<string>();
            var altas = new List<string>();

            foreach (var par in aqui)
            {
                string destino;
                if (!alla.TryGetValue(par.Key, out destino)) { perdidas.Add(par.Key + " -> " + par.Value); continue; }
                if (!MismoValor(par.Value, destino))
                    cambios.Add(par.Key + ": pasa a " + destino + " (aqui era " + par.Value + ")");
            }
            foreach (var par in alla)
                if (!aqui.ContainsKey(par.Key)) altas.Add(par.Key + " -> " + par.Value);

            if (perdidas.Count > 0)
                Agregar(inf, NivelDif.Perdida, TituloPerdida(perdidas.Count, "regla de vidrio"),
                        perdidas, perdidas.Count, 0, 0);
            if (cambios.Count > 0)
                Agregar(inf, NivelDif.Cambio, TituloCambio(cambios.Count, "regla de vidrio"),
                        cambios, 0, cambios.Count, 0);
            if (altas.Count > 0)
                Agregar(inf, NivelDif.Alta, TituloAlta(altas.Count, "regla de vidrio"),
                        altas, 0, 0, altas.Count);
        }

        /// <summary>Reglas de la base viva, ya traducidas a "SISTEMA · TIPO · CODIGO_ORIGEN" -> "CODIGO_DESTINO".</summary>
        private static Dictionary<string, string> ReglasVivas(Generals.Conexion con, DataTable reglas)
        {
            var mapa = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            Dictionary<string, string> sis = Diccionario(LeerTabla(con, "beta_vidrio_sistema"), "Id", "Prefijo");
            Dictionary<string, string> tip = Diccionario(LeerTabla(con, "beta_vidrio_tipo"), "Id", "Nombre");
            Dictionary<string, string> sub = Diccionario(LeerTabla(con, "subcomponentes"), "Id_Subcomponente", "Codigo_Homologacion");

            foreach (DataRow r in reglas.Rows)
            {
                string clave = Busca(sis, Texto(r["Id_Sistema"])) + " · " + Busca(tip, Texto(r["Id_Tipo"]))
                             + " · " + Busca(sub, Texto(r["Id_Sub_Origen"]));
                mapa[clave] = Busca(sub, Texto(r["Id_Sub_Destino"]));
            }
            return mapa;
        }

        /// <summary>Lo mismo, pero traduciendo con las tablas que vienen DENTRO del respaldo.</summary>
        private static Dictionary<string, string> ReglasDump(Dictionary<string, TablaDump> dump, TablaDump reglas)
        {
            var mapa = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            Dictionary<string, string> sis = DiccionarioDump(dump, "beta_vidrio_sistema", "Id", "Prefijo");
            Dictionary<string, string> tip = DiccionarioDump(dump, "beta_vidrio_tipo", "Id", "Nombre");
            Dictionary<string, string> sub = DiccionarioDump(dump, "subcomponentes", "Id_Subcomponente", "Codigo_Homologacion");

            foreach (string[] f in reglas.Filas)
            {
                string clave = Busca(sis, reglas.Valor(f, "Id_Sistema")) + " · " + Busca(tip, reglas.Valor(f, "Id_Tipo"))
                             + " · " + Busca(sub, reglas.Valor(f, "Id_Sub_Origen"));
                mapa[clave] = Busca(sub, reglas.Valor(f, "Id_Sub_Destino"));
            }
            return mapa;
        }

        private static string Busca(Dictionary<string, string> d, string id)
        {
            string v;
            if (id != null && d.TryGetValue(id, out v) && !string.IsNullOrEmpty(v)) return v;
            return "(id " + (id ?? "?") + " sin resolver)";
        }

        private static Dictionary<string, string> Diccionario(DataTable t, string clave, string valor)
        {
            var d = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (t == null || !t.Columns.Contains(clave) || !t.Columns.Contains(valor)) return d;
            foreach (DataRow r in t.Rows) d[Texto(r[clave])] = Texto(r[valor]);
            return d;
        }

        private static Dictionary<string, string> DiccionarioDump(Dictionary<string, TablaDump> dump,
                                                                  string tabla, string clave, string valor)
        {
            var d = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            TablaDump t;
            if (!dump.TryGetValue(tabla, out t)) return d;
            foreach (string[] f in t.Filas)
            {
                string k = t.Valor(f, clave);
                if (k != null) d[k] = t.Valor(f, valor);
            }
            return d;
        }

        // ==================================================================
        // RESUMEN DE TODAS LAS TABLAS
        // ==================================================================
        private static void ResumenPorTabla(Generals.Conexion con,
                                            Dictionary<string, TablaDump> dump, InformeImport inf)
        {
            string fail = "";
            DataSet ds = con.ExecuteDataSet(
                "SELECT TABLE_NAME FROM information_schema.TABLES " +
                "WHERE TABLE_SCHEMA = DATABASE() AND TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_NAME", out fail);
            if (ds == null || ds.Tables.Count == 0) return;

            var lineas = new List<string>();
            var intactas = new List<string>();
            var nuevas = new List<string>(dump.Keys);

            foreach (DataRow r in ds.Tables[0].Rows)
            {
                string tabla = Texto(r[0]);
                nuevas.RemoveAll(x => string.Equals(x, tabla, StringComparison.OrdinalIgnoreCase));

                int aqui = Contar(con, tabla);
                TablaDump t;
                if (!dump.TryGetValue(tabla, out t)) { intactas.Add(tabla + " (" + aqui + ")"); continue; }

                int alla = t.Filas.Count;
                lineas.Add(string.Format("{0}: {1} -> {2}{3}", tabla, aqui, alla,
                           aqui == alla ? "" : (alla > aqui ? "  (+" + (alla - aqui) + ")" : "  (" + (alla - aqui) + ")")));
            }

            if (lineas.Count > 0)
                Agregar(inf, NivelDif.Info, "Filas por tabla (aqui -> tras importar)", lineas, 0, 0, 0);

            if (intactas.Count > 0)
                Agregar(inf, NivelDif.Info, "Tablas que el respaldo NO trae (se quedan como estan)", intactas, 0, 0, 0);

            if (nuevas.Count > 0)
            {
                var l = nuevas.Select(n => n + " (" + dump[n].Filas.Count + " filas)").ToList();
                Agregar(inf, NivelDif.Info, "Tablas que el respaldo CREA (no existen aqui)", l, 0, 0, 0);
            }
        }

        private static int Contar(Generals.Conexion con, string tabla)
        {
            try
            {
                string fail = "";
                DataSet ds = con.ExecuteDataSet("SELECT COUNT(*) FROM `" + tabla + "`", out fail);
                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0) return -1;
                return Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            }
            catch { return -1; }
        }

        // ==================================================================
        // AUXILIARES
        // ==================================================================
        private static DataTable LeerTabla(Generals.Conexion con, string tabla)
        {
            try
            {
                string fail = "";
                DataSet ds = con.ExecuteDataSet("SELECT * FROM `" + tabla + "`", out fail);
                if (ds == null || ds.Tables.Count == 0) return null;
                return string.IsNullOrEmpty(fail) ? ds.Tables[0] : null;
            }
            catch { return null; }   // la tabla no existe en esta base
        }

        private static void Agregar(InformeImport inf, NivelDif nivel, string titulo,
                                    List<string> lineas, int perdidas, int cambios, int altas)
        {
            var b = new BloqueInforme { Titulo = titulo, Nivel = nivel };

            lineas.Sort(StringComparer.OrdinalIgnoreCase);
            if (lineas.Count > MAX_LINEAS)
            {
                b.Lineas.AddRange(lineas.Take(MAX_LINEAS));
                b.Lineas.Add("… y " + (lineas.Count - MAX_LINEAS) + " mas.");
            }
            else b.Lineas.AddRange(lineas);

            inf.Bloques.Add(b);
            inf.Perdidas += perdidas;
            inf.Cambios += cambios;
            inf.Altas += altas;
        }

        private static string ClaveViva(DataRow r, string[] cols)
        {
            var partes = new List<string>();
            foreach (string c in cols)
            {
                if (!r.Table.Columns.Contains(c)) return null;
                partes.Add(Texto(r[c]));
            }
            return string.Join(" · ", partes);
        }

        private static string ClaveDump(TablaDump t, string[] fila, string[] cols)
        {
            var partes = new List<string>();
            foreach (string c in cols)
            {
                if (t.IndiceDe(c) < 0) return null;
                partes.Add(t.Valor(fila, c) ?? "");
            }
            return string.Join(" · ", partes);
        }

        private static string Texto(object v)
        {
            if (v == null || v == DBNull.Value) return "";
            if (v is DateTime) return ((DateTime)v).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            if (v is bool) return ((bool)v) ? "1" : "0";
            if (v is byte[]) return BitConverter.ToString((byte[])v).Replace("-", "");
            return Convert.ToString(v, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Compara dos valores tolerando las diferencias de formato entre la base y el texto
        /// del archivo (1 vs 1.00, nulo vs vacio, espacios de sobra).
        /// </summary>
        private static bool MismoValor(string a, string b)
        {
            a = (a ?? "").Trim();
            b = (b ?? "").Trim();
            if (a == b) return true;
            if (a.Length == 0 && b.Length == 0) return true;

            decimal da, db;
            if (decimal.TryParse(a, NumberStyles.Any, CultureInfo.InvariantCulture, out da) &&
                decimal.TryParse(b, NumberStyles.Any, CultureInfo.InvariantCulture, out db))
                return da == db;

            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }

        // Titulos de bloque. El verbo va concordado con el numero: nada de "1 componente que CAMBIAN".
        // Los textos se quedan sin genero ("que ENTRAN", no "nuevos"/"nuevas") para que valgan
        // igual con componentes que con reglas.
        private static string TituloPerdida(int n, string singular)
        {
            return Plural(n, singular) + (n == 1 ? " que SE PIERDE" : " que SE PIERDEN")
                 + (n == 1 ? " (esta aqui y el respaldo no)" : " (estan aqui y el respaldo no)");
        }

        private static string TituloCambio(int n, string singular)
        {
            return Plural(n, singular) + (n == 1 ? " que CAMBIA" : " que CAMBIAN");
        }

        private static string TituloAlta(int n, string singular)
        {
            return Plural(n, singular) + (n == 1 ? " que ENTRA (no esta aqui todavia)" : " que ENTRAN (no estan aqui todavia)");
        }

        /// <summary>
        /// "3" + "regla de vidrio" -> "3 reglas de vidrio": el plural va en la PRIMERA palabra,
        /// que si no salen cosas como "reglas de vidrios".
        /// </summary>
        private static string Plural(int n, string singular)
        {
            if (n == 1) return "1 " + singular;

            int esp = singular.IndexOf(' ');
            string cabeza = esp < 0 ? singular : singular.Substring(0, esp);
            string resto = esp < 0 ? "" : singular.Substring(esp);
            if (!cabeza.EndsWith("s", StringComparison.OrdinalIgnoreCase)) cabeza += "s";

            return n + " " + cabeza + resto;
        }

        private static string Bonito(string columna)
        {
            switch (columna.ToLowerInvariant())
            {
                case "contrasena": return "contraseña";
                case "rol": return "rol";
                case "nombre": return "nombre";
                case "descripcion": return "descripción";
                case "nosubcomponente": return "nº de subcomponentes";
                case "acabadoprincipal": return "acabado principal";
                case "cod_resultado": return "resultado";
                default: return columna;
            }
        }

        private static string Corto(string s)
        {
            if (s == null) return "";
            s = s.Replace('\n', ' ').Replace('\r', ' ').Trim();
            return s.Length <= 60 ? s : s.Substring(0, 57) + "…";
        }
    }
}
