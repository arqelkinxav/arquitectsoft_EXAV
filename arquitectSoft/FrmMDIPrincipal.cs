using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
            Mdi_nameConnect.Text = Mdi_nameConnect.Text + " " + Generals.Global.NameConnect.ToUpper();
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
            if (this.MdiChildren.Count() == 0){
                View.FrmComponente formComp = new View.FrmComponente();
                formComp.MdiParent = this;
                formComp.StartPosition = FormStartPosition.CenterScreen;
                formComp.Show();
            }
            else{
                MessageBox.Show("Ya hay una ventana activa!","Mensaje Alerta",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
            }
            
        }

        private void acercaDeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.MdiChildren.Count() == 0)
            {
                FrmAbout formAbout = new FrmAbout();
                formAbout.MdiParent = this;
                formAbout.StartPosition = FormStartPosition.CenterScreen;
                formAbout.Show();
            }
            else
            {
                MessageBox.Show("Ya hay una ventana activa!", "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void TMSItem_subComponente_Click(object sender, EventArgs e)
        {
            if (this.MdiChildren.Count() == 0)
            {
                View.FrmSubComponente formAbout = new View.FrmSubComponente();
                formAbout.MdiParent = this;
                formAbout.StartPosition = FormStartPosition.CenterScreen;
                formAbout.Show();
            }
            else
            {
                MessageBox.Show("Ya hay una ventana activa!", "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}
