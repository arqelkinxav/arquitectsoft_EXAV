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
    public partial class FrmCorte : Form
    {
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wMsg, int wParam, int lParam);
        public string Opc;
        public FrmCorte()
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
            bsc.Consulta = "Corte";
            bsc.ShowDialog();
            if (bsc.ReturnItem1 == null)
            {
                return;
            }

            txtCodigo.Text = bsc.ReturnItem0;
            txtDescripcion.Text = bsc.ReturnItem1;
            NudRight.Value = Int32.Parse(bsc.ReturnItem2);
            NudLeft.Value = Int32.Parse(bsc.ReturnItem3);

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
            Dto.CorteDto dto = new Dto.CorteDto();
            string fail = "";

            bool SwSave = dto.ValilidationSaveCorte(txtCodigo.Text, txtDescripcion.Text, out fail);

            if (SwSave == true)
            {
                SaveCorte(dto);
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

                Dto.CorteDto dto = new Dto.CorteDto();
                string resul = txtCodigo.Text;

                resul = dto.DeleteCorte(Int32.Parse(resul));

                BloquearCancelar();
                ClearComponent();

                MessageBox.Show(resul, "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion

        #region Metodos
        private void FrmCorte_Load(object sender, EventArgs e)
        {
            
            txtCodigo.Enabled = false;
            txtDescripcion.Enabled = false;
            NudLeft.Enabled = false;
            NudRight.Enabled = false;          
       
        }
        private void habilitarNuevo(string opcion)
        {

            Dto.CorteDto dto = new Dto.CorteDto();
            BtnGuardar.Enabled = true;
            BtnCancelar.Enabled = true;
            NudLeft.Enabled = true;
            NudRight.Enabled = true;
            txtCodigo.Enabled = false;
            if (Opc != "Editar")
            {
                txtCodigo.Text = dto.MaximoCorte();
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
            NudLeft.Value = 0;
            NudRight.Value = 0;
           
        }
        private void BloquearCancelar()
        {
            BtnCancelar.Enabled = false;
            BtnGuardar.Enabled = false;
            txtCodigo.Enabled = false;
            txtDescripcion.Enabled = false;
            NudLeft.Enabled = false;
            NudRight.Enabled = false;

            BtnNuevo.Enabled = true;
            BtnEditar.Enabled = false;
            BtnEliminar.Enabled = false;
            BtnBuscar.Enabled = true;
        }
        private void SaveCorte(Dto.CorteDto dto)
        {
            string resul = "0";

            //resul = dto.ExistCorte(txtCodigo.Text, txtDescripcion.Text);

            if (resul == "0" || Opc == "Editar")
            {

                resul = dto.SaveCorte(txtCodigo.Text, txtDescripcion.Text, Opc, (int)NudLeft.Value, (int)NudRight.Value);
                ClearComponent();
                MessageBox.Show(resul, "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Acabado ya Existe", "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        #endregion

        private void elipseControl2_Click(object sender, EventArgs e)
        {

        }

        private void EliCtrlButtons_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }
    }
}
