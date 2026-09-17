using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace arquitectSoft.Engine
{
    /// <summary>
    /// "Actualización instalada": al abrir una versión nueva, cada usuario ve UNA vez la lista
    /// de cambios (los commits) desde la última versión que abrió.
    ///
    /// La lista la mete el .csproj dentro del .exe al compilar (git log → recurso
    /// Novedades.txt). Lo que ha visto cada usuario se guarda en novedades_vistas.txt junto al
    /// .exe: la beta se abre desde la carpeta compartida, así que vale para todos los equipos.
    /// Si ahí no se puede escribir, se guarda en el perfil de Windows (%AppData%).
    /// </summary>
    public static class Novedades
    {
        public class Cambio
        {
            public string Hash { get; set; }
            public string Titulo { get; set; }
            public DateTime Fecha { get; set; }
            public string FechaTexto { get { return Fecha.ToString("dd/MM/yyyy"); } }
        }

        private const string RECURSO = "arquitectSoft.Novedades.txt";
        private const string ARCHIVO = "novedades_vistas.txt";
        private const int MAX = 25;

        /// <summary>Commits incluidos en este .exe, del más nuevo al más viejo (vacío si no se incrustaron).</summary>
        public static List<Cambio> Todos()
        {
            var l = new List<Cambio>();
            try
            {
                using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(RECURSO))
                {
                    if (s == null) return l;
                    using (var r = new StreamReader(s, Encoding.UTF8))
                    {
                        string linea;
                        while ((linea = r.ReadLine()) != null)
                        {
                            // Formato "--pretty=reference": abc1234 (Título del commit, 2026-09-17)
                            linea = linea.Trim();
                            int esp = linea.IndexOf(" (", StringComparison.Ordinal);
                            int coma = linea.LastIndexOf(", ", StringComparison.Ordinal);
                            if (esp <= 0 || coma <= esp || !linea.EndsWith(")")) continue;
                            DateTime f;
                            string fecha = linea.Substring(coma + 2, linea.Length - coma - 3);
                            if (!DateTime.TryParseExact(fecha, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out f))
                                continue;
                            l.Add(new Cambio
                            {
                                Hash = linea.Substring(0, esp),
                                Titulo = linea.Substring(esp + 2, coma - esp - 2),
                                Fecha = f
                            });
                        }
                    }
                }
            }
            catch { }
            return l;
        }

        /// <summary>
        /// Cambios que le toca ver a <paramref name="usuario"/> (vacío si ya vio esta versión).
        /// La primera vez, los de las últimas semanas.
        /// </summary>
        public static List<Cambio> Pendientes(string usuario)
        {
            var todos = Todos();
            if (todos.Count == 0 || string.IsNullOrWhiteSpace(usuario)) return new List<Cambio>();

            string visto = Visto(usuario);
            if (visto == todos[0].Hash) return new List<Cambio>();

            int i = visto == null ? -1 : todos.FindIndex(c => c.Hash == visto);
            if (i >= 0) return todos.Take(Math.Min(i, MAX)).ToList();

            // Nunca vio ninguna (o la suya es tan vieja que ya no está en la lista).
            DateTime desde = todos[0].Fecha.AddDays(-30);
            var recientes = todos.TakeWhile(c => c.Fecha >= desde).Take(MAX).ToList();
            return recientes.Count >= 5 ? recientes : todos.Take(Math.Min(5, todos.Count)).ToList();
        }

        /// <summary>Apunta que <paramref name="usuario"/> ya vio la versión actual.</summary>
        public static void MarcarVisto(string usuario)
        {
            var todos = Todos();
            if (todos.Count == 0 || string.IsNullOrWhiteSpace(usuario)) return;
            if (!Guardar(RutaCompartida(), usuario, todos[0].Hash))
                Guardar(RutaLocal(), usuario, todos[0].Hash);
        }

        // ===== archivo "usuario<TAB>hash" =====

        private static string RutaCompartida()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ARCHIVO);
        }

        private static string RutaLocal()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                "arquitectSoft", ARCHIVO);
        }

        private static string Visto(string usuario)
        {
            return Leer(RutaCompartida(), usuario) ?? Leer(RutaLocal(), usuario);
        }

        private static string Leer(string ruta, string usuario)
        {
            try
            {
                if (!File.Exists(ruta)) return null;
                foreach (string l in File.ReadAllLines(ruta, Encoding.UTF8))
                {
                    string[] p = l.Split('\t');
                    if (p.Length >= 2 && string.Equals(p[0].Trim(), usuario.Trim(), StringComparison.OrdinalIgnoreCase))
                        return p[1].Trim();
                }
            }
            catch { }
            return null;
        }

        private static bool Guardar(string ruta, string usuario, string hash)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ruta));
                var lineas = File.Exists(ruta) ? File.ReadAllLines(ruta, Encoding.UTF8).ToList() : new List<string>();
                lineas.RemoveAll(l => string.Equals(l.Split('\t')[0].Trim(), usuario.Trim(), StringComparison.OrdinalIgnoreCase));
                lineas.Add(usuario.Trim() + "\t" + hash + "\t" + DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                File.WriteAllLines(ruta, lineas, new UTF8Encoding(false));
                return true;
            }
            catch { return false; }
        }
    }
}
