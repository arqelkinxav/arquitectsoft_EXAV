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
            bool filter = chkVidriospanles.Checked;
            Buscar(filter);

            switch (Consulta)
            {
                case "Umed":
                    break;
                case "Corte":
                    break;
                case "Acaba":
                    break;
                case "SubComp":
                    GridViewBusqueda.Columns[3].Visible = false;
                    GridViewBusqueda.Columns[4].Visible = false;
                    break;
                default:
                    GridViewBusqueda.Columns[3].Visible = false;
                    break;
            }
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            bool filter = chkVidriospanles.Checked;
            Buscar(filter);
        }

        private void BtnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void GridViewBusqueda_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {

            switch (Consulta)
            {
                case "Umed":
                    ReturnItem0 = GridViewBusqueda.SelectedCells[0].Value.ToString();
                    ReturnItem1 = GridViewBusqueda.SelectedCells[1].Value.ToString();
                    ReturnItem2 = GridViewBusqueda.SelectedCells[2].Value.ToString();
                    break;
                case "Corte":
                    ReturnItem0 = GridViewBusqueda.SelectedCells[0].Value.ToString();
                    ReturnItem1 = GridViewBusqueda.SelectedCells[1].Value.ToString();
                    ReturnItem2 = GridViewBusqueda.SelectedCells[2].Value.ToString();
                    ReturnItem3 = GridViewBusqueda.SelectedCells[3].Value.ToString();
                    break;
                case "Acaba":
                    ReturnItem0 = GridViewBusqueda.SelectedCells[0].Value.ToString();
                    ReturnItem1 = GridViewBusqueda.SelectedCells[1].Value.ToString();
                    ReturnItem2 = GridViewBusqueda.SelectedCells[2].Value.ToString();
                    break;
                case "SubComp":
                    ReturnItem0 = GridViewBusqueda.SelectedCells[0].Value.ToString();
                    ReturnItem1 = GridViewBusqueda.SelectedCells[1].Value.ToString();
                    ReturnItem2 = GridViewBusqueda.SelectedCells[2].Value.ToString();
                    ReturnItem3 = GridViewBusqueda.SelectedCells[3].Value.ToString();
                    ReturnItem4 = GridViewBusqueda.SelectedCells[4].Value.ToString();
                    break;
                default:
                    ReturnItem0 = GridViewBusqueda.SelectedCells[0].Value.ToString();
                    ReturnItem1 = GridViewBusqueda.SelectedCells[1].Value.ToString();
                    ReturnItem2 = GridViewBusqueda.SelectedCells[2].Value.ToString();
                    ReturnItem3 = GridViewBusqueda.SelectedCells[3].Value.ToString();
                    ReturnItem4 = GridViewBusqueda.SelectedCells[4].Value.ToString();
                    break;
            }

           
            this.Close();
        }

        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            bool filter = chkVidriospanles.Checked;

            if (e.KeyCode == Keys.Enter)
            {
                Buscar(filter);//Refactoring
            }

   
        }

        private void Buscar(bool filter)
        {
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            string Fil = "";
            try
            {
                con.Open(out fail);
                string sqlquery = "";
                Fil = filter == true ? "1" : "0";



                switch (Consulta)
                {
                    case "Umed":
                        sqlquery = Generals.Constantes.QUERY_UNIDADMEDIDA + " where Descripcion lIKE '%" + txtBuscar.Text + "%'";
                        break;
                    case "Corte":
                        sqlquery = Generals.Constantes.QUERY_CORTE + " where Descripcion lIKE '%" + txtBuscar.Text + "%'";
                        break;
                    case "Acaba":
                        sqlquery = Generals.Constantes.QUERY_ACABADO + " where CONCAT(Codigo_Homologacion,' - ',Descripcion) lIKE '%" + txtBuscar.Text +"%'";
                        break;
                    case "SubComp":
                        sqlquery = Generals.Constantes.QUERY_SUBCOMPONENTES + " where CONCAT(subcomponentes.Codigo_Homologacion,' - ',subcomponentes.Descripcion) lIKE '%" + txtBuscar.Text + "%' and Especial = " + Fil;
                        break;
                    default:
                        sqlquery = Generals.Constantes.QUERY_COMPONENTES + " where CONCAT(CONCAT(Codigo , IFNULL(concat('-',acabados.Codigo_Homologacion),'')),' - ',arquitectdb.componentes.Descripcion) lIKE '%" + txtBuscar.Text + "%' and Especial = " + Fil;
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

        private void chkVidriospanles_CheckedChanged(object sender, EventArgs e)
        {
            bool filter = chkVidriospanles.Checked;
            Buscar(filter);//Refactoring
        }
    }
}
