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
            Acabado1 = CmbAcabado1.Text;
            Acabado2 = CmbAcabado2.Text;           

            this.Close();

        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            Acabado1 = null;
            Acabado2 = null;
            this.Close();
        }
    }
}
