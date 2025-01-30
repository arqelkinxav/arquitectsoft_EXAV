
namespace arquitectSoft.View
{
    partial class FrmComponente
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmComponente));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.ImgLista = new System.Windows.Forms.ImageList(this.components);
            this.GridViewComponente = new System.Windows.Forms.DataGridView();
            this.arquitectdbDataSet = new arquitectSoft.arquitectdbDataSet();
            this.unidadescalculadasBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.unidades_calculadasTableAdapter = new arquitectSoft.arquitectdbDataSetTableAdapters.unidades_calculadasTableAdapter();
            this.GridViewComponenteEsp = new System.Windows.Forms.DataGridView();
            this.chkEspecial = new System.Windows.Forms.CheckBox();
            this.BtnCheck = new System.Windows.Forms.Button();
            this.CmbAcabado = new System.Windows.Forms.ComboBox();
            this.ImgListFinal = new System.Windows.Forms.ImageList(this.components);
            this.txtDescripcion = new System.Windows.Forms.TextBox();
            this.txtCodigo = new System.Windows.Forms.TextBox();
            this.lblEtiquetaCodigo = new System.Windows.Forms.Label();
            this.lbletiquetaDescripcion = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.bindingSource2 = new System.Windows.Forms.BindingSource(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.BtnNuevo = new System.Windows.Forms.ToolStripMenuItem();
            this.BtnGuardar = new System.Windows.Forms.ToolStripMenuItem();
            this.BtnEditar = new System.Windows.Forms.ToolStripMenuItem();
            this.BtnEliminar = new System.Windows.Forms.ToolStripMenuItem();
            this.BtnBuscar = new System.Windows.Forms.ToolStripMenuItem();
            this.BtnCancelar = new System.Windows.Forms.ToolStripMenuItem();
            this.BtnDuplicar = new System.Windows.Forms.ToolStripMenuItem();
            this.salirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnMaximizar = new System.Windows.Forms.ToolStripMenuItem();
            this.iconMenuItem1 = new FontAwesome.Sharp.IconMenuItem();
            this.BtnAgregar = new System.Windows.Forms.ToolStripMenuItem();
            this.BtnBorrar = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.codigoDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.descripcionDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cxdefectoDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cAdicionalDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.unidadCalculadaDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.aDecrementoDataGridViewCheckBoxColumn1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.idSubcomponenteDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.elevadoDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cortesDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.subComponentBindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.subComponentBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.elipseComponent1 = new arquitectSoft.Generals.ElipseComponent();
            ((System.ComponentModel.ISupportInitialize)(this.GridViewComponente)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.arquitectdbDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.unidadescalculadasBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GridViewComponenteEsp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource2)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.subComponentBindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.subComponentBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // ImgLista
            // 
            this.ImgLista.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ImgLista.ImageStream")));
            this.ImgLista.TransparentColor = System.Drawing.Color.Transparent;
            this.ImgLista.Images.SetKeyName(0, "001-bag.png");
            this.ImgLista.Images.SetKeyName(1, "002-book.png");
            this.ImgLista.Images.SetKeyName(2, "003-box.png");
            this.ImgLista.Images.SetKeyName(3, "004-box.png");
            this.ImgLista.Images.SetKeyName(4, "005-box.png");
            this.ImgLista.Images.SetKeyName(5, "006-browser.png");
            this.ImgLista.Images.SetKeyName(6, "007-cabinet.png");
            this.ImgLista.Images.SetKeyName(7, "008-calculator.png");
            this.ImgLista.Images.SetKeyName(8, "009-calendar.png");
            this.ImgLista.Images.SetKeyName(9, "010-chart.png");
            this.ImgLista.Images.SetKeyName(10, "011-clock.png");
            this.ImgLista.Images.SetKeyName(11, "012-computer.png");
            this.ImgLista.Images.SetKeyName(12, "013-control.png");
            this.ImgLista.Images.SetKeyName(13, "014-fax.png");
            this.ImgLista.Images.SetKeyName(14, "015-cabinet.png");
            this.ImgLista.Images.SetKeyName(15, "016-file.png");
            this.ImgLista.Images.SetKeyName(16, "017-files.png");
            this.ImgLista.Images.SetKeyName(17, "018-flashdrive.png");
            this.ImgLista.Images.SetKeyName(18, "019-folder.png");
            this.ImgLista.Images.SetKeyName(19, "020-floppy disk.png");
            this.ImgLista.Images.SetKeyName(20, "021-goal.png");
            this.ImgLista.Images.SetKeyName(21, "022-harddisk.png");
            this.ImgLista.Images.SetKeyName(22, "023-id card.png");
            this.ImgLista.Images.SetKeyName(23, "024-life ring.png");
            this.ImgLista.Images.SetKeyName(24, "025-lock.png");
            this.ImgLista.Images.SetKeyName(25, "026-mail.png");
            this.ImgLista.Images.SetKeyName(26, "027-marker.png");
            this.ImgLista.Images.SetKeyName(27, "028-message.png");
            this.ImgLista.Images.SetKeyName(28, "029-megaphone.png");
            this.ImgLista.Images.SetKeyName(29, "030-note.png");
            this.ImgLista.Images.SetKeyName(30, "031-notepad.png");
            this.ImgLista.Images.SetKeyName(31, "032-office desk.png");
            this.ImgLista.Images.SetKeyName(32, "033-office.png");
            this.ImgLista.Images.SetKeyName(33, "034-paper clip.png");
            this.ImgLista.Images.SetKeyName(34, "035-pen.png");
            this.ImgLista.Images.SetKeyName(35, "036-pencil.png");
            this.ImgLista.Images.SetKeyName(36, "037-pin.png");
            this.ImgLista.Images.SetKeyName(37, "038-printer.png");
            this.ImgLista.Images.SetKeyName(38, "039-protect.png");
            this.ImgLista.Images.SetKeyName(39, "040-recycle bin.png");
            this.ImgLista.Images.SetKeyName(40, "041-reward.png");
            this.ImgLista.Images.SetKeyName(41, "042-rubber.png");
            this.ImgLista.Images.SetKeyName(42, "043-send.png");
            this.ImgLista.Images.SetKeyName(43, "044-setting.png");
            this.ImgLista.Images.SetKeyName(44, "045-sharpener.png");
            this.ImgLista.Images.SetKeyName(45, "046-clip.png");
            this.ImgLista.Images.SetKeyName(46, "047-stamp.png");
            this.ImgLista.Images.SetKeyName(47, "048-tea.png");
            this.ImgLista.Images.SetKeyName(48, "049-tool.png");
            this.ImgLista.Images.SetKeyName(49, "050-view.png");
            this.ImgLista.Images.SetKeyName(50, "051-Close.png");
            this.ImgLista.Images.SetKeyName(51, "10.png");
            // 
            // GridViewComponente
            // 
            this.GridViewComponente.AllowUserToAddRows = false;
            this.GridViewComponente.AllowUserToDeleteRows = false;
            this.GridViewComponente.AllowUserToResizeColumns = false;
            this.GridViewComponente.AllowUserToResizeRows = false;
            this.GridViewComponente.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GridViewComponente.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.GridViewComponente.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Desktop;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.GridViewComponente.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.GridViewComponente.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.GridViewComponente.Enabled = false;
            this.GridViewComponente.EnableHeadersVisualStyles = false;
            this.GridViewComponente.GridColor = System.Drawing.SystemColors.Control;
            this.GridViewComponente.Location = new System.Drawing.Point(0, 232);
            this.GridViewComponente.Margin = new System.Windows.Forms.Padding(4);
            this.GridViewComponente.Name = "GridViewComponente";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.GridViewComponente.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.GridViewComponente.RowHeadersWidth = 51;
            this.GridViewComponente.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.GridViewComponente.Size = new System.Drawing.Size(1795, 317);
            this.GridViewComponente.TabIndex = 23;
            this.GridViewComponente.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.GridViewComponente_CellContentDoubleClick);
            this.GridViewComponente.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.GridViewComponente_CellMouseDown);
            this.GridViewComponente.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.GridViewComponente_DataError);
            this.GridViewComponente.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.GridViewComponente_EditingControlShowing);
            // 
            // arquitectdbDataSet
            // 
            this.arquitectdbDataSet.DataSetName = "arquitectdbDataSet";
            this.arquitectdbDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // unidadescalculadasBindingSource
            // 
            this.unidadescalculadasBindingSource.DataMember = "unidades_calculadas";
            this.unidadescalculadasBindingSource.DataSource = this.arquitectdbDataSet;
            // 
            // unidades_calculadasTableAdapter
            // 
            this.unidades_calculadasTableAdapter.ClearBeforeFill = true;
            // 
            // GridViewComponenteEsp
            // 
            this.GridViewComponenteEsp.AllowUserToAddRows = false;
            this.GridViewComponenteEsp.AllowUserToDeleteRows = false;
            this.GridViewComponenteEsp.AllowUserToResizeColumns = false;
            this.GridViewComponenteEsp.AllowUserToResizeRows = false;
            this.GridViewComponenteEsp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GridViewComponenteEsp.AutoGenerateColumns = false;
            this.GridViewComponenteEsp.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.GridViewComponenteEsp.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.GridViewComponenteEsp.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.GridViewComponenteEsp.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.GridViewComponenteEsp.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.codigoDataGridViewTextBoxColumn1,
            this.descripcionDataGridViewTextBoxColumn1,
            this.cxdefectoDataGridViewTextBoxColumn1,
            this.cAdicionalDataGridViewTextBoxColumn1,
            this.unidadCalculadaDataGridViewTextBoxColumn1,
            this.aDecrementoDataGridViewCheckBoxColumn1,
            this.idSubcomponenteDataGridViewTextBoxColumn1,
            this.elevadoDataGridViewTextBoxColumn1,
            this.cortesDataGridViewTextBoxColumn1});
            this.GridViewComponenteEsp.DataSource = this.subComponentBindingSource1;
            this.GridViewComponenteEsp.Enabled = false;
            this.GridViewComponenteEsp.EnableHeadersVisualStyles = false;
            this.GridViewComponenteEsp.Location = new System.Drawing.Point(0, 557);
            this.GridViewComponenteEsp.Margin = new System.Windows.Forms.Padding(4);
            this.GridViewComponenteEsp.Name = "GridViewComponenteEsp";
            this.GridViewComponenteEsp.RowHeadersWidth = 51;
            this.GridViewComponenteEsp.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.GridViewComponenteEsp.Size = new System.Drawing.Size(1795, 334);
            this.GridViewComponenteEsp.TabIndex = 29;
            this.GridViewComponenteEsp.Visible = false;
            // 
            // chkEspecial
            // 
            this.chkEspecial.AutoSize = true;
            this.chkEspecial.BackColor = System.Drawing.Color.White;
            this.chkEspecial.ForeColor = System.Drawing.Color.Black;
            this.chkEspecial.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chkEspecial.Location = new System.Drawing.Point(1176, 39);
            this.chkEspecial.Margin = new System.Windows.Forms.Padding(4);
            this.chkEspecial.Name = "chkEspecial";
            this.chkEspecial.Size = new System.Drawing.Size(114, 19);
            this.chkEspecial.TabIndex = 22;
            this.chkEspecial.Text = "Vidrios/Paneles";
            this.chkEspecial.UseVisualStyleBackColor = false;
            this.chkEspecial.CheckedChanged += new System.EventHandler(this.chkEspecial_CheckedChanged);
            // 
            // BtnCheck
            // 
            this.BtnCheck.ImageList = this.ImgLista;
            this.BtnCheck.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnCheck.Location = new System.Drawing.Point(195, 64);
            this.BtnCheck.Margin = new System.Windows.Forms.Padding(4);
            this.BtnCheck.Name = "BtnCheck";
            this.BtnCheck.Size = new System.Drawing.Size(85, 28);
            this.BtnCheck.TabIndex = 24;
            this.BtnCheck.Text = "Validar";
            this.BtnCheck.UseVisualStyleBackColor = true;
            this.BtnCheck.Click += new System.EventHandler(this.BtnCheck_Click);
            // 
            // CmbAcabado
            // 
            this.CmbAcabado.FormattingEnabled = true;
            this.CmbAcabado.Location = new System.Drawing.Point(922, 63);
            this.CmbAcabado.Margin = new System.Windows.Forms.Padding(4);
            this.CmbAcabado.Name = "CmbAcabado";
            this.CmbAcabado.Size = new System.Drawing.Size(387, 24);
            this.CmbAcabado.TabIndex = 44;
            // 
            // ImgListFinal
            // 
            this.ImgListFinal.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ImgListFinal.ImageStream")));
            this.ImgListFinal.TransparentColor = System.Drawing.Color.Transparent;
            this.ImgListFinal.Images.SetKeyName(0, "01.png");
            this.ImgListFinal.Images.SetKeyName(1, "02.png");
            this.ImgListFinal.Images.SetKeyName(2, "03.png");
            this.ImgListFinal.Images.SetKeyName(3, "05.png");
            this.ImgListFinal.Images.SetKeyName(4, "06.png");
            this.ImgListFinal.Images.SetKeyName(5, "07.png");
            this.ImgListFinal.Images.SetKeyName(6, "09.png");
            this.ImgListFinal.Images.SetKeyName(7, "10.png");
            this.ImgListFinal.Images.SetKeyName(8, "04.png");
            // 
            // txtDescripcion
            // 
            this.txtDescripcion.Location = new System.Drawing.Point(303, 63);
            this.txtDescripcion.Margin = new System.Windows.Forms.Padding(4);
            this.txtDescripcion.Name = "txtDescripcion";
            this.txtDescripcion.Size = new System.Drawing.Size(587, 20);
            this.txtDescripcion.TabIndex = 63;
            // 
            // txtCodigo
            // 
            this.txtCodigo.Location = new System.Drawing.Point(27, 63);
            this.txtCodigo.Margin = new System.Windows.Forms.Padding(4);
            this.txtCodigo.Name = "txtCodigo";
            this.txtCodigo.ReadOnly = true;
            this.txtCodigo.Size = new System.Drawing.Size(159, 20);
            this.txtCodigo.TabIndex = 64;
            // 
            // lblEtiquetaCodigo
            // 
            this.lblEtiquetaCodigo.AutoSize = true;
            this.lblEtiquetaCodigo.BackColor = System.Drawing.Color.White;
            this.lblEtiquetaCodigo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEtiquetaCodigo.ForeColor = System.Drawing.Color.Black;
            this.lblEtiquetaCodigo.Location = new System.Drawing.Point(10, 38);
            this.lblEtiquetaCodigo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblEtiquetaCodigo.Name = "lblEtiquetaCodigo";
            this.lblEtiquetaCodigo.Size = new System.Drawing.Size(66, 20);
            this.lblEtiquetaCodigo.TabIndex = 97;
            this.lblEtiquetaCodigo.Text = "Codigo:";
            // 
            // lbletiquetaDescripcion
            // 
            this.lbletiquetaDescripcion.AutoSize = true;
            this.lbletiquetaDescripcion.BackColor = System.Drawing.Color.White;
            this.lbletiquetaDescripcion.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbletiquetaDescripcion.ForeColor = System.Drawing.Color.Black;
            this.lbletiquetaDescripcion.Location = new System.Drawing.Point(292, 39);
            this.lbletiquetaDescripcion.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbletiquetaDescripcion.Name = "lbletiquetaDescripcion";
            this.lbletiquetaDescripcion.Size = new System.Drawing.Size(104, 20);
            this.lbletiquetaDescripcion.TabIndex = 98;
            this.lbletiquetaDescripcion.Text = "Descripcion:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.White;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(906, 37);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 20);
            this.label2.TabIndex = 99;
            this.label2.Text = "Acabado:";
            // 
            // menuStrip1
            // 
            this.menuStrip1.AllowMerge = false;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BtnNuevo,
            this.BtnGuardar,
            this.BtnEditar,
            this.BtnEliminar,
            this.BtnBuscar,
            this.BtnCancelar,
            this.BtnDuplicar,
            this.salirToolStripMenuItem,
            this.btnMaximizar,
            this.iconMenuItem1,
            this.BtnAgregar,
            this.BtnBorrar});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1803, 76);
            this.menuStrip1.TabIndex = 100;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // BtnNuevo
            // 
            this.BtnNuevo.Image = global::arquitectSoft.Properties.Resources.New;
            this.BtnNuevo.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.BtnNuevo.Name = "BtnNuevo";
            this.BtnNuevo.Size = new System.Drawing.Size(66, 72);
            this.BtnNuevo.Text = "Nuevo";
            this.BtnNuevo.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.BtnNuevo.Click += new System.EventHandler(this.BtnNuevo_Click);
            // 
            // BtnGuardar
            // 
            this.BtnGuardar.Image = global::arquitectSoft.Properties.Resources.icons8_save_48;
            this.BtnGuardar.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.BtnGuardar.Name = "BtnGuardar";
            this.BtnGuardar.Size = new System.Drawing.Size(76, 72);
            this.BtnGuardar.Text = "Guardar";
            this.BtnGuardar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.BtnGuardar.Click += new System.EventHandler(this.BtnGuardar_Click);
            // 
            // BtnEditar
            // 
            this.BtnEditar.Image = global::arquitectSoft.Properties.Resources.icons8_edit_48;
            this.BtnEditar.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.BtnEditar.Name = "BtnEditar";
            this.BtnEditar.Size = new System.Drawing.Size(62, 72);
            this.BtnEditar.Text = "Editar";
            this.BtnEditar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.BtnEditar.Click += new System.EventHandler(this.BtnEditar_Click);
            // 
            // BtnEliminar
            // 
            this.BtnEliminar.Image = global::arquitectSoft.Properties.Resources.icons8_trash_48;
            this.BtnEliminar.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.BtnEliminar.Name = "BtnEliminar";
            this.BtnEliminar.Size = new System.Drawing.Size(64, 72);
            this.BtnEliminar.Text = "Borrar";
            this.BtnEliminar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.BtnEliminar.Click += new System.EventHandler(this.BtnEliminar_Click);
            // 
            // BtnBuscar
            // 
            this.BtnBuscar.Image = global::arquitectSoft.Properties.Resources.icons8_search_48;
            this.BtnBuscar.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.BtnBuscar.Name = "BtnBuscar";
            this.BtnBuscar.Size = new System.Drawing.Size(85, 72);
            this.BtnBuscar.Text = "Consultar";
            this.BtnBuscar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.BtnBuscar.Click += new System.EventHandler(this.BtnBuscar_Click);
            // 
            // BtnCancelar
            // 
            this.BtnCancelar.Image = global::arquitectSoft.Properties.Resources.icons8_cancel_48;
            this.BtnCancelar.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.BtnCancelar.Name = "BtnCancelar";
            this.BtnCancelar.Size = new System.Drawing.Size(80, 72);
            this.BtnCancelar.Text = "Cancelar";
            this.BtnCancelar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.BtnCancelar.Click += new System.EventHandler(this.BtnCancelar_Click);
            // 
            // BtnDuplicar
            // 
            this.BtnDuplicar.Image = global::arquitectSoft.Properties.Resources.icons8_copy_48;
            this.BtnDuplicar.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.BtnDuplicar.Name = "BtnDuplicar";
            this.BtnDuplicar.Size = new System.Drawing.Size(79, 72);
            this.BtnDuplicar.Text = "Duplicar";
            this.BtnDuplicar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.BtnDuplicar.Click += new System.EventHandler(this.BtnDuplicar_Click);
            // 
            // salirToolStripMenuItem
            // 
            this.salirToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.salirToolStripMenuItem.Image = global::arquitectSoft.Properties.Resources.icons8_exit_48;
            this.salirToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.salirToolStripMenuItem.Name = "salirToolStripMenuItem";
            this.salirToolStripMenuItem.Size = new System.Drawing.Size(62, 72);
            this.salirToolStripMenuItem.Text = "Salir";
            this.salirToolStripMenuItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.salirToolStripMenuItem.Click += new System.EventHandler(this.BtnSalir_Click);
            // 
            // btnMaximizar
            // 
            this.btnMaximizar.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnMaximizar.Image = global::arquitectSoft.Properties.Resources.icons8_expand_48;
            this.btnMaximizar.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnMaximizar.Name = "btnMaximizar";
            this.btnMaximizar.Size = new System.Drawing.Size(92, 72);
            this.btnMaximizar.Text = "Maximizar";
            this.btnMaximizar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnMaximizar.Click += new System.EventHandler(this.btnMaximizar_Click);
            // 
            // iconMenuItem1
            // 
            this.iconMenuItem1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.iconMenuItem1.Enabled = false;
            this.iconMenuItem1.IconChar = FontAwesome.Sharp.IconChar.None;
            this.iconMenuItem1.IconColor = System.Drawing.Color.Black;
            this.iconMenuItem1.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconMenuItem1.Name = "iconMenuItem1";
            this.iconMenuItem1.Size = new System.Drawing.Size(59, 72);
            this.iconMenuItem1.Text = "------";
            // 
            // BtnAgregar
            // 
            this.BtnAgregar.Image = global::arquitectSoft.Properties.Resources.icons8_insert_table_48;
            this.BtnAgregar.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.BtnAgregar.Name = "BtnAgregar";
            this.BtnAgregar.Size = new System.Drawing.Size(77, 72);
            this.BtnAgregar.Text = "Agregar";
            this.BtnAgregar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.BtnAgregar.Click += new System.EventHandler(this.BtnAgregar_Click);
            // 
            // BtnBorrar
            // 
            this.BtnBorrar.Image = global::arquitectSoft.Properties.Resources.icons8_delete_table_48;
            this.BtnBorrar.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.BtnBorrar.Name = "BtnBorrar";
            this.BtnBorrar.Size = new System.Drawing.Size(64, 72);
            this.BtnBorrar.Text = "Borrar";
            this.BtnBorrar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.BtnBorrar.Click += new System.EventHandler(this.BtnBorrar_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.txtCodigo);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.chkEspecial);
            this.groupBox1.Controls.Add(this.lbletiquetaDescripcion);
            this.groupBox1.Controls.Add(this.BtnCheck);
            this.groupBox1.Controls.Add(this.lblEtiquetaCodigo);
            this.groupBox1.Controls.Add(this.CmbAcabado);
            this.groupBox1.Controls.Add(this.txtDescripcion);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.groupBox1.Location = new System.Drawing.Point(0, 103);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1803, 122);
            this.groupBox1.TabIndex = 101;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Info Data";
            // 
            // codigoDataGridViewTextBoxColumn1
            // 
            this.codigoDataGridViewTextBoxColumn1.DataPropertyName = "Codigo";
            this.codigoDataGridViewTextBoxColumn1.HeaderText = "Codigo";
            this.codigoDataGridViewTextBoxColumn1.MinimumWidth = 6;
            this.codigoDataGridViewTextBoxColumn1.Name = "codigoDataGridViewTextBoxColumn1";
            this.codigoDataGridViewTextBoxColumn1.Width = 81;
            // 
            // descripcionDataGridViewTextBoxColumn1
            // 
            this.descripcionDataGridViewTextBoxColumn1.DataPropertyName = "Descripcion";
            this.descripcionDataGridViewTextBoxColumn1.HeaderText = "Descripcion";
            this.descripcionDataGridViewTextBoxColumn1.MinimumWidth = 6;
            this.descripcionDataGridViewTextBoxColumn1.Name = "descripcionDataGridViewTextBoxColumn1";
            this.descripcionDataGridViewTextBoxColumn1.Width = 111;
            // 
            // cxdefectoDataGridViewTextBoxColumn1
            // 
            this.cxdefectoDataGridViewTextBoxColumn1.DataPropertyName = "Cxdefecto";
            this.cxdefectoDataGridViewTextBoxColumn1.HeaderText = "Cxdefecto";
            this.cxdefectoDataGridViewTextBoxColumn1.MinimumWidth = 6;
            this.cxdefectoDataGridViewTextBoxColumn1.Name = "cxdefectoDataGridViewTextBoxColumn1";
            this.cxdefectoDataGridViewTextBoxColumn1.Width = 99;
            // 
            // cAdicionalDataGridViewTextBoxColumn1
            // 
            this.cAdicionalDataGridViewTextBoxColumn1.DataPropertyName = "CAdicional";
            this.cAdicionalDataGridViewTextBoxColumn1.HeaderText = "CAdicional";
            this.cAdicionalDataGridViewTextBoxColumn1.MinimumWidth = 6;
            this.cAdicionalDataGridViewTextBoxColumn1.Name = "cAdicionalDataGridViewTextBoxColumn1";
            this.cAdicionalDataGridViewTextBoxColumn1.Width = 103;
            // 
            // unidadCalculadaDataGridViewTextBoxColumn1
            // 
            this.unidadCalculadaDataGridViewTextBoxColumn1.DataPropertyName = "UnidadCalculada";
            this.unidadCalculadaDataGridViewTextBoxColumn1.HeaderText = "UnidadCalculada";
            this.unidadCalculadaDataGridViewTextBoxColumn1.MinimumWidth = 6;
            this.unidadCalculadaDataGridViewTextBoxColumn1.Name = "unidadCalculadaDataGridViewTextBoxColumn1";
            this.unidadCalculadaDataGridViewTextBoxColumn1.Width = 144;
            // 
            // aDecrementoDataGridViewCheckBoxColumn1
            // 
            this.aDecrementoDataGridViewCheckBoxColumn1.DataPropertyName = "ADecremento";
            this.aDecrementoDataGridViewCheckBoxColumn1.HeaderText = "ADecremento";
            this.aDecrementoDataGridViewCheckBoxColumn1.MinimumWidth = 6;
            this.aDecrementoDataGridViewCheckBoxColumn1.Name = "aDecrementoDataGridViewCheckBoxColumn1";
            // 
            // idSubcomponenteDataGridViewTextBoxColumn1
            // 
            this.idSubcomponenteDataGridViewTextBoxColumn1.DataPropertyName = "IdSubcomponente";
            this.idSubcomponenteDataGridViewTextBoxColumn1.HeaderText = "IdSubcomponente";
            this.idSubcomponenteDataGridViewTextBoxColumn1.MinimumWidth = 6;
            this.idSubcomponenteDataGridViewTextBoxColumn1.Name = "idSubcomponenteDataGridViewTextBoxColumn1";
            this.idSubcomponenteDataGridViewTextBoxColumn1.Width = 151;
            // 
            // elevadoDataGridViewTextBoxColumn1
            // 
            this.elevadoDataGridViewTextBoxColumn1.DataPropertyName = "Elevado";
            this.elevadoDataGridViewTextBoxColumn1.HeaderText = "Elevado";
            this.elevadoDataGridViewTextBoxColumn1.MinimumWidth = 6;
            this.elevadoDataGridViewTextBoxColumn1.Name = "elevadoDataGridViewTextBoxColumn1";
            this.elevadoDataGridViewTextBoxColumn1.Width = 88;
            // 
            // cortesDataGridViewTextBoxColumn1
            // 
            this.cortesDataGridViewTextBoxColumn1.DataPropertyName = "Cortes";
            this.cortesDataGridViewTextBoxColumn1.HeaderText = "Cortes";
            this.cortesDataGridViewTextBoxColumn1.MinimumWidth = 6;
            this.cortesDataGridViewTextBoxColumn1.Name = "cortesDataGridViewTextBoxColumn1";
            this.cortesDataGridViewTextBoxColumn1.Width = 78;
            // 
            // subComponentBindingSource1
            // 
            this.subComponentBindingSource1.DataSource = typeof(arquitectSoft.Class.Sub_Component);
            // 
            // subComponentBindingSource
            // 
            this.subComponentBindingSource.DataSource = typeof(arquitectSoft.Class.Sub_Component);
            this.subComponentBindingSource.Sort = "Descripcion";
            // 
            // elipseComponent1
            // 
            this.elipseComponent1.CornerRadius = 15;
            this.elipseComponent1.TargetControl = this;
            // 
            // FrmComponente
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.ClientSize = new System.Drawing.Size(1803, 893);
            this.ControlBox = false;
            this.Controls.Add(this.GridViewComponenteEsp);
            this.Controls.Add(this.GridViewComponente);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimizeBox = false;
            this.Name = "FrmComponente";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Componentes";
            this.Load += new System.EventHandler(this.FrmComponente_Load);
            ((System.ComponentModel.ISupportInitialize)(this.GridViewComponente)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.arquitectdbDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.unidadescalculadasBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GridViewComponenteEsp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource2)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.subComponentBindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.subComponentBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ImageList ImgLista;
        private System.Windows.Forms.DataGridView GridViewComponente;
        private arquitectdbDataSet arquitectdbDataSet;
        private System.Windows.Forms.BindingSource unidadescalculadasBindingSource;
        private arquitectdbDataSetTableAdapters.unidades_calculadasTableAdapter unidades_calculadasTableAdapter;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.BindingSource subComponentBindingSource1;
        private System.Windows.Forms.DataGridView GridViewComponenteEsp;
        private System.Windows.Forms.BindingSource bindingSource2;
        private System.Windows.Forms.DataGridViewTextBoxColumn codigoDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn descripcionDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn cxdefectoDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn cAdicionalDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn unidadCalculadaDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn aDecrementoDataGridViewCheckBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn idSubcomponenteDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn elevadoDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn cortesDataGridViewTextBoxColumn1;
        private Generals.ElipseComponent elipseComponent1;
        private System.Windows.Forms.ComboBox CmbAcabado;
        private System.Windows.Forms.Button BtnCheck;
        private System.Windows.Forms.CheckBox chkEspecial;
        private System.Windows.Forms.ImageList ImgListFinal;
        private System.Windows.Forms.TextBox txtDescripcion;
        private System.Windows.Forms.TextBox txtCodigo;
        private System.Windows.Forms.Label lblEtiquetaCodigo;
        private System.Windows.Forms.Label lbletiquetaDescripcion;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem BtnNuevo;
        private System.Windows.Forms.ToolStripMenuItem BtnGuardar;
        private System.Windows.Forms.ToolStripMenuItem BtnEditar;
        private System.Windows.Forms.ToolStripMenuItem BtnEliminar;
        private System.Windows.Forms.ToolStripMenuItem BtnBuscar;
        private System.Windows.Forms.ToolStripMenuItem BtnCancelar;
        private System.Windows.Forms.ToolStripMenuItem BtnDuplicar;
        private System.Windows.Forms.ToolStripMenuItem salirToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem btnMaximizar;
        private System.Windows.Forms.GroupBox groupBox1;
        private FontAwesome.Sharp.IconMenuItem iconMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem BtnAgregar;
        private System.Windows.Forms.ToolStripMenuItem BtnBorrar;
        public System.Windows.Forms.BindingSource subComponentBindingSource;
    }
}