using arquitectSoft.Class;
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
    public partial class FrmSubComponente : Form
    {
        public string Opc;
        public FrmSubComponente()
        {
            InitializeComponent();
        }


        #region Botones

        private void BtnNuevo_Click(object sender, EventArgs e)
        {
            Opc = "Nuevo";
            ClearComponent();
            habilitarNuevo(null);
        }
        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            ClearComponent();
            BloquearCancelar();
        }
        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            
            FrmBuscar bsc = new FrmBuscar();
            bsc.Consulta = "SubComp";
            bsc.ShowDialog();
            if (bsc.ReturnItem1 == null)
            {
                return;
            }

            txtCodigo.Text = bsc.ReturnItem1.Split('-')[0].Trim();
            txtDescripcion.Text = bsc.ReturnItem2.Split('(')[0].Trim();
            CmbAcabado.SelectedValue = bsc.ReturnItem3;
            chkVidriospanles.Checked = bsc.ReturnItem4 == "1" ? true : false;

            txtCodigo.Enabled = false;
            txtDescripcion.Enabled = false;
            chkVidriospanles.Enabled = false;

            BtnCancelar.Enabled = true;
            BtnEditar.Enabled = true;
            BtnEliminar.Enabled = true;
            dataGridViewRC.DataSource = null;
            dataGridViewRC.DataSource = GetDataRelationComponent(bsc.ReturnItem1.Split('-')[0].Trim());
        }
        private void BtnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void BtnEditar_Click(object sender, EventArgs e)
        {
            Opc = "Editar";
            DialogResult result = MessageBox.Show("Esta seguro de editar el registro?", "Mensaje Alerta", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                txtDescripcion.Enabled = true;
                chkVidriospanles.Enabled = true;
                CmbAcabado.Enabled = true;

                habilitarNuevo(Opc);
            }
        }
        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            Dto.SubComponenteDto dto = new Dto.SubComponenteDto();
            string fail = "";
            int acabado = Int32.Parse(CmbAcabado.SelectedValue.ToString());
            bool SwSave = dto.ValilidationSaveSubComponenet(txtCodigo.Text, txtDescripcion.Text, acabado, out fail);

            if (SwSave == true)
            {
                SaveComponent(dto);
                BloquearCancelar();
            }
            else
            {
                MessageBox.Show(fail, "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }



        }
        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            Opc = "Eliminar";
            DialogResult result = MessageBox.Show("Esta seguro de eliminar el registro?", "Mensaje Alerta", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {

                Dto.SubComponenteDto dto = new Dto.SubComponenteDto();
                var codSplit = txtCodigo.Text.Split('-')[0].Trim();
                var desSplit = txtDescripcion.Text.Split('(')[0].Trim();
                string resul = dto.ExistSubComponent(codSplit, desSplit,CmbAcabado.SelectedValue.ToString(), Opc);

                resul = dto.DeleteComponent(Int32.Parse(resul));

                BloquearCancelar();
                ClearComponent();

                MessageBox.Show(resul, "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion

        #region Metodos

        private void FrmSubComponente_Load(object sender, EventArgs e)
        {
            txtCodigo.Enabled = false;
            txtDescripcion.Enabled = false;
            chkVidriospanles.Enabled = false;
            CmbAcabado.Enabled = false;
            BtnMultiAcabado.Enabled = false;
            BtnMultiAcabado.Visible = false;
            dataGridViewMA.Visible = false;

            Dto.AcabadoDto Acb = new Dto.AcabadoDto();
            CmbAcabado.DataSource = Acb.GetAcabado();
            CmbAcabado.DisplayMember = "Descripcion";
            CmbAcabado.ValueMember = "Id_Acabado";
        }
        private void habilitarNuevo(string opcion)
        {
            BtnGuardar.Enabled = true;
            BtnCancelar.Enabled = true;
            switch (opcion)
            {
                case "Editar":
                    txtCodigo.Enabled = false;
                    break;
                default:
                    txtCodigo.Enabled = true;
                    break;
            }

            txtDescripcion.Enabled = true;
            chkVidriospanles.Enabled = true;
            CmbAcabado.Enabled = true;

            BtnNuevo.Enabled = false;
            BtnEditar.Enabled = false;
            BtnEliminar.Enabled = false;
            BtnBuscar.Enabled = false;

            BtnMultiAcabado.Enabled = true;
            BtnMultiAcabado.Visible = true;
            dataGridViewMA.Visible = true;
        }
        private void ClearComponent()
        {
            txtCodigo.Text = "";
            txtDescripcion.Text = "";
            chkVidriospanles.Checked = false;
            CmbAcabado.SelectedIndex = 0;
            dataGridViewRC.DataSource = null;
            dataGridViewMA.DataSource = null;
            bindingSource1.Clear();
        }
        private void BloquearCancelar()
        {
            BtnCancelar.Enabled = false;
            BtnGuardar.Enabled = false;
            txtCodigo.Enabled = false;
            txtDescripcion.Enabled = false;
            chkVidriospanles.Enabled = false;
            CmbAcabado.Enabled = false;

            BtnNuevo.Enabled = true;
            BtnEditar.Enabled = false;
            BtnEliminar.Enabled = false;
            BtnBuscar.Enabled = true;

            BtnMultiAcabado.Enabled = false;
            BtnMultiAcabado.Visible = false;
            dataGridViewMA.Visible = false;
        }
        private void SaveComponent(Dto.SubComponenteDto dto)
        {
            string resul = "0";

            resul = dto.ExistSubComponent(txtCodigo.Text, txtDescripcion.Text, CmbAcabado.SelectedValue.ToString(), Opc);

            if (resul == "0" || Opc == "Editar")
            {

                resul = dto.SaveSubComponent(txtCodigo.Text, txtDescripcion.Text, CmbAcabado.SelectedValue.ToString(), chkVidriospanles.Checked, Opc, resul);               

                if (dataGridViewMA.Rows.Count > 0)
                {
                    foreach (DataGridViewRow r in dataGridViewMA.Rows)
                    {
                       string CodigoAcabado = r.Cells[0].Value.ToString();
                       resul = dto.SaveSubComponent(txtCodigo.Text, txtDescripcion.Text, CodigoAcabado, chkVidriospanles.Checked, Opc, resul);
                    }
                }

                ClearComponent();
                MessageBox.Show(resul, "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("El codigo ya se encuentra registrado en el sistema", "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private DataTable GetDataRelationComponent(string codigo)
        {
            Dto.SubComponenteDto dto = new Dto.SubComponenteDto();
            return dto.GetComponentRelation(codigo); ;
        }

        #endregion

        private void BtnMultiAcabado_Click(object sender, EventArgs e)
        {
            
            Dto.ComponenteDto dto = new Dto.ComponenteDto();
            FrmBuscar bsc = new FrmBuscar();
            bsc.Consulta = "Acaba";
            bsc.ShowDialog();
            if (bsc.ReturnItem1 == null)
            {
                return;
            }

          
            bindingSource1.Add(new MultiAcabado(bsc.ReturnItem0, bsc.ReturnItem2.ToString()));
            dataGridViewMA.DataSource = bindingSource1;


            dataGridViewMA.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void dataGridViewMA_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DialogResult result = MessageBox.Show("Esta seguro de eliminar el registro?", "Mensaje Alerta", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                dataGridViewMA.Rows.RemoveAt(e.RowIndex);
            }
                
        }
    }
}
