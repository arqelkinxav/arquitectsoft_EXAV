using ClosedXML.Excel;
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
        DataTable dtPuertas = new DataTable();
        DataTable dtPerfil = new DataTable();
        DataTable dtPerfilOfVidrioPanel = new DataTable();

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

                int pageinitial = 0;
                bool perfilandvidrios = false;
                List<int> UseTab = new List<int>();

                foreach (String file in openFileDialog1.FileNames)
                {
                    FileInfo Archivo = new FileInfo(file);
                    int idDocumento = int.Parse(Archivo.Name.ToString().Split('-')[0].Trim());

                    UseTab.Add(idDocumento);

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

                        DataTable dtresulVP = new DataTable();
                        dtPerfilOfVidrioPanel = dto.CalculateTab(1, dtResul, dtPuertas, true);
                        dtresulVP = dtPerfilOfVidrioPanel.Copy();
                        dtresulVP.Merge(dtPerfil);

                        dataGridViewPMCalculate.DataSource = dtresulVP;
                        dataGridViewPMCalculate.Columns[0].Visible = false;
                        dataGridViewPMCalculate.Columns[6].Visible = false;
                    }

                    dtcalculate = dto.CalculateTab(idDocumento, dtResul, dtPuertas, perfilandvidrios);

                    if (idDocumento == 1)
                    {
                        dtPerfil = dtcalculate;

                        DataTable dtresulPM = new DataTable();
                        dtresulPM = dtPerfilOfVidrioPanel.Copy();
                        dtresulPM.Merge(dtPerfil);
                        dtcalculate = dtresulPM;
                    }

                    SetDataView(dtResul, dtcalculate, idDocumento);

                }

                switch (UseTab.First())
                {
                    case 1:
                        tabPrincipal.SelectTab(tabPerfilMetallico);
                        break;
                    case 2:
                        tabPrincipal.SelectTab(tabVidrioPaneles);
                        break;
                    case 3:
                        tabPrincipal.SelectTab(tabPuertas);
                        break;
                    case 4:
                        tabPrincipal.SelectTab(tabTubosMetalicos);
                        break;
                    case 5:
                        tabPrincipal.SelectTab(tabMamparas);
                        break;
                }

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
                }
                else if (r.Cells[0].Value.ToString().Contains("Puerta"))
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
            tabPrincipal.SelectTab(tabPerfilMetallico);
            dtPuertas.Rows.Clear();
            dtPerfil.Rows.Clear();
            dtPerfilOfVidrioPanel.Rows.Clear();

            lblestadosAnalitica.Text = "";
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {

            bool swexport = false;
            if (dataGridViewPMCalculate.RowCount > 0)
            {
                swexport = true;
            }else if (dataGridViewVPCalculate.RowCount > 0)
            {
                swexport = true;
            }else if (dataGridViewPCalculate.RowCount > 0)
            {
                swexport = true;
            }else if (dataGridViewTMCalculate.RowCount > 0)
            {
                swexport = true;
            }
            else if (dataGridViewMCalculate.RowCount > 0)
            {
                swexport = true;
            }

            if (swexport)
            {
                FnExportar();
            }
            else
            {
                MessageBox.Show("No Existen Datos Analizados para Exportar!!", "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            
        }


        private void FnExportar()
        {
            string folderPath = "";
            string filefinish = "";
            FolderBrowserDialog profilePath = new FolderBrowserDialog();
            if (profilePath.ShowDialog() == DialogResult.OK)
            {
                folderPath = profilePath.SelectedPath;
            }
           
            if (folderPath != "")
            {
                //Exporting to Excel.

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                using (XLWorkbook wb = new XLWorkbook())
                {

                    string Range = "A1:G1";
                    string rangetwo = "A{0}:G{0}";
                    string sheets = "";
                    int sheetscount = 0;
                    DataGridView table = new DataGridView();
                    for (int Datagrid = 1; Datagrid <= 5; Datagrid++)
                    {
                        switch (Datagrid)
                        {
                            case 1:
                                sheets = "PERFIL METALICO";
                                table = dataGridViewPMCalculate;
                                break;
                            case 2:
                                sheets = "VIDRIOS Y PANELES";
                                table = dataGridViewVPCalculate;
                                break;
                            case 3:
                                sheets = "PUERTAS";
                                table = dataGridViewPCalculate;
                                break;
                            case 4:
                                sheets = "TUBO METALICOS";
                                table = dataGridViewTMCalculate;
                                break;
                            case 5:
                                Range = "A1:E1";
                                rangetwo = "A{0}:E{0}";
                                sheets = "MAMPARAS";
                                table = dataGridViewMCalculate;
                                break;
                        }

                        if (table.Rows.Count > 0)
                        {
                            sheetscount += 1;
                            //Creating DataTable.
                            DataTable dt = new DataTable();

                            //Adding the Columns.
                            foreach (DataGridViewColumn column in table.Columns)
                            {
                                dt.Columns.Add(column.HeaderText, column.ValueType);
                            }

                            //Adding the Rows.
                            foreach (DataGridViewRow row in table.Rows)
                            {
                                dt.Rows.Add();
                                foreach (DataGridViewCell cell in row.Cells)
                                {
                                    dt.Rows[dt.Rows.Count - 1][cell.ColumnIndex] = cell.Value.ToString();
                                }
                            }

                            wb.Worksheets.Add(dt, sheets);

                            //Set the color of Header Row.
                            //A resembles First Column while C resembles Third column.
                            wb.Worksheet(sheetscount).Cells(Range).Style.Fill.BackgroundColor = XLColor.DarkCoral;
                            for (int i = 1; i <= dt.Rows.Count; i++)
                            {
                                //A resembles First Column while C resembles Third column.
                                //Header row is at Position 1 and hence First row starts from Index 2.
                                string cellRange = string.Format(rangetwo, i + 1);
                                string cellIniPuertas = string.Format("A{0}", i + 1);
                                string valueP = wb.Worksheet(sheetscount).Cell(cellIniPuertas).Value.ToString();
                                if (valueP.Contains("Puerta"))
                                {
                                    wb.Worksheet(sheetscount).Cells(cellRange).Style.Fill.BackgroundColor = XLColor.LightGreen;
                                }
                                else
                                {
                                    if (i % 2 != 0)
                                    {
                                        wb.Worksheet(sheetscount).Cells(cellRange).Style.Fill.BackgroundColor = XLColor.White;
                                    }
                                    else
                                    {
                                        wb.Worksheet(sheetscount).Cells(cellRange).Style.Fill.BackgroundColor = XLColor.LightGray;
                                    }
                                }
                               

                            }
                            //Adjust widths of Columns.
                            wb.Worksheet(sheetscount).Columns().AdjustToContents();
                        }


                    }

                    //Save the Excel file.
                    filefinish = folderPath + "\\DataExport.xlsx";
                    wb.SaveAs(filefinish);
                }
            }
       
            DialogResult result = MessageBox.Show("Se ha Exportado Correctamente los datos!!, Desea Abrirlo En este Momento?", "Mensaje Alerta", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start(filefinish);
            }
            else
            {
                MessageBox.Show("Recuerda que encontraras el archivo en la siguiente ruta: " + filefinish, "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            

        }
    }
}
