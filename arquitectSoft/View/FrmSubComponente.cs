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

        private void FrmSubComponente_Load(object sender, EventArgs e)
        {
            txtCodigo.Enabled = false;
            txtDescripcion.Enabled = false;
            chkVidriospanles.Enabled = false;
            CmbAcabado.Enabled = false;


            Dto.AcabadoDto Acb = new Dto.AcabadoDto();
            CmbAcabado.DataSource = Acb.GetAcabado();
            CmbAcabado.DisplayMember = "Descripcion";
            CmbAcabado.ValueMember = "Id_Acabado";
        }

        private void BtnNuevo_Click(object sender, EventArgs e)
        {
            Opc = "Nuevo";
            ClearComponent();
            habilitarNuevo(null);
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
        }

        private void ClearComponent()
        {
            txtCodigo.Text = "";
            txtDescripcion.Text = "";
            chkVidriospanles.Checked = false;
            CmbAcabado.SelectedIndex = 0;
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
            bsc.Consulta = "SubComp";
            bsc.ShowDialog();
            if (bsc.ReturnItem1 == null)
            {
                return;
            }

            txtCodigo.Text = bsc.ReturnItem1;
            txtDescripcion.Text = bsc.ReturnItem2;
            CmbAcabado.SelectedValue = bsc.ReturnItem3;
            chkVidriospanles.Checked = bsc.ReturnItem4 == "1" ? true : false;

            txtCodigo.Enabled = false;
            txtDescripcion.Enabled = false;
            chkVidriospanles.Enabled = false;

            BtnCancelar.Enabled = true;
            BtnEditar.Enabled = true;
            BtnEliminar.Enabled = true;
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

            bool SwSave = dto.ValilidationSaveSubComponenet(txtCodigo.Text, txtDescripcion.Text,out fail);

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

        private void SaveComponent(Dto.SubComponenteDto dto)
        {
            string resul = "0";

            resul = dto.ExistSubComponent(txtCodigo.Text, txtDescripcion.Text);

            if (resul == "0" || Opc == "Editar")
            {
              
                resul = dto.SaveSubComponent(txtCodigo.Text, txtDescripcion.Text,CmbAcabado.SelectedValue.ToString(), chkVidriospanles.Checked, Opc, resul);
                ClearComponent();
                MessageBox.Show(resul, "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(resul, "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            Opc = "Eliminar";
            DialogResult result = MessageBox.Show("Esta seguro de eliminar el registro?", "Mensaje Alerta", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {

                Dto.SubComponenteDto dto = new Dto.SubComponenteDto();
                string resul = dto.ExistSubComponent(txtCodigo.Text, txtDescripcion.Text);

                resul = dto.DeleteComponent(Int32.Parse(resul));

                BloquearCancelar();

                MessageBox.Show(resul, "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
