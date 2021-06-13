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
    public partial class FrmComponenteBorrar : Form
    {

        public string SetItem0 { get; set; }
        public string SetItem1 { get; set; }
        public string SetItem2 { get; set; }
        public string SetItem3 { get; set; }
        public string ReturnItem1 { get; set; }
        public string ReturnItem2 { get; set; }
        public string SetValidation { get; set; }
        public string SetValidationEsp { get; set; }


        public FrmComponenteBorrar()
        {
            InitializeComponent();
        }

        private void FrmComponenteBorrar_Load(object sender, EventArgs e)
        {
            TxtIndexComp.Text = SetItem0;
            TxtIndexCompEsp.Text = SetItem2;

            TxtDescripcion.Text = SetItem1;
            TxtDescripcionEsp.Text = SetItem3;

            GroupComp.Visible = SetValidation == "true" ? true : false;
            GroupEspecial.Visible = SetValidationEsp == "true" ? true : false;
        }

        private void BtnBorrar_Click(object sender, EventArgs e)
        {
            ReturnItem1 = ChkCompOriginal.Checked == true ? TxtIndexComp.Text : "";
            ReturnItem2 = ChkCompEspecial.Checked == true ? TxtIndexCompEsp.Text : "";

            this.Close();
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            ReturnItem1 = "";
            ReturnItem2 = "";
            this.Close();
        }
    }
}
