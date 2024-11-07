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

            Controls.OfType<MdiClient>().FirstOrDefault().BackColor = Color.FromArgb(176, 196, 222);
            Mdi_nameConnect2.Text = Mdi_nameConnect2.Text + " " + Generals.Global.NameConnect.ToUpper().Split('-')[1];
            Mdi_nameConnect2.ForeColor = Color.White;
            this.BackgroundImage = Properties.Resources.Wallpaper_final;
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

            View.FrmComponente formComp = new View.FrmComponente();
            formComp.MdiParent = this;
            formComp.StartPosition = FormStartPosition.CenterScreen;
            
    
            
            formComp.Show();


        }

        private void acercaDeToolStripMenuItem_Click(object sender, EventArgs e)
        {

            FrmAcercade formAbout = new FrmAcercade();
            formAbout.MdiParent = this;
            formAbout.StartPosition = FormStartPosition.CenterScreen;
            formAbout.Show();

        }

        private void TMSItem_subComponente_Click(object sender, EventArgs e)
        {

            View.FrmSubComponente formSubcomp = new View.FrmSubComponente();
            formSubcomp.MdiParent = this;
            formSubcomp.StartPosition = FormStartPosition.CenterScreen;
            formSubcomp.Show();

        }

        private void TMSItem_acabados_Click(object sender, EventArgs e)
        {

            View.FrmAcabados formAbout = new View.FrmAcabados();
            formAbout.MdiParent = this;
            formAbout.StartPosition = FormStartPosition.CenterScreen;
            formAbout.Show();

        }

        private void calcularCantidadesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            View.FrmAnalisisDatos formDataAnalitics = new View.FrmAnalisisDatos();
            formDataAnalitics.MdiParent = this;
            formDataAnalitics.StartPosition = FormStartPosition.CenterScreen;
            formDataAnalitics.Show();

        }

        private void TMSItem_cortes_Click(object sender, EventArgs e)
        {
            View.FrmCorte formcortes = new View.FrmCorte();
            formcortes.MdiParent = this;
            formcortes.StartPosition = FormStartPosition.CenterScreen;
            formcortes.Show();
        }

        private void unidadDeMedidaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            View.FrmUnidadMedida formUnidadMedida = new View.FrmUnidadMedida();
            formUnidadMedida.MdiParent = this;
            formUnidadMedida.StartPosition = FormStartPosition.CenterScreen;
            formUnidadMedida.Show();
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
            View.FrmMecanizado formMecanizado = new View.FrmMecanizado();
            formMecanizado.MdiParent = this;
            formMecanizado.StartPosition = FormStartPosition.CenterScreen;
            formMecanizado.Show();
        }

        private void analisisDePuertasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            View.FrmAnalisisDatos_Puertas formDataAnalitics = new View.FrmAnalisisDatos_Puertas();
            formDataAnalitics.MdiParent = this;
            formDataAnalitics.StartPosition = FormStartPosition.CenterScreen;
            formDataAnalitics.Show();
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
            FrmDBA formDba = new FrmDBA();
            formDba.MdiParent = this;
            formDba.StartPosition = FormStartPosition.CenterScreen;
            formDba.Show();
        }

        private void importDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Generals.Conexion con = new Generals.Conexion();
            DialogResult dr = this.openFileDialog1.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                foreach (String file in openFileDialog1.FileNames)
                {
                    FileInfo Archivo = new FileInfo(file);
                    con.ImportBackupMysql(Archivo.FullName);

                }
            }
        }
    }
}
