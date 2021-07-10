using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace arquitectSoft.View
{
    public partial class FrmAnalisisDatos : Form
    {
        public FrmAnalisisDatos()
        {
            InitializeComponent();
        }

        private void BtnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnCargar_Click(object sender, EventArgs e)
        {
            Dto.AnalisisDatosDto dto = new Dto.AnalisisDatosDto();

            DialogResult dr = this.openFileDialog1.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                FrmLoading frmloading = new FrmLoading();
                frmloading.Show();

                Task t = Task.Run(() => {
                    Random rnd = new Random();
                    long sum = 0;
                    int n = 5000000;
                    for (int ctr = 1; ctr <= n; ctr++)
                    {
                        int number = rnd.Next(0, 101);
                        sum += number;
                    }
                    Console.WriteLine("Total:   {0:N0}", sum);
                    Console.WriteLine("Mean:    {0:N2}", sum / n);
                    Console.WriteLine("N:       {0:N0}", n);
                });
                TimeSpan ts = TimeSpan.FromMilliseconds(15000);
                if (!t.Wait(ts))
                    Console.WriteLine("The timeout interval elapsed.");

                DataTable dtPuertas = new DataTable();
                DataTable dtPerfil = new DataTable();
                int pageinitial = 0;
                bool perfilandvidrios = false;
                           

                foreach (String file in openFileDialog1.FileNames)
                {
                    FileInfo Archivo = new FileInfo(file);
                    int idDocumento = int.Parse(Archivo.Name.ToString().Split('-')[0].Trim());
                    

                    pageinitial = idDocumento < pageinitial ? idDocumento : pageinitial;

                    List<Object[]> listData = new List<Object[]>();
                    List<String> listColumns = new List<String>();

                    listColumns = dto.setCreateColumns(idDocumento);

                    listData = dto.readFileTxt(file, dto.ValidationSplit(file));

                    DataTable dtResul = new DataTable();
                    DataTable dtcalculate = new DataTable();

                    dtResul = dto.showTab(idDocumento, listColumns, listData);
                
                    dtPuertas = idDocumento == 3 ? dtResul : dtPuertas;

                    if (idDocumento == 2)
                    {
                        dtcalculate = dto.CalculateTab(1, dtResul, dtPuertas, true);
                        dtcalculate.Merge(dtPerfil);
                        dataGridViewPMCalculate.DataSource = dtcalculate;
                        dataGridViewPMCalculate.Columns[0].Visible = false;
                        dataGridViewPMCalculate.Columns[6].Visible = false;
                    }  

                    dtcalculate = dto.CalculateTab(idDocumento, dtResul, dtPuertas, perfilandvidrios);

                    if (idDocumento == 1) { dtPerfil = dtcalculate; }

                    SetDataView(dtResul, dtcalculate, idDocumento);
                    
                }
                frmloading.Close();
                lblestadosAnalitica.Text = "Analitica Aplicada Correctamente!";

            }
        } 

        private void FrmAnalisisDatos_Load(object sender, EventArgs e)
        {
            InitializeOpenFileDialog();
        }

        private void InitializeOpenFileDialog()
        {
            // Set the file dialog to filter for graphics files.
            this.openFileDialog1.Filter = "All Files *.txt | *.txt";

            //  Allow the user to select multiple images.
            this.openFileDialog1.Multiselect = true;
            //                   ^  ^  ^  ^  ^  ^  ^

            this.openFileDialog1.Title = "My Image Browser";
        }
       

        private void SetDataView(DataTable dt, DataTable dtcalculate, int index)
        {
            switch (index)
            {

                case 1:
                    dataGridViewPM.DataSource = dt;
                    dataGridViewPMCalculate.DataSource = dtcalculate;
                    dataGridViewPMCalculate.Columns[0].Visible = false;
                    dataGridViewPMCalculate.Columns[6].Visible = false;
                    break;
                case 2:
                    dataGridViewVP.DataSource = dt;
                    dataGridViewVPCalculate.DataSource = dtcalculate;
                    dataGridViewVPCalculate.Columns[0].Visible = false;
                    break;
                case 3:
                    dataGridViewP.DataSource = dt;
                    dataGridViewPCalculate.DataSource = dtcalculate;
                    dataGridViewPCalculate.Refresh();
                    dataGridViewPCalculate.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                    break;
                case 4:
                    dataGridViewTM.DataSource = dt;
                    dataGridViewTMCalculate.DataSource = dtcalculate;
                    dataGridViewTMCalculate.Columns[0].Visible = false;
                    dataGridViewTMCalculate.Columns[6].Visible = false;
                    break;
                case 5:
                    dataGridViewM.DataSource = dt;
                    dataGridViewMCalculate.DataSource = dtcalculate;
                    break;
            }


        }

        private void dataGridViewPCalculate_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            foreach (DataGridViewRow r in this.dataGridViewPCalculate.Rows)
            {
                if (r.Cells[0].Value.ToString() == "")
                {
                    r.DefaultCellStyle.BackColor = Color.Gray;
                }else if (r.Cells[0].Value.ToString().Contains("Puerta"))
                {
                    r.DefaultCellStyle.BackColor = Color.Orange;
                }
            }
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            foreach (Control c in this.tabPrincipal.Controls)
            {
                if (c is TabPage)
                {
                    foreach (Control d in c.Controls)
                    {
                        if (d is DataGridView)
                        {
                            DataGridView dgv = (DataGridView)d;
                            dgv.DataSource = "";                            
                        }
                    }
                }
            }

            lblestadosAnalitica.Text = "";
        }
    }
}
