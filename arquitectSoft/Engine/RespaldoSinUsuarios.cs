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
    /// </summary>
    public static class RespaldoSinUsuarios
    {
        // El bloque va desde "-- Definition of usuario" hasta el siguiente encabezado de
        // sección. "Dumping data for table usuario" queda DENTRO (no corta).
        private static readonly Regex INICIO = new Regex(@"^--\s*Definition of usuario\s*$");
        private static readonly Regex FIN = new Regex(@"^--\s*(Dumping functions|Dumping procedures|Definition of )");
        private static readonly Regex RESTOS = new Regex(
            @"(DROP TABLE IF EXISTS|CREATE TABLE|INSERT INTO)\s+`?usuario`?[\s(]", RegexOptions.IgnoreCase);

        /// <summary>¿El respaldo trae la tabla `usuario`?</summary>
        public static bool TraeUsuarios(string ruta)
        {
            foreach (string l in File.ReadLines(ruta, Encoding.UTF8))
                if (INICIO.IsMatch(l)) return true;
            return false;
        }

        /// <summary>
        /// Escribe en <paramref name="destino"/> el respaldo sin la tabla `usuario` (UTF-8 SIN
        /// BOM: el BOM se cuela delante de la primera sentencia y puede tumbar el import).
        /// Devuelve las líneas retiradas. Lanza si el resultado sigue tocando `usuario`.
        /// </summary>
        public static int Quitar(string origen, string destino)
        {
            string[] lineas = File.ReadAllLines(origen, Encoding.UTF8);

            int ini = -1, fin = -1;
            for (int i = 0; i < lineas.Length; i++)
            {
                if (ini < 0) { if (INICIO.IsMatch(lineas[i])) ini = i; continue; }
                if (FIN.IsMatch(lineas[i])) { fin = i; break; }
            }
            if (ini < 0) throw new InvalidOperationException("El respaldo no trae la tabla usuario.");
            if (fin < 0) fin = lineas.Length;

            // El comentario abre con un "--" suelto encima; se recorta también.
            int desde = ini;
            if (desde > 0 && lineas[desde - 1].Trim() == "--") desde--;

            var salida = new List<string>(lineas.Length);
            for (int i = 0; i < desde; i++) salida.Add(lineas[i]);
            salida.Add("-- [arquitectSoft] Bloque de la tabla `usuario` retirado a proposito:");
            salida.Add("-- en la beta manda la tabla de usuarios de la beta, no la de Olimpo.");
            salida.Add("");
            for (int i = fin; i < lineas.Length; i++) salida.Add(lineas[i]);

            foreach (string l in salida)
                if (RESTOS.IsMatch(l))
                    throw new InvalidOperationException(
                        "Después de quitar la tabla usuario siguen apareciendo sentencias sobre ella:\n" +
                        (l.Length > 100 ? l.Substring(0, 100) + "…" : l));

            File.WriteAllLines(destino, salida, new UTF8Encoding(false));
            return fin - desde;
        }

        /// <summary>
        /// Para Importar: si el respaldo trae `usuario`, deja una copia sin ella en %TEMP% y
        /// devuelve su ruta (hay que borrarla al acabar). Si no la trae, devuelve null y se
        /// importa el original tal cual.
        /// </summary>
        public static string CopiaTemporal(string ruta)
        {
            if (!TraeUsuarios(ruta)) return null;
            string tmp = Path.Combine(Path.GetTempPath(),
                "arquitectSoft_" + Path.GetFileNameWithoutExtension(ruta) + "_sin_usuarios_" +
                DateTime.Now.ToString("HHmmss") + ".sql");
            Quitar(ruta, tmp);
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
