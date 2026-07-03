using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace arquitectSoft.View.Wpf.Panels
{
    /// <summary>
    /// Versión "panel" de DBA Respaldo para hospedarse dentro del escritorio (MdiChild):
    /// genera un respaldo .sql de la base en la carpeta elegida. Sin chrome ni liquid glass:
    /// lo aporta la ventana hija. Reutiliza Generals.Conexion.ExportBackupMysql.
    /// </summary>
    public partial class DbaBackupPanel : UserControl
    {
        public DbaBackupPanel()
        {
            InitializeComponent();
        }

        private Window Owner { get { return Window.GetWindow(this); } }

        private void Examinar_Click(object sender, RoutedEventArgs e)
        {
            using (var dlg = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    TxtPath.Text = dlg.SelectedPath;
            }
        }

        private void Backup_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtPath.Text))
            {
                GlassDialog.Informar(Owner, "Respaldo", "Debes seleccionar una carpeta destino.");
                return;
            }

            try
            {
                const string database = "arquitectdb";
                string fileName = database + "_backup_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".sql";
                string backupFilePath = Path.Combine(TxtPath.Text, fileName);

                LblEstado.Text = "Generando respaldo…";
                var con = new Generals.Conexion();
                string result = con.ExportBackupMysql(backupFilePath);

                // Header necesario para reimportar (funciones/triggers).
                string header = "SET GLOBAL log_bin_trust_function_creators = 1;\n";
                string existing = File.ReadAllText(backupFilePath);
                File.WriteAllText(backupFilePath, header + existing);

                LblEstado.Text = "Respaldo creado: " + fileName;
                GlassDialog.Informar(Owner, "Respaldo", result);
            }
            catch (Exception ex)
            {
                LblEstado.Text = "Error al respaldar.";
                GlassDialog.Informar(Owner, "Respaldo", "No se pudo crear el respaldo:\n" + ex.Message);
            }
        }
    }
}
