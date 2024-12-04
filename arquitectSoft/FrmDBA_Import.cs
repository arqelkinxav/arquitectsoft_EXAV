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
using System.Data;

namespace arquitectSoft
{
    public partial class FrmDBA_Import : Form
    {
        public FrmDBA_Import()
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
                MessageBox.Show("Debe Seleccionar un archivo", "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            string FileBackup = txtPath.Text;

            Generals.Conexion con = new Generals.Conexion();
            con.ImportBackupMysql(FileBackup);

            
            string fail = "";
            string[] param = { lblfilename.Text};
            con.Open(out fail);
            MySqlDataReader drResult = con.ExecuteReader(Generals.Constantes.QUERY_INSERT_dbmanagmet, out fail, param);
            con.Close();

            GetLastImportData();
            MessageBox.Show("Archivo Cargado Correctamente", "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void btnExaminar_Click(object sender, EventArgs e)
        {
            DialogResult dr = this.openFileDialog1.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                foreach (String file in openFileDialog1.FileNames)
                {
                    FileInfo Archivo = new FileInfo(file);
                    txtPath.Text = Archivo.FullName;
                    lblfilename.Text = Archivo.Name.ToString().Replace(Archivo.Extension, "");
                }
            }
        }

        private void FrmDBA_Load(object sender, EventArgs e)
        {
            GetLastImportData(); 
        }

        public void GetLastImportData()
        {
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            con.Open(out fail);
            
            DataTable dt = con.ExecuteDataSet("SELECT filename,created_at FROM `dbmanagments` ORDER BY `created_at` DESC LIMIT 1;", out fail).Tables[0];
            con.Close();

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                labelFilename.Text = "Archivo Cargado: " + (string)row["filename"];
                lblcurrentdate.Text = "Fecha Local Actualizacón: " +  row["created_at"].ToString();
            }
   
        }
    }
}
