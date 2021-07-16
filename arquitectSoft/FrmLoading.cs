using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace arquitectSoft
{
    partial class FrmLoading : Form
    {
        public string Fecha { get; set; }
        public string Numero { get; set; }
        public string Nombre { get; set; }
        public string Tecnico { get; set; }
        public string Acabado1 { get; set; }
        public string Acabado2 { get; set; }

        public FrmLoading()
        {
            InitializeComponent();
            
            
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

        private void FrmLoading_Load(object sender, EventArgs e)
        {
            Dto.AcabadoDto Acb = new Dto.AcabadoDto();
            CmbAcabado1.DataSource = Acb.GetAcabado();
            CmbAcabado1.DisplayMember = "Descripcion";
            CmbAcabado1.ValueMember = "Id_Acabado";

            CmbAcabado2.DataSource = Acb.GetAcabado();
            CmbAcabado2.DisplayMember = "Descripcion";
            CmbAcabado2.ValueMember = "Id_Acabado";
        }

        private void BtnAceptar_Click(object sender, EventArgs e)
        {
            Fecha = datetimePFecha.Value.ToString("yyyy-MM-dd");
            Numero = txtnumero.Text;
            Nombre = txtNombre.Text;
            Tecnico = txtTecnico.Text;
            Acabado1 = CmbAcabado1.Text;
            Acabado2 = CmbAcabado2.Text;
            
            this.Close();
        }

        private void btncancelar_Click(object sender, EventArgs e)
        {
            Fecha = null;
            Numero = null;
            Nombre = null;
            Tecnico = null;
            Acabado1 = null;
            Acabado2 = null;
            this.Close();
        }
    }
}
