using System;

namespace arquitectSoft.Engine
{
    /// <summary>
    /// Cómo se reconoce la fila de CABECERA de una puerta en las tablas de resultado del
    /// análisis (las que produce AnalisisDatosDto.getComponentePuertas): la que lleva la
    /// nomenclatura en la columna 0, el código en la 1 y la descripción con el acabado entre
    /// paréntesis en la 2. Debajo van sus "Item (…)", y entre puerta y puerta, separadores
    /// con la fila entera vacía.
    ///
    /// Hasta el commit f6478fc la nomenclatura se escribía "Puerta 1, Puerta 2", así que por
    /// todo el programa se preguntaba si la celda contenía la palabra "Puerta". Desde que se
    /// lista tal cual la escribe el usuario (P1, COCINA, ALACENA) esa pregunta no acierta
    /// nunca, y se llevó por delante —en silencio— el color de la fila, el resaltado del
    /// Excel y el cambio de acabado del enunciado, cada uno en su rincón.
    ///
    /// De ahí que la respuesta viva ahora en un único sitio: si vuelve a cambiar la forma de
    /// la tabla, se cambia aquí y no en diecisiete.
    /// </summary>
    public static class FilaPuerta
    {
        /// <summary>
        /// true si esa celda de la columna 0 es la cabecera de una puerta: tiene contenido y
        /// no es una fila de detalle. Las de detalle empiezan por "Item"; los separadores
        /// vienen vacíos.
        /// </summary>
        public static bool EsCabecera(string col0)
        {
            string s = (col0 ?? "").Trim();
            return s != "" && !s.StartsWith("Item", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>Igual, para celdas que llegan como object (DataRow, DataGridViewCell…).</summary>
        public static bool EsCabecera(object col0)
        {
            return EsCabecera(col0 == null || col0 == DBNull.Value ? "" : col0.ToString());
        }
    }
}
