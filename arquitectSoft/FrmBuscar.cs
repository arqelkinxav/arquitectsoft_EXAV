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
    public partial class FrmBuscar : Form
    {
        public string Consulta { get; set; }
        public string ReturnItem0 { get; set; }
        public string ReturnItem1{ get; set; }
        public string ReturnItem2 { get; set; }
        public string ReturnItem3 { get; set; }
        public string ReturnItem4 { get; set; }
        public FrmBuscar()
        {
            InitializeComponent();
        }

        private void FrmBuscar_Load(object sender, EventArgs e)
        {
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";

            try
            {

                string sqlquery = "";
                switch (Consulta)
                {
                    case "SubComp":
                        sqlquery = Generals.Constantes.QUERY_SUBCOMPONENTES;
                        break;
                    default:
                        sqlquery = Generals.Constantes.QUERY_COMPONENTES;
                        break;
                }

                con.Open(out fail);             
                GridViewBusqueda.AutoGenerateColumns = true;
                GridViewBusqueda.DataSource = con.ExecuteDataSet(sqlquery, out fail).Tables[0];
                con.Close();

                switch (Consulta)
                {
                    case "SubComp":
                        GridViewBusqueda.Columns[3].Visible = false;
                        GridViewBusqueda.Columns[4].Visible = false;
                        break;
                    default:
                        GridViewBusqueda.Columns[3].Visible = false;
                        break;
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                con.Close();
            }

        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            Buscar();
        }

        private void BtnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

           private void GridViewBusqueda_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {

            switch (Consulta)
            {
                case "SubComp":
                    ReturnItem0 = GridViewBusqueda.SelectedCells[0].Value.ToString();
                    ReturnItem1 = GridViewBusqueda.SelectedCells[1].Value.ToString();
                    ReturnItem2 = GridViewBusqueda.SelectedCells[2].Value.ToString();
                    ReturnItem3 = GridViewBusqueda.SelectedCells[3].Value.ToString();
                    ReturnItem4 = GridViewBusqueda.SelectedCells[4].Value.ToString();
                    break;
                default:
                    ReturnItem1 = GridViewBusqueda.SelectedCells[1].Value.ToString();
                    ReturnItem2 = GridViewBusqueda.SelectedCells[2].Value.ToString();
                    ReturnItem3 = GridViewBusqueda.SelectedCells[3].Value.ToString();
                    ReturnItem4 = GridViewBusqueda.SelectedCells[0].Value.ToString();
                    break;
            }

           
            this.Close();
        }

        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Buscar();
            }

   
        }

        private void Buscar()
        {
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";

            try
            {
                con.Open(out fail);
                string sqlquery = "";
                switch (Consulta)
                {
                    case "SubComp":
                        sqlquery = Generals.Constantes.QUERY_SUBCOMPONENTES + " where CONCAT(Codigo_Homologacion,' - ',Descripcion) lIKE '%" + txtBuscar.Text + "%'";
                        break;
                    default:
                        sqlquery = Generals.Constantes.QUERY_COMPONENTES + " where CONCAT(Codigo,' - ',Descripcion) lIKE '%" + txtBuscar.Text + "%'";
                        break;
                }


                GridViewBusqueda.AutoGenerateColumns = true;
                GridViewBusqueda.DataSource = con.ExecuteDataSet(sqlquery, out fail).Tables[0];

                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                con.Close();
            }
        }
    
    }
}
