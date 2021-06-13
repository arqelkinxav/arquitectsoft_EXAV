
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.ImgLista = new System.Windows.Forms.ImageList(this.components);
            this.lblEtiquetaCodigo = new System.Windows.Forms.Label();
            this.txtCodigo = new System.Windows.Forms.TextBox();
            this.txtDescripcion = new System.Windows.Forms.TextBox();
            this.lbletiquetaDescripcion = new System.Windows.Forms.Label();
            this.chkEspecial = new System.Windows.Forms.CheckBox();
            this.GridViewComponente = new System.Windows.Forms.DataGridView();
            this.BtnCheck = new System.Windows.Forms.Button();
            this.BtnAgregar = new System.Windows.Forms.Button();
            this.BtnBorrar = new System.Windows.Forms.Button();
            this.arquitectdbDataSet = new arquitectSoft.arquitectdbDataSet();
            this.unidadescalculadasBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.unidades_calculadasTableAdapter = new arquitectSoft.arquitectdbDataSetTableAdapters.unidades_calculadasTableAdapter();
            this.BtnCancelar = new System.Windows.Forms.Button();
            this.BtnSalir = new System.Windows.Forms.Button();
            this.BtnBuscar = new System.Windows.Forms.Button();
            this.BtnNuevo = new System.Windows.Forms.Button();
            this.BtnEliminar = new System.Windows.Forms.Button();
            this.BtnEditar = new System.Windows.Forms.Button();
            this.BtnGuardar = new System.Windows.Forms.Button();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.BtnDuplicar = new System.Windows.Forms.Button();
            this.GridViewComponenteEsp = new System.Windows.Forms.DataGridView();
            this.bindingSource2 = new System.Windows.Forms.BindingSource(this.components);
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
            this.codigoDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.descripcionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cxdefectoDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cAdicionalDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.unidadCalculadaDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.aDecrementoDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.idSubcomponenteDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.elevadoDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cortesDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.subComponentBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GridViewComponente)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.arquitectdbDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.unidadescalculadasBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GridViewComponenteEsp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.subComponentBindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.subComponentBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(158)))), ((int)(((byte)(158)))));
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(1, 75);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(919, 32);
            this.panel1.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(389, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 24);
            this.label1.TabIndex = 6;
            this.label1.Text = "Componente";
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
            // 
            // lblEtiquetaCodigo
            // 
            this.lblEtiquetaCodigo.AutoSize = true;
            this.lblEtiquetaCodigo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEtiquetaCodigo.ForeColor = System.Drawing.SystemColors.Control;
            this.lblEtiquetaCodigo.Location = new System.Drawing.Point(20, 124);
            this.lblEtiquetaCodigo.Name = "lblEtiquetaCodigo";
            this.lblEtiquetaCodigo.Size = new System.Drawing.Size(55, 16);
            this.lblEtiquetaCodigo.TabIndex = 18;
            this.lblEtiquetaCodigo.Text = "Codigo:";
            // 
            // txtCodigo
            // 
            this.txtCodigo.Location = new System.Drawing.Point(23, 143);
            this.txtCodigo.Name = "txtCodigo";
            this.txtCodigo.Size = new System.Drawing.Size(122, 20);
            this.txtCodigo.TabIndex = 19;
            // 
            // txtDescripcion
            // 
            this.txtDescripcion.Location = new System.Drawing.Point(236, 143);
            this.txtDescripcion.Name = "txtDescripcion";
            this.txtDescripcion.Size = new System.Drawing.Size(417, 20);
            this.txtDescripcion.TabIndex = 21;
            // 
            // lbletiquetaDescripcion
            // 
            this.lbletiquetaDescripcion.AutoSize = true;
            this.lbletiquetaDescripcion.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbletiquetaDescripcion.ForeColor = System.Drawing.SystemColors.Control;
            this.lbletiquetaDescripcion.Location = new System.Drawing.Point(233, 124);
            this.lbletiquetaDescripcion.Name = "lbletiquetaDescripcion";
            this.lbletiquetaDescripcion.Size = new System.Drawing.Size(83, 16);
            this.lbletiquetaDescripcion.TabIndex = 20;
            this.lbletiquetaDescripcion.Text = "Descripcion:";
            // 
            // chkEspecial
            // 
            this.chkEspecial.AutoSize = true;
            this.chkEspecial.ForeColor = System.Drawing.SystemColors.Control;
            this.chkEspecial.Location = new System.Drawing.Point(727, 145);
            this.chkEspecial.Name = "chkEspecial";
            this.chkEspecial.Size = new System.Drawing.Size(66, 17);
            this.chkEspecial.TabIndex = 22;
            this.chkEspecial.Text = "Especial";
            this.chkEspecial.UseVisualStyleBackColor = true;
            this.chkEspecial.CheckedChanged += new System.EventHandler(this.chkEspecial_CheckedChanged);
            // 
            // GridViewComponente
            // 
            this.GridViewComponente.AllowUserToAddRows = false;
            this.GridViewComponente.AllowUserToDeleteRows = false;
            this.GridViewComponente.AllowUserToResizeColumns = false;
            this.GridViewComponente.AllowUserToResizeRows = false;
            this.GridViewComponente.AutoGenerateColumns = false;
            this.GridViewComponente.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.GridViewComponente.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.GridViewComponente.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.codigoDataGridViewTextBoxColumn,
            this.descripcionDataGridViewTextBoxColumn,
            this.cxdefectoDataGridViewTextBoxColumn,
            this.cAdicionalDataGridViewTextBoxColumn,
            this.unidadCalculadaDataGridViewTextBoxColumn,
            this.aDecrementoDataGridViewCheckBoxColumn,
            this.idSubcomponenteDataGridViewTextBoxColumn,
            this.elevadoDataGridViewTextBoxColumn,
            this.cortesDataGridViewTextBoxColumn});
            this.GridViewComponente.DataSource = this.subComponentBindingSource;
            this.GridViewComponente.Enabled = false;
            this.GridViewComponente.Location = new System.Drawing.Point(1, 179);
            this.GridViewComponente.Name = "GridViewComponente";
            this.GridViewComponente.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.GridViewComponente.Size = new System.Drawing.Size(919, 300);
            this.GridViewComponente.TabIndex = 23;
            this.GridViewComponente.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.GridViewComponente_DataError);
            // 
            // BtnCheck
            // 
            this.BtnCheck.ImageList = this.ImgLista;
            this.BtnCheck.Location = new System.Drawing.Point(151, 141);
            this.BtnCheck.Name = "BtnCheck";
            this.BtnCheck.Size = new System.Drawing.Size(64, 23);
            this.BtnCheck.TabIndex = 24;
            this.BtnCheck.Text = "Validar";
            this.BtnCheck.UseVisualStyleBackColor = true;
            this.BtnCheck.Click += new System.EventHandler(this.BtnCheck_Click);
            // 
            // BtnAgregar
            // 
            this.BtnAgregar.ImageList = this.ImgLista;
            this.BtnAgregar.Location = new System.Drawing.Point(3, 491);
            this.BtnAgregar.Name = "BtnAgregar";
            this.BtnAgregar.Size = new System.Drawing.Size(58, 23);
            this.BtnAgregar.TabIndex = 25;
            this.BtnAgregar.Text = "Agregar";
            this.BtnAgregar.UseVisualStyleBackColor = true;
            this.BtnAgregar.Click += new System.EventHandler(this.BtnAgregar_Click);
            // 
            // BtnBorrar
            // 
            this.BtnBorrar.ImageList = this.ImgLista;
            this.BtnBorrar.Location = new System.Drawing.Point(66, 491);
            this.BtnBorrar.Name = "BtnBorrar";
            this.BtnBorrar.Size = new System.Drawing.Size(58, 23);
            this.BtnBorrar.TabIndex = 26;
            this.BtnBorrar.Text = "Borrar";
            this.BtnBorrar.UseVisualStyleBackColor = true;
            this.BtnBorrar.Click += new System.EventHandler(this.BtnBorrar_Click);
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
            // BtnCancelar
            // 
            this.BtnCancelar.BackColor = System.Drawing.SystemColors.Control;
            this.BtnCancelar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.BtnCancelar.Enabled = false;
            this.BtnCancelar.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.BtnCancelar.ImageIndex = 3;
            this.BtnCancelar.ImageList = this.ImgLista;
            this.BtnCancelar.Location = new System.Drawing.Point(332, 9);
            this.BtnCancelar.Name = "BtnCancelar";
            this.BtnCancelar.Size = new System.Drawing.Size(58, 60);
            this.BtnCancelar.TabIndex = 27;
            this.BtnCancelar.Text = "Cancelar";
            this.BtnCancelar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.BtnCancelar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.BtnCancelar.UseVisualStyleBackColor = false;
            this.BtnCancelar.Click += new System.EventHandler(this.BtnCancelar_Click);
            // 
            // BtnSalir
            // 
            this.BtnSalir.BackColor = System.Drawing.SystemColors.Control;
            this.BtnSalir.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.BtnSalir.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.BtnSalir.ImageIndex = 50;
            this.BtnSalir.ImageList = this.ImgLista;
            this.BtnSalir.Location = new System.Drawing.Point(850, 9);
            this.BtnSalir.Name = "BtnSalir";
            this.BtnSalir.Size = new System.Drawing.Size(58, 60);
            this.BtnSalir.TabIndex = 17;
            this.BtnSalir.Text = "Salir";
            this.BtnSalir.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.BtnSalir.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.BtnSalir.UseVisualStyleBackColor = false;
            this.BtnSalir.Click += new System.EventHandler(this.BtnSalir_Click);
            // 
            // BtnBuscar
            // 
            this.BtnBuscar.BackColor = System.Drawing.SystemColors.Control;
            this.BtnBuscar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.BtnBuscar.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.BtnBuscar.ImageIndex = 49;
            this.BtnBuscar.ImageList = this.ImgLista;
            this.BtnBuscar.Location = new System.Drawing.Point(268, 9);
            this.BtnBuscar.Name = "BtnBuscar";
            this.BtnBuscar.Size = new System.Drawing.Size(58, 60);
            this.BtnBuscar.TabIndex = 16;
            this.BtnBuscar.Text = "Buscar";
            this.BtnBuscar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.BtnBuscar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.BtnBuscar.UseVisualStyleBackColor = false;
            this.BtnBuscar.Click += new System.EventHandler(this.BtnBuscar_Click);
            // 
            // BtnNuevo
            // 
            this.BtnNuevo.BackColor = System.Drawing.SystemColors.Control;
            this.BtnNuevo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.BtnNuevo.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.BtnNuevo.ImageIndex = 4;
            this.BtnNuevo.ImageList = this.ImgLista;
            this.BtnNuevo.Location = new System.Drawing.Point(12, 9);
            this.BtnNuevo.Name = "BtnNuevo";
            this.BtnNuevo.Size = new System.Drawing.Size(58, 60);
            this.BtnNuevo.TabIndex = 15;
            this.BtnNuevo.Text = "Nuevo";
            this.BtnNuevo.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.BtnNuevo.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.BtnNuevo.UseVisualStyleBackColor = false;
            this.BtnNuevo.Click += new System.EventHandler(this.BtnNuevo_Click);
            // 
            // BtnEliminar
            // 
            this.BtnEliminar.BackColor = System.Drawing.SystemColors.Control;
            this.BtnEliminar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.BtnEliminar.Enabled = false;
            this.BtnEliminar.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.BtnEliminar.ImageIndex = 39;
            this.BtnEliminar.ImageList = this.ImgLista;
            this.BtnEliminar.Location = new System.Drawing.Point(204, 9);
            this.BtnEliminar.Name = "BtnEliminar";
            this.BtnEliminar.Size = new System.Drawing.Size(58, 60);
            this.BtnEliminar.TabIndex = 14;
            this.BtnEliminar.Text = "Eliminar";
            this.BtnEliminar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.BtnEliminar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.BtnEliminar.UseVisualStyleBackColor = false;
            this.BtnEliminar.Click += new System.EventHandler(this.BtnEliminar_Click);
            // 
            // BtnEditar
            // 
            this.BtnEditar.BackColor = System.Drawing.SystemColors.Control;
            this.BtnEditar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.BtnEditar.Enabled = false;
            this.BtnEditar.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.BtnEditar.ImageIndex = 35;
            this.BtnEditar.ImageList = this.ImgLista;
            this.BtnEditar.Location = new System.Drawing.Point(140, 9);
            this.BtnEditar.Name = "BtnEditar";
            this.BtnEditar.Size = new System.Drawing.Size(58, 60);
            this.BtnEditar.TabIndex = 13;
            this.BtnEditar.Text = "Editar";
            this.BtnEditar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.BtnEditar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.BtnEditar.UseVisualStyleBackColor = false;
            this.BtnEditar.Click += new System.EventHandler(this.BtnEditar_Click);
            // 
            // BtnGuardar
            // 
            this.BtnGuardar.BackColor = System.Drawing.SystemColors.Control;
            this.BtnGuardar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.BtnGuardar.Enabled = false;
            this.BtnGuardar.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.BtnGuardar.ImageIndex = 19;
            this.BtnGuardar.ImageList = this.ImgLista;
            this.BtnGuardar.Location = new System.Drawing.Point(76, 9);
            this.BtnGuardar.Name = "BtnGuardar";
            this.BtnGuardar.Size = new System.Drawing.Size(58, 60);
            this.BtnGuardar.TabIndex = 12;
            this.BtnGuardar.Text = "Guardar";
            this.BtnGuardar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.BtnGuardar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.BtnGuardar.UseVisualStyleBackColor = false;
            this.BtnGuardar.Click += new System.EventHandler(this.BtnGuardar_Click);
            // 
            // BtnDuplicar
            // 
            this.BtnDuplicar.BackColor = System.Drawing.SystemColors.Control;
            this.BtnDuplicar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.BtnDuplicar.Enabled = false;
            this.BtnDuplicar.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.BtnDuplicar.ImageIndex = 6;
            this.BtnDuplicar.ImageList = this.ImgLista;
            this.BtnDuplicar.Location = new System.Drawing.Point(396, 9);
            this.BtnDuplicar.Name = "BtnDuplicar";
            this.BtnDuplicar.Size = new System.Drawing.Size(58, 60);
            this.BtnDuplicar.TabIndex = 28;
            this.BtnDuplicar.Text = "Duplicar";
            this.BtnDuplicar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.BtnDuplicar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.BtnDuplicar.UseVisualStyleBackColor = false;
            this.BtnDuplicar.Click += new System.EventHandler(this.BtnDuplicar_Click);
            // 
            // GridViewComponenteEsp
            // 
            this.GridViewComponenteEsp.AllowUserToAddRows = false;
            this.GridViewComponenteEsp.AllowUserToDeleteRows = false;
            this.GridViewComponenteEsp.AllowUserToResizeColumns = false;
            this.GridViewComponenteEsp.AllowUserToResizeRows = false;
            this.GridViewComponenteEsp.AutoGenerateColumns = false;
            this.GridViewComponenteEsp.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
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
            this.GridViewComponenteEsp.Location = new System.Drawing.Point(1, 328);
            this.GridViewComponenteEsp.Name = "GridViewComponenteEsp";
            this.GridViewComponenteEsp.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.GridViewComponenteEsp.Size = new System.Drawing.Size(919, 151);
            this.GridViewComponenteEsp.TabIndex = 29;
            this.GridViewComponenteEsp.Visible = false;
            // 
            // codigoDataGridViewTextBoxColumn1
            // 
            this.codigoDataGridViewTextBoxColumn1.DataPropertyName = "Codigo";
            this.codigoDataGridViewTextBoxColumn1.HeaderText = "Codigo";
            this.codigoDataGridViewTextBoxColumn1.Name = "codigoDataGridViewTextBoxColumn1";
            this.codigoDataGridViewTextBoxColumn1.Width = 65;
            // 
            // descripcionDataGridViewTextBoxColumn1
            // 
            this.descripcionDataGridViewTextBoxColumn1.DataPropertyName = "Descripcion";
            this.descripcionDataGridViewTextBoxColumn1.HeaderText = "Descripcion";
            this.descripcionDataGridViewTextBoxColumn1.Name = "descripcionDataGridViewTextBoxColumn1";
            this.descripcionDataGridViewTextBoxColumn1.Width = 88;
            // 
            // cxdefectoDataGridViewTextBoxColumn1
            // 
            this.cxdefectoDataGridViewTextBoxColumn1.DataPropertyName = "Cxdefecto";
            this.cxdefectoDataGridViewTextBoxColumn1.HeaderText = "Cxdefecto";
            this.cxdefectoDataGridViewTextBoxColumn1.Name = "cxdefectoDataGridViewTextBoxColumn1";
            this.cxdefectoDataGridViewTextBoxColumn1.Width = 80;
            // 
            // cAdicionalDataGridViewTextBoxColumn1
            // 
            this.cAdicionalDataGridViewTextBoxColumn1.DataPropertyName = "CAdicional";
            this.cAdicionalDataGridViewTextBoxColumn1.HeaderText = "CAdicional";
            this.cAdicionalDataGridViewTextBoxColumn1.Name = "cAdicionalDataGridViewTextBoxColumn1";
            this.cAdicionalDataGridViewTextBoxColumn1.Width = 82;
            // 
            // unidadCalculadaDataGridViewTextBoxColumn1
            // 
            this.unidadCalculadaDataGridViewTextBoxColumn1.DataPropertyName = "UnidadCalculada";
            this.unidadCalculadaDataGridViewTextBoxColumn1.HeaderText = "UnidadCalculada";
            this.unidadCalculadaDataGridViewTextBoxColumn1.Name = "unidadCalculadaDataGridViewTextBoxColumn1";
            this.unidadCalculadaDataGridViewTextBoxColumn1.Width = 113;
            // 
            // aDecrementoDataGridViewCheckBoxColumn1
            // 
            this.aDecrementoDataGridViewCheckBoxColumn1.DataPropertyName = "ADecremento";
            this.aDecrementoDataGridViewCheckBoxColumn1.HeaderText = "ADecremento";
            this.aDecrementoDataGridViewCheckBoxColumn1.Name = "aDecrementoDataGridViewCheckBoxColumn1";
            this.aDecrementoDataGridViewCheckBoxColumn1.Width = 78;
            // 
            // idSubcomponenteDataGridViewTextBoxColumn1
            // 
            this.idSubcomponenteDataGridViewTextBoxColumn1.DataPropertyName = "IdSubcomponente";
            this.idSubcomponenteDataGridViewTextBoxColumn1.HeaderText = "IdSubcomponente";
            this.idSubcomponenteDataGridViewTextBoxColumn1.Name = "idSubcomponenteDataGridViewTextBoxColumn1";
            this.idSubcomponenteDataGridViewTextBoxColumn1.Width = 119;
            // 
            // elevadoDataGridViewTextBoxColumn1
            // 
            this.elevadoDataGridViewTextBoxColumn1.DataPropertyName = "Elevado";
            this.elevadoDataGridViewTextBoxColumn1.HeaderText = "Elevado";
            this.elevadoDataGridViewTextBoxColumn1.Name = "elevadoDataGridViewTextBoxColumn1";
            this.elevadoDataGridViewTextBoxColumn1.Width = 71;
            // 
            // cortesDataGridViewTextBoxColumn1
            // 
            this.cortesDataGridViewTextBoxColumn1.DataPropertyName = "Cortes";
            this.cortesDataGridViewTextBoxColumn1.HeaderText = "Cortes";
            this.cortesDataGridViewTextBoxColumn1.Name = "cortesDataGridViewTextBoxColumn1";
            this.cortesDataGridViewTextBoxColumn1.Width = 62;
            // 
            // subComponentBindingSource1
            // 
            this.subComponentBindingSource1.DataSource = typeof(arquitectSoft.Class.Sub_Component);
            // 
            // codigoDataGridViewTextBoxColumn
            // 
            this.codigoDataGridViewTextBoxColumn.DataPropertyName = "Codigo";
            this.codigoDataGridViewTextBoxColumn.HeaderText = "Codigo";
            this.codigoDataGridViewTextBoxColumn.Name = "codigoDataGridViewTextBoxColumn";
            this.codigoDataGridViewTextBoxColumn.Width = 65;
            // 
            // descripcionDataGridViewTextBoxColumn
            // 
            this.descripcionDataGridViewTextBoxColumn.DataPropertyName = "Descripcion";
            this.descripcionDataGridViewTextBoxColumn.HeaderText = "Descripcion";
            this.descripcionDataGridViewTextBoxColumn.Name = "descripcionDataGridViewTextBoxColumn";
            this.descripcionDataGridViewTextBoxColumn.Width = 88;
            // 
            // cxdefectoDataGridViewTextBoxColumn
            // 
            this.cxdefectoDataGridViewTextBoxColumn.DataPropertyName = "Cxdefecto";
            this.cxdefectoDataGridViewTextBoxColumn.HeaderText = "Cxdefecto";
            this.cxdefectoDataGridViewTextBoxColumn.Name = "cxdefectoDataGridViewTextBoxColumn";
            this.cxdefectoDataGridViewTextBoxColumn.Width = 80;
            // 
            // cAdicionalDataGridViewTextBoxColumn
            // 
            this.cAdicionalDataGridViewTextBoxColumn.DataPropertyName = "CAdicional";
            this.cAdicionalDataGridViewTextBoxColumn.HeaderText = "CAdicional";
            this.cAdicionalDataGridViewTextBoxColumn.Name = "cAdicionalDataGridViewTextBoxColumn";
            this.cAdicionalDataGridViewTextBoxColumn.Width = 82;
            // 
            // unidadCalculadaDataGridViewTextBoxColumn
            // 
            this.unidadCalculadaDataGridViewTextBoxColumn.DataPropertyName = "UnidadCalculada";
            this.unidadCalculadaDataGridViewTextBoxColumn.HeaderText = "UnidadCalculada";
            this.unidadCalculadaDataGridViewTextBoxColumn.Name = "unidadCalculadaDataGridViewTextBoxColumn";
            this.unidadCalculadaDataGridViewTextBoxColumn.Width = 113;
            // 
            // aDecrementoDataGridViewCheckBoxColumn
            // 
            this.aDecrementoDataGridViewCheckBoxColumn.DataPropertyName = "ADecremento";
            this.aDecrementoDataGridViewCheckBoxColumn.HeaderText = "ADecremento";
            this.aDecrementoDataGridViewCheckBoxColumn.Name = "aDecrementoDataGridViewCheckBoxColumn";
            this.aDecrementoDataGridViewCheckBoxColumn.Width = 78;
            // 
            // idSubcomponenteDataGridViewTextBoxColumn
            // 
            this.idSubcomponenteDataGridViewTextBoxColumn.DataPropertyName = "IdSubcomponente";
            this.idSubcomponenteDataGridViewTextBoxColumn.HeaderText = "IdSubcomponente";
            this.idSubcomponenteDataGridViewTextBoxColumn.Name = "idSubcomponenteDataGridViewTextBoxColumn";
            this.idSubcomponenteDataGridViewTextBoxColumn.Width = 119;
            // 
            // elevadoDataGridViewTextBoxColumn
            // 
            this.elevadoDataGridViewTextBoxColumn.DataPropertyName = "Elevado";
            this.elevadoDataGridViewTextBoxColumn.HeaderText = "Elevado";
            this.elevadoDataGridViewTextBoxColumn.Name = "elevadoDataGridViewTextBoxColumn";
            this.elevadoDataGridViewTextBoxColumn.Width = 71;
            // 
            // cortesDataGridViewTextBoxColumn
            // 
            this.cortesDataGridViewTextBoxColumn.DataPropertyName = "Cortes";
            this.cortesDataGridViewTextBoxColumn.HeaderText = "Cortes";
            this.cortesDataGridViewTextBoxColumn.Name = "cortesDataGridViewTextBoxColumn";
            this.cortesDataGridViewTextBoxColumn.Width = 62;
            // 
            // subComponentBindingSource
            // 
            this.subComponentBindingSource.DataSource = typeof(arquitectSoft.Class.Sub_Component);
            // 
            // FrmComponente
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(86)))));
            this.ClientSize = new System.Drawing.Size(920, 526);
            this.Controls.Add(this.GridViewComponenteEsp);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.chkEspecial);
            this.Controls.Add(this.BtnDuplicar);
            this.Controls.Add(this.BtnCancelar);
            this.Controls.Add(this.BtnBorrar);
            this.Controls.Add(this.BtnAgregar);
            this.Controls.Add(this.BtnCheck);
            this.Controls.Add(this.GridViewComponente);
            this.Controls.Add(this.txtDescripcion);
            this.Controls.Add(this.lbletiquetaDescripcion);
            this.Controls.Add(this.txtCodigo);
            this.Controls.Add(this.lblEtiquetaCodigo);
            this.Controls.Add(this.BtnSalir);
            this.Controls.Add(this.BtnBuscar);
            this.Controls.Add(this.BtnNuevo);
            this.Controls.Add(this.BtnEliminar);
            this.Controls.Add(this.BtnEditar);
            this.Controls.Add(this.BtnGuardar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmComponente";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Componentes";
            this.Load += new System.EventHandler(this.FrmComponente_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GridViewComponente)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.arquitectdbDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.unidadescalculadasBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GridViewComponenteEsp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.subComponentBindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.subComponentBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BtnGuardar;
        private System.Windows.Forms.ImageList ImgLista;
        private System.Windows.Forms.Button BtnEditar;
        private System.Windows.Forms.Button BtnEliminar;
        private System.Windows.Forms.Button BtnNuevo;
        private System.Windows.Forms.Button BtnBuscar;
        private System.Windows.Forms.Button BtnSalir;
        private System.Windows.Forms.Label lblEtiquetaCodigo;
        private System.Windows.Forms.TextBox txtCodigo;
        private System.Windows.Forms.TextBox txtDescripcion;
        private System.Windows.Forms.Label lbletiquetaDescripcion;
        private System.Windows.Forms.CheckBox chkEspecial;
        private System.Windows.Forms.DataGridView GridViewComponente;
        private System.Windows.Forms.Button BtnCheck;
        private System.Windows.Forms.Button BtnAgregar;
        private System.Windows.Forms.Button BtnBorrar;
        private System.Windows.Forms.Button BtnCancelar;
        private arquitectdbDataSet arquitectdbDataSet;
        private System.Windows.Forms.BindingSource unidadescalculadasBindingSource;
        private arquitectdbDataSetTableAdapters.unidades_calculadasTableAdapter unidades_calculadasTableAdapter;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.BindingSource subComponentBindingSource;
        private System.Windows.Forms.BindingSource subComponentBindingSource1;
        private System.Windows.Forms.Button BtnDuplicar;
        private System.Windows.Forms.DataGridViewTextBoxColumn codigoDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn descripcionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn cxdefectoDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn cAdicionalDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn unidadCalculadaDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn aDecrementoDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn idSubcomponenteDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn elevadoDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn cortesDataGridViewTextBoxColumn;
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
    }
}