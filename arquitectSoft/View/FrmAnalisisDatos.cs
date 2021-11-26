using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace arquitectSoft.View
{
    public partial class FrmAnalisisDatos : Form
    {

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wMsg, int wParam, int lParam);

        DataTable dtPuertas = new DataTable();
        DataTable dtPerfil = new DataTable();
        DataTable dtPerfilR = new DataTable();
        DataTable dtPerfilOfVidrioPanel = new DataTable();

        public FrmAnalisisDatos()
        {
            InitializeComponent();
        }



        private void BtnCargar_Click(object sender, EventArgs e)
        {
            Dto.AnalisisDatosDto dto = new Dto.AnalisisDatosDto();

            DialogResult dr = this.openFileDialog1.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                int medidabase = Int32.Parse(NUpDownMedidaBase.Value.ToString());
                decimal Desperdicio = (decimal.Parse(NUpDownDesperdicio.Value.ToString())/100) + 1;

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
                        dtPerfilOfVidrioPanel = dto.CalculateTab(1, dtResul, dtPuertas, true, medidabase, Desperdicio);
                        dtresulVP = dtPerfilOfVidrioPanel.Copy();
                        dtresulVP.Merge(dtPerfil);

                        dataGridViewPMCalculate.DataSource = dtresulVP;
                        dataGridViewPMCalculate.Columns[0].Visible = false;
                        dataGridViewPMCalculate.Columns[6].Visible = false;
                    }



                    dtcalculate = dto.CalculateTab(idDocumento, dtResul, dtPuertas, perfilandvidrios, medidabase, Desperdicio);

                    if (idDocumento == 1)
                    {
                        dtPerfil = dtcalculate;
                        dtPerfilR = dtResul;
                        DataTable dtresulPM = new DataTable();
                        dtresulPM = dtPerfilOfVidrioPanel.Copy();
                        dtresulPM.Merge(dtPerfil);
                        dtcalculate = dtresulPM;

                        DataTable dtresulPMHerraje = new DataTable();
                        dtresulPMHerraje = dto.CalculateTab(8, dtResul, dtPuertas, perfilandvidrios, medidabase, Desperdicio);
                        dataGridViewPMHerrajeCalculate.DataSource = dtresulPMHerraje;
                        dataGridViewPMHerrajeCalculate.Columns[0].Visible = false;
                        dataGridViewPMHerrajeCalculate.Columns[6].Visible = false;

                    }
                    if (idDocumento == 3)
                    {

                        DataTable dtresulPC = new DataTable();
                        dtresulPC = dto.CalculateTab(6, dtResul, dtPuertas, true, medidabase, Desperdicio);
                        dataGridViewP2Calculate.DataSource = dtresulPC;
                        dataGridViewP2Calculate.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                        DataTable dtresulPHerraje = new DataTable();
                        dtresulPHerraje = dto.CalculateTab(7, dtResul, dtPuertas, true, medidabase, Desperdicio);
                        dataGridViewPHerrajeCalculate.DataSource = dtresulPHerraje;
                        dataGridViewPHerrajeCalculate.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
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

            NUpDownMedidaBase.Value = 2960;
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
                    dataGridViewPMHerraje.DataSource = dt;
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
                    dataGridViewP2.DataSource = dt;
                    dataGridViewPHerraje.DataSource = dt;
                    dataGridViewPCalculate.DataSource = dtcalculate;
                    dataGridViewPCalculate.Refresh();
                    dataGridViewPCalculate.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                    foreach (DataGridViewColumn col in dataGridViewPCalculate.Columns)
                    {
                        col.SortMode = DataGridViewColumnSortMode.NotSortable;
                    }

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

            FrmLoading bsc = new FrmLoading();
            bsc.ShowDialog();

            if (bsc.Numero == null)
            {
                return;
            }

            string[] param = { bsc.Numero, bsc.Nombre, bsc.Tecnico, bsc.Fecha, bsc.Acabado1, bsc.Acabado2 };

            bool swexport = false;
            if (dataGridViewPMCalculate.RowCount > 0)
            {
                swexport = true;
            }
            else if (dataGridViewVPCalculate.RowCount > 0)
            {
                swexport = true;
            }
            else if (dataGridViewPCalculate.RowCount > 0)
            {
                swexport = true;
            }
            else if (dataGridViewTMCalculate.RowCount > 0)
            {
                swexport = true;
            }
            else if (dataGridViewMCalculate.RowCount > 0)
            {
                swexport = true;
            }
            else if (dataGridViewP2Calculate.RowCount > 0)
            {
                swexport = true;
            }

            if (swexport)
            {
                FnExportar(param);
            }
            else
            {
                MessageBox.Show("No Existen Datos Analizados para Exportar!!", "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }


        private void FnExportar(string[] param)
        {
            string folderPath = "";
            string filefinish = "";
            bool swend = true;

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

                    int valueinitial = 8;
                    string Range = string.Format("A{0}:G{0}", valueinitial);
                    string Rangeheader = "A2:G4";
                    string RangeSubheader = "A5:G6";
                    string rangetwo = "A{0}:G{0}";
                    string sheets = "";

                    //int sheetscount = 1;

                    //wb.Worksheets.Add("PRINCIPAL");
                    //wb.Worksheet(1).ShowGridLines = new BooleanValue(false);
                    //wb.Worksheet(1).Cell("B7").Value = "DATOS";
                    //var range = wb.Worksheet(1).Range("B7:C7");
                    //range.Merge().Style.Font.SetBold().Font.FontSize = 16;
                    //range.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    ////var rangelogo = wb.Worksheet(1).Range("B2:C6");
                    ////rangelogo.Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    string path = Directory.GetCurrentDirectory();
                    var imagePath = @"\LOGO.jpg";
                    //wb.Worksheet(1).AddPicture(path + imagePath)
                    //    .MoveTo(wb.Worksheet(1).Cell("B2"))
                    //    .Scale(.5); // optional: resize picture

                    //wb.Worksheet(1).Cell("C4").Value = "INFORMACION DEL PROYECTO";
                    //wb.Worksheet(1).Cell("C4").Style.Font.SetBold().Font.SetFontColor(XLColor.DarkCoral);
                    //string rangeboder = "B7:C13";
                    //wb.Worksheet(1).Range(rangeboder).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    //wb.Worksheet(1).Range(rangeboder).Style.Border.InsideBorder = XLBorderStyleValues.Dotted;
                    //wb.Worksheet(1).Range(rangeboder).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    //wb.Worksheet(1).Range(rangeboder).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    //wb.Worksheet(1).Range(rangeboder).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    //wb.Worksheet(1).Range(rangeboder).Style.Border.TopBorder = XLBorderStyleValues.Thin;

                    //Style Cell
                    //for (int x = 8; x <= 13; x++)
                    //{
                    //    wb.Worksheet(1).Cell(string.Format("B{0}", x)).Style.Font.SetBold();
                    //    wb.Worksheet(1).Cell(string.Format("C{0}", x)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    //}



                    //wb.Worksheet(1).Cell(string.Format("B{0}", 8)).Value = "Numero del proyecto:";
                    //wb.Worksheet(1).Cell(string.Format("B{0}", 9)).Value = "Nombre del proyecto:";
                    //wb.Worksheet(1).Cell(string.Format("B{0}", 10)).Value = "Tecnico a Cargo:";
                    //wb.Worksheet(1).Cell(string.Format("B{0}", 11)).Value = "Fecha:";
                    //wb.Worksheet(1).Cell(string.Format("B{0}", 12)).Value = "Acabado de Perfileria:";
                    //wb.Worksheet(1).Cell(string.Format("B{0}", 13)).Value = "Acabado de Melamina:";

                    //wb.Worksheet(1).Cell(string.Format("C{0}", 8)).Value = param[0];
                    //wb.Worksheet(1).Cell(string.Format("C{0}", 9)).Value = param[1];
                    //wb.Worksheet(1).Cell(string.Format("C{0}", 10)).Value = param[2];
                    //wb.Worksheet(1).Cell(string.Format("C{0}", 11)).Value = param[3];
                    //wb.Worksheet(1).Cell(string.Format("C{0}", 12)).Value = param[4];
                    //wb.Worksheet(1).Cell(string.Format("C{0}", 13)).Value = param[5];
                    //wb.Worksheet(1).Columns().AdjustToContents();

                    DataGridView table = new DataGridView();
                    for (int Datagrid = 1; Datagrid <= 6; Datagrid++)
                    {
                        bool wrapTextDefault = true;
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
                                Range = string.Format("A{0}:H{0}", valueinitial);
                                rangetwo = "A{0}:H{0}";
                                sheets = "PUERTAS";
                                table = dataGridViewPCalculate;
                                break;
                            case 4:
                                sheets = "TUBO METALICOS";
                                table = dataGridViewTMCalculate;
                                break;
                            case 5:
                                Range = string.Format("A{0}:E{0}", valueinitial);
                                rangetwo = "A{0}:E{0}";
                                sheets = "MAMPARAS";
                                table = dataGridViewMCalculate;
                                wrapTextDefault = false;
                                break;
                            case 6:
                                Range = string.Format("A{0}:E{0}", valueinitial);
                                rangetwo = "A{0}:E{0}";
                                sheets = "PUERTAS CANTIDAD";
                                table = dataGridViewP2Calculate;
                                wrapTextDefault = false;
                                break;
                        }

                        if (table.Rows.Count > 0)
                        {
                            //sheetscount += 1;
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
                        
                            if (Datagrid == 1)
                            {
                                dt = dt.AsEnumerable()
                                        .GroupBy(r => new { Cod = r["Codigo"], med = r["medida"], cal = r["Se_Calcula_Por"] })
                                        .Select(g =>
                                        {
                                            var row = dt.NewRow();

                                            row["id_Subcomponente"] = g.Min(r => r.Field<string>("id_Subcomponente"));
                                            row["Codigo"] = g.Key.Cod;
                                            row["descripcion"] = g.Min(r => r.Field<string>("descripcion"));
                                            row["acabado"] = g.Min(r => r.Field<string>("acabado"));
                                            row["cantidad"] = g.Sum(r => r.Field<float>("cantidad"));
                                            row["medida"] = g.Key.med;
                                            row["Medidida Calculada"] = g.Min(r => r.Field<string>("Medidida Calculada"));
                                            row["Se_Calcula_Por"] = g.Key.cal;
                                            return row;

                                        })
                                        .CopyToDataTable();
                            }


                            var ws = wb.Worksheets.Add(dt, sheets);
                            ws.Row(1).InsertRowsAbove(7);


                            wb.Worksheet(sheets).AddPicture(path + imagePath)
                           .MoveTo(150, 25)
                           .Scale(.3); // optional: resize picture


                            //Diseño Header
                            wb.Worksheet(sheets).ShowGridLines = new BooleanValue(false);

                            var range = wb.Worksheet(sheets).Range(Rangeheader);
                            range.Merge().Style.Font.SetBold().Font.FontSize = 16;
                            range.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            range.Value = sheets;

                            wb.Worksheet(sheets).Range(Rangeheader).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            wb.Worksheet(sheets).Range(Rangeheader).Style.Border.InsideBorder = XLBorderStyleValues.Dotted;
                            wb.Worksheet(sheets).Range(Rangeheader).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            wb.Worksheet(sheets).Range(Rangeheader).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                            //Diseño SubHeader
                            wb.Worksheet(sheets).Cell(string.Format("A{0}", 5)).Value = "Numero del proyecto:";
                            wb.Worksheet(sheets).Cell(string.Format("A{0}", 5)).Style.Font.SetBold();
                            wb.Worksheet(sheets).Cell(string.Format("A{0}", 6)).Value = "Nombre del proyecto:";
                            wb.Worksheet(sheets).Cell(string.Format("A{0}", 6)).Style.Font.SetBold();
                            wb.Worksheet(sheets).Cell(string.Format("C{0}", 5)).Value = "Tecnico a Cargo:";
                            wb.Worksheet(sheets).Cell(string.Format("C{0}", 5)).Style.Font.SetBold();
                            wb.Worksheet(sheets).Cell(string.Format("C{0}", 6)).Value = "Fecha:";
                            wb.Worksheet(sheets).Cell(string.Format("C{0}", 6)).Style.Font.SetBold();
                            wb.Worksheet(sheets).Cell(string.Format("E{0}", 5)).Value = "Acabado de Perfileria:";
                            wb.Worksheet(sheets).Cell(string.Format("E{0}", 5)).Style.Font.SetBold();
                            wb.Worksheet(sheets).Cell(string.Format("E{0}", 6)).Value = "Acabado de Melamina:";
                            wb.Worksheet(sheets).Cell(string.Format("E{0}", 6)).Style.Font.SetBold();



                            wb.Worksheet(sheets).Cell(string.Format("B{0}", 5)).Value = param[0];
                            wb.Worksheet(sheets).Cell(string.Format("B{0}", 6)).Value = param[1];
                            wb.Worksheet(sheets).Cell(string.Format("D{0}", 5)).Value = param[2];
                            wb.Worksheet(sheets).Cell(string.Format("D{0}", 6)).Value = param[3];
                            wb.Worksheet(sheets).Cell(string.Format("F{0}", 5)).Value = param[4];
                            wb.Worksheet(sheets).Cell(string.Format("F{0}", 6)).Value = param[5];
                            wb.Worksheet(sheets).Range("F5:G5").Merge();
                            wb.Worksheet(sheets).Range("F6:G6").Merge();

                            wb.Worksheet(sheets).Range(RangeSubheader).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            wb.Worksheet(sheets).Range(RangeSubheader).Style.Border.InsideBorder = XLBorderStyleValues.Dotted;
                            wb.Worksheet(sheets).Range(RangeSubheader).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            wb.Worksheet(sheets).Range(RangeSubheader).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            wb.Worksheet(sheets).Range(RangeSubheader).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            wb.Worksheet(sheets).Range(RangeSubheader).Style.Border.TopBorder = XLBorderStyleValues.Thin;


                            //Set the color of Header Row.
                            //A resembles First Column while C resembles Third column.

                            wb.Worksheet(sheets).Cells(Range).Style.Fill.BackgroundColor = XLColor.DarkCoral;
                            for (int i = 1; i <= dt.Rows.Count; i++)
                            {
                                //A resembles First Column while C resembles Third column.
                                //Header row is at Position 1 and hence First row starts from Index 2.
                                string cellRange = string.Format(rangetwo, i + valueinitial);
                                string cellIniPuertas = string.Format("A{0}", i + valueinitial);
                                string valueP = wb.Worksheet(sheets).Cell(cellIniPuertas).Value.ToString();
                                if (valueP.Contains("Puerta"))
                                {
                                    wb.Worksheet(sheets).Cells(cellRange).Style.Fill.BackgroundColor = XLColor.LightGreen;
                                }
                                else
                                {
                                    if (i % 2 != 0)
                                    {
                                        wb.Worksheet(sheets).Cells(cellRange).Style.Fill.BackgroundColor = XLColor.White;
                                    }
                                    else
                                    {
                                        wb.Worksheet(sheets).Cells(cellRange).Style.Fill.BackgroundColor = XLColor.LightGray;
                                    }
                                }

                                if (wrapTextDefault)
                                {
                                    wb.Worksheet(sheets).Cell(string.Format("C{0}", i + valueinitial)).Style.Alignment.WrapText = true;
                                }
                                else
                                {
                                    wb.Worksheet(sheets).Cell(string.Format("B{0}", i + valueinitial)).Style.Alignment.WrapText = true;
                                }


                            }
                            //Adjust widths of Columns.
                            wb.Worksheet(sheets).Columns().AdjustToContents();
                            wb.Worksheet(sheets).Column(wrapTextDefault ? 3 : 2).Width = 57;
                        }


                    }

                    //Save the Excel file.
                    string FileNameStr = param[0] + " " + param[1];
                    filefinish = folderPath + "\\" + FileNameStr + ".xlsx";
                    try
                    {
                        wb.SaveAs(filefinish);

                    }
                    catch (Exception ex)
                    {
                        swend = false;
                        MessageBox.Show("Un Archivo se encontraba abierto por favor cerrarlo e intentalo nuevamente", "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                }
            }

            if (swend)
            {
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

        private void BtnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void EliCtrlButtons_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void dataGridViewPHerrajeCalculate_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            foreach (DataGridViewRow r in this.dataGridViewPHerrajeCalculate.Rows)
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

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void NUpDownMedidaBase_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar < 48 || e.KeyChar > 57)
            {
                e.Handled = true;
            }
        }

        private void NUpDownDesperdicio_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar < 48 || e.KeyChar > 57)
            {
                e.Handled = true;
            }
        }
    }
}
