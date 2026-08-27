using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace arquitectSoft.Engine
{
    /// <summary>
    /// Una tabla tal y como viene dentro de un archivo .sql de respaldo.
    /// Las filas se guardan alineadas a <see cref="Columnas"/>.
    /// </summary>
    public class TablaDump
    {
        public string Nombre;
        public readonly List<string> Columnas = new List<string>();
        public readonly List<string[]> Filas = new List<string[]>();

        /// <summary>El archivo trae DROP/CREATE de esta tabla: al importar la reemplaza entera.</summary>
        public bool Reemplaza;

        public int IndiceDe(string columna)
        {
            for (int i = 0; i < Columnas.Count; i++)
                if (string.Equals(Columnas[i], columna, StringComparison.OrdinalIgnoreCase)) return i;
            return -1;
        }

        public string Valor(string[] fila, string columna)
        {
            int i = IndiceDe(columna);
            return (i < 0 || i >= fila.Length) ? null : fila[i];
        }
    }

    /// <summary>
    /// Lector de archivos .sql de respaldo: saca, por tabla, sus columnas y sus filas.
    ///
    /// Sirve tanto para lo que escribe el boton Respaldo (MySqlBackup.NET) como para un
    /// volcado de mysqldump, porque los dos escriben el mismo tipo de sentencias. NO ejecuta
    /// nada contra la base, solo lee el texto: por eso se puede usar para enseñar por
    /// adelantado que haria un import antes de tocar nada.
    /// </summary>
    public static class DumpSql
    {
        public static Dictionary<string, TablaDump> Leer(string ruta)
        {
            var tablas = new Dictionary<string, TablaDump>(StringComparer.OrdinalIgnoreCase);
            string texto = File.ReadAllText(ruta, Encoding.UTF8);

            foreach (string sent in Sentencias(texto))
            {
                string s = sent.TrimStart();
                if (s.Length < 6) continue;

                if (EmpiezaPor(s, "INSERT")) LeerInsert(s, tablas);
                else if (EmpiezaPor(s, "CREATE TABLE")) LeerCreate(s, tablas);
                else if (EmpiezaPor(s, "DROP TABLE"))
                {
                    string n = PrimerNombre(s, 10);
                    if (n != null) Obtener(tablas, n).Reemplaza = true;
                }
            }
            return tablas;
        }

        private static TablaDump Obtener(Dictionary<string, TablaDump> tablas, string nombre)
        {
            TablaDump t;
            if (!tablas.TryGetValue(nombre, out t))
            {
                t = new TablaDump { Nombre = nombre };
                tablas[nombre] = t;
            }
            return t;
        }

        private static bool EmpiezaPor(string s, string prefijo)
        {
            return s.Length >= prefijo.Length &&
                   string.Equals(s.Substring(0, prefijo.Length), prefijo, StringComparison.OrdinalIgnoreCase);
        }

        // ------------------------------------------------------------------
        // TROCEADO EN SENTENCIAS
        // Parte por ';' pero solo cuando esta fuera de comillas y de comentarios:
        // si no, un punto y coma dentro de una descripcion parte la sentencia.
        // ------------------------------------------------------------------
        private static IEnumerable<string> Sentencias(string t)
        {
            var sb = new StringBuilder();
            int i = 0;
            while (i < t.Length)
            {
                char c = t[i];

                if (c == '-' && i + 1 < t.Length && t[i + 1] == '-')       // comentario de linea
                {
                    while (i < t.Length && t[i] != '\n') i++;
                    continue;
                }
                if (c == '#')
                {
                    while (i < t.Length && t[i] != '\n') i++;
                    continue;
                }
                if (c == '/' && i + 1 < t.Length && t[i + 1] == '*')       // comentario de bloque
                {
                    int fin = t.IndexOf("*/", i + 2, StringComparison.Ordinal);
                    if (fin < 0) fin = t.Length - 2;
                    i = fin + 2;
                    continue;
                }

                if (c == '\'' || c == '"' || c == '`')                     // literal o identificador
                {
                    char cierre = c;
                    sb.Append(c); i++;
                    while (i < t.Length)
                    {
                        char d = t[i];
                        if (d == '\\' && cierre != '`' && i + 1 < t.Length) { sb.Append(d).Append(t[i + 1]); i += 2; continue; }
                        if (d == cierre)
                        {
                            if (i + 1 < t.Length && t[i + 1] == cierre) { sb.Append(d).Append(d); i += 2; continue; }  // comilla duplicada
                            sb.Append(d); i++; break;
                        }
                        sb.Append(d); i++;
                    }
                    continue;
                }

                if (c == ';')
                {
                    string sent = sb.ToString().Trim();
                    sb.Length = 0;
                    i++;
                    if (sent.Length > 0) yield return sent;
                    continue;
                }

                sb.Append(c); i++;
            }

            string ultima = sb.ToString().Trim();
            if (ultima.Length > 0) yield return ultima;
        }

        // ------------------------------------------------------------------
        // CREATE TABLE  ->  nombre y orden de columnas
        // ------------------------------------------------------------------
        private static void LeerCreate(string s, Dictionary<string, TablaDump> tablas)
        {
            string nombre = PrimerNombre(s, 12);
            if (nombre == null) return;

            TablaDump t = Obtener(tablas, nombre);
            t.Reemplaza = true;
            if (t.Columnas.Count > 0) return;

            int ini = s.IndexOf('(');
            if (ini < 0) return;

            // Cada definicion de columna empieza por su nombre entre backticks, al principio de la linea.
            foreach (string bruta in s.Substring(ini + 1).Split('\n'))
            {
                string linea = bruta.Trim();
                if (!linea.StartsWith("`")) continue;
                int cierre = linea.IndexOf('`', 1);
                if (cierre <= 1) continue;
                t.Columnas.Add(linea.Substring(1, cierre - 1));
            }
        }

        /// <summary>Primer identificador entre backticks a partir de la posicion indicada.</summary>
        private static string PrimerNombre(string s, int desde)
        {
            if (desde >= s.Length) return null;
            int a = s.IndexOf('`', desde);
            if (a < 0) return null;
            int b = s.IndexOf('`', a + 1);
            if (b <= a) return null;
            return s.Substring(a + 1, b - a - 1);
        }

        // ------------------------------------------------------------------
        // INSERT INTO `tabla` (`c1`,`c2`) VALUES (..),(..)
        // La lista de columnas es opcional: si no viene, se usa la del CREATE.
        // ------------------------------------------------------------------
        private static void LeerInsert(string s, Dictionary<string, TablaDump> tablas)
        {
            int aperturaNombre = s.IndexOf('`', 6);
            if (aperturaNombre < 0) return;
            int cierreNombre = s.IndexOf('`', aperturaNombre + 1);
            if (cierreNombre <= aperturaNombre) return;

            string nombre = s.Substring(aperturaNombre + 1, cierreNombre - aperturaNombre - 1);
            TablaDump t = Obtener(tablas, nombre);

            int pos = cierreNombre + 1;
            int values = IndiceDeValues(s, pos);
            if (values < 0) return;

            // Columnas de ESTA sentencia (puede no traerlas).
            var cols = new List<string>();
            string cabecera = s.Substring(pos, values - pos);
            int p = cabecera.IndexOf('(');
            if (p >= 0)
            {
                int q = cabecera.LastIndexOf(')');
                if (q > p)
                    foreach (string trozo in cabecera.Substring(p + 1, q - p - 1).Split(','))
                        cols.Add(trozo.Trim().Trim('`').Trim());
            }
            if (cols.Count == 0) cols.AddRange(t.Columnas);               // sin lista: la del CREATE
            if (cols.Count == 0) return;                                  // no hay forma de saber el orden

            if (t.Columnas.Count == 0) t.Columnas.AddRange(cols);

            // Mapa de las columnas de la sentencia a las canonicas de la tabla.
            var destino = new int[cols.Count];
            for (int i = 0; i < cols.Count; i++)
            {
                int d = t.IndiceDe(cols[i]);
                if (d < 0) { t.Columnas.Add(cols[i]); d = t.Columnas.Count - 1; }
                destino[i] = d;
            }

            foreach (List<string> tupla in Tuplas(s, values + 6))
            {
                var fila = new string[t.Columnas.Count];
                int n = Math.Min(tupla.Count, destino.Length);
                for (int i = 0; i < n; i++) fila[destino[i]] = tupla[i];
                t.Filas.Add(fila);
            }
        }

        /// <summary>Busca la palabra VALUES fuera de comillas.</summary>
        private static int IndiceDeValues(string s, int desde)
        {
            for (int i = desde; i + 6 <= s.Length; i++)
            {
                char c = s[i];
                if (c == '\'' || c == '`' || c == '"') { i = FinLiteral(s, i); continue; }
                if ((c == 'V' || c == 'v') &&
                    string.Equals(s.Substring(i, 6), "VALUES", StringComparison.OrdinalIgnoreCase)) return i;
            }
            return -1;
        }

        private static int FinLiteral(string s, int i)
        {
            char cierre = s[i];
            i++;
            while (i < s.Length)
            {
                char d = s[i];
                if (d == '\\' && cierre != '`') { i += 2; continue; }
                if (d == cierre)
                {
                    if (i + 1 < s.Length && s[i + 1] == cierre) { i += 2; continue; }
                    return i;
                }
                i++;
            }
            return s.Length - 1;
        }

        /// <summary>Recorre las tuplas (v1,v2,...) que van detras de VALUES.</summary>
        private static IEnumerable<List<string>> Tuplas(string s, int desde)
        {
            int i = desde;
            while (i < s.Length)
            {
                while (i < s.Length && s[i] != '(') i++;
                if (i >= s.Length) yield break;
                i++;

                var valores = new List<string>();
                var sb = new StringBuilder();
                bool hayValor = false;

                while (i < s.Length)
                {
                    char c = s[i];

                    if (c == '\'' || c == '"')                            // literal
                    {
                        int fin = FinLiteral(s, i);
                        sb.Append(Desescapar(s.Substring(i + 1, Math.Max(0, fin - i - 1)), c));
                        hayValor = true;
                        i = fin + 1;
                        continue;
                    }
                    if (c == ',') { valores.Add(Cerrar(sb, hayValor)); hayValor = false; i++; continue; }
                    if (c == ')') { valores.Add(Cerrar(sb, hayValor)); i++; break; }

                    if (!char.IsWhiteSpace(c)) hayValor = true;
                    sb.Append(c); i++;
                }

                yield return valores;

                // Tras la tupla solo puede venir ',' (otra tupla) o el final.
                while (i < s.Length && (char.IsWhiteSpace(s[i]) || s[i] == ',')) i++;
                if (i >= s.Length || s[i] != '(') yield break;
            }
        }

        private static string Cerrar(StringBuilder sb, bool hayValor)
        {
            string v = sb.ToString().Trim();
            sb.Length = 0;
            if (!hayValor) return "";
            if (string.Equals(v, "NULL", StringComparison.OrdinalIgnoreCase)) return null;
            return v;
        }

        private static string Desescapar(string s, char comilla)
        {
            var sb = new StringBuilder(s.Length);
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (c == comilla && i + 1 < s.Length && s[i + 1] == comilla) { sb.Append(comilla); i++; continue; }
                if (c != '\\' || i + 1 >= s.Length) { sb.Append(c); continue; }

                char d = s[++i];
                switch (d)
                {
                    case 'n': sb.Append('\n'); break;
                    case 'r': sb.Append('\r'); break;
                    case 't': sb.Append('\t'); break;
                    case '0': sb.Append('\0'); break;
                    case 'b': sb.Append('\b'); break;
                    case 'Z': sb.Append(''); break;
                    default: sb.Append(d); break;
                }
            }
            return sb.ToString();
        }
    }
}
