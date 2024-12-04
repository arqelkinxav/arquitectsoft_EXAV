
namespace arquitectSoft
{
    partial class FrmDBA_Import
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.labelFilename = new System.Windows.Forms.Label();
            this.labelProductName = new System.Windows.Forms.Label();
            this.lblcurrentdate = new System.Windows.Forms.Label();
            this.lblfilename = new System.Windows.Forms.Label();
            this.btnExaminar = new arquitectSoft.Generals.RJButton();
            this.BtnBackup = new arquitectSoft.Generals.RJButton();
            this.BtnCancelar = new arquitectSoft.Generals.RJButton();
            this.elipseComponent1 = new arquitectSoft.Generals.ElipseComponent();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImage = global::arquitectSoft.Properties.Resources.logo2;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Right;
            this.pictureBox1.Location = new System.Drawing.Point(312, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(174, 213);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // txtPath
            // 
            this.txtPath.Location = new System.Drawing.Point(12, 135);
            this.txtPath.Name = "txtPath";
            this.txtPath.ReadOnly = true;
            this.txtPath.Size = new System.Drawing.Size(259, 20);
            this.txtPath.TabIndex = 32;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(13, 116);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 15);
            this.label1.TabIndex = 33;
            this.label1.Text = "Cargar Archivo Backup";
            // 
            // labelFilename
            // 
            this.labelFilename.AutoSize = true;
            this.labelFilename.ForeColor = System.Drawing.SystemColors.Control;
            this.labelFilename.Location = new System.Drawing.Point(13, 39);
            this.labelFilename.Name = "labelFilename";
            this.labelFilename.Size = new System.Drawing.Size(102, 15);
            this.labelFilename.TabIndex = 36;
            this.labelFilename.Text = "Archivo Cargado: ";
            // 
            // labelProductName
            // 
            this.labelProductName.AutoSize = true;
            this.labelProductName.ForeColor = System.Drawing.SystemColors.Control;
            this.labelProductName.Location = new System.Drawing.Point(13, 14);
            this.labelProductName.Name = "labelProductName";
            this.labelProductName.Size = new System.Drawing.Size(170, 15);
            this.labelProductName.TabIndex = 35;
            this.labelProductName.Text = "Ultimo Datos de Actualización";
            // 
            // lblcurrentdate
            // 
            this.lblcurrentdate.AutoSize = true;
            this.lblcurrentdate.ForeColor = System.Drawing.SystemColors.Control;
            this.lblcurrentdate.Location = new System.Drawing.Point(13, 56);
            this.lblcurrentdate.Name = "lblcurrentdate";
            this.lblcurrentdate.Size = new System.Drawing.Size(152, 15);
            this.lblcurrentdate.TabIndex = 37;
            this.lblcurrentdate.Text = "Fecha Local Actualizacón: ";
            // 
            // lblfilename
            // 
            this.lblfilename.AutoSize = true;
            this.lblfilename.ForeColor = System.Drawing.SystemColors.Desktop;
            this.lblfilename.Location = new System.Drawing.Point(13, 84);
            this.lblfilename.Name = "lblfilename";
            this.lblfilename.Size = new System.Drawing.Size(16, 15);
            this.lblfilename.TabIndex = 38;
            this.lblfilename.Text = "...";
            // 
            // btnExaminar
            // 
            this.btnExaminar.BackColor = System.Drawing.SystemColors.Control;
            this.btnExaminar.BackgroundColor = System.Drawing.SystemColors.Control;
            this.btnExaminar.BorderColor = System.Drawing.Color.Black;
            this.btnExaminar.BorderRadius = 10;
            this.btnExaminar.BorderSize = 0;
            this.btnExaminar.FlatAppearance.BorderSize = 0;
            this.btnExaminar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExaminar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExaminar.ForeColor = System.Drawing.Color.DimGray;
            this.btnExaminar.Location = new System.Drawing.Point(272, 133);
            this.btnExaminar.Name = "btnExaminar";
            this.btnExaminar.Size = new System.Drawing.Size(36, 22);
            this.btnExaminar.TabIndex = 34;
            this.btnExaminar.Text = "...";
            this.btnExaminar.TextColor = System.Drawing.Color.DimGray;
            this.btnExaminar.UseVisualStyleBackColor = false;
            this.btnExaminar.Click += new System.EventHandler(this.btnExaminar_Click);
            // 
            // BtnBackup
            // 
            this.BtnBackup.BackColor = System.Drawing.SystemColors.Control;
            this.BtnBackup.BackgroundColor = System.Drawing.SystemColors.Control;
            this.BtnBackup.BorderColor = System.Drawing.Color.Black;
            this.BtnBackup.BorderRadius = 10;
            this.BtnBackup.BorderSize = 0;
            this.BtnBackup.FlatAppearance.BorderSize = 0;
            this.BtnBackup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnBackup.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnBackup.ForeColor = System.Drawing.Color.DimGray;
            this.BtnBackup.Location = new System.Drawing.Point(31, 162);
            this.BtnBackup.Name = "BtnBackup";
            this.BtnBackup.Size = new System.Drawing.Size(104, 21);
            this.BtnBackup.TabIndex = 30;
            this.BtnBackup.Text = "Cargar";
            this.BtnBackup.TextColor = System.Drawing.Color.DimGray;
            this.BtnBackup.UseVisualStyleBackColor = false;
            this.BtnBackup.Click += new System.EventHandler(this.BtnBackup_Click);
            // 
            // BtnCancelar
            // 
            this.BtnCancelar.BackColor = System.Drawing.SystemColors.Control;
            this.BtnCancelar.BackgroundColor = System.Drawing.SystemColors.Control;
            this.BtnCancelar.BorderColor = System.Drawing.Color.Black;
            this.BtnCancelar.BorderRadius = 10;
            this.BtnCancelar.BorderSize = 0;
            this.BtnCancelar.FlatAppearance.BorderSize = 0;
            this.BtnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnCancelar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnCancelar.ForeColor = System.Drawing.Color.DimGray;
            this.BtnCancelar.Location = new System.Drawing.Point(167, 161);
            this.BtnCancelar.Name = "BtnCancelar";
            this.BtnCancelar.Size = new System.Drawing.Size(104, 22);
            this.BtnCancelar.TabIndex = 29;
            this.BtnCancelar.Text = "Salir";
            this.BtnCancelar.TextColor = System.Drawing.Color.DimGray;
            this.BtnCancelar.UseVisualStyleBackColor = false;
            this.BtnCancelar.Click += new System.EventHandler(this.BtnCancelar_Click);
            // 
            // elipseComponent1
            // 
            this.elipseComponent1.CornerRadius = 40;
            this.elipseComponent1.TargetControl = this;
            // 
            // FrmDBA_Import
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(486, 213);
            this.Controls.Add(this.lblfilename);
            this.Controls.Add(this.lblcurrentdate);
            this.Controls.Add(this.labelFilename);
            this.Controls.Add(this.labelProductName);
            this.Controls.Add(this.btnExaminar);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.BtnBackup);
            this.Controls.Add(this.BtnCancelar);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmDBA_Import";
            this.Text = "FrmAcercade";
            this.Load += new System.EventHandler(this.FrmDBA_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private Generals.ElipseComponent elipseComponent1;
        private Generals.RJButton BtnCancelar;
        private Generals.RJButton BtnBackup;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPath;
        private Generals.RJButton btnExaminar;
        private System.Windows.Forms.Label lblcurrentdate;
        private System.Windows.Forms.Label labelFilename;
        private System.Windows.Forms.Label labelProductName;
        private System.Windows.Forms.Label lblfilename;
    }
}