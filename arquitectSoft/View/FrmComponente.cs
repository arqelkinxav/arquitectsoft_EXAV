using arquitectSoft.Class;
using arquitectSoft.Generals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace arquitectSoft.View
{
    public partial class FrmComponente : Form
    {
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wMsg, int wParam, int lParam);

        public FrmComponente()
        {
            InitializeComponent();
        }

        public decimal CantiAdiAnch = 30;
        public bool AplDecreAnch;

        public string Opc;
        public string condicionAcabado = "";
        private int selectedRowIndex = -1;

        private void FrmComponente_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'arquitectdbDataSet.unidades_calculadas' table. You can move, or remove it, as needed.

            txtCodigo.Enabled = false;
            txtDescripcion.Enabled = false;
            chkEspecial.Enabled = false;
            CmbAcabado.Enabled = false;
            BtnCheck.Enabled = false;
            BtnAgregar.Enabled = false;
            BtnBorrar.Enabled = false;


            initialize_datagrid();
            BloquearCancelar();
        }

        #region Botones

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
            int acabado = Int32.Parse(CmbAcabado.SelectedValue.ToString());
            bool SwSave = dto.ValilidationSaveComponenet(txtCodigo.Text, txtDescripcion.Text, chkEspecial.Checked, acabado, GridViewComponente.RowCount, GridViewComponenteEsp.RowCount, out fail);

            foreach (DataGridViewRow row in GridViewComponente.Rows)
            {

                string dataanchura = row.Cells["elevado"].Value.ToString();

                if (row.Cells["UnidadCalculada"].Value == null)
                {
                    fail = "La unidad calculada en uno de los Sub Componentes se encuentra Vacia!!";
                    SwSave = false;
                    break;
                }
                else if (row.Cells["MedidaHA"].Value == null && (int)row.Cells["UnidadCalculada"].Value != 6)
                {
                    fail = "La Seleccion de medida en uno de los Sub Componentes se encuentra Vacia!!";
                    SwSave = false;
                    break;
                }
                else if (row.Cells["elevado"].Value.ToString() == "0" && (int)row.Cells["UnidadCalculada"].Value == 6)
                {
                    fail = "No se ha seleccionado datos para la anchura!!";
                    SwSave = false;
                    break;
                }
            }

            if (!chkEspecial.Checked)
            {
                foreach (DataGridViewRow row in GridViewComponenteEsp.Rows)
                {
                    if (row.Cells["Columna"].Value == null)
                    {
                        fail = "No se ha Seleccionado una Columna en uno de los Sub Componentes especiales se encuentra Vacia!!";
                        SwSave = false;
                        break;
                    }
                }
            }


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
        private void BtnEditar_Click(object sender, EventArgs e)
        {
            Opc = "Editar";
            DialogResult result = MessageBox.Show("Esta seguro de editar el registro?", "Mensaje Alerta", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                txtDescripcion.Enabled = true;
                chkEspecial.Enabled = true;
                BtnAgregar.Enabled = true;
                BtnBorrar.Enabled = true;

                habilitarNuevo(Opc);
            }


        }
        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            Opc = "Eliminar";
            DialogResult result = MessageBox.Show("Esta seguro de eliminar el registro?", "Mensaje Alerta", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {

                Dto.ComponenteDto dto = new Dto.ComponenteDto();
                string resul = dto.ExistComponent(txtCodigo.Text, txtDescripcion.Text, CmbAcabado.SelectedValue.ToString());

                resul = dto.DeleteComponent(Int32.Parse(resul));

                BloquearCancelar();

                MessageBox.Show(resul, "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }


        }
        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            GridViewComponente.DataSource = null;
            GridViewComponenteEsp.DataSource = null;
            bindingSource1.Clear();
            bindingSource2.Clear();
            Dto.ComponenteDto dto = new Dto.ComponenteDto();
            FrmBuscar bsc = new FrmBuscar();
            bsc.ShowDialog();

            if (bsc.ReturnItem1 == null)
            {
                return;
            }
            txtCodigo.Text = bsc.ReturnItem1;
            txtDescripcion.Text = bsc.ReturnItem2;
            chkEspecial.Checked = bsc.ReturnItem3 == "1" ? true : false;


            //Obtener detalle componente generico
            DataTable dt = dto.GetComponentDetalle(bsc.ReturnItem0);
            CargarDataDetalle(dt);

            //Obtener detalle componente especial
            DataTable dtespecial = dto.GetComponentEspecialDetalle(bsc.ReturnItem0);
            bool swesp = dtespecial.Rows.Count > 0 ? true : false;
            HabilitarEspecial(swesp);
            if (swesp)
            {
                CargarDataDetalleEspecial(dtespecial);
            }

            CmbAcabado.SelectedValue = bsc.ReturnItem4 == "" ? "0" : bsc.ReturnItem4;

            if (bsc.ReturnItem4 != "" && CmbAcabado.SelectedValue == null)
            {
                MessageBox.Show("El Codigo del componente tiene Asociado un acabado que ya no esta entre los subcomponente, por favor Actualizarlo", "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            CmbAcabado.SelectedValue = CmbAcabado.SelectedValue == null ? 0 : CmbAcabado.SelectedValue;
            txtCodigo.Enabled = false;
            txtDescripcion.Enabled = false;
            chkEspecial.Enabled = false;

            BtnCancelar.Enabled = true;
            BtnEditar.Enabled = true;
            BtnEliminar.Enabled = true;
            BtnDuplicar.Enabled = true;



        }
        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            ClearComponent();
            BloquearCancelar();
            HabilitarEspecial(false);
        }
        private void BtnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void BtnCheck_Click(object sender, EventArgs e)
        {
            Dto.ComponenteDto dto = new Dto.ComponenteDto();
            string resul = dto.ExistComponent(txtCodigo.Text, txtDescripcion.Text, CmbAcabado.SelectedValue.ToString());
            resul = (resul != "0") ? "Componente ya Existe" : "Componente Disponible para Guardar";
            MessageBox.Show(resul, "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnBorrar_Click(object sender, EventArgs e)
        {
            if (GridViewComponente.Rows.Count == 0 && GridViewComponenteEsp.Rows.Count == 0)
            {
                MessageBox.Show("No Existe un Registro para eliminar!!", "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                int RI = 0;
                int RIEsp = 0;
                string descripcionori = "";
                string descripcionEsp = "";

                if (GridViewComponente.CurrentCell != null)
                {
                    RI = GridViewComponente.CurrentCell.RowIndex;
                    descripcionori = GridViewComponente.Rows[GridViewComponente.CurrentCell.RowIndex].Cells[2].Value.ToString();
                }
                if (GridViewComponenteEsp.CurrentCell != null)
                {
                    RIEsp = GridViewComponenteEsp.CurrentCell.RowIndex;
                    descripcionEsp = GridViewComponenteEsp.Rows[GridViewComponenteEsp.CurrentCell.RowIndex].Cells[2].Value.ToString();
                }

                View.FrmComponenteBorrar bsc = new FrmComponenteBorrar();

                bsc.SetItem0 = RI.ToString();
                bsc.SetItem1 = descripcionori.ToString();
                bsc.SetItem2 = RIEsp.ToString();
                bsc.SetItem3 = descripcionEsp.ToString();
                bsc.SetValidation = descripcionori.ToString() != "" ? "true" : "false";
                bsc.SetValidationEsp = descripcionEsp.ToString() != "" ? "true" : "false";
                bsc.ShowDialog();

                if (bsc.ReturnItem1 != "")
                {
                    DialogResult result = MessageBox.Show(string.Format("Esta seguro de Borrar el Item de Detalle: {0} ?", descripcionori), "Mensaje Alerta", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        GridViewComponente.Rows.RemoveAt(GridViewComponente.CurrentCell.RowIndex);
                    }
                }

                if (bsc.ReturnItem2 != "")
                {
                    DialogResult result = MessageBox.Show(string.Format("Esta seguro de Borrar el Item de Detalle: {0} ?", descripcionEsp), "Mensaje Alerta", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        GridViewComponenteEsp.Rows.RemoveAt(GridViewComponenteEsp.CurrentCell.RowIndex);
                    }
                }
            }
        }
        private void BtnDuplicar_Click(object sender, EventArgs e)
        {
            Opc = "Duplicar";
            txtCodigo.Text = "";
            txtDescripcion.Text = "";

            habilitarNuevo(Opc);
        }

        #endregion


        #region Metodos

        private void initialize_datagrid()
        {
            GridViewComponente.Columns.Clear();
            GridViewComponenteEsp.Columns.Clear();
            GridViewComponente.Refresh();
            GridViewComponenteEsp.Refresh();
            GridViewComponente.AutoGenerateColumns = false;
            GridViewComponenteEsp.AutoGenerateColumns = false;

            GridViewComponente.Columns.Add(DGV_Handler.CreateTextBox("IdSubcomponente", "Id", "IdSubcomponente", false));
            GridViewComponente.Columns.Add(DGV_Handler.CreateTextBox("Codigo", "Codigo", "Codigo", false));
            GridViewComponente.Columns.Add(DGV_Handler.CreateTextBox("Descripcion", "Descripcion", "Descripcion", false));
            GridViewComponente.Columns.Add(DGV_Handler.CreateUnidadCalculadaComboBox());
            GridViewComponente.Columns.Add(DGV_Handler.CreateTextBox("Cxdefecto", "Cx. efecto", "Cxdefecto", true));
            GridViewComponente.Columns.Add(DGV_Handler.CreateTextBox("CAdicional", "C. Adicional", "CAdicional", true));
            GridViewComponente.Columns.Add(DGV_Handler.CreateCheckBox("ADecremento", "A. Decremento", "ADecremento"));
            GridViewComponente.Columns.Add(DGV_Handler.CreateTextBox("Elevado", "Data Anchura", "Elevado", true)); //Se reutiliza campo elevado para guardar data de anchura
            GridViewComponente.Columns.Add(DGV_Handler.CreateCorteComboBox());
            GridViewComponente.Columns.Add(DGV_Handler.CreateCheckBox("Extra", "Extra", "Extra"));
            GridViewComponente.Columns.Add(DGV_Handler.CreateSeleccionMedidaComboBox());
            GridViewComponente.Columns.Add(DGV_Handler.CreateMecanizadoComboBox());
            GridViewComponente.Columns.Add(DGV_Handler.CreateTextBox("Asignacion_puertas", "Asignacion_puertas", "Asignacion_puertas", false));


            GridViewComponenteEsp.Columns.Add(DGV_Handler.CreateTextBox("IdSubcomponente", "Id", "IdSubcomponente", false));
            GridViewComponenteEsp.Columns.Add(DGV_Handler.CreateTextBox("Codigo", "Codigo", "Codigo", false));
            GridViewComponenteEsp.Columns.Add(DGV_Handler.CreateTextBox("Descripcion", "Descripcion", "Descripcion", false));
            GridViewComponenteEsp.Columns.Add(DGV_Handler.CreateColumnasComboBox());
            GridViewComponenteEsp.Columns.Add(DGV_Handler.CreateTextBox("Cxdefecto", "Cx. efecto", "Cxdefecto", true));
            GridViewComponenteEsp.Columns.Add(DGV_Handler.CreateTextBox("CAdicional", "C. Adicional", "CAdicional", true));


            GridViewComponente.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            GridViewComponente.Columns[0].ReadOnly = true;
            GridViewComponente.Columns[1].ReadOnly = true;
            GridViewComponente.Columns[2].ReadOnly = true;
            GridViewComponente.Columns[7].Visible = false; //Se reutiliza campo elevado para guardar data de anchura
            GridViewComponente.Columns[11].Visible = true; //Se reutiliza campo elevado para guardar data de anchura
            GridViewComponente.Columns[12].Visible = false; //Se reutiliza campo elevado para guardar Asignacion Puertas

            GridViewComponenteEsp.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            GridViewComponenteEsp.Columns[0].ReadOnly = true;
            GridViewComponenteEsp.Columns[1].ReadOnly = true;
            GridViewComponenteEsp.Columns[2].ReadOnly = true;
            //GridViewComponente.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            GridViewComponente.RowHeadersVisible = false;
            GridViewComponenteEsp.RowHeadersVisible = false;

            ContextMenuStrip contextMenu = new ContextMenuStrip();
            ToolStripMenuItem item1 = new ToolStripMenuItem("Asignacion Puertas");
            //ToolStripMenuItem item2 = new ToolStripMenuItem("Opción 2");

            item1.Click += Item1_AP_Click;

            contextMenu.Items.Add(item1);

            GridViewComponente.ContextMenuStrip = contextMenu;

            GridViewComponente.CellMouseDown += GridViewComponente_CellMouseDown;

        }
        private void habilitarNuevo(string opcion)
        {
            BtnGuardar.Enabled = true;
            BtnCancelar.Enabled = true;
            BtnCheck.Enabled = true;
            switch (opcion)
            {
                case "Editar":
                    txtCodigo.Enabled = false;
                    break;
                case "Duplicar":
                    txtCodigo.Enabled = true;
                    break;
                default:
                    txtCodigo.Enabled = true;
                    condicionAcabado = "";
                    GetAcabadoSelect("");
                    break;
            }

            txtDescripcion.Enabled = true;
            chkEspecial.Enabled = true;
            CmbAcabado.Enabled = true;
            BtnAgregar.Enabled = true;
            BtnBorrar.Enabled = true;
            GridViewComponente.Enabled = true;
            GridViewComponenteEsp.Enabled = true;

            BtnNuevo.Enabled = false;
            BtnEditar.Enabled = false;
            BtnEliminar.Enabled = false;
            BtnBuscar.Enabled = false;
            BtnDuplicar.Enabled = false;

        }
        private void BloquearCancelar()
        {
            BtnCancelar.Enabled = false;
            BtnGuardar.Enabled = false;
            BtnCheck.Enabled = false;
            txtCodigo.Enabled = false;
            txtDescripcion.Enabled = false;
            chkEspecial.Enabled = false;
            CmbAcabado.Enabled = false;
            BtnAgregar.Enabled = false;
            BtnBorrar.Enabled = false;
            GridViewComponente.Enabled = false;
            GridViewComponenteEsp.Enabled = false;

            BtnNuevo.Enabled = true;
            BtnEditar.Enabled = false;
            BtnEliminar.Enabled = false;
            BtnDuplicar.Enabled = false;
            BtnBuscar.Enabled = true;

            bindingSource1.Clear();
            bindingSource2.Clear();
            GetAcabadoSelect("");
            condicionAcabado = "";

        }
        private void SaveComponent(Dto.ComponenteDto dto)
        {
            string resul = "0";

            resul = dto.ExistComponent(txtCodigo.Text, txtDescripcion.Text, CmbAcabado.SelectedValue.ToString());

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
                    string Elevado = row.Cells["Elevado"].Value.ToString();


                    int Corte = (row.Cells["Cortes"].Value != null) ? Int32.Parse(row.Cells["Cortes"].Value.ToString()) : 5;
                    bool Extra = (bool)row.Cells["Extra"].Value;

                    int medida = (row.Cells["MedidaHA"].Value != null) ? (int)row.Cells["MedidaHA"].Value : 1;

                    string Mecanizado = (row.Cells["Mecanizado"].Value != null) ? row.Cells["Mecanizado"].Value.ToString() : "0";

                    string A_Puerta = (row.Cells["Asignacion_puertas"].Value != null) ? row.Cells["Asignacion_puertas"].Value.ToString() : "0";

                    if (unidadcalculada == "6") { medida = 0; }

                    Sub_Component sub = new Sub_Component(codigo, descripcion, Cxdefecto, CAdicional, unidadcalculada, ADecremento, id, Elevado, Corte, Extra, medida, int.Parse(Mecanizado), int.Parse(A_Puerta));

                    Sbarray[row.Index] = sub;
                }

                Sub_ComponentEspecial[] SbarrayEsp = new Sub_ComponentEspecial[GridViewComponenteEsp.RowCount];
                if (chkEspecial.Checked)
                {

                    foreach (DataGridViewRow row in GridViewComponenteEsp.Rows)
                    {
                        int id = (int)row.Cells["IdSubcomponente"].Value;
                        string codigo = row.Cells["Codigo"].Value.ToString();
                        string descripcion = row.Cells["Descripcion"].Value.ToString();
                        string columna = row.Cells["Columna"].Value.ToString();
                        int Cxdefecto = (int)row.Cells["Cxdefecto"].Value;
                        int CAdicional = (int)row.Cells["CAdicional"].Value;

                        Sub_ComponentEspecial subEsp = new Sub_ComponentEspecial(codigo, descripcion, columna, Cxdefecto, CAdicional, id);

                        SbarrayEsp[row.Index] = subEsp;
                    }
                }



                resul = dto.SaveComponent(txtCodigo.Text, txtDescripcion.Text, chkEspecial.Checked, CmbAcabado.SelectedValue.ToString(), Opc, Sbarray, SbarrayEsp, resul);
                ClearComponent();
                MessageBox.Show(resul, "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(resul, "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void ClearComponent()
        {
            txtCodigo.Text = String.Empty;
            txtDescripcion.Text = String.Empty;
            chkEspecial.Checked = false;
            GridViewComponente.DataSource = "";

        }
        private void CargarDataDetalle(DataTable dt)
        {
            condicionAcabado = "";
            foreach (DataRow row in dt.Rows)
            {
                int decre = Int32.Parse(row["Aplica_Decremento"].ToString());
                int extra = Int32.Parse(row["extra"].ToString());
                int cort = Int32.Parse(row["corte"].ToString());
                int mecanizado = row["mecanizado"].ToString() != "" ? Int32.Parse(row["mecanizado"].ToString()) : 0;
                bool adrecre = decre == 1 ? true : false;
                bool extracomp = extra == 1 ? true : false;
                bindingSource1.Add(new Sub_Component(row["Codigo"].ToString(), row["Descripcion"].ToString(), (int)row["Cantidad_Default"], (int)row["Cantidad_Adicional"], row["Id_Unidad_Calculada"].ToString(), adrecre, (int)row["Id_Subcomponente"], row["elevado"].ToString(), cort, extracomp, (int)row["Medida"], mecanizado, (int)row["Asignacion_puertas"]));


                string valorinicial = condicionAcabado == "" ? "'" : ",'";
                condicionAcabado += valorinicial + row["Codigo"].ToString().Split('-')[1] + "'";
            }

            GridViewComponente.DataSource = bindingSource1;
            GridViewComponente.Refresh();

            GetAcabadoSelect(condicionAcabado);

            if (dt.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in GridViewComponente.Rows)
                {
                    int uni = Int32.Parse(dt.Rows[row.Index]["Id_Unidad_Calculada"].ToString());
                    if (uni > 0)
                    {
                        DataGridViewComboBoxCell comboBoxCell = (DataGridViewComboBoxCell)(row.Cells[3]);
                        comboBoxCell.Value = uni;
                    }

                    long Cort = Int64.Parse(dt.Rows[row.Index]["Corte"].ToString());
                    if (Cort > 0)
                    {
                        DataGridViewComboBoxCell comboBoxCell = (DataGridViewComboBoxCell)(row.Cells[8]);
                        comboBoxCell.Value = Cort;
                    }

                    int Medida = Int32.Parse(dt.Rows[row.Index]["Medida"].ToString());
                    if (Medida > 0)
                    {
                        DataGridViewComboBoxCell comboBoxCell = (DataGridViewComboBoxCell)(row.Cells[10]);
                        comboBoxCell.Value = Medida;
                    }

                    long Mecanizado = dt.Rows[row.Index]["Mecanizado"].ToString() == "" ? 0 : Int64.Parse(dt.Rows[row.Index]["Mecanizado"].ToString());
                    if (Mecanizado > 0)
                    {
                        DataGridViewComboBoxCell comboBoxCell = (DataGridViewComboBoxCell)(row.Cells[11]);
                        comboBoxCell.Value = Mecanizado;
                    }

                    int A_Puerta = Int32.Parse(dt.Rows[row.Index]["Asignacion_puertas"].ToString());
                    row.DefaultCellStyle.BackColor = A_Puerta > 0 ? Color.LightGreen : Color.White;


                }
            }

            GridViewComponente.Enabled = false;

        }
        private void CargarDataDetalleEspecial(DataTable dt)
        {
            condicionAcabado = condicionAcabado == "" ? "" : condicionAcabado;
            foreach (DataRow row in dt.Rows)
            {
                bindingSource2.Add(new Sub_ComponentEspecial(row["Codigo"].ToString(), row["Descripcion"].ToString(), row["Id_Columna"].ToString(), (int)row["Cantidad_Default"], (int)row["Cantidad_Adicional"], (int)row["Id_Subcomponente"]));
                string valorinicial = condicionAcabado == "" ? "'" : ",'";
                condicionAcabado += valorinicial + row["Codigo"].ToString().Split('-')[1] + "'";
            }

            GridViewComponenteEsp.DataSource = bindingSource2;
            GridViewComponenteEsp.Refresh();
            GetAcabadoSelect(condicionAcabado);

            if (dt.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in GridViewComponenteEsp.Rows)
                {
                    int uni = Int32.Parse(dt.Rows[row.Index]["Id_Columna"].ToString());
                    if (uni > 0)
                    {
                        DataGridViewComboBoxCell comboBoxCell = (DataGridViewComboBoxCell)(row.Cells[3]);
                        comboBoxCell.Value = uni;
                    }
                }
            }

            GridViewComponenteEsp.Enabled = false;
        }
        private void HabilitarEspecial(bool swespecial)
        {
            if (swespecial == true)
            {
                GridViewComponente.Height = 143;
                GridViewComponenteEsp.Visible = true;
            }
            else
            {
                GridViewComponente.Height = 300;
                GridViewComponenteEsp.DataSource = "";
                GridViewComponenteEsp.Visible = false;
            }
        }

        private void GetAcabadoSelect(string condicion)
        {
            Dto.AcabadoDto Acb = new Dto.AcabadoDto();
            CmbAcabado.DataSource = Acb.GetAcabadoParam(condicion);
            CmbAcabado.DisplayMember = "Descripcion";
            CmbAcabado.ValueMember = "Id_Acabado";
        }
        #endregion

        private void GridViewComponente_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            try
            {
                //To handle 'ConstraintException' default error dialog (for example, unique value)
                if ((e.Exception) is System.Data.ConstraintException)
                {
                    // ErrorText glyphs show
                    GridViewComponente.Rows[e.RowIndex].ErrorText = "must be unique value";
                    GridViewComponente.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "must be unique value";

                    //...or MessageBox show
                    MessageBox.Show(e.Exception.Message, "Error ConstraintException",
                                                   MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //Suppress a ConstraintException
                    e.ThrowException = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR: dataGridView1_DataError",
                                         MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void chkEspecial_CheckedChanged(object sender, EventArgs e)
        {

            HabilitarEspecial(this.chkEspecial.Checked);
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            FrmBuscar bsc = new FrmBuscar();
            bsc.Consulta = "SubComp";
            bsc.ShowDialog();


            if (bsc.ReturnItem0 == null && bsc.ReturnItem4 == null)
            {
                return;
            }


            if (bsc.ReturnItem0 != null && bsc.ReturnItem4 == "0")
            {
                bindingSource1.Add(new Sub_Component(bsc.ReturnItem1, bsc.ReturnItem2, 1, 30, "", false, Int32.Parse(bsc.ReturnItem0), "", 0, false, 1,0,0));

                GridViewComponente.DataSource = bindingSource1;

                string valorinicial = condicionAcabado == "" ? "'" : ",'";
                condicionAcabado += valorinicial + bsc.ReturnItem1.ToString().Split('-')[1].Trim() + "'";

                GetAcabadoSelect(condicionAcabado);
            }
            else
            {
                if (bsc.ReturnItem0 != null && chkEspecial.Checked)
                {
                    bindingSource2.Add(new Sub_ComponentEspecial(bsc.ReturnItem1, bsc.ReturnItem2, "", 1, 1, Int32.Parse(bsc.ReturnItem0)));

                    GridViewComponenteEsp.DataSource = bindingSource2;

                    string valorinicial = condicionAcabado == "" ? "'" : ",'";
                    condicionAcabado += valorinicial + bsc.ReturnItem1.ToString().Split('-')[1].Trim() + "'";

                    GetAcabadoSelect(condicionAcabado);

                }
                else
                {
                    MessageBox.Show("Debe Primero Identificar que es un Componente Especial!!", "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
        }

        private void EliCtrlButtons_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void GridViewComponente_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (GridViewComponente.Rows[e.RowIndex].Cells[3].Value.ToString() == "6") {

                FrmDataAmbas bsc = new FrmDataAmbas();
                string datavalue = GridViewComponente.Rows[e.RowIndex].Cells[7].Value.ToString();
                if (datavalue == "0" || datavalue == "")
                {
                    bsc.ReturnItem0 = CantiAdiAnch;
                    bsc.ReturnItem1 = AplDecreAnch;
                }
                else
                {
                    bsc.ReturnItem0 = decimal.Parse(GridViewComponente.Rows[e.RowIndex].Cells[7].Value.ToString().Split(';')[0].Split('|')[1]);
                    bsc.ReturnItem1 = bool.Parse(GridViewComponente.Rows[e.RowIndex].Cells[7].Value.ToString().Split(';')[1].Split('|')[1]);
                }
                
               
                bsc.ShowDialog();
                GridViewComponente.Rows[e.RowIndex].Cells[7].Value = "Cant-Adi|" + bsc.ReturnItem0 + ";Apli-Decr|" + bsc.ReturnItem1;
                CantiAdiAnch = bsc.ReturnItem0;
                AplDecreAnch = bsc.ReturnItem1;
            }

                
           


            
        
    }

        private void GridViewComponente_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                // Seleccionar la celda
                GridViewComponente.ClearSelection();
                GridViewComponente.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;
                selectedRowIndex = e.RowIndex;

                // Mostrar el menú contextual
                GridViewComponente.ContextMenuStrip.Show(Cursor.Position);
            }
        }

        private void Item1_AP_Click(object sender, EventArgs e)
        {
            if (selectedRowIndex >= 0)
            {
                string value = GridViewComponente.Rows[selectedRowIndex].Cells[12].Value.ToString() == "1" ? "0" : "1"; // Obtener el valor de AP             
                GridViewComponente.Rows[selectedRowIndex].Cells[12].Value = value;
                GridViewComponente.Rows[selectedRowIndex].DefaultCellStyle.BackColor = value == "1" ? Color.LightGreen : Color.White;                
            }
        }

    }
}
