namespace arquitectSoft
{
    partial class FrmChange
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
            this.label2 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.ChkTempDest = new System.Windows.Forms.CheckBox();
            this.txtAcabado1 = new System.Windows.Forms.TextBox();
            this.btnacabado1 = new System.Windows.Forms.Button();
            this.btnacabado2 = new System.Windows.Forms.Button();
            this.txtAcabado2 = new System.Windows.Forms.TextBox();
            this.ChkTempOrig = new System.Windows.Forms.CheckBox();
            this.BtnCancelar = new arquitectSoft.Generals.RJButton();
            this.BtnAceptar = new arquitectSoft.Generals.RJButton();
            this.elipseControl3 = new arquitectSoft.Generals.ElipseControl();
            this.elipseControl6 = new arquitectSoft.Generals.ElipseControl();
            this.elipseControl1 = new arquitectSoft.Generals.ElipseControl();
            this.elipseControl5 = new arquitectSoft.Generals.ElipseControl();
            this.elipseControl2 = new arquitectSoft.Generals.ElipseControl();
            this.elipseComponent1 = new arquitectSoft.Generals.ElipseComponent();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Black;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(44, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(162, 18);
            this.label2.TabIndex = 100;
            this.label2.Text = "Cambio de Informacion";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.White;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(8, 153);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(108, 13);
            this.label7.TabIndex = 106;
            this.label7.Text = "Acabado Destino:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.White;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(7, 123);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(102, 13);
            this.label6.TabIndex = 105;
            this.label6.Text = "Acabado Origen:";
            // 
            // ChkTempDest
            // 
            this.ChkTempDest.AutoSize = true;
            this.ChkTempDest.BackColor = System.Drawing.Color.White;
            this.ChkTempDest.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.ChkTempDest.Location = new System.Drawing.Point(142, 91);
            this.ChkTempDest.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ChkTempDest.Name = "ChkTempDest";
            this.ChkTempDest.Size = new System.Drawing.Size(125, 17);
            this.ChkTempDest.TabIndex = 109;
            this.ChkTempDest.Text = "Temporal Destino";
            this.ChkTempDest.UseVisualStyleBackColor = false;
            this.ChkTempDest.CheckedChanged += new System.EventHandler(this.ChkTempDest_CheckedChanged);
            // 
            // txtAcabado1
            // 
            this.txtAcabado1.Location = new System.Drawing.Point(142, 122);
            this.txtAcabado1.Name = "txtAcabado1";
            this.txtAcabado1.ReadOnly = true;
            this.txtAcabado1.Size = new System.Drawing.Size(308, 20);
            this.txtAcabado1.TabIndex = 111;
            // 
            // btnacabado1
            // 
            this.btnacabado1.Location = new System.Drawing.Point(457, 121);
            this.btnacabado1.Name = "btnacabado1";
            this.btnacabado1.Size = new System.Drawing.Size(32, 23);
            this.btnacabado1.TabIndex = 112;
            this.btnacabado1.Text = "...";
            this.btnacabado1.UseVisualStyleBackColor = true;
            this.btnacabado1.Click += new System.EventHandler(this.btnacabado1_Click);
            // 
            // btnacabado2
            // 
            this.btnacabado2.Location = new System.Drawing.Point(457, 152);
            this.btnacabado2.Name = "btnacabado2";
            this.btnacabado2.Size = new System.Drawing.Size(32, 23);
            this.btnacabado2.TabIndex = 114;
            this.btnacabado2.Text = "...";
            this.btnacabado2.UseVisualStyleBackColor = true;
            this.btnacabado2.Click += new System.EventHandler(this.btnacabado2_Click);
            // 
            // txtAcabado2
            // 
            this.txtAcabado2.Location = new System.Drawing.Point(142, 154);
            this.txtAcabado2.Name = "txtAcabado2";
            this.txtAcabado2.ReadOnly = true;
            this.txtAcabado2.Size = new System.Drawing.Size(308, 20);
            this.txtAcabado2.TabIndex = 113;
            // 
            // ChkTempOrig
            // 
            this.ChkTempOrig.AutoSize = true;
            this.ChkTempOrig.BackColor = System.Drawing.Color.White;
            this.ChkTempOrig.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.ChkTempOrig.Location = new System.Drawing.Point(17, 91);
            this.ChkTempOrig.Margin = new System.Windows.Forms.Padding(2);
            this.ChkTempOrig.Name = "ChkTempOrig";
            this.ChkTempOrig.Size = new System.Drawing.Size(119, 17);
            this.ChkTempOrig.TabIndex = 115;
            this.ChkTempOrig.Text = "Temporal Origen";
            this.ChkTempOrig.UseVisualStyleBackColor = false;
            this.ChkTempOrig.CheckedChanged += new System.EventHandler(this.ChkTempOrig_CheckedChanged);
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
            this.BtnCancelar.Location = new System.Drawing.Point(105, 198);
            this.BtnCancelar.Name = "BtnCancelar";
            this.BtnCancelar.Size = new System.Drawing.Size(84, 22);
            this.BtnCancelar.TabIndex = 104;
            this.BtnCancelar.Text = "Cancelar";
            this.BtnCancelar.TextColor = System.Drawing.Color.DimGray;
            this.BtnCancelar.UseVisualStyleBackColor = false;
            this.BtnCancelar.Click += new System.EventHandler(this.BtnCancelar_Click);
            // 
            // BtnAceptar
            // 
            this.BtnAceptar.BackColor = System.Drawing.Color.DimGray;
            this.BtnAceptar.BackgroundColor = System.Drawing.Color.DimGray;
            this.BtnAceptar.BorderColor = System.Drawing.Color.Black;
            this.BtnAceptar.BorderRadius = 10;
            this.BtnAceptar.BorderSize = 0;
            this.BtnAceptar.FlatAppearance.BorderSize = 0;
            this.BtnAceptar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnAceptar.ForeColor = System.Drawing.Color.White;
            this.BtnAceptar.Location = new System.Drawing.Point(8, 198);
            this.BtnAceptar.Name = "BtnAceptar";
            this.BtnAceptar.Size = new System.Drawing.Size(84, 22);
            this.BtnAceptar.TabIndex = 103;
            this.BtnAceptar.Text = "Aceptar";
            this.BtnAceptar.TextColor = System.Drawing.Color.White;
            this.BtnAceptar.UseCompatibleTextRendering = true;
            this.BtnAceptar.UseVisualStyleBackColor = false;
            this.BtnAceptar.Click += new System.EventHandler(this.BtnAceptar_Click);
            // 
            // elipseControl3
            // 
            this.elipseControl3.BackColor = System.Drawing.Color.Black;
            this.elipseControl3.CornerRadius = 15;
            this.elipseControl3.Location = new System.Drawing.Point(0, 190);
            this.elipseControl3.Name = "elipseControl3";
            this.elipseControl3.Size = new System.Drawing.Size(500, 40);
            this.elipseControl3.TabIndex = 102;
            this.elipseControl3.Text = "elipseControl1";
            // 
            // elipseControl6
            // 
            this.elipseControl6.BackColor = System.Drawing.Color.Black;
            this.elipseControl6.CornerRadius = 15;
            this.elipseControl6.Location = new System.Drawing.Point(14, 44);
            this.elipseControl6.Name = "elipseControl6";
            this.elipseControl6.Size = new System.Drawing.Size(467, 32);
            this.elipseControl6.TabIndex = 99;
            this.elipseControl6.Text = "elipseControl1";
            // 
            // elipseControl1
            // 
            this.elipseControl1.BackColor = System.Drawing.Color.Black;
            this.elipseControl1.CornerRadius = 15;
            this.elipseControl1.Location = new System.Drawing.Point(17, 43);
            this.elipseControl1.Name = "elipseControl1";
            this.elipseControl1.Size = new System.Drawing.Size(467, 32);
            this.elipseControl1.TabIndex = 99;
            this.elipseControl1.Text = "elipseControl1";
            // 
            // elipseControl5
            // 
            this.elipseControl5.BackColor = System.Drawing.Color.Black;
            this.elipseControl5.CornerRadius = 15;
            this.elipseControl5.Location = new System.Drawing.Point(-2, 0);
            this.elipseControl5.Name = "elipseControl5";
            this.elipseControl5.Size = new System.Drawing.Size(501, 48);
            this.elipseControl5.TabIndex = 98;
            this.elipseControl5.Text = "elipseControl1";
            // 
            // elipseControl2
            // 
            this.elipseControl2.BackColor = System.Drawing.Color.White;
            this.elipseControl2.CornerRadius = 15;
            this.elipseControl2.Location = new System.Drawing.Point(3, 63);
            this.elipseControl2.Name = "elipseControl2";
            this.elipseControl2.Size = new System.Drawing.Size(498, 130);
            this.elipseControl2.TabIndex = 101;
            this.elipseControl2.Text = "elipseControl1";
            // 
            // elipseComponent1
            // 
            this.elipseComponent1.CornerRadius = 15;
            this.elipseComponent1.TargetControl = this;
            // 
            // FrmChange
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.ClientSize = new System.Drawing.Size(500, 230);
            this.ControlBox = false;
            this.Controls.Add(this.ChkTempOrig);
            this.Controls.Add(this.btnacabado2);
            this.Controls.Add(this.txtAcabado2);
            this.Controls.Add(this.btnacabado1);
            this.Controls.Add(this.txtAcabado1);
            this.Controls.Add(this.ChkTempDest);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.BtnCancelar);
            this.Controls.Add(this.BtnAceptar);
            this.Controls.Add(this.elipseControl3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.elipseControl6);
            this.Controls.Add(this.elipseControl1);
            this.Controls.Add(this.elipseControl5);
            this.Controls.Add(this.elipseControl2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "FrmChange";
            this.Padding = new System.Windows.Forms.Padding(9, 9, 9, 9);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmChange";
            this.Load += new System.EventHandler(this.FrmChange_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Generals.ElipseControl elipseControl3;
        private System.Windows.Forms.Label label2;
        private Generals.ElipseControl elipseControl1;
        private Generals.ElipseControl elipseControl2;
        private Generals.ElipseControl elipseControl5;
        private Generals.ElipseControl elipseControl6;
        private Generals.RJButton BtnCancelar;
        private Generals.RJButton BtnAceptar;
        private Generals.ElipseComponent elipseComponent1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox ChkTempDest;
        private System.Windows.Forms.Button btnacabado1;
        private System.Windows.Forms.TextBox txtAcabado1;
        private System.Windows.Forms.Button btnacabado2;
        private System.Windows.Forms.TextBox txtAcabado2;
        private System.Windows.Forms.CheckBox ChkTempOrig;
    }
}