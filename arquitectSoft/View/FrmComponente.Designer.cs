
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
            this.BtnSalir = new System.Windows.Forms.Button();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.GridViewComponenteEsp = new System.Windows.Forms.DataGridView();
            this.bindingSource2 = new System.Windows.Forms.BindingSource(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.chkEspecial = new System.Windows.Forms.CheckBox();
            this.BtnCheck = new System.Windows.Forms.Button();
            this.CmbAcabado = new System.Windows.Forms.ComboBox();
            this.ImgListFinal = new System.Windows.Forms.ImageList(this.components);
            this.txtDescripcion = new System.Windows.Forms.TextBox();
            this.txtCodigo = new System.Windows.Forms.TextBox();
            this.lblEtiquetaCodigo = new System.Windows.Forms.Label();
            this.lbletiquetaDescripcion = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.BtnDuplicar = new arquitectSoft.Generals.RJButton();
            this.BtnCancelar = new arquitectSoft.Generals.RJButton();
            this.BtnBuscar = new arquitectSoft.Generals.RJButton();
            this.BtnEliminar = new arquitectSoft.Generals.RJButton();
            this.BtnEditar = new arquitectSoft.Generals.RJButton();
            this.BtnGuardar = new arquitectSoft.Generals.RJButton();
            this.BtnNuevo = new arquitectSoft.Generals.RJButton();
            this.BtnBorrar = new arquitectSoft.Generals.RJButton();
            this.BtnAgregar = new arquitectSoft.Generals.RJButton();
            this.elipseControl1 = new arquitectSoft.Generals.ElipseControl();
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
            this.EliCtrlButtons = new arquitectSoft.Generals.ElipseControl();
            this.elipseControl2 = new arquitectSoft.Generals.ElipseControl();
            this.elipseControl3 = new arquitectSoft.Generals.ElipseControl();
            this.elipseComponent1 = new arquitectSoft.Generals.ElipseComponent();
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
            this.GridViewComponente.AutoGenerateColumns = false;
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
            this.GridViewComponente.EnableHeadersVisualStyles = false;
            this.GridViewComponente.GridColor = System.Drawing.SystemColors.Control;
            this.GridViewComponente.Location = new System.Drawing.Point(1, 143);
            this.GridViewComponente.Name = "GridViewComponente";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.GridViewComponente.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.GridViewComponente.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.GridViewComponente.Size = new System.Drawing.Size(919, 300);
            this.GridViewComponente.TabIndex = 23;
            this.GridViewComponente.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.GridViewComponente_DataError);
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
            // BtnSalir
            // 
            this.BtnSalir.BackColor = System.Drawing.Color.Black;
            this.BtnSalir.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.BtnSalir.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BtnSalir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnSalir.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnSalir.ForeColor = System.Drawing.Color.White;
            this.BtnSalir.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.BtnSalir.ImageIndex = 51;
            this.BtnSalir.ImageList = this.ImgLista;
            this.BtnSalir.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnSalir.Location = new System.Drawing.Point(845, 14);
            this.BtnSalir.Name = "BtnSalir";
            this.BtnSalir.Size = new System.Drawing.Size(72, 35);
            this.BtnSalir.TabIndex = 17;
            this.BtnSalir.Text = "Salir";
            this.BtnSalir.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.BtnSalir.UseVisualStyleBackColor = false;
            this.BtnSalir.Click += new System.EventHandler(this.BtnSalir_Click);
            // 
            // GridViewComponenteEsp
            // 
            this.GridViewComponenteEsp.AllowUserToAddRows = false;
            this.GridViewComponenteEsp.AllowUserToDeleteRows = false;
            this.GridViewComponenteEsp.AllowUserToResizeColumns = false;
            this.GridViewComponenteEsp.AllowUserToResizeRows = false;
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
            this.GridViewComponenteEsp.Location = new System.Drawing.Point(1, 292);
            this.GridViewComponenteEsp.Name = "GridViewComponenteEsp";
            this.GridViewComponenteEsp.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.GridViewComponenteEsp.Size = new System.Drawing.Size(919, 151);
            this.GridViewComponenteEsp.TabIndex = 29;
            this.GridViewComponenteEsp.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Black;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(33, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 24);
            this.label1.TabIndex = 49;
            this.label1.Text = "Componente";
            // 
            // chkEspecial
            // 
            this.chkEspecial.AutoSize = true;
            this.chkEspecial.BackColor = System.Drawing.Color.White;
            this.chkEspecial.ForeColor = System.Drawing.Color.Black;
            this.chkEspecial.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chkEspecial.Location = new System.Drawing.Point(817, 89);
            this.chkEspecial.Name = "chkEspecial";
            this.chkEspecial.Size = new System.Drawing.Size(100, 17);
            this.chkEspecial.TabIndex = 22;
            this.chkEspecial.Text = "Vidrios/Paneles";
            this.chkEspecial.UseVisualStyleBackColor = false;
            this.chkEspecial.CheckedChanged += new System.EventHandler(this.chkEspecial_CheckedChanged);
            // 
            // BtnCheck
            // 
            this.BtnCheck.ImageList = this.ImgLista;
            this.BtnCheck.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnCheck.Location = new System.Drawing.Point(138, 108);
            this.BtnCheck.Name = "BtnCheck";
            this.BtnCheck.Size = new System.Drawing.Size(64, 23);
            this.BtnCheck.TabIndex = 24;
            this.BtnCheck.Text = "Validar";
            this.BtnCheck.UseVisualStyleBackColor = true;
            this.BtnCheck.Click += new System.EventHandler(this.BtnCheck_Click);
            // 
            // CmbAcabado
            // 
            this.CmbAcabado.FormattingEnabled = true;
            this.CmbAcabado.Location = new System.Drawing.Point(626, 107);
            this.CmbAcabado.Name = "CmbAcabado";
            this.CmbAcabado.Size = new System.Drawing.Size(291, 21);
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
            this.txtDescripcion.Location = new System.Drawing.Point(219, 107);
            this.txtDescripcion.Name = "txtDescripcion";
            this.txtDescripcion.Size = new System.Drawing.Size(392, 20);
            this.txtDescripcion.TabIndex = 63;
            // 
            // txtCodigo
            // 
            this.txtCodigo.Location = new System.Drawing.Point(12, 107);
            this.txtCodigo.Name = "txtCodigo";
            this.txtCodigo.Size = new System.Drawing.Size(120, 20);
            this.txtCodigo.TabIndex = 64;
            // 
            // lblEtiquetaCodigo
            // 
            this.lblEtiquetaCodigo.AutoSize = true;
            this.lblEtiquetaCodigo.BackColor = System.Drawing.Color.White;
            this.lblEtiquetaCodigo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEtiquetaCodigo.ForeColor = System.Drawing.Color.Black;
            this.lblEtiquetaCodigo.Location = new System.Drawing.Point(-1, 90);
            this.lblEtiquetaCodigo.Name = "lblEtiquetaCodigo";
            this.lblEtiquetaCodigo.Size = new System.Drawing.Size(55, 16);
            this.lblEtiquetaCodigo.TabIndex = 97;
            this.lblEtiquetaCodigo.Text = "Codigo:";
            // 
            // lbletiquetaDescripcion
            // 
            this.lbletiquetaDescripcion.AutoSize = true;
            this.lbletiquetaDescripcion.BackColor = System.Drawing.Color.White;
            this.lbletiquetaDescripcion.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbletiquetaDescripcion.ForeColor = System.Drawing.Color.Black;
            this.lbletiquetaDescripcion.Location = new System.Drawing.Point(211, 90);
            this.lbletiquetaDescripcion.Name = "lbletiquetaDescripcion";
            this.lbletiquetaDescripcion.Size = new System.Drawing.Size(83, 16);
            this.lbletiquetaDescripcion.TabIndex = 98;
            this.lbletiquetaDescripcion.Text = "Descripcion:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.White;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(614, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 16);
            this.label2.TabIndex = 99;
            this.label2.Text = "Acabado:";
            // 
            // BtnDuplicar
            // 
            this.BtnDuplicar.BackColor = System.Drawing.Color.DimGray;
            this.BtnDuplicar.BackgroundColor = System.Drawing.Color.DimGray;
            this.BtnDuplicar.BorderColor = System.Drawing.Color.Black;
            this.BtnDuplicar.BorderRadius = 20;
            this.BtnDuplicar.BorderSize = 0;
            this.BtnDuplicar.FlatAppearance.BorderSize = 0;
            this.BtnDuplicar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnDuplicar.ForeColor = System.Drawing.Color.White;
            this.BtnDuplicar.ImageIndex = 5;
            this.BtnDuplicar.ImageList = this.ImgListFinal;
            this.BtnDuplicar.Location = new System.Drawing.Point(299, 13);
            this.BtnDuplicar.Name = "BtnDuplicar";
            this.BtnDuplicar.Size = new System.Drawing.Size(42, 36);
            this.BtnDuplicar.TabIndex = 62;
            this.BtnDuplicar.TextColor = System.Drawing.Color.White;
            this.BtnDuplicar.UseVisualStyleBackColor = false;
            this.BtnDuplicar.Click += new System.EventHandler(this.BtnDuplicar_Click);
            // 
            // BtnCancelar
            // 
            this.BtnCancelar.BackColor = System.Drawing.Color.DimGray;
            this.BtnCancelar.BackgroundColor = System.Drawing.Color.DimGray;
            this.BtnCancelar.BorderColor = System.Drawing.Color.Black;
            this.BtnCancelar.BorderRadius = 20;
            this.BtnCancelar.BorderSize = 0;
            this.BtnCancelar.FlatAppearance.BorderSize = 0;
            this.BtnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnCancelar.ForeColor = System.Drawing.Color.White;
            this.BtnCancelar.ImageIndex = 4;
            this.BtnCancelar.ImageList = this.ImgListFinal;
            this.BtnCancelar.Location = new System.Drawing.Point(251, 13);
            this.BtnCancelar.Name = "BtnCancelar";
            this.BtnCancelar.Size = new System.Drawing.Size(42, 36);
            this.BtnCancelar.TabIndex = 61;
            this.BtnCancelar.TextColor = System.Drawing.Color.White;
            this.BtnCancelar.UseVisualStyleBackColor = false;
            this.BtnCancelar.Click += new System.EventHandler(this.BtnCancelar_Click);
            // 
            // BtnBuscar
            // 
            this.BtnBuscar.BackColor = System.Drawing.Color.DimGray;
            this.BtnBuscar.BackgroundColor = System.Drawing.Color.DimGray;
            this.BtnBuscar.BorderColor = System.Drawing.Color.Black;
            this.BtnBuscar.BorderRadius = 20;
            this.BtnBuscar.BorderSize = 0;
            this.BtnBuscar.FlatAppearance.BorderSize = 0;
            this.BtnBuscar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnBuscar.ForeColor = System.Drawing.Color.White;
            this.BtnBuscar.ImageIndex = 3;
            this.BtnBuscar.ImageList = this.ImgListFinal;
            this.BtnBuscar.Location = new System.Drawing.Point(203, 12);
            this.BtnBuscar.Name = "BtnBuscar";
            this.BtnBuscar.Size = new System.Drawing.Size(42, 36);
            this.BtnBuscar.TabIndex = 60;
            this.BtnBuscar.TextColor = System.Drawing.Color.White;
            this.BtnBuscar.UseVisualStyleBackColor = false;
            this.BtnBuscar.Click += new System.EventHandler(this.BtnBuscar_Click);
            // 
            // BtnEliminar
            // 
            this.BtnEliminar.BackColor = System.Drawing.Color.DimGray;
            this.BtnEliminar.BackgroundColor = System.Drawing.Color.DimGray;
            this.BtnEliminar.BorderColor = System.Drawing.Color.Black;
            this.BtnEliminar.BorderRadius = 20;
            this.BtnEliminar.BorderSize = 0;
            this.BtnEliminar.FlatAppearance.BorderSize = 0;
            this.BtnEliminar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnEliminar.ForeColor = System.Drawing.Color.White;
            this.BtnEliminar.ImageIndex = 8;
            this.BtnEliminar.ImageList = this.ImgListFinal;
            this.BtnEliminar.Location = new System.Drawing.Point(156, 13);
            this.BtnEliminar.Name = "BtnEliminar";
            this.BtnEliminar.Size = new System.Drawing.Size(42, 36);
            this.BtnEliminar.TabIndex = 59;
            this.BtnEliminar.TextColor = System.Drawing.Color.White;
            this.BtnEliminar.UseVisualStyleBackColor = false;
            this.BtnEliminar.Click += new System.EventHandler(this.BtnEliminar_Click);
            // 
            // BtnEditar
            // 
            this.BtnEditar.BackColor = System.Drawing.Color.DimGray;
            this.BtnEditar.BackgroundColor = System.Drawing.Color.DimGray;
            this.BtnEditar.BorderColor = System.Drawing.Color.Black;
            this.BtnEditar.BorderRadius = 20;
            this.BtnEditar.BorderSize = 0;
            this.BtnEditar.FlatAppearance.BorderSize = 0;
            this.BtnEditar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnEditar.ForeColor = System.Drawing.Color.White;
            this.BtnEditar.ImageIndex = 2;
            this.BtnEditar.ImageList = this.ImgListFinal;
            this.BtnEditar.Location = new System.Drawing.Point(108, 13);
            this.BtnEditar.Name = "BtnEditar";
            this.BtnEditar.Size = new System.Drawing.Size(42, 36);
            this.BtnEditar.TabIndex = 58;
            this.BtnEditar.TextColor = System.Drawing.Color.White;
            this.BtnEditar.UseVisualStyleBackColor = false;
            this.BtnEditar.Click += new System.EventHandler(this.BtnEditar_Click);
            // 
            // BtnGuardar
            // 
            this.BtnGuardar.BackColor = System.Drawing.Color.DimGray;
            this.BtnGuardar.BackgroundColor = System.Drawing.Color.DimGray;
            this.BtnGuardar.BorderColor = System.Drawing.Color.Black;
            this.BtnGuardar.BorderRadius = 20;
            this.BtnGuardar.BorderSize = 0;
            this.BtnGuardar.FlatAppearance.BorderSize = 0;
            this.BtnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnGuardar.ForeColor = System.Drawing.Color.White;
            this.BtnGuardar.ImageIndex = 1;
            this.BtnGuardar.ImageList = this.ImgListFinal;
            this.BtnGuardar.Location = new System.Drawing.Point(60, 12);
            this.BtnGuardar.Name = "BtnGuardar";
            this.BtnGuardar.Size = new System.Drawing.Size(42, 36);
            this.BtnGuardar.TabIndex = 57;
            this.BtnGuardar.TextColor = System.Drawing.Color.White;
            this.BtnGuardar.UseVisualStyleBackColor = false;
            this.BtnGuardar.Click += new System.EventHandler(this.BtnGuardar_Click);
            // 
            // BtnNuevo
            // 
            this.BtnNuevo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.BtnNuevo.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.BtnNuevo.BorderColor = System.Drawing.Color.Black;
            this.BtnNuevo.BorderRadius = 20;
            this.BtnNuevo.BorderSize = 0;
            this.BtnNuevo.FlatAppearance.BorderSize = 0;
            this.BtnNuevo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnNuevo.ForeColor = System.Drawing.Color.White;
            this.BtnNuevo.ImageIndex = 0;
            this.BtnNuevo.ImageList = this.ImgListFinal;
            this.BtnNuevo.Location = new System.Drawing.Point(12, 13);
            this.BtnNuevo.Name = "BtnNuevo";
            this.BtnNuevo.Size = new System.Drawing.Size(42, 36);
            this.BtnNuevo.TabIndex = 56;
            this.BtnNuevo.TextColor = System.Drawing.Color.White;
            this.BtnNuevo.UseVisualStyleBackColor = false;
            this.BtnNuevo.Click += new System.EventHandler(this.BtnNuevo_Click);
            // 
            // BtnBorrar
            // 
            this.BtnBorrar.BackColor = System.Drawing.SystemColors.Control;
            this.BtnBorrar.BackgroundColor = System.Drawing.SystemColors.Control;
            this.BtnBorrar.BorderColor = System.Drawing.Color.Black;
            this.BtnBorrar.BorderRadius = 10;
            this.BtnBorrar.BorderSize = 0;
            this.BtnBorrar.FlatAppearance.BorderSize = 0;
            this.BtnBorrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnBorrar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnBorrar.ForeColor = System.Drawing.Color.DimGray;
            this.BtnBorrar.Location = new System.Drawing.Point(109, 482);
            this.BtnBorrar.Name = "BtnBorrar";
            this.BtnBorrar.Size = new System.Drawing.Size(91, 22);
            this.BtnBorrar.TabIndex = 53;
            this.BtnBorrar.Text = "Borrar";
            this.BtnBorrar.TextColor = System.Drawing.Color.DimGray;
            this.BtnBorrar.UseVisualStyleBackColor = false;
            this.BtnBorrar.Click += new System.EventHandler(this.BtnBorrar_Click);
            // 
            // BtnAgregar
            // 
            this.BtnAgregar.BackColor = System.Drawing.Color.DimGray;
            this.BtnAgregar.BackgroundColor = System.Drawing.Color.DimGray;
            this.BtnAgregar.BorderColor = System.Drawing.Color.Black;
            this.BtnAgregar.BorderRadius = 10;
            this.BtnAgregar.BorderSize = 0;
            this.BtnAgregar.FlatAppearance.BorderSize = 0;
            this.BtnAgregar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnAgregar.ForeColor = System.Drawing.Color.White;
            this.BtnAgregar.Location = new System.Drawing.Point(12, 482);
            this.BtnAgregar.Name = "BtnAgregar";
            this.BtnAgregar.Size = new System.Drawing.Size(91, 22);
            this.BtnAgregar.TabIndex = 52;
            this.BtnAgregar.Text = "Agregar";
            this.BtnAgregar.TextColor = System.Drawing.Color.White;
            this.BtnAgregar.UseCompatibleTextRendering = true;
            this.BtnAgregar.UseVisualStyleBackColor = false;
            this.BtnAgregar.Click += new System.EventHandler(this.BtnAgregar_Click);
            // 
            // elipseControl1
            // 
            this.elipseControl1.BackColor = System.Drawing.Color.Black;
            this.elipseControl1.CornerRadius = 15;
            this.elipseControl1.Location = new System.Drawing.Point(23, 50);
            this.elipseControl1.Name = "elipseControl1";
            this.elipseControl1.Size = new System.Drawing.Size(871, 32);
            this.elipseControl1.TabIndex = 48;
            this.elipseControl1.Text = "elipseControl1";
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
            // EliCtrlButtons
            // 
            this.EliCtrlButtons.BackColor = System.Drawing.Color.Black;
            this.EliCtrlButtons.CornerRadius = 15;
            this.EliCtrlButtons.Location = new System.Drawing.Point(0, -1);
            this.EliCtrlButtons.Name = "EliCtrlButtons";
            this.EliCtrlButtons.Size = new System.Drawing.Size(921, 61);
            this.EliCtrlButtons.TabIndex = 47;
            this.EliCtrlButtons.Text = "elipseControl1";
            this.EliCtrlButtons.MouseDown += new System.Windows.Forms.MouseEventHandler(this.EliCtrlButtons_MouseDown);
            // 
            // elipseControl2
            // 
            this.elipseControl2.BackColor = System.Drawing.Color.White;
            this.elipseControl2.CornerRadius = 15;
            this.elipseControl2.Location = new System.Drawing.Point(-1, 67);
            this.elipseControl2.Name = "elipseControl2";
            this.elipseControl2.Size = new System.Drawing.Size(921, 70);
            this.elipseControl2.TabIndex = 50;
            this.elipseControl2.Text = "elipseControl1";
            // 
            // elipseControl3
            // 
            this.elipseControl3.BackColor = System.Drawing.Color.Black;
            this.elipseControl3.CornerRadius = 15;
            this.elipseControl3.Location = new System.Drawing.Point(-1, 465);
            this.elipseControl3.Name = "elipseControl3";
            this.elipseControl3.Size = new System.Drawing.Size(921, 61);
            this.elipseControl3.TabIndex = 54;
            this.elipseControl3.Text = "elipseControl1";
            // 
            // elipseComponent1
            // 
            this.elipseComponent1.CornerRadius = 15;
            this.elipseComponent1.TargetControl = this;
            // 
            // FrmComponente
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.ClientSize = new System.Drawing.Size(920, 526);
            this.ControlBox = false;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbletiquetaDescripcion);
            this.Controls.Add(this.lblEtiquetaCodigo);
            this.Controls.Add(this.txtCodigo);
            this.Controls.Add(this.txtDescripcion);
            this.Controls.Add(this.BtnDuplicar);
            this.Controls.Add(this.BtnCancelar);
            this.Controls.Add(this.BtnBuscar);
            this.Controls.Add(this.BtnEliminar);
            this.Controls.Add(this.BtnEditar);
            this.Controls.Add(this.BtnGuardar);
            this.Controls.Add(this.BtnNuevo);
            this.Controls.Add(this.BtnBorrar);
            this.Controls.Add(this.BtnAgregar);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CmbAcabado);
            this.Controls.Add(this.BtnCheck);
            this.Controls.Add(this.elipseControl1);
            this.Controls.Add(this.chkEspecial);
            this.Controls.Add(this.GridViewComponenteEsp);
            this.Controls.Add(this.GridViewComponente);
            this.Controls.Add(this.BtnSalir);
            this.Controls.Add(this.EliCtrlButtons);
            this.Controls.Add(this.elipseControl2);
            this.Controls.Add(this.elipseControl3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmComponente";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Componentes";
            this.Load += new System.EventHandler(this.FrmComponente_Load);
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
        private System.Windows.Forms.ImageList ImgLista;
        private System.Windows.Forms.Button BtnSalir;
        private System.Windows.Forms.DataGridView GridViewComponente;
        private arquitectdbDataSet arquitectdbDataSet;
        private System.Windows.Forms.BindingSource unidadescalculadasBindingSource;
        private arquitectdbDataSetTableAdapters.unidades_calculadasTableAdapter unidades_calculadasTableAdapter;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.BindingSource subComponentBindingSource;
        private System.Windows.Forms.BindingSource subComponentBindingSource1;
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
        private Generals.ElipseControl EliCtrlButtons;
        private Generals.ElipseComponent elipseComponent1;
        private Generals.ElipseControl elipseControl1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox CmbAcabado;
        private System.Windows.Forms.Button BtnCheck;
        private System.Windows.Forms.CheckBox chkEspecial;
        private Generals.ElipseControl elipseControl2;
        private Generals.RJButton BtnAgregar;
        private Generals.ElipseControl elipseControl3;
        private Generals.RJButton BtnBorrar;
        private Generals.RJButton BtnNuevo;
        private Generals.RJButton BtnDuplicar;
        private Generals.RJButton BtnCancelar;
        private Generals.RJButton BtnBuscar;
        private Generals.RJButton BtnEliminar;
        private Generals.RJButton BtnEditar;
        private Generals.RJButton BtnGuardar;
        private System.Windows.Forms.ImageList ImgListFinal;
        private System.Windows.Forms.TextBox txtDescripcion;
        private System.Windows.Forms.TextBox txtCodigo;
        private System.Windows.Forms.Label lblEtiquetaCodigo;
        private System.Windows.Forms.Label lbletiquetaDescripcion;
        private System.Windows.Forms.Label label2;
    }
}