
namespace arquitectSoft.View
{
    partial class FrmComponenteBorrar
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
            this.BtnBorrar = new System.Windows.Forms.Button();
            this.BtnCancelar = new System.Windows.Forms.Button();
            this.GroupComp = new System.Windows.Forms.GroupBox();
            this.GroupEspecial = new System.Windows.Forms.GroupBox();
            this.TxtIndexComp = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.TxtDescripcion = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.TxtDescripcionEsp = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.TxtIndexCompEsp = new System.Windows.Forms.TextBox();
            this.ChkCompOriginal = new System.Windows.Forms.CheckBox();
            this.ChkCompEspecial = new System.Windows.Forms.CheckBox();
            this.GroupComp.SuspendLayout();
            this.GroupEspecial.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnBorrar
            // 
            this.BtnBorrar.Location = new System.Drawing.Point(114, 185);
            this.BtnBorrar.Name = "BtnBorrar";
            this.BtnBorrar.Size = new System.Drawing.Size(75, 23);
            this.BtnBorrar.TabIndex = 2;
            this.BtnBorrar.Text = "Borrar";
            this.BtnBorrar.UseVisualStyleBackColor = true;
            this.BtnBorrar.Click += new System.EventHandler(this.BtnBorrar_Click);
            // 
            // BtnCancelar
            // 
            this.BtnCancelar.Location = new System.Drawing.Point(215, 185);
            this.BtnCancelar.Name = "BtnCancelar";
            this.BtnCancelar.Size = new System.Drawing.Size(75, 23);
            this.BtnCancelar.TabIndex = 3;
            this.BtnCancelar.Text = "Cancelar";
            this.BtnCancelar.UseVisualStyleBackColor = true;
            this.BtnCancelar.Click += new System.EventHandler(this.BtnCancelar_Click);
            // 
            // GroupComp
            // 
            this.GroupComp.Controls.Add(this.ChkCompOriginal);
            this.GroupComp.Controls.Add(this.label2);
            this.GroupComp.Controls.Add(this.TxtDescripcion);
            this.GroupComp.Controls.Add(this.label1);
            this.GroupComp.Controls.Add(this.TxtIndexComp);
            this.GroupComp.ForeColor = System.Drawing.SystemColors.Control;
            this.GroupComp.Location = new System.Drawing.Point(12, 12);
            this.GroupComp.Name = "GroupComp";
            this.GroupComp.Size = new System.Drawing.Size(427, 71);
            this.GroupComp.TabIndex = 4;
            this.GroupComp.TabStop = false;
            this.GroupComp.Text = "Componente";
            // 
            // GroupEspecial
            // 
            this.GroupEspecial.Controls.Add(this.ChkCompEspecial);
            this.GroupEspecial.Controls.Add(this.label3);
            this.GroupEspecial.Controls.Add(this.TxtDescripcionEsp);
            this.GroupEspecial.Controls.Add(this.label4);
            this.GroupEspecial.Controls.Add(this.TxtIndexCompEsp);
            this.GroupEspecial.ForeColor = System.Drawing.SystemColors.Control;
            this.GroupEspecial.Location = new System.Drawing.Point(12, 89);
            this.GroupEspecial.Name = "GroupEspecial";
            this.GroupEspecial.Size = new System.Drawing.Size(427, 71);
            this.GroupEspecial.TabIndex = 5;
            this.GroupEspecial.TabStop = false;
            this.GroupEspecial.Text = "Componente Especial";
            // 
            // TxtIndexComp
            // 
            this.TxtIndexComp.Location = new System.Drawing.Point(45, 33);
            this.TxtIndexComp.Name = "TxtIndexComp";
            this.TxtIndexComp.Size = new System.Drawing.Size(33, 20);
            this.TxtIndexComp.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Index:";
            // 
            // TxtDescripcion
            // 
            this.TxtDescripcion.Location = new System.Drawing.Point(165, 33);
            this.TxtDescripcion.Name = "TxtDescripcion";
            this.TxtDescripcion.Size = new System.Drawing.Size(218, 20);
            this.TxtDescripcion.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(99, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Descripción:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(99, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Descripción:";
            // 
            // TxtDescripcionEsp
            // 
            this.TxtDescripcionEsp.Location = new System.Drawing.Point(165, 35);
            this.TxtDescripcionEsp.Name = "TxtDescripcionEsp";
            this.TxtDescripcionEsp.Size = new System.Drawing.Size(218, 20);
            this.TxtDescripcionEsp.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 38);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Index:";
            // 
            // TxtIndexCompEsp
            // 
            this.TxtIndexCompEsp.Location = new System.Drawing.Point(45, 35);
            this.TxtIndexCompEsp.Name = "TxtIndexCompEsp";
            this.TxtIndexCompEsp.Size = new System.Drawing.Size(33, 20);
            this.TxtIndexCompEsp.TabIndex = 4;
            // 
            // ChkCompOriginal
            // 
            this.ChkCompOriginal.AutoSize = true;
            this.ChkCompOriginal.Location = new System.Drawing.Point(406, 36);
            this.ChkCompOriginal.Name = "ChkCompOriginal";
            this.ChkCompOriginal.Size = new System.Drawing.Size(15, 14);
            this.ChkCompOriginal.TabIndex = 6;
            this.ChkCompOriginal.UseVisualStyleBackColor = true;
            // 
            // ChkCompEspecial
            // 
            this.ChkCompEspecial.AutoSize = true;
            this.ChkCompEspecial.Location = new System.Drawing.Point(406, 38);
            this.ChkCompEspecial.Name = "ChkCompEspecial";
            this.ChkCompEspecial.Size = new System.Drawing.Size(15, 14);
            this.ChkCompEspecial.TabIndex = 8;
            this.ChkCompEspecial.UseVisualStyleBackColor = true;
            // 
            // FrmComponenteBorrar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(86)))));
            this.ClientSize = new System.Drawing.Size(449, 220);
            this.Controls.Add(this.GroupEspecial);
            this.Controls.Add(this.GroupComp);
            this.Controls.Add(this.BtnCancelar);
            this.Controls.Add(this.BtnBorrar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmComponenteBorrar";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FrmComponenteBorrar";
            this.Load += new System.EventHandler(this.FrmComponenteBorrar_Load);
            this.GroupComp.ResumeLayout(false);
            this.GroupComp.PerformLayout();
            this.GroupEspecial.ResumeLayout(false);
            this.GroupEspecial.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button BtnBorrar;
        private System.Windows.Forms.Button BtnCancelar;
        private System.Windows.Forms.GroupBox GroupComp;
        private System.Windows.Forms.GroupBox GroupEspecial;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TxtDescripcion;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TxtIndexComp;
        private System.Windows.Forms.CheckBox ChkCompOriginal;
        private System.Windows.Forms.CheckBox ChkCompEspecial;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TxtDescripcionEsp;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox TxtIndexCompEsp;
    }
}