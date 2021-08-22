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

namespace arquitectSoft.View
{
    public partial class FrmMecanizado : Form
    {

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wMsg, int wParam, int lParam);

        public string Opc;

        public FrmMecanizado()
        {
            InitializeComponent();
        }

        private void BtnNuevo_Click(object sender, EventArgs e)
        {
            Opc = "Nuevo";
            ClearComponent();
            habilitarNuevo(null);
        }

        private void FrmMecanizado_Load(object sender, EventArgs e)
        {
            txtCodigo.Enabled = false;
            txtDescripcion.Enabled = false;
            BloquearCancelar();
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            Dto.MecanizadoDto dto = new Dto.MecanizadoDto();
            string fail = "";

            bool SwSave = dto.ValilidationSaveMecanizado(txtCodigo.Text, txtDescripcion.Text, out fail);

            if (SwSave == true)
            {
                SaveMecanizado(dto);
                BloquearCancelar();
            }
            else
            {
                MessageBox.Show(fail, "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
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

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            Opc = "Eliminar";
            DialogResult result = MessageBox.Show("Esta seguro de eliminar el registro?", "Mensaje Alerta", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {

                Dto.MecanizadoDto dto = new Dto.MecanizadoDto();
                string resul = dto.ExistMecanizado(txtCodigo.Text, txtDescripcion.Text);

                resul = dto.DeleteMecanizado(Int32.Parse(resul));

                BloquearCancelar();
                ClearComponent();

                MessageBox.Show(resul, "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
          

            FrmBuscar bsc = new FrmBuscar();
            bsc.Consulta = "Mecan";
            bsc.ShowDialog();
            if (bsc.ReturnItem1 == null)
            {
                return;
            }

            txtCodigo.Text = bsc.ReturnItem1;
            txtDescripcion.Text = bsc.ReturnItem2.ToString();

            txtCodigo.Enabled = false;
            txtDescripcion.Enabled = false;

            BtnCancelar.Enabled = true;
            BtnEditar.Enabled = true;
            BtnEliminar.Enabled = true;
        }

        private void SaveMecanizado(Dto.MecanizadoDto dto)
        {
            string resul = "0";

            resul = dto.ExistMecanizado(txtCodigo.Text, txtDescripcion.Text);

            if (resul == "0" || Opc == "Editar")
            {

                resul = dto.SaveMecanizado(txtCodigo.Text, txtDescripcion.Text, Opc, resul);
                ClearComponent();
                MessageBox.Show(resul, "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Acabado ya Existe", "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void ClearComponent()
        {
            txtCodigo.Text = String.Empty;
            txtDescripcion.Text = String.Empty;

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

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            ClearComponent();
            BloquearCancelar();
        }

        private void BtnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }



        private void EliCtrlButtons_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }
    }
}
