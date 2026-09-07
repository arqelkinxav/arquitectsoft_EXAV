using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using arquitectSoft.Generals;

namespace arquitectSoft.Engine
{
    // =========================================================================
    // CATÁLOGO DE CÓDIGOS PARA REVIT (EXAV_Tools)
    //
    // La auditoría del add-in de Revit comprueba que la nota clave de cada tipo
    // de puerta sea un código que EXISTA en este catálogo. Revit no habla con
    // MySQL: meter el conector dentro del proceso de Revit arrastra dependencias
    // (System.Memory, Buffers…) que ya tiene cargadas el propio Revit, y un
    // choque ahí se lleva por delante todo el add-in. Así que quien tiene la
    // base —este programa— escribe la lista en un XML y el add-in solo lee.
    //
    // El archivo se llama EXAV_* y vive en %AppData% a propósito: es donde el
    // add-in busca su configuración y es lo que recoge el respaldo de EXAV, así
    // que el vínculo lo reparte al resto de equipos sin trabajo extra. No entra
    // en el cofre cifrado: si entrara, el add-in leería siempre la copia del
    // cofre y esta exportación se quedaría eternamente vieja.
    //
    // Se exportan los DOS códigos de cada componente porque según por dónde se
    // mire arquitectSoft enseña uno u otro: el crudo de la tabla (ITS0501) y el
    // homologado con el acabado principal pegado (ITS0501-BL), que es el que
    // arma QUERY_COMPONENTES. El add-in acepta cualquiera de los dos.
    // =========================================================================
    public static class CatalogoRevitExporter
    {
        public const string NOMBRE_ARCHIVO = "EXAV_CatalogoCodigos_V1.xml";

        /// <summary>%AppData%\EXAV_CatalogoCodigos_V1.xml — donde lo busca el add-in.</summary>
        public static string RutaPorDefecto
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    NOMBRE_ARCHIVO);
            }
        }

        private const string SQL =
            "SELECT c.Codigo, c.Descripcion, a.Codigo_Homologacion " +
            "FROM componentes c " +
            "LEFT JOIN acabados a ON a.Id_Acabado = c.AcabadoPrincipal " +
            "ORDER BY c.Codigo";

        /// <summary>
        /// Vuelca los códigos del catálogo al XML. Devuelve cuántos componentes se
        /// escribieron, o -1 si algo falló (el motivo va en <paramref name="fail"/>).
        /// </summary>
        public static int Exportar(string ruta, out string fail)
        {
            fail = "";
            if (string.IsNullOrWhiteSpace(ruta)) ruta = RutaPorDefecto;

            Conexion cn = new Conexion();
            if (!cn.Open(out fail)) return -1;

            DataSet ds;
            try
            {
                ds = cn.ExecuteDataSet(SQL, out fail);
            }
            finally
            {
                try { cn.Close(); } catch { }
            }

            if (ds == null || ds.Tables.Count == 0)
            {
                if (string.IsNullOrEmpty(fail)) fail = "La consulta no devolvió ninguna tabla.";
                return -1;
            }

            DataTable dt = ds.Tables[0];
            int total = 0;

            try
            {
                // Se escribe en un temporal y se mueve al final: si el programa muere a
                // medias, el add-in no se queda con medio catálogo (que le haría cantar
                // como inexistentes códigos que sí están).
                string tmp = ruta + ".tmp";

                var ajustes = new XmlWriterSettings
                {
                    Indent = true,
                    Encoding = new UTF8Encoding(false)
                };

                using (XmlWriter w = XmlWriter.Create(tmp, ajustes))
                {
                    w.WriteStartDocument();
                    w.WriteStartElement("CatalogoCodigos");
                    w.WriteAttributeString("Generado", DateTime.Now.ToString("s", CultureInfo.InvariantCulture));
                    w.WriteAttributeString("Origen", Conexion.Destino);

                    foreach (DataRow r in dt.Rows)
                    {
                        string cod = Texto(r, "Codigo");
                        if (cod.Length == 0) continue;

                        string acab = Texto(r, "Codigo_Homologacion");

                        w.WriteStartElement("C");
                        w.WriteAttributeString("Cod", cod);
                        if (acab.Length > 0) w.WriteAttributeString("CodHom", cod + "-" + acab);
                        w.WriteAttributeString("Desc", Texto(r, "Descripcion"));
                        w.WriteEndElement();
                        total++;
                    }

                    w.WriteEndElement();
                    w.WriteEndDocument();
                }

                if (File.Exists(ruta)) File.Delete(ruta);
                File.Move(tmp, ruta);
            }
            catch (Exception ex)
            {
                fail = "No se pudo escribir el catálogo: " + ex.Message;
                return -1;
            }

            return total;
        }

        private static string Texto(DataRow r, string columna)
        {
            try
            {
                if (!r.Table.Columns.Contains(columna)) return "";
                object v = r[columna];
                return (v == null || v == DBNull.Value) ? "" : v.ToString().Trim();
            }
            catch { return ""; }
        }
    }
}
