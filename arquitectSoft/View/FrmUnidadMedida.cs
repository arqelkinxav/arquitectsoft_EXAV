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
    public partial class FrmUnidadMedida : Form
    {
        public string Opc;
        public FrmUnidadMedida()
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
            Dto.ComponenteDto dto = new Dto.ComponenteDto();
            FrmBuscar bsc = new FrmBuscar();
            bsc.Consulta = "Umed";
            bsc.ShowDialog();
            if (bsc.ReturnItem1 == null)
            {
                return;
            }

            txtCodigo.Text = bsc.ReturnItem0;
            txtDescripcion.Text = bsc.ReturnItem1;
            txtConvencion.Text = bsc.ReturnItem2;

            txtCodigo.Enabled = false;
            txtDescripcion.Enabled = false;
            txtConvencion.Enabled = false;

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

                habilitarNuevo(Opc);
            }
        }
        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            Dto.UnidadMedidaDto dto = new Dto.UnidadMedidaDto();
            string fail = "";

            bool SwSave = dto.ValilidationSaveUnidadMedida(txtCodigo.Text, txtDescripcion.Text,txtConvencion.Text, out fail);

            if (SwSave == true)
            {
                SaveUnidadMedida(dto);
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

                Dto.UnidadMedidaDto dto = new Dto.UnidadMedidaDto();
                //string resul = dto.ExistUnidadMedida(txtDescripcion.Text);

                string resul = dto.DeleteUnidadMedida(Int32.Parse(txtCodigo.Text));

                BloquearCancelar();
                ClearComponent();

                MessageBox.Show(resul, "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion

        #region Metodos
        private void FrmAcabados_Load(object sender, EventArgs e)
        {
            txtCodigo.Enabled = false;
            txtDescripcion.Enabled = false;
        }
        private void habilitarNuevo(string opcion)
        {
            Dto.UnidadMedidaDto dto = new Dto.UnidadMedidaDto();
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

            if (Opc != "Editar")
            {
                txtCodigo.Text = dto.MaximaUnidadMedida();
            }

            txtDescripcion.Enabled = true;
            txtConvencion.Enabled = true;

            BtnNuevo.Enabled = false;
            BtnEditar.Enabled = false;
            BtnEliminar.Enabled = false;
            BtnBuscar.Enabled = false;
        }
        private void ClearComponent()
        {
            txtCodigo.Text = "";
            txtDescripcion.Text = "";
            txtConvencion.Text = "";           
        }
        private void BloquearCancelar()
        {
            BtnCancelar.Enabled = false;
            BtnGuardar.Enabled = false;
            txtCodigo.Enabled = false;
            txtDescripcion.Enabled = false;
            txtConvencion.Enabled = false;

            BtnNuevo.Enabled = true;
            BtnEditar.Enabled = false;
            BtnEliminar.Enabled = false;
            BtnBuscar.Enabled = true;
        }
        private void SaveUnidadMedida(Dto.UnidadMedidaDto dto)
        {
            string resul = "0";

            resul = dto.ExistUnidadMedida(txtCodigo.Text);

            if (resul == "0" || Opc == "Editar")
            {

                resul = dto.SaveUnidadMedida(txtCodigo.Text, txtDescripcion.Text,txtConvencion.Text, Opc, resul);
                ClearComponent();
                MessageBox.Show(resul, "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Unidad de Medida ya Existe", "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion


    }
}
