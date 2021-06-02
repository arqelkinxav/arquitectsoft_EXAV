using arquitectSoft.Class;
using arquitectSoft.Generals;
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
    public partial class FrmComponente : Form
    {
        public FrmComponente()
        {
            InitializeComponent();
        }

        public string Opc;

        private void BtnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnNuevo_Click(object sender, EventArgs e)
        {
            Opc = "Nuevo";
            ClearComponent();
            habilitarNuevo(null);
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            Dto.ComponenteDto dto = new Dto.ComponenteDto();
            string fail = "";

            bool SwSave = dto.ValilidationSaveComponenet(txtCodigo.Text, txtDescripcion.Text, chkNoSubComp.Checked, GridViewComponente.RowCount, out fail);

            if (chkNoSubComp.Checked == true)
            {
                DialogResult result = MessageBox.Show("Esta seguro de marcar que no posee subcomponente?", "Mensaje Alerta", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                {
                    fail = "No se Guardo ningun Registro!";
                    SwSave = false;
                }
            }


            if (SwSave == true)
            {
                SaveComponent(dto);
            }
            else
            {
                MessageBox.Show(fail, "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            BloquearCancelar();

        }

        private void BtnCheck_Click(object sender, EventArgs e)
        {
            Dto.ComponenteDto dto = new Dto.ComponenteDto();
            string resul = dto.ExistComponent(txtCodigo.Text, txtDescripcion.Text);
            resul = (resul!="") ? "Componente ya Existe" : "Componente Disponible para Guardar";
            MessageBox.Show(resul, "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            ClearComponent();
            BloquearCancelar();
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            Dto.ComponenteDto dto = new Dto.ComponenteDto();
            FrmBuscar bsc = new FrmBuscar();
            bsc.ShowDialog();

            txtCodigo.Text = bsc.ReturnItem1;
            txtDescripcion.Text = bsc.ReturnItem2;
            chkNoSubComp.Checked = bsc.ReturnItem3 == "1" ? true : false;

            DataTable dt = dto.GetComponentDetalle(bsc.ReturnItem4);

            foreach (DataRow row in dt.Rows)
            {
                int decre = Int32.Parse(row["ADecremento"].ToString());
                bool adrecre = decre == 1 ? true : false;

                bindingSource1.Add(new Sub_Component(row["Codigo"].ToString(), row["Descripcion"].ToString(), (int)row["Cxdefecto"], (int)row["CAdicional"], row["UnidadCalculada"].ToString(), adrecre, (int)row["IdSubcomponente"])); 
            }            

            GridViewComponente.DataSource = bindingSource1;
            

            txtCodigo.Enabled = false;
            txtDescripcion.Enabled = false;
            chkNoSubComp.Enabled = false;

            BtnCancelar.Enabled = true;
            BtnEditar.Enabled = true;
            BtnEliminar.Enabled = true;
            
        }

        private void FrmComponente_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'arquitectdbDataSet.unidades_calculadas' table. You can move, or remove it, as needed.
           
            txtCodigo.Enabled = false;
            txtDescripcion.Enabled = false;
            chkNoSubComp.Enabled = false;
            BtnCheck.Enabled = false;
            BtnAgregar.Enabled = false;
            BtnBorrar.Enabled = false;
            initialize_datagrid();
        }

        private void BtnEditar_Click(object sender, EventArgs e)
        {
            Opc = "Editar";
            DialogResult result = MessageBox.Show("Esta seguro de editar el registro?", "Mensaje Alerta", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                txtDescripcion.Enabled = true;
                chkNoSubComp.Enabled = true;
                BtnAgregar.Enabled = true;
                BtnBorrar.Enabled = true;
                habilitarNuevo(Opc);
            }

            
        }

        private void ClearComponent()
        {
            txtCodigo.Text = "";
            txtDescripcion.Text = "";
            chkNoSubComp.Checked = false;
            GridViewComponente.DataSource = "";

        }

        private void habilitarNuevo(string opcion)
        {
            BtnGuardar.Enabled = true;
            BtnCancelar.Enabled = true;
            BtnCheck.Enabled = true; 
            switch (opcion)
            {
                case "Editar": txtCodigo.Enabled = false;
                    break;
                default: txtCodigo.Enabled = true;
                    break;
            }
            
            txtDescripcion.Enabled = true;
            chkNoSubComp.Enabled = true;
            BtnAgregar.Enabled = true;
            BtnBorrar.Enabled = true;

            BtnNuevo.Enabled = false;
            BtnEditar.Enabled = false;
            BtnEliminar.Enabled = false;
            BtnBuscar.Enabled = false;
        }

        private void BloquearCancelar()
        {
            BtnCancelar.Enabled = false;
            BtnGuardar.Enabled = false;
            BtnCheck.Enabled = false;
            txtCodigo.Enabled = false;
            txtDescripcion.Enabled = false;
            chkNoSubComp.Enabled = false;
            BtnAgregar.Enabled = false;
            BtnBorrar.Enabled = false;

            BtnNuevo.Enabled = true;
            BtnEditar.Enabled = false;
            BtnEliminar.Enabled = false;
            BtnBuscar.Enabled = true;

            bindingSource1.Clear();
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            if (!chkNoSubComp.Checked)
            {
               
                FrmBuscar bsc = new FrmBuscar();
                bsc.Consulta = "SubComp";
                bsc.ShowDialog();

                bindingSource1.Add(new Sub_Component(bsc.ReturnItem2, bsc.ReturnItem3, 1, 30, "", false, Int32.Parse(bsc.ReturnItem1)));

                GridViewComponente.DataSource = bindingSource1;
            }
            else
            {
                MessageBox.Show("Debe Primero Quitar el Check de No Sub componente", "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

           

        }

        private void initialize_datagrid()
        {
            GridViewComponente.Columns.Clear();
            GridViewComponente.Refresh();
            GridViewComponente.AutoGenerateColumns = false;

            GridViewComponente.Columns.Add(DGV_Handler.CreateTextBox("IdSubcomponente", "Id", "IdSubcomponente", false));
            GridViewComponente.Columns.Add(DGV_Handler.CreateTextBox("Codigo", "Codigo", "Codigo", false));
            GridViewComponente.Columns.Add(DGV_Handler.CreateTextBox("Descripcion", "Descripcion", "Descripcion", false));
            GridViewComponente.Columns.Add(DGV_Handler.CreateUnidadCalculadaComboBox());
            GridViewComponente.Columns.Add(DGV_Handler.CreateTextBox("Cxdefecto", "Cx. efecto", "Cxdefecto", true));
            GridViewComponente.Columns.Add(DGV_Handler.CreateTextBox("CAdicional", "C. Adicional", "CAdicional", true));
            GridViewComponente.Columns.Add(DGV_Handler.CreateCheckBox("ADecremento", "A. Decremento", "ADecremento"));

            GridViewComponente.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            GridViewComponente.Columns[0].ReadOnly = true;
            GridViewComponente.Columns[1].ReadOnly = true;

            //GridViewComponente.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            GridViewComponente.RowHeadersVisible = false;

        }

        private void SaveComponent(Dto.ComponenteDto dto)
        {
            string resul = "0";

            resul = dto.ExistComponent(txtCodigo.Text, txtDescripcion.Text);

            if (resul == "0" || Opc == "Editar")
            {
                Sub_Component[] Sbarray = new Sub_Component[GridViewComponente.RowCount];

                foreach (DataGridViewRow row in GridViewComponente.Rows)
                {
                    int id = (int)row.Cells["IdSubcomponente"].Value;
                    string codigo = row.Cells["Codigo"].Value.ToString();
                    string descripcion = row.Cells["Descripcion"].Value.ToString();
                    string unidadcalculada = row.Cells["UnidadCalculada"].Value.ToString();
                    int Cxdefecto = (int)row.Cells["Cxdefecto"].Value;
                    int CAdicional = (int)row.Cells["CAdicional"].Value;
                    bool ADecremento = (bool)row.Cells["ADecremento"].Value;

                    Sub_Component sub = new Sub_Component(codigo, descripcion, Cxdefecto, CAdicional, unidadcalculada, ADecremento, id);

                    Sbarray[row.Index] = sub;

                }



                resul = dto.SaveComponent(txtCodigo.Text, txtDescripcion.Text, chkNoSubComp.Checked,Opc, Sbarray,resul);
                ClearComponent();
                MessageBox.Show(resul, "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(resul, "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }



    }
}
