using arquitectSoft.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace arquitectSoft
{
    public partial class FrmMDIPrincipal : Form
    {

        private int childFormNumber = 0;

        public FrmMDIPrincipal()
        {
            InitializeComponent();
        }



        private void ShowNewForm(object sender, EventArgs e)
        {
            Form childForm = new Form();
            childForm.MdiParent = this;
            childForm.Text = "Ventana " + childFormNumber++;
            childForm.Show();
        }


        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        private void FrmMDIPrincipal_Load(object sender, EventArgs e)
        {

            // Fondo del área de trabajo (lo que se ve tras el cristal de las ventanas WPF):
            // negro neutro + imagen de fondo estirada. Antes era azul acero (176,196,222),
            // que el material acrílico difuminaba dando un tinte azulado.
            // Fondo del área de trabajo: negro neutro + imagen de fondo (lo que se ve
            // tras el cristal de las ventanas WPF). Antes era azul acero (176,196,222),
            // que el material acrílico difuminaba dando un tinte azulado.
            var mdiClient = Controls.OfType<MdiClient>().FirstOrDefault();
            if (mdiClient != null)
            {
                mdiClient.BackColor = Color.Black;
                string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FondoApp.png");
                if (File.Exists(ruta))
                {
                    try
                    {
                        using (var fs = new FileStream(ruta, FileMode.Open, FileAccess.Read))
                            mdiClient.BackgroundImage = Image.FromStream(fs);
                        mdiClient.BackgroundImageLayout = ImageLayout.Stretch;   // cubre toda la pantalla
                    }
                    catch { /* imagen corrupta: se queda en negro */ }
                }
            }

            Mdi_nameConnect2.Text = Mdi_nameConnect2.Text + " " + Generals.Global.NameConnect.ToUpper().Split('-')[1];
            Mdi_nameConnect2.ForeColor = Color.White;
            this.BackColor = Color.Black;
            this.BackgroundImage = null;
            pictureBoxMdiPrincipal.Visible = false;   // logo inferior derecho oculto
            this.DoubleBuffered = true;

            if(Generals.Global.NameConnect.ToUpper().Split('-')[0] == "DBA")
            {
                dBAToolStripMenuItem.Visible = true;
            }




        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }

            this.Close();
            FrmLogin fl = new FrmLogin();            
            fl.Show();
        }

        private void TMSItem_componente_Click(object sender, EventArgs e)
        {

            new View.Wpf.ComponenteWindow().Show();


        }

        private void acercaDeToolStripMenuItem_Click(object sender, EventArgs e)
        {

            new View.Wpf.AcercaWindow().Show();

        }

        private void TMSItem_subComponente_Click(object sender, EventArgs e)
        {

            new View.Wpf.SubComponenteWindow().Show();

        }

        private void TMSItem_acabados_Click(object sender, EventArgs e)
        {
            // Versión WPF (tema cristal). Ventana de nivel superior, no hija MDI.
            new View.Wpf.AcabadosWindow().Show();
        }

        private void calcularCantidadesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            View.FrmAnalisisDatos formDataAnalitics = new View.FrmAnalisisDatos();
            formDataAnalitics.MdiParent = this;
            formDataAnalitics.StartPosition = FormStartPosition.CenterScreen;
            formDataAnalitics.Show();

        }

        // Versión WPF de "Análisis de Mamparas". Una Window de WPF no puede ser
        // hijo MDI de un formulario WinForms, así que se abre como ventana propia
        // de nivel superior. Comparte el mismo hilo/bucle de mensajes de la UI.
        private void calcularCantidadesWpfToolStripMenuItem_Click(object sender, EventArgs e)
        {
            View.Wpf.AnalisisWindow ventana = new View.Wpf.AnalisisWindow();
            ventana.Show();
        }

        private void TMSItem_cortes_Click(object sender, EventArgs e)
        {
            new View.Wpf.CorteWindow().Show();
        }

        private void unidadDeMedidaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new View.Wpf.UnidadMedidaWindow().Show();
        }

        private void minimizarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void FrmMDIPrincipal_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }

            
            FrmLogin fl = new FrmLogin();
            fl.Show();
        }

        #region Administracion
            private void TMSItem_Administracion_MouseEnter(object sender, EventArgs e)
            {
                ((ToolStripMenuItem)sender).ForeColor = Color.Black; //new color
            }

            private void TMSItem_Administracion_MouseLeave(object sender, EventArgs e)
            {
                ((ToolStripMenuItem)sender).ForeColor = Color.White; //new color
            }

        #endregion

        #region Procesos
        private void TMSItem_procesos_MouseEnter(object sender, EventArgs e)
            {
                ((ToolStripMenuItem)sender).ForeColor = Color.Black; //new color
            }

            private void TMSItem_procesos_MouseLeave(object sender, EventArgs e)
            {
                ((ToolStripMenuItem)sender).ForeColor = Color.White; //new color
            }

    

        #endregion

        #region Acercade
            private void acercaDeToolStripMenuItem_MouseEnter(object sender, EventArgs e)
            {
                ((ToolStripMenuItem)sender).ForeColor = Color.Black; //new color
            }

            private void acercaDeToolStripMenuItem_MouseLeave(object sender, EventArgs e)
            {
                ((ToolStripMenuItem)sender).ForeColor = Color.White; //new color
            }






        #endregion



        private void mecanizadoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new View.Wpf.MecanizadoWindow().Show();
        }

        private void analisisDePuertasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new View.Wpf.PuertasWindow().Show();
        }

        private void dBAToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            ((ToolStripMenuItem)sender).ForeColor = Color.Black; //new color
        }

        private void dBAToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            ((ToolStripMenuItem)sender).ForeColor = Color.White; //new color
        }

        private void exportDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new View.Wpf.DbaBackupWindow().Show();
        }

        private void importDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new View.Wpf.DbaImportWindow().Show();
        }
    }
}
