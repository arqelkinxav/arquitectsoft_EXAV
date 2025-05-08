
namespace arquitectSoft
{
    partial class FrmMDIPrincipal
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMDIPrincipal));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.TMSItem_Administracion = new System.Windows.Forms.ToolStripMenuItem();
            this.TMSItem_componente = new System.Windows.Forms.ToolStripMenuItem();
            this.TMSItem_subComponente = new System.Windows.Forms.ToolStripMenuItem();
            this.TMSItem_configuracion = new System.Windows.Forms.ToolStripMenuItem();
            this.TMSItem_acabados = new System.Windows.Forms.ToolStripMenuItem();
            this.TMSItem_cortes = new System.Windows.Forms.ToolStripMenuItem();
            this.unidadDeMedidaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mecanizadoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.salirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TMSItem_procesos = new System.Windows.Forms.ToolStripMenuItem();
            this.calcularCantidadesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.analisisDePuertasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.acercaDeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dBAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.Mdi_nameConnect2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.pictureBoxMdiPrincipal = new System.Windows.Forms.PictureBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMdiPrincipal)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.BackColor = System.Drawing.SystemColors.Desktop;
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TMSItem_Administracion,
            this.salirToolStripMenuItem,
            this.TMSItem_procesos,
            this.acercaDeToolStripMenuItem,
            this.dBAToolStripMenuItem});
            this.menuStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            resources.ApplyResources(this.menuStrip, "menuStrip");
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            // 
            // TMSItem_Administracion
            // 
            this.TMSItem_Administracion.BackColor = System.Drawing.SystemColors.Desktop;
            this.TMSItem_Administracion.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TMSItem_componente,
            this.TMSItem_subComponente,
            this.TMSItem_configuracion});
            this.TMSItem_Administracion.ForeColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.TMSItem_Administracion, "TMSItem_Administracion");
            this.TMSItem_Administracion.Name = "TMSItem_Administracion";
            this.TMSItem_Administracion.MouseEnter += new System.EventHandler(this.TMSItem_Administracion_MouseEnter);
            this.TMSItem_Administracion.MouseLeave += new System.EventHandler(this.TMSItem_Administracion_MouseLeave);
            // 
            // TMSItem_componente
            // 
            this.TMSItem_componente.BackColor = System.Drawing.SystemColors.Control;
            this.TMSItem_componente.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.TMSItem_componente.Name = "TMSItem_componente";
            resources.ApplyResources(this.TMSItem_componente, "TMSItem_componente");
            this.TMSItem_componente.Click += new System.EventHandler(this.TMSItem_componente_Click);
            // 
            // TMSItem_subComponente
            // 
            this.TMSItem_subComponente.BackColor = System.Drawing.SystemColors.Control;
            this.TMSItem_subComponente.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.TMSItem_subComponente.Name = "TMSItem_subComponente";
            resources.ApplyResources(this.TMSItem_subComponente, "TMSItem_subComponente");
            this.TMSItem_subComponente.Click += new System.EventHandler(this.TMSItem_subComponente_Click);
            // 
            // TMSItem_configuracion
            // 
            this.TMSItem_configuracion.BackColor = System.Drawing.SystemColors.Control;
            this.TMSItem_configuracion.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TMSItem_acabados,
            this.TMSItem_cortes,
            this.unidadDeMedidaToolStripMenuItem,
            this.mecanizadoToolStripMenuItem});
            this.TMSItem_configuracion.ForeColor = System.Drawing.SystemColors.Desktop;
            this.TMSItem_configuracion.Name = "TMSItem_configuracion";
            resources.ApplyResources(this.TMSItem_configuracion, "TMSItem_configuracion");
            // 
            // TMSItem_acabados
            // 
            this.TMSItem_acabados.BackColor = System.Drawing.SystemColors.Control;
            this.TMSItem_acabados.ForeColor = System.Drawing.SystemColors.Desktop;
            this.TMSItem_acabados.Name = "TMSItem_acabados";
            resources.ApplyResources(this.TMSItem_acabados, "TMSItem_acabados");
            this.TMSItem_acabados.Click += new System.EventHandler(this.TMSItem_acabados_Click);
            // 
            // TMSItem_cortes
            // 
            this.TMSItem_cortes.BackColor = System.Drawing.SystemColors.Control;
            this.TMSItem_cortes.ForeColor = System.Drawing.SystemColors.Desktop;
            this.TMSItem_cortes.Name = "TMSItem_cortes";
            resources.ApplyResources(this.TMSItem_cortes, "TMSItem_cortes");
            this.TMSItem_cortes.Click += new System.EventHandler(this.TMSItem_cortes_Click);
            // 
            // unidadDeMedidaToolStripMenuItem
            // 
            this.unidadDeMedidaToolStripMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.unidadDeMedidaToolStripMenuItem.ForeColor = System.Drawing.SystemColors.Desktop;
            this.unidadDeMedidaToolStripMenuItem.Name = "unidadDeMedidaToolStripMenuItem";
            resources.ApplyResources(this.unidadDeMedidaToolStripMenuItem, "unidadDeMedidaToolStripMenuItem");
            this.unidadDeMedidaToolStripMenuItem.Click += new System.EventHandler(this.unidadDeMedidaToolStripMenuItem_Click);
            // 
            // mecanizadoToolStripMenuItem
            // 
            this.mecanizadoToolStripMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.mecanizadoToolStripMenuItem.ForeColor = System.Drawing.SystemColors.Desktop;
            this.mecanizadoToolStripMenuItem.Name = "mecanizadoToolStripMenuItem";
            resources.ApplyResources(this.mecanizadoToolStripMenuItem, "mecanizadoToolStripMenuItem");
            this.mecanizadoToolStripMenuItem.Click += new System.EventHandler(this.mecanizadoToolStripMenuItem_Click);
            // 
            // salirToolStripMenuItem
            // 
            this.salirToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.salirToolStripMenuItem.Name = "salirToolStripMenuItem";
            resources.ApplyResources(this.salirToolStripMenuItem, "salirToolStripMenuItem");
            this.salirToolStripMenuItem.Click += new System.EventHandler(this.salirToolStripMenuItem_Click);
            // 
            // TMSItem_procesos
            // 
            this.TMSItem_procesos.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.calcularCantidadesToolStripMenuItem,
            this.analisisDePuertasToolStripMenuItem});
            this.TMSItem_procesos.ForeColor = System.Drawing.SystemColors.Control;
            this.TMSItem_procesos.Name = "TMSItem_procesos";
            resources.ApplyResources(this.TMSItem_procesos, "TMSItem_procesos");
            this.TMSItem_procesos.MouseEnter += new System.EventHandler(this.TMSItem_procesos_MouseEnter);
            this.TMSItem_procesos.MouseLeave += new System.EventHandler(this.TMSItem_procesos_MouseLeave);
            // 
            // calcularCantidadesToolStripMenuItem
            // 
            this.calcularCantidadesToolStripMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.calcularCantidadesToolStripMenuItem.ForeColor = System.Drawing.SystemColors.Desktop;
            this.calcularCantidadesToolStripMenuItem.Name = "calcularCantidadesToolStripMenuItem";
            resources.ApplyResources(this.calcularCantidadesToolStripMenuItem, "calcularCantidadesToolStripMenuItem");
            this.calcularCantidadesToolStripMenuItem.Click += new System.EventHandler(this.calcularCantidadesToolStripMenuItem_Click);
            // 
            // analisisDePuertasToolStripMenuItem
            // 
            this.analisisDePuertasToolStripMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.analisisDePuertasToolStripMenuItem.ForeColor = System.Drawing.SystemColors.Desktop;
            this.analisisDePuertasToolStripMenuItem.Name = "analisisDePuertasToolStripMenuItem";
            resources.ApplyResources(this.analisisDePuertasToolStripMenuItem, "analisisDePuertasToolStripMenuItem");
            this.analisisDePuertasToolStripMenuItem.Click += new System.EventHandler(this.analisisDePuertasToolStripMenuItem_Click);
            // 
            // acercaDeToolStripMenuItem
            // 
            this.acercaDeToolStripMenuItem.ForeColor = System.Drawing.SystemColors.Control;
            this.acercaDeToolStripMenuItem.Name = "acercaDeToolStripMenuItem";
            resources.ApplyResources(this.acercaDeToolStripMenuItem, "acercaDeToolStripMenuItem");
            this.acercaDeToolStripMenuItem.Click += new System.EventHandler(this.acercaDeToolStripMenuItem_Click);
            this.acercaDeToolStripMenuItem.MouseEnter += new System.EventHandler(this.acercaDeToolStripMenuItem_MouseEnter);
            this.acercaDeToolStripMenuItem.MouseLeave += new System.EventHandler(this.acercaDeToolStripMenuItem_MouseLeave);
            // 
            // dBAToolStripMenuItem
            // 
            this.dBAToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportDataToolStripMenuItem,
            this.importDataToolStripMenuItem});
            this.dBAToolStripMenuItem.ForeColor = System.Drawing.SystemColors.Control;
            this.dBAToolStripMenuItem.Name = "dBAToolStripMenuItem";
            resources.ApplyResources(this.dBAToolStripMenuItem, "dBAToolStripMenuItem");
            this.dBAToolStripMenuItem.MouseEnter += new System.EventHandler(this.dBAToolStripMenuItem_MouseEnter);
            this.dBAToolStripMenuItem.MouseLeave += new System.EventHandler(this.dBAToolStripMenuItem_MouseLeave);
            // 
            // exportDataToolStripMenuItem
            // 
            this.exportDataToolStripMenuItem.Name = "exportDataToolStripMenuItem";
            resources.ApplyResources(this.exportDataToolStripMenuItem, "exportDataToolStripMenuItem");
            this.exportDataToolStripMenuItem.Click += new System.EventHandler(this.exportDataToolStripMenuItem_Click);
            // 
            // importDataToolStripMenuItem
            // 
            this.importDataToolStripMenuItem.Name = "importDataToolStripMenuItem";
            resources.ApplyResources(this.importDataToolStripMenuItem, "importDataToolStripMenuItem");
            this.importDataToolStripMenuItem.Click += new System.EventHandler(this.importDataToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.Black;
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Mdi_nameConnect2});
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            // 
            // Mdi_nameConnect2
            // 
            this.Mdi_nameConnect2.BackColor = System.Drawing.SystemColors.Control;
            this.Mdi_nameConnect2.ForeColor = System.Drawing.SystemColors.Control;
            this.Mdi_nameConnect2.Name = "Mdi_nameConnect2";
            resources.ApplyResources(this.Mdi_nameConnect2, "Mdi_nameConnect2");
            // 
            // pictureBoxMdiPrincipal
            // 
            resources.ApplyResources(this.pictureBoxMdiPrincipal, "pictureBoxMdiPrincipal");
            this.pictureBoxMdiPrincipal.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxMdiPrincipal.Name = "pictureBoxMdiPrincipal";
            this.pictureBoxMdiPrincipal.TabStop = false;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // FrmMDIPrincipal
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(82)))));
            this.Controls.Add(this.pictureBoxMdiPrincipal);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip;
            this.Name = "FrmMDIPrincipal";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMDIPrincipal_FormClosing);
            this.Load += new System.EventHandler(this.FrmMDIPrincipal_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMdiPrincipal)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion


        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem TMSItem_Administracion;
        private System.Windows.Forms.ToolStripMenuItem TMSItem_componente;
        private System.Windows.Forms.ToolStripMenuItem TMSItem_subComponente;
        private System.Windows.Forms.ToolStripMenuItem TMSItem_configuracion;
        private System.Windows.Forms.ToolStripMenuItem TMSItem_acabados;
        private System.Windows.Forms.ToolStripMenuItem TMSItem_cortes;
        private System.Windows.Forms.ToolStripMenuItem TMSItem_procesos;
        private System.Windows.Forms.ToolStripMenuItem calcularCantidadesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem salirToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripMenuItem acercaDeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unidadDeMedidaToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel Mdi_nameConnect2;
        private System.Windows.Forms.ToolStripMenuItem mecanizadoToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureBoxMdiPrincipal;
        private System.Windows.Forms.ToolStripMenuItem analisisDePuertasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dBAToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importDataToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}



