using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace arquitectSoft
{
    public partial class FrmBuscar : Form
    {

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wMsg, int wParam, int lParam);

        public string Consulta { get; set; }
        public string ReturnItem0 { get; set; }
        public string ReturnItem1{ get; set; }
        public string ReturnItem2 { get; set; }
        public string ReturnItem3 { get; set; }
        public string ReturnItem4 { get; set; }
        public string ReturnItem5 { get; set; }


        public DataTable ArrayMultiSelect;
        
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
                case "Acaba-Multi":
                    btnMultiSelect.Visible = true;
                    break;
                case "Acaba" :               
                case "Mecan":
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
                case "Acaba-Multi":
                case "Mecan":
                    ReturnItem0 = GridViewBusqueda.SelectedCells[0].Value.ToString();
                    ReturnItem1 = GridViewBusqueda.SelectedCells[1].Value.ToString();
                    ReturnItem2 = GridViewBusqueda.SelectedCells[2].Value.ToString();
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Item1");
                    dt.Columns.Add("Item2");
                    dt.Columns.Add("Item3");
                    ArrayMultiSelect = dt;
                    break;
                case "SubComp":
                    ReturnItem0 = GridViewBusqueda.SelectedCells[0].Value.ToString();
                    ReturnItem1 = GridViewBusqueda.SelectedCells[1].Value.ToString();
                    ReturnItem2 = GridViewBusqueda.SelectedCells[2].Value.ToString();
                    ReturnItem3 = GridViewBusqueda.SelectedCells[3].Value.ToString();
                    ReturnItem4 = GridViewBusqueda.SelectedCells[4].Value.ToString();
                    ReturnItem5 = GridViewBusqueda.SelectedCells[5].Value.ToString();
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
            string Condicion = "";
            try
            {
                con.Open(out fail);
                string sqlquery = "";
                Fil = filter == true ? "1" : "0";

                var searchsplit = txtBuscar.Text.Split(' ');

                switch (Consulta)
                {
                    case "Umed":
                        Condicion = " WHERE ";

                        for (int i = 0; i < searchsplit.Length; i++)
                        {
                            if (i > 0)
                            {
                                Condicion += " AND "; // Usa OR si prefieres
                            }
                            Condicion += " Descripcion lIKE '%" + searchsplit[i] + "%'";
                        }

                        sqlquery = Generals.Constantes.QUERY_UNIDADMEDIDA + Condicion;
                        break;
                    case "Corte":
                        Condicion = " WHERE ";

                        for (int i = 0; i < searchsplit.Length; i++)
                        {
                            if (i > 0)
                            {
                                Condicion += " AND "; // Usa OR si prefieres
                            }
                            Condicion += " Descripcion lIKE '%" + searchsplit[i] + "%'";
                        }

                        sqlquery = Generals.Constantes.QUERY_CORTE + Condicion;
                        break;
                    case "Acaba-Multi":
                    case "Acaba":
                        Condicion = " WHERE ";

                        for (int i = 0; i < searchsplit.Length; i++)
                        {
                            if (i > 0)
                            {
                                Condicion += " AND "; // Usa OR si prefieres
                            }
                            Condicion += " CONCAT(Codigo_Homologacion,' - ',Descripcion) lIKE '%" + searchsplit[i] + "%'";
                        }

                        sqlquery = Generals.Constantes.QUERY_ACABADO + Condicion;
                        break;
                    case "Mecan":
                        Condicion = " WHERE ";

                        for (int i = 0; i < searchsplit.Length; i++)
                        {
                            if (i > 0)
                            {
                                Condicion += " AND "; // Usa OR si prefieres
                            }
                            Condicion += " CONCAT(Codigo_Homologacion,' - ',Descripcion) lIKE '%" + searchsplit[i] + "%'";
                        }

                        sqlquery = Generals.Constantes.QUERY_MECANIZADO + Condicion;
                        break;
                    case "SubComp":

                        Condicion = " WHERE Especial = " + Fil + " AND ";

                        for (int i = 0; i < searchsplit.Length; i++)
                        {
                            if (i > 0)
                            {
                                Condicion += " AND "; // Usa OR si prefieres
                            }
                            Condicion += " CONCAT(subcomponentes.Codigo_Homologacion,' - ',subcomponentes.Descripcion) LIKE '%" + searchsplit[i] + "%'";
                        }

                        sqlquery = Generals.Constantes.QUERY_SUBCOMPONENTES + Condicion;
                        break;
                    default:

                        Condicion = " WHERE Especial = " + Fil + " AND ";

                        for (int i = 0; i < searchsplit.Length; i++)
                        {
                            if (i > 0)
                            {
                                Condicion += " AND "; // Usa OR si prefieres
                            }
                            Condicion += " CONCAT(CONCAT(Codigo , IFNULL(concat('-',acabados.Codigo_Homologacion),'')),' - ',componentes.Descripcion) LIKE '%" + searchsplit[i] + "%'";
                        }                  

                        sqlquery = Generals.Constantes.QUERY_COMPONENTES + Condicion;
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

        private void BtnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnBuscar_Click_1(object sender, EventArgs e)
        {
            bool filter = chkVidriospanles.Checked;
            Buscar(filter);
        }

        private void EliCtrlButtons_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void btnMultiSelect_Click(object sender, EventArgs e)
        {

            DataTable dt = new DataTable();

            dt.Columns.Add("Item1");
            dt.Columns.Add("Item2");
            dt.Columns.Add("Item3");

            foreach (DataGridViewRow row in GridViewBusqueda.SelectedRows)
            {
                //MessageBox.Show(row.Cells[0].Value.ToString());
                dt.Rows.Add(row.Cells[0].Value.ToString(), row.Cells[1].Value.ToString(), row.Cells[2].Value.ToString());
            }

            ArrayMultiSelect = dt;
            this.Close();
        }
    }
}
