
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
            this.TMSItem_procesos = new System.Windows.Forms.ToolStripMenuItem();
            this.calcularCantidadesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.salirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.acercaDeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.Mdi_nameConnect = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();

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
            this.menuStrip.Size = new System.Drawing.Size(632, 24);
            this.menuStrip.TabIndex = 0;
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
            this.TMSItem_Administracion.MouseEnter += new System.EventHandler(this.TMSItem_Administracion_MouseEnter);
            this.TMSItem_Administracion.MouseLeave += new System.EventHandler(this.TMSItem_Administracion_MouseLeave);
            // 
            // TMSItem_componente
            // 
            this.TMSItem_componente.BackColor = System.Drawing.SystemColors.Desktop;
            this.TMSItem_componente.ForeColor = System.Drawing.SystemColors.Control;
            this.TMSItem_componente.Name = "TMSItem_componente";
            this.TMSItem_componente.Size = new System.Drawing.Size(164, 22);
            this.TMSItem_componente.Text = "Componente";
            this.TMSItem_componente.Click += new System.EventHandler(this.TMSItem_componente_Click);
            this.TMSItem_componente.MouseEnter += new System.EventHandler(this.TMSItem_componente_MouseEnter);
            this.TMSItem_componente.MouseLeave += new System.EventHandler(this.TMSItem_componente_MouseLeave);
            // 
            // TMSItem_subComponente
            // 
            this.TMSItem_subComponente.BackColor = System.Drawing.SystemColors.Desktop;
            this.TMSItem_subComponente.ForeColor = System.Drawing.SystemColors.Control;
            this.TMSItem_subComponente.Name = "TMSItem_subComponente";
            this.TMSItem_subComponente.Size = new System.Drawing.Size(164, 22);
            this.TMSItem_subComponente.Text = "SubComponente";
            this.TMSItem_subComponente.Click += new System.EventHandler(this.TMSItem_subComponente_Click);
            this.TMSItem_subComponente.MouseEnter += new System.EventHandler(this.TMSItem_subComponente_MouseEnter);
            this.TMSItem_subComponente.MouseLeave += new System.EventHandler(this.TMSItem_subComponente_MouseLeave);
            // 
            // TMSItem_configuracion
            // 
            this.TMSItem_configuracion.BackColor = System.Drawing.SystemColors.Desktop;
            this.TMSItem_configuracion.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TMSItem_acabados,
            this.TMSItem_cortes,
            this.unidadDeMedidaToolStripMenuItem});
            this.TMSItem_configuracion.ForeColor = System.Drawing.SystemColors.Control;
            this.TMSItem_configuracion.Name = "TMSItem_configuracion";
            this.TMSItem_configuracion.Size = new System.Drawing.Size(164, 22);
            this.TMSItem_configuracion.Text = "Configuracion";
            this.TMSItem_configuracion.MouseEnter += new System.EventHandler(this.TMSItem_configuracion_MouseEnter);
            this.TMSItem_configuracion.MouseLeave += new System.EventHandler(this.TMSItem_configuracion_MouseLeave);
            // 
            // TMSItem_acabados
            // 
            this.TMSItem_acabados.BackColor = System.Drawing.SystemColors.Desktop;
            this.TMSItem_acabados.ForeColor = System.Drawing.SystemColors.Control;
            this.TMSItem_acabados.Name = "TMSItem_acabados";
            this.TMSItem_acabados.Size = new System.Drawing.Size(171, 22);
            this.TMSItem_acabados.Text = "Acabados";
            this.TMSItem_acabados.Click += new System.EventHandler(this.TMSItem_acabados_Click);
            this.TMSItem_acabados.MouseEnter += new System.EventHandler(this.TMSItem_acabados_MouseEnter);
            this.TMSItem_acabados.MouseLeave += new System.EventHandler(this.TMSItem_acabados_MouseLeave);
            // 
            // TMSItem_cortes
            // 
            this.TMSItem_cortes.BackColor = System.Drawing.SystemColors.Desktop;
            this.TMSItem_cortes.ForeColor = System.Drawing.SystemColors.Control;
            this.TMSItem_cortes.Name = "TMSItem_cortes";
            this.TMSItem_cortes.Size = new System.Drawing.Size(171, 22);
            this.TMSItem_cortes.Text = "Cortes";
            this.TMSItem_cortes.Click += new System.EventHandler(this.TMSItem_cortes_Click);
            this.TMSItem_cortes.MouseEnter += new System.EventHandler(this.TMSItem_cortes_MouseEnter);
            this.TMSItem_cortes.MouseLeave += new System.EventHandler(this.TMSItem_cortes_MouseLeave);
            // 
            // unidadDeMedidaToolStripMenuItem
            // 
            this.unidadDeMedidaToolStripMenuItem.BackColor = System.Drawing.SystemColors.Desktop;
            this.unidadDeMedidaToolStripMenuItem.ForeColor = System.Drawing.SystemColors.Control;
            this.unidadDeMedidaToolStripMenuItem.Name = "unidadDeMedidaToolStripMenuItem";
            this.unidadDeMedidaToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.unidadDeMedidaToolStripMenuItem.Text = "Unidad de Medida";
            this.unidadDeMedidaToolStripMenuItem.Click += new System.EventHandler(this.unidadDeMedidaToolStripMenuItem_Click);
            this.unidadDeMedidaToolStripMenuItem.MouseEnter += new System.EventHandler(this.unidadDeMedidaToolStripMenuItem_MouseEnter);
            this.unidadDeMedidaToolStripMenuItem.MouseLeave += new System.EventHandler(this.unidadDeMedidaToolStripMenuItem_MouseLeave);
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
            this.TMSItem_procesos.MouseEnter += new System.EventHandler(this.TMSItem_procesos_MouseEnter);
            this.TMSItem_procesos.MouseLeave += new System.EventHandler(this.TMSItem_procesos_MouseLeave);
            // 
            // calcularCantidadesToolStripMenuItem
            // 
            this.calcularCantidadesToolStripMenuItem.BackColor = System.Drawing.SystemColors.Desktop;
            this.calcularCantidadesToolStripMenuItem.ForeColor = System.Drawing.SystemColors.Control;
            this.calcularCantidadesToolStripMenuItem.Name = "calcularCantidadesToolStripMenuItem";
            this.calcularCantidadesToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.calcularCantidadesToolStripMenuItem.Text = "Analisis de Datos";
            this.calcularCantidadesToolStripMenuItem.Click += new System.EventHandler(this.calcularCantidadesToolStripMenuItem_Click);
            this.calcularCantidadesToolStripMenuItem.MouseEnter += new System.EventHandler(this.calcularCantidadesToolStripMenuItem_MouseEnter);
            this.calcularCantidadesToolStripMenuItem.MouseLeave += new System.EventHandler(this.calcularCantidadesToolStripMenuItem_MouseLeave);
            // 
            // salirToolStripMenuItem
            // 
            this.salirToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.salirToolStripMenuItem.Name = "salirToolStripMenuItem";
            this.salirToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.salirToolStripMenuItem.Text = "Salir";
            this.salirToolStripMenuItem.Visible = false;
            this.salirToolStripMenuItem.Click += new System.EventHandler(this.salirToolStripMenuItem_Click);
            // 
            // acercaDeToolStripMenuItem
            // 
            this.acercaDeToolStripMenuItem.ForeColor = System.Drawing.SystemColors.Control;
            this.acercaDeToolStripMenuItem.Name = "acercaDeToolStripMenuItem";
            this.acercaDeToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.acercaDeToolStripMenuItem.Text = "Acerca de";
            this.acercaDeToolStripMenuItem.Click += new System.EventHandler(this.acercaDeToolStripMenuItem_Click);
            this.acercaDeToolStripMenuItem.MouseEnter += new System.EventHandler(this.acercaDeToolStripMenuItem_MouseEnter);
            this.acercaDeToolStripMenuItem.MouseLeave += new System.EventHandler(this.acercaDeToolStripMenuItem_MouseLeave);
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.Black;
            this.statusStrip1.Location = new System.Drawing.Point(0, 431);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(632, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // Mdi_nameConnect
            // 
            this.Mdi_nameConnect.BackColor = System.Drawing.SystemColors.Control;
            this.Mdi_nameConnect.ForeColor = System.Drawing.SystemColors.Control;
            this.Mdi_nameConnect.Name = "Mdi_nameConnect";
            this.Mdi_nameConnect.Size = new System.Drawing.Size(111, 17);
            this.Mdi_nameConnect.Text = "Usuario Conectado:";
            // 
            // FrmMDIPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(82)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(632, 453);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip;
            this.Name = "FrmMDIPrincipal";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMDIPrincipal_FormClosing);
            this.Load += new System.EventHandler(this.FrmMDIPrincipal_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
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
        private System.Windows.Forms.ToolStripStatusLabel Mdi_nameConnect;
        private System.Windows.Forms.ToolStripMenuItem unidadDeMedidaToolStripMenuItem;
    }
}



