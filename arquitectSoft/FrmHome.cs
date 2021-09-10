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
    public partial class FrmHome : Form
    {
        public FrmHome()
        {
            InitializeComponent();
        }

        private void TMSItem_componente_Click(object sender, EventArgs e)
        {
            View.FrmComponente formComp = new View.FrmComponente();
            formComp.StartPosition = FormStartPosition.CenterScreen;
            formComp.Show();
        }
    }
}
