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
    public partial class FrmChange : Form
    {
        public string Acabado1 { get; set; }
        public string Acabado2 { get; set; }

        public FrmChange()
        {
            InitializeComponent();
        }

        private void FrmChange_Load(object sender, EventArgs e)
        {
            //Dto.AcabadoDto Acb = new Dto.AcabadoDto();
            //CmbAcabado1.DataSource = Acb.GetAcabado();
            //CmbAcabado1.DisplayMember = "Descripcion";
            //CmbAcabado1.ValueMember = "Id_Acabado";

            //CmbAcabado2.DataSource = Acb.GetAcabado();
            //CmbAcabado2.DisplayMember = "Descripcion";
            //CmbAcabado2.ValueMember = "Id_Acabado";
        }

        private void BtnAceptar_Click(object sender, EventArgs e)
        {
            Acabado1 = txtAcabado1.Text;
            Acabado2 = txtAcabado2.Text;           

            this.Close();

        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            Acabado1 = null;
            Acabado2 = null;
            this.Close();
        }

        private void ChkTempDest_CheckedChanged(object sender, EventArgs e)
        {
            if (ChkTempDest.Checked)
            {
                txtAcabado2.Text = "";
                txtAcabado2.ReadOnly = false;
            }
            else
            {
                txtAcabado2.Text = "";
                txtAcabado2.ReadOnly = true;
            }
        }

        private void ChkTempOrig_CheckedChanged(object sender, EventArgs e)
        {
            if (ChkTempOrig.Checked)
            {
                txtAcabado1.Text = "";
                txtAcabado1.ReadOnly = false;
            }
            else
            {
                txtAcabado1.Text = "";
                txtAcabado1.ReadOnly = true;
            }
        }

        private void btnacabado1_Click(object sender, EventArgs e)
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

        private void btnacabado2_Click(object sender, EventArgs e)
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
