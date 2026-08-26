using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;
using MySqlX.XDevAPI.Relational;
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
using Color = System.Drawing.Color;
using Control = System.Windows.Forms.Control;

namespace arquitectSoft.View
{
    public partial class FrmAnalisisDatos_Puertas : Form
    {

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wMsg, int wParam, int lParam);

        DataTable dtPuertas = new DataTable();
        DataTable dtVidrio = new DataTable();
        DataTable dtTubos = new DataTable();
        DataTable dtPerfil = new DataTable();
        DataTable dtPerfilHR = new DataTable();
        DataTable dtPerfilR = new DataTable();
        DataTable dtPerfilOfVidrioPanel = new DataTable();
        DataTable dtPerfilOfTubos = new DataTable();
        DataTable dtAddRowsP = new DataTable();
        public FrmAnalisisDatos_Puertas()
        {
            InitializeComponent();
            
        }


        private void BtnCargar_Click(object sender, EventArgs e)
        {
            Dto.AnalisisDatosDto dto = new Dto.AnalisisDatosDto();
            

            DialogResult dr = this.openFileDialog1.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                int wantedFiles = 0;
                int wantedFilesHerrajesPuertas = 0;
                
                List<string> File_124 = new List<string>();
                List<string> File_35 = new List<string>();

                foreach (String file in openFileDialog1.FileNames)
                {                    
                    FileInfo Archivo = new FileInfo(file);
                    int idDocumento = int.Parse(Archivo.Name.ToString().Split('-')[0].Trim());
                    if (idDocumento == 1 || idDocumento == 2 || idDocumento == 4)
                    {

                        wantedFiles += idDocumento;

                        File_124.Add(file);


                    }
                    else 
                    {
                        wantedFilesHerrajesPuertas ++;
                        File_35.Add(file);
                    }
                }


                if (File_124.Count > 0)
                {
                    SetDataAll(dto, wantedFiles, File_124.OrderByDescending(x => x).ToList());
                }

                if (File_35.Count > 0)
                {
                    SetDataAll(dto, wantedFiles, File_35);
                }



                if (FnValidateData())
                {
                    BtnChange.Visible = true;
                }
                
                lblestadosAnalitica.Text = "Analitica Aplicada Correctamente!";

            }
        }

        private bool FnValidateData()
        {
            bool resul = false;

            if (dataGridViewPMCalculate.RowCount> 0 ||
                dataGridViewPCalculate.RowCount > 0 )
            {
                resul = true;

            }

            return resul;
        }


        private void SetDataAll(Dto.AnalisisDatosDto dto, int wantedFiles, List<string> Filenames)
        {
            int pageinitial = 0;
            bool perfilandvidrios = false;
            List<int> UseTab = new List<int>();
            int medidabase = Int32.Parse(NUpDownMedidaBase.Value.ToString());
            decimal Desperdicio = (decimal.Parse(NUpDownDesperdicio.Value.ToString()) / 100) + 1;

            foreach (String file in Filenames)
            {
                bool swmergePM = true;
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

                if (idDocumento == 3)
                {
                    dtcalculate = dto.CalculateTab(idDocumento, dtResul, dtPuertas, perfilandvidrios, medidabase, Desperdicio, swmergePM,1);
                }      

                SetDataView(dtResul, dtcalculate, idDocumento);

            }

            switch (UseTab.First())
            {
                case 3:
                    tabPrincipal.SelectTab(tabPuertas);
                    break;
            }
        }

        private void FrmAnalisisDatos_Load(object sender, EventArgs e)
        {
            InitializeOpenFileDialog();

            NUpDownMedidaBase.Value = 2960;
            dataGridViewP.Visible = false;
            Generals.Global.AnalisisType = "2";
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
                case 3:
                    dataGridViewP.DataSource = dt;                  
                    dataGridViewPCalculate.DataSource = dtcalculate;
                    dataGridViewPCalculate.Refresh();
                    dataGridViewPCalculate.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                    foreach (DataGridViewColumn col in dataGridViewPCalculate.Columns)
                    {
                        col.SortMode = DataGridViewColumnSortMode.NotSortable;
                    }

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
            tabPrincipal.SelectTab(tabPuertas);
            dtPuertas.Rows.Clear();
            dtVidrio.Rows.Clear();
            dtTubos.Rows.Clear();
            dtPerfil.Rows.Clear();
            dtPerfilHR.Rows.Clear();
            dtPerfilOfVidrioPanel.Rows.Clear();
            dtPerfilOfTubos.Rows.Clear();
            dtAddRowsP.Rows.Clear();
            dtAddRowsP.Columns.Clear();
            lblestadosAnalitica.Text = "";
            txtCodigo.Text = "";
            txtacabado.Text = "";
            txtDescripcion.Text = "";
            txtAltura.Text = "";
            txtAnchura.Text = "";
            NUpDownRowsP.Value = 1;
            BtnChange.Visible = false;
            btnAnalizar.Visible = false;

        }

        private void btnExportar_Click(object sender, EventArgs e)
        {

            FrmLoading bsc = new FrmLoading();
            bsc.ShowDialog();


            if (bsc.Numero == null)
            {
                return;
            }

            if (bsc.Albaran == null)
            {
                MessageBox.Show("No selecciono datos para cargar albaran!!", "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);                
            }

            string[] param = { bsc.Numero, bsc.Nombre, bsc.Tecnico, bsc.Fecha, bsc.Acabado1, bsc.Acabado2, bsc.Albaran };

            bool swexport = false;
            if (dataGridViewPMCalculate.RowCount > 0)
            {
                swexport = true;
            }
            else if (dataGridViewPCalculate.RowCount > 0)
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
                    int valuecountDoor = dataGridViewPCalculate.RowCount;
                    int valueinitialFoot = 0;
                    int PMValueFinish = 0;
                    string Range = string.Format("A{0}:G{0}", valueinitial);
                    string Descheader = "";
                    string Rangeheader = "A2:G4";
                    string RangeSubheader = "A5:J6";
                    string Rangetopfooter;
                    string RangeSubfooter = "H5:J6"; ;
                    string rangetwo = "A{0}:G{0}";
                    string sheets = "";


                    string path = Directory.GetCurrentDirectory();
                    var imagePath = @"\LOGO.jpg";


                    DataGridView table = new DataGridView();
                    for (int Datagrid = 1; Datagrid <= 4; Datagrid++)
                    {
                        valueinitial = 8;
                        valueinitialFoot = 0;
                        Rangeheader = "A2:G4";
                        RangeSubheader = "A5:J6";
                        int valuesubheaderDescr = 5;
                        int valuesubheaderValue = 6;
                        bool wrapTextDefault = true;
                        switch (Datagrid)
                        {
                            case 1:
                                sheets = "PUERTAS";
                                table = dataGridViewPMCalculate;
                                PMValueFinish = dataGridViewPMCalculate.RowCount ;
                                Range = string.Format("A{0}:H{0}", valueinitial);
                                Descheader = sheets;
                                break;
                            case 2:
                                if (PMValueFinish > 0)
                                {
                                    valueinitial = valueinitial + PMValueFinish + 8;
                                    Rangeheader = string.Format("A{0}:G{1}", valueinitial - 6, valueinitial - 4);
                                    valuesubheaderDescr = valueinitial - 3;
                                    valuesubheaderValue = valueinitial - 2;
                                }

                                rangetwo = "A{0}:H{0}";
                                sheets = "PUERTAS";
                                Descheader = "PUERTAS";
                                RangeSubheader = string.Format("A{0}:G{1}", valuesubheaderDescr, valuesubheaderValue);
                                Range = string.Format("A{0}:H{0}", valueinitial);
                                table = dataGridViewPCalculate;
                                break;
                            case 3: 
                                rangetwo = "A{0}:H{0}";
                                sheets = "PUERTAS HERRAJES";
                                Descheader = "PUERTAS HERRAJES";
                                Range = string.Format("A{0}:H{0}", valueinitial);
                                RangeSubheader = string.Format("A{0}:G{1}", valuesubheaderDescr, valuesubheaderValue);
                                table = dataGridViewPHerrajeCalculate;
                                break;
                            case 4:
                                Range = string.Format("A{0}:H{0}", valueinitial);
                                rangetwo = "A{0}:E{0}";
                                sheets = "ALBARAN";
                                Descheader = sheets;
                                table = dataGridViewPMCalculate;
                                wrapTextDefault = false;
                                break;

                        }

                        if (table.Rows.Count > 0 && Datagrid != 5)
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
                                            row["Corte"] = g.Min(r => r.Field<string>("Corte"));
                                            row["Se_Calcula_Por"] = g.Key.cal;
                                            return row;

                                        })
                                        .CopyToDataTable();
                            }

                           
                            if (Datagrid == 2)
                            {
                                if (PMValueFinish == 0)
                                {
                                    DataTable dtnew = new DataTable();
                                    var wsDoorOutPm = wb.Worksheets.Add(dtnew, sheets);
                                    wsDoorOutPm.Name = sheets;
                                    wsDoorOutPm.Row(1).InsertRowsAbove(7);
                                    wb.Worksheet(sheets).AddPicture(path + imagePath)
                                      .MoveTo(150, 25)
                                      .Scale(.3); // optional: resize picture
                                }
                                
                                var ws = wb.Worksheets.Add(dt, sheets + "Puerta");
                                string RangeSrcDoor = string.Format("A{0}:H{1}", 1, dt.Rows.Count + 1);
                                var rangeDoor = wb.Worksheet(sheets + "Puerta").Range(RangeSrcDoor);

                                var wsPM = wb.Worksheet(1);
                                string RangeDstDoor = string.Format("A{0}:H{1}", valueinitial, valueinitial + dt.Rows.Count);
                                rangeDoor.CopyTo(wb.Worksheet(sheets).Range(RangeDstDoor));

                                valueinitialFoot = valueinitial + dt.Rows.Count + 2;

                                wb.Worksheet(sheets + "Puerta").Delete();

                            }
                            else if (Datagrid == 4)
                            {
                                var ws = wb.Worksheets.Add(dt, sheets);
                                ws.Row(1).InsertRowsAbove(7);
                                wb.Worksheet(sheets).AddPicture(path + imagePath)
                                  .MoveTo(150, 25)
                                  .Scale(.3); // optional: resize picture

                                

                            }
                            else
                            {
                                var ws = wb.Worksheets.Add(dt, sheets);
                                ws.Row(1).InsertRowsAbove(7);
                                wb.Worksheet(sheets).AddPicture(path + imagePath)
                                  .MoveTo(150, 25)
                                  .Scale(.3); // optional: resize picture

                                if (valuecountDoor == 0 || Datagrid > 4 )
                                {
                                    valueinitialFoot = dt.Rows.Count + 10;
                                }
                                else
                                {
                                    valueinitialFoot = 0;
                                }


                            }

                            if (valueinitialFoot > 0)
                            {
                                //Diseño del footer
                                wb.Worksheet(sheets).Cell(string.Format("H{0}", 5)).Value = "VERIFICACIÓN DE DISEÑO";
                                wb.Worksheet(sheets).Cell(string.Format("I{0}", 5)).Value = "OK";
                                wb.Worksheet(sheets).Cell(string.Format("J{0}", 5)).Value = "FECHA";
                                Rangetopfooter = string.Format("H{0}:J{1}", 5, 6);
                                wb.Worksheet(sheets).Cells(Rangetopfooter).Style.Fill.BackgroundColor = XLColor.LightGray;

                                wb.Worksheet(sheets).Cell(string.Format("H{0}", valueinitialFoot)).Value = "REVISION DE FABRICACIÓN";
                                wb.Worksheet(sheets).Cell(string.Format("I{0}", valueinitialFoot)).Value = "OK";
                                wb.Worksheet(sheets).Cell(string.Format("J{0}", valueinitialFoot)).Value = "FECHA";
                                RangeSubfooter = string.Format("H{0}:J{1}", valueinitialFoot, valueinitialFoot + 1);
                                wb.Worksheet(sheets).Cells(RangeSubfooter).Style.Fill.BackgroundColor = XLColor.LightGray;
                                //Cuadricula footer
                                wb.Worksheet(sheets).Range(RangeSubfooter).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                wb.Worksheet(sheets).Range(RangeSubfooter).Style.Border.InsideBorder = XLBorderStyleValues.Dotted;
                                wb.Worksheet(sheets).Range(RangeSubfooter).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                                wb.Worksheet(sheets).Range(RangeSubfooter).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                wb.Worksheet(sheets).Range(RangeSubfooter).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                wb.Worksheet(sheets).Range(RangeSubfooter).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            }


                            //Diseño Header
                            wb.Worksheet(sheets).ShowGridLines = new BooleanValue(false);

                            var range = wb.Worksheet(sheets).Range(Rangeheader);
                            range.Merge().Style.Font.SetBold().Font.FontSize = 16;
                            range.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            range.Value = Descheader;

                            wb.Worksheet(sheets).Range(Rangeheader).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            wb.Worksheet(sheets).Range(Rangeheader).Style.Border.InsideBorder = XLBorderStyleValues.Dotted;
                            wb.Worksheet(sheets).Range(Rangeheader).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            wb.Worksheet(sheets).Range(Rangeheader).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                            //Diseño SubHeader
                            wb.Worksheet(sheets).Cell(string.Format("A{0}", valuesubheaderDescr)).Value = "Numero del proyecto:";
                            wb.Worksheet(sheets).Cell(string.Format("A{0}", valuesubheaderDescr)).Style.Font.SetBold();
                            wb.Worksheet(sheets).Cell(string.Format("A{0}", valuesubheaderValue)).Value = "Nombre del proyecto:";
                            wb.Worksheet(sheets).Cell(string.Format("A{0}", valuesubheaderValue)).Style.Font.SetBold();
                            wb.Worksheet(sheets).Cell(string.Format("C{0}", valuesubheaderDescr)).Value = "Tecnico a Cargo:";
                            wb.Worksheet(sheets).Cell(string.Format("C{0}", valuesubheaderDescr)).Style.Font.SetBold();
                            wb.Worksheet(sheets).Cell(string.Format("C{0}", valuesubheaderValue)).Value = "Fecha:";
                            wb.Worksheet(sheets).Cell(string.Format("C{0}", valuesubheaderValue)).Style.Font.SetBold();
                            wb.Worksheet(sheets).Cell(string.Format("E{0}", valuesubheaderDescr)).Value = "Acabado de Perfileria:";
                            wb.Worksheet(sheets).Cell(string.Format("E{0}", valuesubheaderDescr)).Style.Font.SetBold();
                            wb.Worksheet(sheets).Cell(string.Format("E{0}", valuesubheaderValue)).Value = "Acabado de Melamina:";
                            wb.Worksheet(sheets).Cell(string.Format("E{0}", valuesubheaderValue)).Style.Font.SetBold();



                            wb.Worksheet(sheets).Cell(string.Format("B{0}", valuesubheaderDescr)).Value = param[0];
                            wb.Worksheet(sheets).Cell(string.Format("B{0}", valuesubheaderValue)).Value = param[1];
                            wb.Worksheet(sheets).Cell(string.Format("D{0}", valuesubheaderDescr)).Value = param[2];
                            wb.Worksheet(sheets).Cell(string.Format("D{0}", valuesubheaderValue)).Value = param[3];
                            wb.Worksheet(sheets).Cell(string.Format("F{0}", valuesubheaderDescr)).Value = param[4];
                            wb.Worksheet(sheets).Cell(string.Format("F{0}", valuesubheaderValue)).Value = param[5];
                            wb.Worksheet(sheets).Range(string.Format("F{0}:G{0}", valuesubheaderDescr)).Merge();
                            wb.Worksheet(sheets).Range(string.Format("F{0}:G{0}", valuesubheaderValue)).Merge();


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
                        else if (Datagrid == 4)
                        {
                            
                            Dto.AnalisisDatosDto dto = new Dto.AnalisisDatosDto();
                            DataTable dt = new DataTable();   
                            dto.setCreateColumns(12).ForEach(delegate (string s)
                            {
                                dt.Columns.Add(s, s == "cantidad" ? typeof(float) : typeof(string));
                            });

                            dataGridViewCeroAlbaran.DataSource = dt;

                            DataTable dt1 = new DataTable();                            
                            DataGridView table1 = new DataGridView();
                            DataGridView table2 = new DataGridView();
                            
                            foreach (DataGridViewColumn column in dataGridViewCeroAlbaran.Columns)
                            {
                                dt1.Columns.Add(column.HeaderText, column.ValueType);
                            }



                            foreach (string item in param[6].Split('|'))
                            {
                                string categoryname = "";
                                switch (item)
                                {
                                    case "0":
                                      
                                        table1 = dataGridViewPMCalculate;                                        
                                        foreach (DataGridViewRow row in table1.Rows)
                                        {
                                            dt1.Rows.Add(row.Cells[1].Value, row.Cells[2].Value, row.Cells[3].Value, row.Cells[4].Value, row.Cells[5].Value, "Perfil Metalico"); 
                                        }

                                        foreach (DataGridViewRow row in table2.Rows)
                                        {
                                            dt1.Rows.Add(row.Cells[1].Value, row.Cells[2].Value, row.Cells[3].Value, row.Cells[4].Value, row.Cells[5].Value, "Perfil Metalico Herraje");
                                        }

                                        break;
                                    case "2":
                                        table1 = dataGridViewPCalculate;
                                        string acabadopuertas = "";
                                        string medidaPuertas = "";
                                        foreach (DataGridViewRow row in table1.Rows)
                                        {
                                            if (row.Cells[3].Value.ToString() != "")
                                            {
                                                medidaPuertas = row.Cells[4].Value.ToString();
                                                if (row.Cells[4].Value.ToString() == "0")
                                                    medidaPuertas = row.Cells[5].Value.ToString();

                                                dt1.Rows.Add(row.Cells[1].Value, row.Cells[2].Value, acabadopuertas, row.Cells[3].Value, medidaPuertas, "Puertas");
                                            }
                                            else if (row.Cells[2].Value.ToString() != "")
                                            {
                                                acabadopuertas = row.Cells[2].Value.ToString();
                                                int pos1 = acabadopuertas.IndexOf("(");
                                                int pos2 = acabadopuertas.IndexOf(")");
                                                int cantacab = pos2 - pos1;
                                                acabadopuertas = acabadopuertas.Substring(pos1+1, cantacab- 1);
                                            }
                                                
                                        }
                                        
                                        table2 = dataGridViewPHerrajeCalculate;
                                        foreach (DataGridViewRow row in table2.Rows)
                                        {
                                            dt1.Rows.Add(row.Cells[1].Value, row.Cells[2].Value, row.Cells[3].Value, row.Cells[4].Value, row.Cells[5].Value, "Puertas Herrajes");
                                        }
                                        break;
                                    

                                }
                            }


                            dt1 = dt1.AsEnumerable()
                                        .GroupBy(r => new { Cod = r["Codigo"], med = r["medida"], cal = r["acabado"] })
                                        .Select(g =>
                                        {
                                            var row = dt1.NewRow();

                                            row["CODIGO"] = g.Key.Cod;
                                            row["categoria"] = g.Min(r => r.Field<string>("categoria"));
                                            row["descripcion"] = g.Min(r => r.Field<string>("descripcion"));
                                            row["cantidad"] = g.Sum(r => r.Field<float>("cantidad"));
                                            row["medida"] = g.Key.med;
                                            row["acabado"] = g.Min(r => r.Field<string>("acabado"));
                                            return row;

                                        })
                                        .CopyToDataTable();

                            var ws = wb.Worksheets.Add(dt1, sheets);
                            ws.Row(1).InsertRowsAbove(18);
                            wb.Worksheet(sheets).AddPicture(path + imagePath)
                              .MoveTo(100, 25)
                              .Scale(.5); // optional: resize picture                           


                            // BEGIN HEADER
                            wb.Worksheet(sheets).Cell(string.Format("B{0}", 7)).Value = "SISTEMAS ARQUIMART S.L.";
                            wb.Worksheet(sheets).Cell(string.Format("B{0}", 8)).Value = "c/ Aitzgorri 6-Pol.Ind.Ansoleta";
                            wb.Worksheet(sheets).Cell(string.Format("B{0}", 9)).Value = "01006 Vitoria-Gasteiz";
                            wb.Worksheet(sheets).Cell(string.Format("B{0}", 10)).Value = "Tfno 945 29 14 89";
                            wb.Worksheet(sheets).Cell(string.Format("B{0}", 11)).Value = "CIF B01472216";

                            wb.Worksheet(sheets).Cell(string.Format("D{0}", 2)).Value = "ENTREGA EN:";
                            wb.Worksheet(sheets).Cell(string.Format("D{0}", 5)).Value = "CLIENTE:";
                            wb.Worksheet(sheets).Cell(string.Format("D{0}", 6)).Value = "REFERENCIA OBRA:";
                            wb.Worksheet(sheets).Cell(string.Format("D{0}", 9)).Value = "HORARIO ENTREGA:";
                            wb.Worksheet(sheets).Cell(string.Format("D{0}", 12)).Value = "PERSONA CONTACTO:";
                            wb.Worksheet(sheets).Cell(string.Format("D{0}", 13)).Value = "TELEFONO DE CONTACTO:";


                            wb.Worksheet(sheets).Cell(string.Format("A{0}", 15)).Value = "ALBARAN:";
                            wb.Worksheet(sheets).Cell(string.Format("C{0}", 15)).Value = "FECHA: " + param[3];                           
                            
                            wb.Worksheet(sheets).Cell(string.Format("A{0}", 16)).Value = param[0] + " - " + param[1];
                            wb.Worksheet(sheets).Cell(string.Format("C{0}", 16)).Value = "PEDIDO:";

                            wb.Worksheet(sheets).Cell(string.Format("A{0}", 17)).Value = "N CAJAS:";
                            wb.Worksheet(sheets).Cell(string.Format("A{0}", 18)).Value = "N PALETS:";

                            wb.Worksheet(sheets).Range(string.Format("A{0}:B{0}", 15)).Merge();
                            wb.Worksheet(sheets).Range(string.Format("A{0}:B{0}", 16)).Merge();

                            wb.Worksheet(sheets).Range(string.Format("A{0}:E{0}", 17)).Merge().Style.Font.SetBold().Font.FontSize = 16;
                            wb.Worksheet(sheets).Range(string.Format("A{0}:E{0}", 17)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            wb.Worksheet(sheets).Range(string.Format("A{0}:E{0}", 18)).Merge().Style.Font.SetBold().Font.FontSize = 16;
                            wb.Worksheet(sheets).Range(string.Format("A{0}:E{0}", 18)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                            wb.Worksheet(sheets).Columns().AdjustToContents();
                           

                            string Rangeheader1 = string.Format("D{0}:E{1}", 2, 7);
                            string Rangeheader2 = string.Format("D{0}:E{1}", 9, 10);
                            string Rangeheader3 = string.Format("D{0}:E{1}", 12, 13);
                            string Rangeheader4 = string.Format("A{0}:E{1}", 17, 18);
                            //wb.Worksheet(sheets).Range(Rangeheader1).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            //wb.Worksheet(sheets).Range(Rangeheader1).Style.Border.InsideBorder = XLBorderStyleValues.Dotted;
                            wb.Worksheet(sheets).Range(Rangeheader1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            wb.Worksheet(sheets).Range(Rangeheader2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            wb.Worksheet(sheets).Range(Rangeheader3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            wb.Worksheet(sheets).Range(Rangeheader4).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            wb.Worksheet(sheets).Range(Rangeheader4).Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                            wb.Worksheet(sheets).Cell(string.Format("A{0}", 15)).Style.Font.SetBold();
                            wb.Worksheet(sheets).Cell(string.Format("C{0}", 15)).Style.Font.SetBold();
                            wb.Worksheet(sheets).Cell(string.Format("C{0}", 16)).Style.Font.SetBold();
                            wb.Worksheet(sheets).Cell(string.Format("D{0}", 2)).Style.Font.SetBold();
                            wb.Worksheet(sheets).Cell(string.Format("D{0}", 5)).Style.Font.SetBold();
                            wb.Worksheet(sheets).Cell(string.Format("D{0}", 6)).Style.Font.SetBold();
                            wb.Worksheet(sheets).Cell(string.Format("D{0}", 9)).Style.Font.SetBold();
                            wb.Worksheet(sheets).Cell(string.Format("D{0}", 12)).Style.Font.SetBold();
                            wb.Worksheet(sheets).Cell(string.Format("D{0}", 13)).Style.Font.SetBold();
                            // END HEADER


                            //BEGIN FOOTER
                            int indexfooter = 21 + dt1.Rows.Count;
                            wb.Worksheet(sheets).Cell(string.Format("A{0}", indexfooter)).Value = "Transportista:";
                            wb.Worksheet(sheets).Cell(string.Format("A{0}", indexfooter + 1)).Value = "Pagador Portes:";
                            wb.Worksheet(sheets).Cell(string.Format("D{0}", indexfooter)).Value = "F:";
                            wb.Worksheet(sheets).Cell(string.Format("D{0}", indexfooter + 1)).Value = "C:";

                            wb.Worksheet(sheets).Range(string.Format("B{0}:C{0}", indexfooter)).Merge();
                            wb.Worksheet(sheets).Range(string.Format("B{0}:C{0}", indexfooter + 1)).Merge();

                            string Rangeheaderfooter = string.Format("A{0}:E{1}", indexfooter, indexfooter + 1);
                            wb.Worksheet(sheets).Range(Rangeheaderfooter).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            wb.Worksheet(sheets).Range(Rangeheaderfooter).Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                            wb.Worksheet(sheets).Cell(string.Format("A{0}", indexfooter + 3)).Value = "SISTEMAS ARQUIMART S.L. c/ Aitzgorri 6 - Pol.Ind. Ansoleta 01006 Vitoria-Gasteiz";
                            wb.Worksheet(sheets).Cell(string.Format("A{0}", indexfooter + 4)).Value = "Tfno:945 29 14 89  e-mail: arquimart@arquimart.es CIF B 01472216";
                            
                            wb.Worksheet(sheets).Range(string.Format("A{0}:E{0}", indexfooter + 3)).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            wb.Worksheet(sheets).Range(string.Format("A{0}:E{0}", indexfooter + 4)).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            //END FOOTER

                            wb.Worksheet(sheets).ShowGridLines = new BooleanValue(false);
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

        private void FnChangeInfo(string[] param)
        {
            DataGridView table = new DataGridView();
            for (int Datagrid = 1; Datagrid <= 2; Datagrid++)
            {
               
                switch (Datagrid)
                {
                    case 1:                        
                        table = dataGridViewPHerrajeCalculate;                        
                        break;
                    case 2:                        
                        table = dataGridViewPCalculate;
                        break;

                }

                if(table.Rows.Count > 0)
                {
                    foreach (DataGridViewRow row in table.Rows)
                    {
                        int posini = 0;
                        int posfin = 0;
                        string AcabadoDesc = "";
                        string valuezero = row.Cells[0].Value.ToString();
                        string Acabado = row.Cells[3].Value.ToString();   
                        
                        if ((Datagrid == 2) && !valuezero.Contains("Puerta"))
                        {
                            // Componentes de la puerta (perfiles): cambiar el código de
                            // acabado solo en los que tienen el acabado de origen
                            // (ej. "ITC0102-01"). Igual que en el proceso de mamparas.
                            string[] codeParts = row.Cells[1].Value.ToString().Split('-');
                            if (codeParts.Length > 1)
                            {
                                string codAcabado = codeParts[1].Trim();
                                string codAcabadoOrigen = param[0].ToString().Split('-')[0].Trim();
                                if (codAcabado == codAcabadoOrigen)
                                {
                                    string acabadocodNew = param[1].ToString().Contains("-") ? param[1].ToString().Split('-')[0].Trim() : "XX";
                                    row.Cells[1].Value = codeParts[0].Trim() + "-" + acabadocodNew;
                                }
                            }
                            continue;
                        }

                        if (Datagrid == 2 || Datagrid == 6)
                        {
                            AcabadoDesc = (Datagrid == 6) ? row.Cells[1].Value.ToString() : row.Cells[2].Value.ToString();
                            posini = AcabadoDesc.IndexOf("(");
                            posfin = AcabadoDesc.IndexOf(")");
                            Acabado = AcabadoDesc.Substring(posini + 1, posfin - (posini +1));                                                  
                        }

                        if (Datagrid == 8)
                        {
                            Acabado = row.Cells[2].Value.ToString();
                        }

                        if (param[0].Contains(Acabado))
                        {
                            if (((Datagrid == 2 || Datagrid == 4) && valuezero.Contains("Puerta")) || Datagrid == 6)
                            {
                                string AcabadoDescini = AcabadoDesc.Substring(0, posini + 1); // + param[1].ToString().Split('-')[1].Trim() + AcabadoDesc.Substring(posfin, AcabadoDesc.Length - posfin);
                                string AcabadoDescfin = AcabadoDesc.Substring(posfin, AcabadoDesc.Length - posfin);
                                //string AcabadoDescOrigen = param[0].ToString().Split('-')[1].Trim();
                                string AcabadoDescDestino = param[1].ToString().Contains("-") ? param[1].ToString().Split('-')[1].Trim() : param[1].ToString();
                                AcabadoDesc = AcabadoDescini + AcabadoDescDestino + AcabadoDescfin;
                                if (Datagrid == 6)
                                {
                                    row.Cells[1].Value = AcabadoDesc;
                                }
                                else
                                {
                                    // Actualizar también el CÓDIGO del acabado en la fila
                                    // de la puerta (antes solo cambiaba la descripción).
                                    string acabadocodNew = param[1].ToString().Contains("-") ? param[1].ToString().Split('-')[0].Trim() : "XX";
                                    row.Cells[1].Value = row.Cells[1].Value.ToString().Split('-')[0].Trim() + "-" + acabadocodNew;
                                    row.Cells[2].Value = AcabadoDesc;
                                }
                                
                            }
                            else if (Datagrid == 8)
                            {
                                row.Cells[2].Value = param[1].ToString().Split('-')[1].Trim();
                            }
                            else
                            {
                                row.Cells[3].Value = param[1].ToString().Contains("-") ? param[1].ToString().Split('-')[1].Trim() : param[1].ToString();
                            }                            
                        }
                    }
                }
                
            }

        }

        private void BtnChange_Click(object sender, EventArgs e)
        {
            FrmChange bsc = new FrmChange();
            bsc.ShowDialog();
            if (bsc.Acabado1 == null)
            {
                return;
            }
            string[] param = { bsc.Acabado1, bsc.Acabado2 };
            FnChangeInfo(param);


        }

        private void txtCodigo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

                Dto.ComponenteDto dto = new Dto.ComponenteDto();
                FrmBuscar bsc = new FrmBuscar();
                bsc.ShowDialog();

                if (bsc.ReturnItem1 == null)
                {
                    return;
                }
                txtCodigo.Text = bsc.ReturnItem1.ToString().Split('-')[0];
                txtacabado.Text = bsc.ReturnItem1.ToString().Split('-')[1];
                txtDescripcion.Text = bsc.ReturnItem2;
            }
        }

        private void btnAddRowDoor_Click(object sender, EventArgs e)
        {
            if (txtCodigo.Text == "" ||
                txtacabado.Text == "" ||
                txtDescripcion.Text == "" ||
                txtAltura.Text == "" ||
                txtAnchura.Text == "")
            {
                MessageBox.Show("Debe digitar todos los datos", "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                if (dtAddRowsP.Rows.Count == 0)
                {
                    dtAddRowsP.Columns.Add("Nomenclatura");
                    dtAddRowsP.Columns.Add("Codigo");
                    dtAddRowsP.Columns.Add("Apertura de Puerta");
                    dtAddRowsP.Columns.Add("Acabado Perfileria Puertas");                
                    dtAddRowsP.Columns.Add("Item");
                    dtAddRowsP.Columns.Add("Altura");
                    dtAddRowsP.Columns.Add("Anchura");
                    dtAddRowsP.Columns.Add("Conectado/pared Tubo L1");
                    dtAddRowsP.Columns.Add("Conectado/pared Tubo L2");
                    dtAddRowsP.Columns.Add("Cantidad");
                    dtAddRowsP.Columns.Add("Ubicación");
                    dtAddRowsP.Columns.Add("Area");
             
                }           

                int n = int.Parse(NUpDownRowsP.Value.ToString());
                int i = 1;
            
                while (i <= n)
                {
                    int rowscount = dtAddRowsP.Rows.Count +1;
                    string rows = rowscount.ToString();
                    string Nomen = "P" + rows;
                    dtAddRowsP.Rows.Add(Nomen, txtCodigo.Text, "", txtacabado.Text, txtDescripcion.Text, txtAltura.Text, txtAnchura.Text, "No", "No","1");
                    i++;
                }
               
                dataGridViewPNew.DataSource = dtAddRowsP;
                dataGridViewPNew.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridViewPNew.Columns[2].Visible = false;
                dataGridViewPNew.Columns[7].Visible = false;
                dataGridViewPNew.Columns[8].Visible = false;
                dataGridViewPNew.Columns[9].Visible = false;
                dataGridViewPNew.Columns[10].Visible = false;
                dataGridViewPNew.Columns[11].Visible = false;

                if (dtAddRowsP.Rows.Count > 0)
                {
                    btnAnalizar.Visible = true;
                }
            }
                
            


            

        }

        private void btnAnalizar_Click(object sender, EventArgs e)
        {
            var __cronometro = System.Diagnostics.Stopwatch.StartNew();
            Dto.AnalisisDatosDto dto = new Dto.AnalisisDatosDto();

            DataTable dtcalculate = new DataTable();
            bool swmergePM = true;
            bool perfilandvidrios = false;
            int medidabase = Int32.Parse(NUpDownMedidaBase.Value.ToString());
            decimal Desperdicio = (decimal.Parse(NUpDownDesperdicio.Value.ToString()) / 100) + 1;

            dtcalculate = dto.CalculateTab(3, dtAddRowsP, dtPuertas, perfilandvidrios, medidabase, Desperdicio, swmergePM, 1);

            DataTable dtresulPHerraje = new DataTable();
            dtresulPHerraje = dto.CalculateTab(7, dtAddRowsP, dtPuertas, true, medidabase, Desperdicio, swmergePM, 1);
            dataGridViewPHerrajeCalculate.DataSource = dtresulPHerraje;
            dataGridViewPHerrajeCalculate.Columns[0].Visible = false;
            dataGridViewPHerrajeCalculate.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            SetDataView(dtAddRowsP, dtcalculate, 3);

            if (FnValidateData())
            {
                BtnChange.Visible = true;
            }

            __cronometro.Stop();
            double __seg = __cronometro.ElapsedMilliseconds / 1000.0;
            lblestadosAnalitica.Text = "Analitica Aplicada Correctamente!  (" + __seg.ToString("0.0") + " s)";
            MessageBox.Show("Análisis completado en " + __seg.ToString("0.0") + " segundos.", "Tiempo de análisis", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnMaximizar_Click(object sender, EventArgs e)
        {
            if (this.WindowState != FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
            }
        }
    }
}
