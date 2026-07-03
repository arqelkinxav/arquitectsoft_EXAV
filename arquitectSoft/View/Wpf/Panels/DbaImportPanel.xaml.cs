using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace arquitectSoft.View.Wpf.Panels
{
    /// <summary>
    /// Versión "panel" de DBA Importar para hospedarse dentro del escritorio (MdiChild):
    /// importa un .sql a la base y registra la importación en dbmanagments. Sin chrome ni
    /// liquid glass: lo aporta la ventana hija. Reutiliza Generals.Conexion.ImportBackupMysql.
    /// </summary>
    public partial class DbaImportPanel : UserControl
    {
        private string _fileName = "";

        public DbaImportPanel()
        {
            InitializeComponent();
            Loaded += (s, e) => CargarUltima();
        }

        private Window Owner { get { return Window.GetWindow(this); } }

        private void Examinar_Click(object sender, RoutedEventArgs e)
        {
            using (var dlg = new System.Windows.Forms.OpenFileDialog())
            {
                dlg.Filter = "Respaldos SQL (*.sql)|*.sql|Todos los archivos (*.*)|*.*";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var fi = new FileInfo(dlg.FileName);
                    TxtPath.Text = fi.FullName;
                    _fileName = fi.Name.Replace(fi.Extension, "");
                }
            }
        }

        private void Importar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtPath.Text))
            {
                GlassDialog.Informar(Owner, "Importar", "Debes seleccionar un archivo .sql.");
                return;
            }

            if (!GlassDialog.Pregunta(Owner, "Importar",
                "Esto REEMPLAZA el contenido actual de la base con el del archivo.\n¿Continuar?")) return;

            try
            {
                LblFecha.Text = "Importando…";
                var con = new Generals.Conexion();
                con.ImportBackupMysql(TxtPath.Text);

                // Registra la importación en dbmanagments.
                string fail = "";
                string[] param = { _fileName };
                con.Open(out fail);
                con.ExecuteReader(Generals.Constantes.QUERY_INSERT_dbmanagmet, out fail, param);
                con.Close();

                CargarUltima();
                GlassDialog.Informar(Owner, "Importar", "Archivo cargado correctamente.");
            }
            catch (Exception ex)
            {
                GlassDialog.Informar(Owner, "Importar", "No se pudo importar:\n" + ex.Message);
            }
        }

        private void CargarUltima()
        {
            try
            {
                var con = new Generals.Conexion();
                string fail = "";
                con.Open(out fail);
                DataTable dt = con.ExecuteDataSet(
                    "SELECT filename,created_at FROM `dbmanagments` ORDER BY `created_at` DESC LIMIT 1;", out fail).Tables[0];
                con.Close();
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    LblUltimo.Text = "Archivo cargado: " + Convert.ToString(row["filename"]);
                    LblFecha.Text = "Última actualización local: " + Convert.ToString(row["created_at"]);
                }
            }
            catch { /* tabla aún sin datos */ }
        }
    }
}
