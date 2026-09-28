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
            string carpeta = SelectorCarpeta.Elegir(Owner, "Carpeta del respaldo", TxtPath.Text);
            if (!string.IsNullOrEmpty(carpeta)) TxtPath.Text = carpeta;
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

                // Copia para la beta: el mismo respaldo sin la tabla usuario, al lado.
                if (ChkSinUsuarios.IsChecked == true)
                {
                    try
                    {
                        string sinUsuarios = Engine.RespaldoSinUsuarios.RutaHermana(backupFilePath);
                        Engine.RespaldoSinUsuarios.Quitar(backupFilePath, sinUsuarios);
                        LblEstado.Text = "Respaldo creado: " + fileName +
                                         "\nPara la beta: " + Path.GetFileName(sinUsuarios);
                        result += "\n\nPara la beta lleva este (no toca los usuarios ni los perfiles de allí):\n" +
                                  Path.GetFileName(sinUsuarios);
                    }
                    catch (Exception ex)
                    {
                        result += "\n\nOJO: el respaldo completo está bien, pero NO se pudo sacar la copia sin usuarios:\n" +
                                  ex.Message;
                    }
                }

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
