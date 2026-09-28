using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace arquitectSoft.Engine
{
    /// <summary>
    /// Quita la tabla `usuario` de un respaldo .sql (lo mismo que hacía a mano
    /// _beta_deploy\quitar_usuarios.ps1). Para qué: el respaldo de Olimpo se importa en la
    /// beta de la empresa, y un respaldo completo hace DROP TABLE `usuario` y se lleva por
    /// delante las cuentas, contraseñas y perfiles de los técnicos. Sin el bloque, la tabla
    /// `usuario` de la beta ni se menciona en el import y se queda tal cual.
    ///
    /// OJO: también deja fuera cambios de ESQUEMA de `usuario`; si algún día se le añaden
    /// columnas, ese ALTER hay que aplicarlo a mano en la beta.
    ///
    /// Los PERFILES (beta_perfil + beta_rol_boton) van con los usuarios: cada base gestiona
    /// los suyos (en la empresa se crean perfiles que Olimpo no tiene, y el Id de un perfil
    /// nuevo de Olimpo pisaría a otro distinto de la empresa). Esas tablas no se borran ni se
    /// rellenan: solo queda su CREATE TABLE IF NOT EXISTS, para que en una base que aún no
    /// las tenga se creen vacías y los triggers del respaldo (que las nombran) no fallen.
    /// </summary>
    public static class RespaldoSinUsuarios
    {
        // El bloque de una tabla va desde "-- Definition of <tabla>" hasta el siguiente
        // encabezado de sección. "Dumping data for table <tabla>" queda DENTRO (no corta).
        private static readonly Regex FIN = new Regex(@"^--\s*(Dumping functions|Dumping procedures|Definition of )");

        /// <summary>
        /// Tablas que un Importar no debe tocar NUNCA, con o sin casilla: el historial de
        /// novedades del catálogo es de la base donde se importa (ver NovedadesDatos).
        /// </summary>
        private static readonly string[] SIEMPRE = { NovedadesDatos.TABLA, NovedadesDatos.VISTAS };

        /// <summary>Tablas de perfiles: con la casilla, se conservan las de la base destino.</summary>
        private static readonly string[] PERFILES = { "beta_perfil", "beta_rol_boton" };

        private static Regex DropOInsert(string tabla)
        {
            return new Regex(@"^\s*(DROP TABLE IF EXISTS|INSERT INTO)\s+`?" + Regex.Escape(tabla) + @"`?[\s(;]",
                             RegexOptions.IgnoreCase);
        }

        private static Regex Inicio(string tabla)
        {
            return new Regex(@"^--\s*Definition of " + Regex.Escape(tabla) + @"\s*$");
        }

        private static Regex Restos(string tabla)
        {
            return new Regex(@"(DROP TABLE IF EXISTS|CREATE TABLE|INSERT INTO)\s+`?" + Regex.Escape(tabla) + @"`?[\s(]",
                             RegexOptions.IgnoreCase);
        }

        /// <summary>¿El respaldo trae la tabla `usuario`?</summary>
        public static bool TraeUsuarios(string ruta)
        {
            return Trae(ruta, "usuario");
        }

        private static bool Trae(string ruta, string tabla)
        {
            Regex ini = Inicio(tabla);
            foreach (string l in File.ReadLines(ruta, Encoding.UTF8))
                if (ini.IsMatch(l)) return true;
            return false;
        }

        /// <summary>
        /// Escribe en <paramref name="destino"/> el respaldo sin la tabla `usuario` (UTF-8 SIN
        /// BOM: el BOM se cuela delante de la primera sentencia y puede tumbar el import).
        /// Devuelve las líneas retiradas. Lanza si el resultado sigue tocando `usuario`.
        /// </summary>
        public static int Quitar(string origen, string destino)
        {
            if (!TraeUsuarios(origen)) throw new InvalidOperationException("El respaldo no trae la tabla usuario.");
            return QuitarTablas(origen, destino, new[] { "usuario" }, PERFILES);
        }

        /// <summary>
        /// Quita los bloques de <paramref name="tablas"/> y deja las <paramref name="conservar"/>
        /// sin DROP ni datos (las que no estén en el respaldo, se ignoran).
        /// </summary>
        private static int QuitarTablas(string origen, string destino, string[] tablas, string[] conservar)
        {
            var lineas = new List<string>(File.ReadAllLines(origen, Encoding.UTF8));
            int total = 0;

            foreach (string tabla in conservar)
            {
                Regex quitar = DropOInsert(tabla);
                int antes = lineas.Count;
                lineas.RemoveAll(l => quitar.IsMatch(l));
                if (lineas.Count == antes) continue;
                total += antes - lineas.Count;
                int ini = lineas.FindIndex(l => Inicio(tabla).IsMatch(l));
                if (ini >= 0)
                    lineas.Insert(ini + 1, "-- [arquitectSoft] Sin DROP ni datos a proposito: los perfiles son de la base donde se importa.");
            }

            foreach (string tabla in tablas)
            {
                Regex ini = Inicio(tabla);
                int desde = lineas.FindIndex(l => ini.IsMatch(l));
                if (desde < 0) continue;
                int fin = lineas.FindIndex(desde + 1, l => FIN.IsMatch(l));
                if (fin < 0) fin = lineas.Count;

                // El comentario abre con un "--" suelto encima; se recorta también.
                if (desde > 0 && lineas[desde - 1].Trim() == "--") desde--;

                lineas.RemoveRange(desde, fin - desde);
                lineas.InsertRange(desde, new[]
                {
                    "-- [arquitectSoft] Bloque de la tabla `" + tabla + "` retirado a proposito:",
                    tabla == "usuario"
                        ? "-- en la beta manda la tabla de usuarios de la beta, no la de Olimpo."
                        : "-- el historial de novedades es de la base donde se importa.",
                    ""
                });
                total += fin - desde;

                Regex restos = Restos(tabla);
                foreach (string l in lineas)
                    if (restos.IsMatch(l))
                        throw new InvalidOperationException(
                            "Después de quitar la tabla " + tabla + " siguen apareciendo sentencias sobre ella:\n" +
                            (l.Length > 100 ? l.Substring(0, 100) + "…" : l));
            }

            File.WriteAllLines(destino, lineas, new UTF8Encoding(false));
            return total;
        }

        /// <summary>
        /// Para Importar: deja en %TEMP% una copia sin las tablas que no se deben tocar
        /// (`usuario` y los perfiles si <paramref name="sinUsuarios"/>, y siempre el historial de novedades) y
        /// devuelve su ruta (hay que borrarla al acabar). Si el archivo no trae ninguna,
        /// devuelve null y se importa el original tal cual.
        /// </summary>
        public static string CopiaTemporal(string ruta, bool sinUsuarios)
        {
            var tablas = new List<string>(SIEMPRE);
            var conservar = new List<string>();
            if (sinUsuarios) { tablas.Add("usuario"); conservar.AddRange(PERFILES); }
            tablas.RemoveAll(t => !Trae(ruta, t));
            conservar.RemoveAll(t => !Trae(ruta, t));
            if (tablas.Count == 0 && conservar.Count == 0) return null;

            string tmp = Path.Combine(Path.GetTempPath(),
                "arquitectSoft_" + Path.GetFileNameWithoutExtension(ruta) + "_import_" +
                DateTime.Now.ToString("HHmmss") + ".sql");
            QuitarTablas(ruta, tmp, tablas.ToArray(), conservar.ToArray());
            return tmp;
        }

        /// <summary>Ruta hermana "&lt;nombre&gt;_sin_usuarios.sql".</summary>
        public static string RutaHermana(string ruta)
        {
            return Path.Combine(Path.GetDirectoryName(ruta),
                Path.GetFileNameWithoutExtension(ruta) + "_sin_usuarios.sql");
        }
    }
}
