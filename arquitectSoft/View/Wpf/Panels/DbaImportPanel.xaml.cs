using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using arquitectSoft.Engine;

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

        private async void Importar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtPath.Text))
            {
                GlassDialog.Informar(Owner, "Importar", "Debes seleccionar un archivo .sql.");
                return;
            }
            if (!File.Exists(TxtPath.Text))
            {
                GlassDialog.Informar(Owner, "Importar", "El archivo ya no esta en esa ruta.");
                return;
            }

            // 1. REVISION. El import reemplaza tabla por tabla, asi que lo que haya aqui y no
            //    venga en el archivo se pierde. Antes de tocar nada se compara y se enseña.
            string ruta = TxtPath.Text;
            InformeImport informe;

            BtnImportar.IsEnabled = false;
            LblFecha.Text = "Revisando el respaldo…";
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                informe = await Task.Run(() => RespaldoDiff.Comparar(ruta));
            }
            catch (Exception ex)
            {
                informe = new InformeImport { Error = "No se pudo revisar el respaldo:\n" + ex.Message };
            }
            finally
            {
                Mouse.OverrideCursor = null;
                BtnImportar.IsEnabled = true;
                CargarUltima();
            }

            var revision = new RevisionImportDialog { Owner = Owner };
            revision.Cargar(informe, Path.GetFileName(ruta));
            if (revision.ShowDialog() != true) return;

            // 2. IMPORT. Ojo: ImportBackupMysql NO lanza, devuelve el fallo en el retorno.
            //    Si se ignora, un import a medias pasa por bueno.
            try
            {
                LblFecha.Text = "Importando…";
                var con = new Generals.Conexion();
                string error = con.ImportBackupMysql(ruta);

                if (!string.IsNullOrEmpty(error))
                {
                    CargarUltima();
                    GlassDialog.Informar(Owner, "Importar",
                        "El import FALLO y la base puede haber quedado a medias:\n\n" + error +
                        "\n\nRevisa la base antes de seguir trabajando.");
                    return;
                }

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
                CargarUltima();
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
