using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace arquitectSoft
{
    public partial class FrmDBA : Form
    {
        public FrmDBA()
        {
            InitializeComponent();
            this.Text = String.Format("Acerca de {0}", AssemblyTitle);
        }

        #region Descriptores de acceso de atributos de ensamblado

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }


        #endregion

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnBackup_Click(object sender, EventArgs e)
        {

            if (txtPath.Text == "")
            {
                MessageBox.Show("Debe Seleccionar una Ruta", "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            string backupFolder = txtPath.Text;
            
            string database = "arquitectdb";
            string fileName = $"{database}_backup_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.sql";
            string backupFilePath = Path.Combine(backupFolder, fileName);
            Generals.Conexion con = new Generals.Conexion();
            string result = con.ExportBackupMysql(backupFilePath);
            MessageBox.Show(result, "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void btnExaminar_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog profilePath = new FolderBrowserDialog();
            if (profilePath.ShowDialog() == DialogResult.OK)
            {
                txtPath.Text = profilePath.SelectedPath;
            }
        }

        private void FrmDBA_Load(object sender, EventArgs e)
        {


        }
    }
}
