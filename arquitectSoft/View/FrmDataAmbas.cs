using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace arquitectSoft.View
{
    public partial class FrmDataAmbas : Form
    {
        public FrmDataAmbas()
        {
            InitializeComponent();
        }

        public decimal ReturnItem0 { get; set; }
        public bool ReturnItem1 { get; set; }

        private void BtnAceptar_Click(object sender, EventArgs e)
        {
            ReturnItem0 = NudCantidadA.Value;
            ReturnItem1 = ChkADecreAnch.Checked;

            this.Close();
        }

        private void FrmDataAmbas_Load(object sender, EventArgs e)
        {
            NudCantidadA.Value = ReturnItem0;
            ChkADecreAnch.Checked = ReturnItem1;
        }
    }
}
