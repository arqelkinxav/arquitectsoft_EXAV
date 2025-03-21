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
        public string Albaran { get; set; }

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
            ChkListAlbaran.SetItemChecked(0, true);
            ChkListAlbaran.SetItemChecked(1, true);
            ChkListAlbaran.SetItemChecked(2, true);

            if(Generals.Global.AnalisisType == "2")
            {
                ChkListAlbaran.Visible = false;
                lblalbaran.Visible=false;
            }

        }

        private void BtnAceptar_Click(object sender, EventArgs e)
        {
            Fecha = datetimePFecha.Value.ToString("yyyy-MM-dd");

            string fail = "";
            if (txtnumero.Text == "")
            {
                fail = "Debe Digitar un Numero de Proyecto";
            }
            else
            {
                Numero = txtnumero.Text;
            }

            if (txtNombre.Text == "")
            {
                fail = "Debe Digitar un Nombre de Proyecto";
            }
            else
            {
                Nombre = txtNombre.Text;
            }

            
            Tecnico = txtTecnico.Text;
            Acabado1 = txtAcabado1.Text;
            Acabado2 = txtAcabado2.Text;

            Albaran = "";
            foreach (int indexChecked in ChkListAlbaran.CheckedIndices)
            {
                // The indexChecked variable contains the index of the item.
                if (Albaran != "")
                {
                    Albaran += "|" + indexChecked.ToString();
                }
                else
                {
                    Albaran = indexChecked.ToString();
                }
            }

            if (fail != "")
            {
                MessageBox.Show(fail, "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                this.Close();
            }
            
            
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            Fecha = null;
            Numero = null;
            Nombre = null;
            Tecnico = null;
            Acabado1 = null;
            Acabado2 = null;
            this.Close();
        }

        private void btnacabadoP_Click(object sender, EventArgs e)
        {
            FrmBuscar bsc = new FrmBuscar();
            bsc.Consulta = "Acaba";
            bsc.ShowDialog();
            if (bsc.ReturnItem1 == null)
            {
                return;
            }

            txtAcabado1.Text = bsc.ReturnItem2;
        }

        private void btnacabadoM_Click(object sender, EventArgs e)
        {
            FrmBuscar bsc = new FrmBuscar();
            bsc.Consulta = "Acaba";
            bsc.ShowDialog();
            if (bsc.ReturnItem1 == null)
            {
                return;
            }

            txtAcabado2.Text = bsc.ReturnItem2;
        }
    }
}
