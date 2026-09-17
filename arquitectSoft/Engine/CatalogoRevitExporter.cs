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

        // Det / DetEsp = cuántas piezas tiene el componente en su despiece normal (perfiles,
        // herrajes) y en el especial (vidrios y paneles). Un código que existe pero no trae
        // nada ahí no da error al analizar: sencillamente no saca piezas. La auditoría lo
        // avisa aparte de "no existe".
        private const string SQL =
            "SELECT c.Codigo, c.Descripcion, a.Codigo_Homologacion, c.Especial, " +
            "(SELECT COUNT(*) FROM componentes_detalle d WHERE d.Id_componente = c.Id_Componente) Det, " +
            "(SELECT COUNT(*) FROM componentes_especial_detalle e WHERE e.Id_Componente_especial = c.Id_Componente) DetEsp " +
            "FROM componentes c " +
            "LEFT JOIN acabados a ON a.Id_Acabado = c.AcabadoPrincipal " +
            "ORDER BY c.Codigo";

        // Sistemas de la pantalla Vidrios, con cuántas sustituciones tienen. Tabla beta_: en
        // una base sin la migración 005 no existe, y eso no debe impedir exportar los códigos.
        private const string SQL_SISTEMAS =
            "SELECT s.Prefijo, IFNULL(s.Descripcion,'') Descripcion, " +
            "(SELECT COUNT(*) FROM beta_vidrio_regla r WHERE r.Id_Sistema = s.Id) Reglas " +
            "FROM beta_vidrio_sistema s ORDER BY s.Prefijo";

        /// <summary>
        /// Exporta a la ruta de siempre sin enseñar nada. Lo llaman el arranque y las pantallas
        /// que cambian códigos o sistemas, para que la auditoría de Revit compare siempre contra
        /// la base al día sin que nadie tenga que acordarse del botón. Un fallo aquí no puede
        /// molestar a quien está guardando un componente: se traga y ya.
        /// </summary>
        public static void ExportarEnSilencio()
        {
            // Dos guardados seguidos lanzan dos exportaciones: sin el candado las dos
            // escribirían el mismo .tmp a la vez.
            lock (Candado)
            {
                try { string fail; Exportar(RutaPorDefecto, out fail); }
                catch { }
            }
        }

        private static readonly object Candado = new object();

        /// <summary>Lo mismo en segundo plano, para no congelar la pantalla.</summary>
        public static void ExportarEnSegundoPlano()
        {
            // Solo en las sesiones que tienen el botón Cód. Revit (el administrador y los
            // perfiles a los que se lo dé): en los demás equipos no se escribe nada.
            if (!Generals.BotonesPerfil.VisibleEnSesion("CodRevit")) return;
            System.Threading.Tasks.Task.Run(() => ExportarEnSilencio());
        }

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
            DataSet dsSistemas = null;
            try
            {
                ds = cn.ExecuteDataSet(SQL, out fail);
                string failSistemas;
                try { dsSistemas = cn.ExecuteDataSet(SQL_SISTEMAS, out failSistemas); } catch { dsSistemas = null; }
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
                    w.WriteAttributeString("Version", "2");

                    // Sin la tabla de sistemas no se escribe el atributo: así el add-in distingue
                    // "no hay ningún sistema dado de alta" de "este catálogo no los trae".
                    DataTable dtSis = (dsSistemas != null && dsSistemas.Tables.Count > 0) ? dsSistemas.Tables[0] : null;
                    if (dtSis != null) w.WriteAttributeString("ConSistemas", "1");

                    foreach (DataRow r in dt.Rows)
                    {
                        string cod = Texto(r, "Codigo");
                        if (cod.Length == 0) continue;

                        string acab = Texto(r, "Codigo_Homologacion");

                        w.WriteStartElement("C");
                        w.WriteAttributeString("Cod", cod);
                        if (acab.Length > 0) w.WriteAttributeString("CodHom", cod + "-" + acab);
                        w.WriteAttributeString("Desc", Texto(r, "Descripcion"));
                        w.WriteAttributeString("Esp", Texto(r, "Especial") == "1" ? "1" : "0");
                        w.WriteAttributeString("Det", Numero(r, "Det"));
                        w.WriteAttributeString("DetEsp", Numero(r, "DetEsp"));
                        w.WriteEndElement();
                        total++;
                    }

                    if (dtSis != null)
                    {
                        foreach (DataRow r in dtSis.Rows)
                        {
                            string prefijo = Texto(r, "Prefijo");
                            if (prefijo.Length == 0) continue;
                            w.WriteStartElement("S");
                            w.WriteAttributeString("Prefijo", prefijo);
                            w.WriteAttributeString("Desc", Texto(r, "Descripcion"));
                            w.WriteAttributeString("Reglas", Numero(r, "Reglas"));
                            w.WriteEndElement();
                        }
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

        private static string Numero(DataRow r, string columna)
        {
            long n;
            return long.TryParse(Texto(r, columna), out n) ? n.ToString(CultureInfo.InvariantCulture) : "0";
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
