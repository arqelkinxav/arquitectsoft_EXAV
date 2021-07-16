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
    public partial class FrmAcabados : Form
    {
        public string Opc;
        public FrmAcabados()
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
            bsc.Consulta = "Acaba";
            bsc.ShowDialog();
            if (bsc.ReturnItem1 == null)
            {
                return;
            }

            txtCodigo.Text = bsc.ReturnItem1;
            txtDescripcion.Text = bsc.ReturnItem2.ToString().Split('-')[1].Trim();

            txtCodigo.Enabled = false;
            txtDescripcion.Enabled = false;

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
            Dto.AcabadoDto dto = new Dto.AcabadoDto();
            string fail = "";

            bool SwSave = dto.ValilidationSaveAcabado(txtCodigo.Text, txtDescripcion.Text, out fail);

            if (SwSave == true)
            {
                SaveAcabado(dto);
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

                Dto.AcabadoDto dto = new Dto.AcabadoDto();
                string resul = dto.ExistAcabado(txtCodigo.Text, txtDescripcion.Text);

                resul = dto.DeleteAcabado(Int32.Parse(resul));

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

            BtnNuevo.Enabled = false;
            BtnEditar.Enabled = false;
            BtnEliminar.Enabled = false;
            BtnBuscar.Enabled = false;
        }
        private void ClearComponent()
        {
            txtCodigo.Text = "";
            txtDescripcion.Text = "";
           
        }
        private void BloquearCancelar()
        {
            BtnCancelar.Enabled = false;
            BtnGuardar.Enabled = false;
            txtCodigo.Enabled = false;
            txtDescripcion.Enabled = false;
           

            BtnNuevo.Enabled = true;
            BtnEditar.Enabled = false;
            BtnEliminar.Enabled = false;
            BtnBuscar.Enabled = true;
        }
        private void SaveAcabado(Dto.AcabadoDto dto)
        {
            string resul = "0";

            resul = dto.ExistAcabado(txtCodigo.Text, txtDescripcion.Text);

            if (resul == "0" || Opc == "Editar")
            {

                resul = dto.SaveAcabado(txtCodigo.Text, txtDescripcion.Text, Opc, resul);
                ClearComponent();
                MessageBox.Show(resul, "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Acabado ya Existe", "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion


    }
}
