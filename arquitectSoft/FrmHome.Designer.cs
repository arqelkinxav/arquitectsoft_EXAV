
namespace arquitectSoft
{
    partial class FrmHome
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmHome));
            this.pictureBoxMdiPrincipal = new System.Windows.Forms.PictureBox();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.TMSItem_Administracion = new System.Windows.Forms.ToolStripMenuItem();
            this.TMSItem_componente = new System.Windows.Forms.ToolStripMenuItem();
            this.TMSItem_subComponente = new System.Windows.Forms.ToolStripMenuItem();
            this.TMSItem_configuracion = new System.Windows.Forms.ToolStripMenuItem();
            this.TMSItem_acabados = new System.Windows.Forms.ToolStripMenuItem();
            this.TMSItem_cortes = new System.Windows.Forms.ToolStripMenuItem();
            this.unidadDeMedidaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mecanizadoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TMSItem_procesos = new System.Windows.Forms.ToolStripMenuItem();
            this.calcularCantidadesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.salirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.acercaDeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.Mdi_nameConnect2 = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMdiPrincipal)).BeginInit();
            this.menuStrip.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBoxMdiPrincipal
            // 
            this.pictureBoxMdiPrincipal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxMdiPrincipal.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxMdiPrincipal.Image = global::arquitectSoft.Properties.Resources.Logo__1_;
            this.pictureBoxMdiPrincipal.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureBoxMdiPrincipal.Location = new System.Drawing.Point(680, 319);
            this.pictureBoxMdiPrincipal.Name = "pictureBoxMdiPrincipal";
            this.pictureBoxMdiPrincipal.Size = new System.Drawing.Size(108, 106);
            this.pictureBoxMdiPrincipal.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxMdiPrincipal.TabIndex = 5;
            this.pictureBoxMdiPrincipal.TabStop = false;
            // 
            // menuStrip
            // 
            this.menuStrip.BackColor = System.Drawing.SystemColors.Desktop;
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TMSItem_Administracion,
            this.TMSItem_procesos,
            this.salirToolStripMenuItem,
            this.acercaDeToolStripMenuItem});
            this.menuStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.menuStrip.Size = new System.Drawing.Size(800, 24);
            this.menuStrip.TabIndex = 6;
            this.menuStrip.Text = "MenuStrip";
            // 
            // TMSItem_Administracion
            // 
            this.TMSItem_Administracion.BackColor = System.Drawing.SystemColors.Desktop;
            this.TMSItem_Administracion.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TMSItem_componente,
            this.TMSItem_subComponente,
            this.TMSItem_configuracion});
            this.TMSItem_Administracion.ForeColor = System.Drawing.SystemColors.Control;
            this.TMSItem_Administracion.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.TMSItem_Administracion.Name = "TMSItem_Administracion";
            this.TMSItem_Administracion.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.A)));
            this.TMSItem_Administracion.Size = new System.Drawing.Size(100, 20);
            this.TMSItem_Administracion.Text = "&Administracion";
            // 
            // TMSItem_componente
            // 
            this.TMSItem_componente.BackColor = System.Drawing.SystemColors.Desktop;
            this.TMSItem_componente.ForeColor = System.Drawing.SystemColors.Control;
            this.TMSItem_componente.Name = "TMSItem_componente";
            this.TMSItem_componente.Size = new System.Drawing.Size(164, 22);
            this.TMSItem_componente.Text = "Componente";
            this.TMSItem_componente.Click += new System.EventHandler(this.TMSItem_componente_Click);
            // 
            // TMSItem_subComponente
            // 
            this.TMSItem_subComponente.BackColor = System.Drawing.SystemColors.Desktop;
            this.TMSItem_subComponente.ForeColor = System.Drawing.SystemColors.Control;
            this.TMSItem_subComponente.Name = "TMSItem_subComponente";
            this.TMSItem_subComponente.Size = new System.Drawing.Size(164, 22);
            this.TMSItem_subComponente.Text = "SubComponente";
            // 
            // TMSItem_configuracion
            // 
            this.TMSItem_configuracion.BackColor = System.Drawing.SystemColors.Desktop;
            this.TMSItem_configuracion.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TMSItem_acabados,
            this.TMSItem_cortes,
            this.unidadDeMedidaToolStripMenuItem,
            this.mecanizadoToolStripMenuItem});
            this.TMSItem_configuracion.ForeColor = System.Drawing.SystemColors.Control;
            this.TMSItem_configuracion.Name = "TMSItem_configuracion";
            this.TMSItem_configuracion.Size = new System.Drawing.Size(164, 22);
            this.TMSItem_configuracion.Text = "Configuracion";
            // 
            // TMSItem_acabados
            // 
            this.TMSItem_acabados.BackColor = System.Drawing.SystemColors.Desktop;
            this.TMSItem_acabados.ForeColor = System.Drawing.SystemColors.Control;
            this.TMSItem_acabados.Name = "TMSItem_acabados";
            this.TMSItem_acabados.Size = new System.Drawing.Size(171, 22);
            this.TMSItem_acabados.Text = "Acabados";
            // 
            // TMSItem_cortes
            // 
            this.TMSItem_cortes.BackColor = System.Drawing.SystemColors.Desktop;
            this.TMSItem_cortes.ForeColor = System.Drawing.SystemColors.Control;
            this.TMSItem_cortes.Name = "TMSItem_cortes";
            this.TMSItem_cortes.Size = new System.Drawing.Size(171, 22);
            this.TMSItem_cortes.Text = "Cortes";
            // 
            // unidadDeMedidaToolStripMenuItem
            // 
            this.unidadDeMedidaToolStripMenuItem.BackColor = System.Drawing.SystemColors.Desktop;
            this.unidadDeMedidaToolStripMenuItem.ForeColor = System.Drawing.SystemColors.Control;
            this.unidadDeMedidaToolStripMenuItem.Name = "unidadDeMedidaToolStripMenuItem";
            this.unidadDeMedidaToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.unidadDeMedidaToolStripMenuItem.Text = "Unidad de Medida";
            this.unidadDeMedidaToolStripMenuItem.Visible = false;
            // 
            // mecanizadoToolStripMenuItem
            // 
            this.mecanizadoToolStripMenuItem.BackColor = System.Drawing.SystemColors.Desktop;
            this.mecanizadoToolStripMenuItem.ForeColor = System.Drawing.SystemColors.Control;
            this.mecanizadoToolStripMenuItem.Name = "mecanizadoToolStripMenuItem";
            this.mecanizadoToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.mecanizadoToolStripMenuItem.Text = "Mecanizados";
            // 
            // TMSItem_procesos
            // 
            this.TMSItem_procesos.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.calcularCantidadesToolStripMenuItem});
            this.TMSItem_procesos.ForeColor = System.Drawing.SystemColors.Control;
            this.TMSItem_procesos.Name = "TMSItem_procesos";
            this.TMSItem_procesos.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.P)));
            this.TMSItem_procesos.Size = new System.Drawing.Size(66, 20);
            this.TMSItem_procesos.Text = "&Procesos";
            // 
            // calcularCantidadesToolStripMenuItem
            // 
            this.calcularCantidadesToolStripMenuItem.BackColor = System.Drawing.SystemColors.Desktop;
            this.calcularCantidadesToolStripMenuItem.ForeColor = System.Drawing.SystemColors.Control;
            this.calcularCantidadesToolStripMenuItem.Name = "calcularCantidadesToolStripMenuItem";
            this.calcularCantidadesToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.calcularCantidadesToolStripMenuItem.Text = "Analisis de Datos";
            // 
            // salirToolStripMenuItem
            // 
            this.salirToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.salirToolStripMenuItem.Name = "salirToolStripMenuItem";
            this.salirToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.salirToolStripMenuItem.Text = "Salir";
            this.salirToolStripMenuItem.Visible = false;
            // 
            // acercaDeToolStripMenuItem
            // 
            this.acercaDeToolStripMenuItem.ForeColor = System.Drawing.SystemColors.Control;
            this.acercaDeToolStripMenuItem.Name = "acercaDeToolStripMenuItem";
            this.acercaDeToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.acercaDeToolStripMenuItem.Text = "Acerca de";
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.Black;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Mdi_nameConnect2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.statusStrip1.Size = new System.Drawing.Size(800, 22);
            this.statusStrip1.TabIndex = 7;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // Mdi_nameConnect2
            // 
            this.Mdi_nameConnect2.BackColor = System.Drawing.SystemColors.Control;
            this.Mdi_nameConnect2.ForeColor = System.Drawing.SystemColors.Control;
            this.Mdi_nameConnect2.Name = "Mdi_nameConnect2";
            this.Mdi_nameConnect2.Size = new System.Drawing.Size(111, 17);
            this.Mdi_nameConnect2.Text = "Usuario Conectado:";
            // 
            // FrmHome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::arquitectSoft.Properties.Resources.Wallpaper_final;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.pictureBoxMdiPrincipal);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmHome";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMdiPrincipal)).EndInit();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxMdiPrincipal;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem TMSItem_Administracion;
        private System.Windows.Forms.ToolStripMenuItem TMSItem_componente;
        private System.Windows.Forms.ToolStripMenuItem TMSItem_subComponente;
        private System.Windows.Forms.ToolStripMenuItem TMSItem_configuracion;
        private System.Windows.Forms.ToolStripMenuItem TMSItem_acabados;
        private System.Windows.Forms.ToolStripMenuItem TMSItem_cortes;
        private System.Windows.Forms.ToolStripMenuItem unidadDeMedidaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mecanizadoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem TMSItem_procesos;
        private System.Windows.Forms.ToolStripMenuItem calcularCantidadesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem salirToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem acercaDeToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel Mdi_nameConnect2;
    }
}