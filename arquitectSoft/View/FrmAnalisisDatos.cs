
using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office2010.CustomUI;
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
    public partial class FrmAnalisisDatos : Form
    {

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wMsg, int wParam, int lParam);

        DataTable dtPuertas = new DataTable();
        DataTable dtVidrio = new DataTable();
        DataTable dtTubos = new DataTable();
        DataTable dtPerfil = new DataTable();    
        DataTable dtPerfilCalculate = new DataTable();
        DataTable dtPerfilHRCalculate = new DataTable();
        
        DataTable dtPerfilOfVidrioPanel = new DataTable();
        DataTable dtPerfilOfTubos = new DataTable();
        private int selectedRowIndex = -1;
        private int swPMVertical = 0;

        // --- Recálculo en tiempo real (Medida Base / % Desperdicio) ---
        private System.Collections.Generic.List<string> _file124;
        private System.Collections.Generic.List<string> _file35;
        private int _wantedFiles;
        private bool _datosCargados = false;
        private System.Windows.Forms.Timer _recalcTimer;

        public FrmAnalisisDatos()
        {
            InitializeComponent();

            // Recalcular con un pequeño retardo tras el último cambio, para no
            // recalcular en cada clic del spinner.
            _recalcTimer = new System.Windows.Forms.Timer();
            _recalcTimer.Interval = 250;
            _recalcTimer.Tick += RecalcTimer_Tick;
            NUpDownMedidaBase.ValueChanged += ValoresAnalisis_Changed;
            NUpDownDesperdicio.ValueChanged += ValoresAnalisis_Changed;
        }



        private void BtnCargar_Click(object sender, EventArgs e)
        {
            Cancelar();
            Generals.Global.SwSegmentadoUbi = "1";
            swPMVertical = 0;
            Dto.AnalisisDatosDto dto = new Dto.AnalisisDatosDto();
            DialogResult result = MessageBox.Show("Le Gustaria Segmentar el analisis de datos en los Perfiles metalicos por Ubicación?", "Mensaje Alerta", MessageBoxButtons.YesNo);
            if (result == DialogResult.No)
            {
                Generals.Global.SwSegmentadoUbi = "0";
            }
            string directoryName = "";
            this.openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
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
                    directoryName = Archivo.DirectoryName;                  

                    string nameFile = Archivo.Name.ToString();


                    int idDocumento = int.Parse(nameFile.ToString().Split('-')[0].Trim());

                    if (idDocumento == 0) { swPMVertical = 1; }                        

                    if (idDocumento == 0 || idDocumento == 1 || idDocumento == 2 || idDocumento == 4)
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


                // Guardar los archivos cargados para poder recalcular en tiempo
                // real al cambiar Medida Base / % Desperdicio (sin volver a elegir
                // archivos ni repetir la pregunta de segmentación).
                _file124 = File_124.OrderByDescending(x => x).ToList();
                _file35 = File_35;
                _wantedFiles = wantedFiles;
                _datosCargados = true;
                lblDirectoryName.Text = directoryName;

                EjecutarAnalisis();
            }
        }

        /// <summary>
        /// Ejecuta el cálculo sobre los archivos ya cargados usando los valores
        /// ACTUALES de Medida Base y % Desperdicio, y refresca los grids.
        /// Se llama al cargar y en cada recálculo en tiempo real.
        /// </summary>
        private void EjecutarAnalisis()
        {
            if (!_datosCargados) return;

            var cronometro = System.Diagnostics.Stopwatch.StartNew();
            int tabActual = tabPrincipal.SelectedIndex;   // conservar la pestaña que se está viendo

            // Indicador de "trabajando" (el cálculo bloquea el hilo de la UI).
            this.Cursor = Cursors.WaitCursor;
            lblestadosAnalitica.ForeColor = System.Drawing.Color.Goldenrod;
            lblestadosAnalitica.Text = "Calculando...";
            lblestadosAnalitica.Refresh();
            try
            {
                Cancelar();
                Dto.AnalisisDatosDto dto = new Dto.AnalisisDatosDto();

                if (_file124 != null && _file124.Count > 0)
                {
                    SetDataAll(dto, _wantedFiles, _file124);
                }
                if (_file35 != null && _file35.Count > 0)
                {
                    SetDataAll(dto, _wantedFiles, _file35);
                }

                if (FnValidateData())
                {
                    BtnChange.Visible = true;
                }

                if (tabActual >= 0 && tabActual < tabPrincipal.TabCount)
                {
                    tabPrincipal.SelectedIndex = tabActual;
                }

                cronometro.Stop();
                lblestadosAnalitica.ForeColor = System.Drawing.Color.LimeGreen;
                lblestadosAnalitica.Text = "✓ Listo  (" + (cronometro.ElapsedMilliseconds / 1000.0).ToString("0.0") + " s)";
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        // Al cambiar Medida Base o % Desperdicio, reinicia el temporizador de
        // recálculo (debounce): recalcula 0.5s después del último cambio.
        private void ValoresAnalisis_Changed(object sender, EventArgs e)
        {
            if (!_datosCargados) return;
            _recalcTimer.Stop();
            _recalcTimer.Start();
        }

        private void RecalcTimer_Tick(object sender, EventArgs e)
        {
            _recalcTimer.Stop();
            EjecutarAnalisis();
        }

        private bool FnValidateData()
        {
            bool resul = false;

            if (dataGridViewPMCalculate.RowCount> 0 ||
                dataGridViewPMHerrajeCalculate.RowCount > 0 ||
                dataGridViewVPCalculate.RowCount > 0 ||
                dataGridViewPCalculate.RowCount > 0 ||
                dataGridViewPHerrajeCalculate.RowCount > 0 ||
                dataGridViewP2Calculate.RowCount > 0 ||
                dataGridViewTMCalculate.RowCount > 0 ||
                dataGridViewMCalculate.RowCount > 0 ||
                dataGridViewPMVCalculate.RowCount > 0 ||
                dataGridViewPMVHerrajeCalculate.RowCount > 0)
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

                if (idDocumento == 4)
                {
                    if (wantedFiles != 4) { swmergePM = false; dtTubos = dtResul; }

                    DataTable dtresulVP = new DataTable();
                    dtPerfilOfTubos = dto.CalculateTab(1, dtResul, dtPuertas, true, medidabase, Desperdicio, swmergePM, 0);
                    dtresulVP = dtPerfilOfTubos.Copy();
                    dtresulVP.Merge(dtPerfilCalculate);

                    dataGridViewPMCalculate.DataSource = dtresulVP;
                    dataGridViewPMCalculate.Columns[0].Visible = false;
                    dataGridViewPMCalculate.Columns[6].Visible = false;

                    if (swmergePM)
                    {
                        DataTable dtresulPMHerraje = new DataTable();
                        Generals.Global.SwSegmentadoUbi = "0";
                        dtresulPMHerraje = dto.CalculateTab(8, dtResul, dtPuertas, true, medidabase, Desperdicio, swmergePM, 0);

                        dataGridViewPMHerrajeCalculate.DataSource = dtresulPMHerraje;
                        dataGridViewPMHerrajeCalculate.Columns[0].Visible = false;
                    }

                }

                if (idDocumento == 2)
                {
                    if ((wantedFiles != 2 && wantedFiles != 6) || swPMVertical == 1) { swmergePM = false; dtVidrio = dtResul; }

                    DataTable dtresulVP = new DataTable();
                    dtPerfilOfVidrioPanel = dto.CalculateTab(1, dtResul, dtPuertas, true, medidabase, Desperdicio, swmergePM, 0);
                    dtresulVP = dtPerfilOfVidrioPanel.Copy();
                    dtresulVP.Merge(dtPerfilCalculate);

                    dataGridViewPMCalculate.DataSource = dtresulVP;
                    dataGridViewPMCalculate.Columns[0].Visible = false;
                    dataGridViewPMCalculate.Columns[6].Visible = false;

                    if (swmergePM)
                    {
                        DataTable dtresulPMHerraje = new DataTable();
                        Generals.Global.SwSegmentadoUbi = "0";
                        dtresulPMHerraje = dto.CalculateTab(8, dtResul, dtPuertas, true, medidabase, Desperdicio, wantedFiles == 6 ? false : true, 0);
                        if (wantedFiles == 6)
                            dtresulPMHerraje.Merge(dto.CalculateTab(8, dtTubos, dtPuertas, true, medidabase, Desperdicio, true, 0));


                        dataGridViewPMHerrajeCalculate.DataSource = dtresulPMHerraje;
                        dataGridViewPMHerrajeCalculate.Columns[0].Visible = false;
                        dataGridViewPMHerrajeCalculate.Columns[9].Visible = false;
                    }

                }

                if (idDocumento != 4)
                {
                    if (idDocumento == 0) { Generals.Global.SwSegmentadoUbi = "0"; };
                    dtcalculate = dto.CalculateTab(idDocumento, dtResul, dtPuertas, perfilandvidrios, medidabase, Desperdicio, swmergePM, 0);
                    if (idDocumento == 0 && dtPerfilCalculate.Rows.Count>0) { Generals.Global.SwSegmentadoUbi = "1"; };
                }

                
                if (idDocumento == 1)
                {
                    dtPerfil = dtResul;
                    dtPerfilCalculate = dtcalculate;

                    if (wantedFiles !=1 ) { swmergePM = false; }

                    if (swPMVertical != 1) 
                    {
                        DataTable dtresulPMHerraje = new DataTable();
                        Generals.Global.SwSegmentadoUbi = (Generals.Global.SwSegmentadoUbi == "1") ? "-1" : "0";
                        dtresulPMHerraje = dto.CalculateTab(8, dtResul, dtPuertas, perfilandvidrios, medidabase, Desperdicio, swmergePM, 0);

                       
                        if (wantedFiles == 7)
                        {
                            swmergePM = (swPMVertical == 1) ? false : true;
                            dtresulPMHerraje = dto.CalculateTab(8, dtVidrio, dtPuertas, true, medidabase, Desperdicio, false, 0);
                            dtresulPMHerraje = dto.CalculateTab(8, dtTubos, dtPuertas, true, medidabase, Desperdicio, swmergePM, 0);
                        }

                        if (wantedFiles == 3)
                        {
                            swmergePM = (swPMVertical == 1) ? false : true;
                            dtresulPMHerraje = dto.CalculateTab(8, dtVidrio, dtPuertas, true, medidabase, Desperdicio, swmergePM, 0);
                        }


                        if (wantedFiles == 5)
                        {
                            swmergePM = (swPMVertical == 1) ? false : true;
                            dtresulPMHerraje = dto.CalculateTab(8, dtTubos, dtPuertas, true, medidabase, Desperdicio, swmergePM, 0);
                        }

                        dtPerfilHRCalculate = dtresulPMHerraje;

                        dataGridViewPMHerrajeCalculate.DataSource = dtresulPMHerraje;
                        dataGridViewPMHerrajeCalculate.Columns[0].Visible = false;
                        dataGridViewPMHerrajeCalculate.Columns[9].Visible = false;

                        Generals.Global.SwSegmentadoUbi = (Generals.Global.SwSegmentadoUbi == "-1") ? "1" : "0";
                    }

                }
                if (idDocumento == 3)
                {
                    DataTable dtresulPC = new DataTable();
                    dtresulPC = dto.CalculateTab(6, dtResul, dtPuertas, true, medidabase, Desperdicio, swmergePM, 0);
                    dataGridViewP2Calculate.DataSource = dtresulPC;
                    dataGridViewP2Calculate.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                    DataTable dtresulPHerraje = new DataTable();
                    dtresulPHerraje = dto.CalculateTab(7, dtResul, dtPuertas, true, medidabase, Desperdicio, swmergePM, 0);
                    dataGridViewPHerrajeCalculate.DataSource = dtresulPHerraje;
                    dataGridViewPHerrajeCalculate.Columns[0].Visible = false;
                    dataGridViewPHerrajeCalculate.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }

                if (idDocumento == 0)
                {
                    if (wantedFiles == 2 && swPMVertical == 1)
                        swmergePM = false;

                    DataTable dtresulPMHerraje = new DataTable();
                    Generals.Global.SwSegmentadoUbi = (Generals.Global.SwSegmentadoUbi == "1") ? "-1" : "0";

                    dtresulPMHerraje = dto.CalculateTab(8, dtPerfil, dtPuertas, perfilandvidrios, medidabase, Desperdicio, false, 0);
                    dtresulPMHerraje = dto.CalculateTab(8, dtResul, dtPuertas, perfilandvidrios, medidabase, Desperdicio, false, 0);
                    dtresulPMHerraje = dto.CalculateTab(8, dtTubos, dtPuertas, true, medidabase, Desperdicio, false, 0);
                    dtresulPMHerraje = dto.CalculateTab(8, dtVidrio, dtPuertas, true, medidabase, Desperdicio, true, 0);

                    dtPerfil.Merge(dtResul);
                    dtPerfilCalculate.Merge(dtcalculate);                   

                    dataGridViewPMVHerrajeCalculate.DataSource = dtresulPMHerraje;
                    dataGridViewPMVHerrajeCalculate.Columns[0].Visible = false;
                    dataGridViewPMVHerrajeCalculate.Columns[9].Visible = false;

                    dataGridViewPMHerrajeCalculate.DataSource = dtresulPMHerraje;
                    dataGridViewPMHerrajeCalculate.Columns[0].Visible = false;
                    dataGridViewPMHerrajeCalculate.Columns[9].Visible = false;

                    Generals.Global.SwSegmentadoUbi = (Generals.Global.SwSegmentadoUbi == "-1") ? "1" : "0";

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
        }

        private void FrmAnalisisDatos_Load(object sender, EventArgs e)
        {
            InitializeOpenFileDialog();

            NUpDownMedidaBase.Value = 2960;
            Generals.Global.AnalisisType = "1";
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
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            ToolStripMenuItem item1 = new ToolStripMenuItem("Remplazar SubComponente");
            ToolStripMenuItem item2 = new ToolStripMenuItem("Cambiar Acabado Temporal");

            switch (index)
            {
                case 0:
                    //dataGridViewPMV.DataSource = dtPerfil;
                    //dataGridViewPMVHerraje.DataSource = dtPerfil;
                    //dataGridViewPMVCalculate.DataSource = dtcalculate;
                    //dataGridViewPMVCalculate.Columns[0].Visible = false;

                    //item1.Click += Item1_PMV_Click;
                    //item2.Click += Item2_PMV_Click;

                    //contextMenu.Items.Add(item1);
                    //contextMenu.Items.Add(item2);

                    //dataGridViewPMVCalculate.ContextMenuStrip = contextMenu;

                    //dataGridViewPMVCalculate.CellMouseDown += dataGridViewPMVCalculate_CellMouseDown;
                    //dataGridViewPMVCalculate.Columns[9].Visible = false;
                    //if (Generals.Global.SwSegmentadoUbi == "1")
                    //{
                    //    dataGridViewPMVCalculate.Columns[9].Visible = true;
                    //}                    
                    
                    dataFinishPM(dtPerfil, dtPerfilCalculate, contextMenu, item1, item2);
                  
                    break;
                case 1:

                    dataFinishPM(dt,dtcalculate,contextMenu,item1,item2);
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
                    //dataGridViewTMCalculate.DataSource = dtcalculate;
                    //dataGridViewTMCalculate.Columns[0].Visible = false;
                    //dataGridViewTMCalculate.Columns[6].Visible = false;
                    break;
                case 5:
                    dataGridViewM.DataSource = dt;
                    dataGridViewMCalculate.DataSource = dtcalculate;
                    break;
            }


        }

        private void dataFinishPM(DataTable dt, DataTable dtcalculate, ContextMenuStrip contextMenu, ToolStripMenuItem item1, ToolStripMenuItem item2)
        {
            dataGridViewPM.DataSource = dt;
            dataGridViewPMHerraje.DataSource = dt;
            dataGridViewPMCalculate.DataSource = dtcalculate;
            dataGridViewPMCalculate.Columns[0].Visible = false;

            item1.Click += Item1_PM_Click;
            item2.Click += Item2_PM_Click;

            contextMenu.Items.Add(item1);
            contextMenu.Items.Add(item2);

            dataGridViewPMCalculate.ContextMenuStrip = contextMenu;

            dataGridViewPMCalculate.CellMouseDown += dataGridViewPMCalculate_CellMouseDown;
            dataGridViewPMCalculate.Columns[9].Visible = false;
            if (Generals.Global.SwSegmentadoUbi == "1")
            {
                dataGridViewPMCalculate.Columns[9].Visible = true;
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
                else if (Engine.FilaPuerta.EsCabecera(r.Cells[0].Value))
                {
                    r.DefaultCellStyle.BackColor = Color.Orange;
                }
                else if (r.Cells[0].Value.ToString().Contains("~"))
                {
                    r.DefaultCellStyle.BackColor = Color.LightGreen;
                }

            }
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            Cancelar();
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

            bool swexport = (dataGridViewPMCalculate.RowCount > 0 ||
                            dataGridViewVPCalculate.RowCount > 0 ||
                            dataGridViewPCalculate.RowCount > 0 ||
                            dataGridViewTMCalculate.RowCount > 0 ||
                            dataGridViewMCalculate.RowCount > 0 ||
                            dataGridViewP2Calculate.RowCount > 0 ||
                            dataGridViewPMVCalculate.RowCount > 0) ? true : false;

            
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
            profilePath.SelectedPath = lblDirectoryName.Text;
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
                    int PMHValueFinish = 0;
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
                    for (int Datagrid = 1; Datagrid <= 9; Datagrid++)
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
                                sheets = "PERFIL METALICO";
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
                                sheets = "PERFIL METALICO";
                                Descheader = "PUERTAS";
                                RangeSubheader = string.Format("A{0}:G{1}", valuesubheaderDescr, valuesubheaderValue);
                                Range = string.Format("A{0}:H{0}", valueinitial);
                                table = dataGridViewPCalculate;
                                break;
                            case 3:
                                sheets = "PERFIL METALICO HERRAJES";
                                table = dataGridViewPMHerrajeCalculate;
                                PMHValueFinish = dataGridViewPMHerrajeCalculate.RowCount;
                                Range = string.Format("A{0}:H{0}", valueinitial);
                                Descheader = sheets;
                                break;
                            case 4:
                                if (PMHValueFinish > 0)
                                {
                                    valueinitial = valueinitial + PMHValueFinish + 8;
                                    Rangeheader = string.Format("A{0}:G{1}", valueinitial - 6, valueinitial - 4);
                                    valuesubheaderDescr = valueinitial - 3;
                                    valuesubheaderValue = valueinitial - 2;
                                }

                                
                                rangetwo = "A{0}:H{0}";
                                sheets = "PERFIL METALICO HERRAJES";
                                Descheader = "PUERTAS HERRAJES";
                                Range = string.Format("A{0}:H{0}", valueinitial);
                                RangeSubheader = string.Format("A{0}:G{1}", valuesubheaderDescr, valuesubheaderValue);
                                table = dataGridViewPHerrajeCalculate;
                                break;
                            case 5:
                                Range = string.Format("A{0}:H{0}", valueinitial);
                                sheets = "VIDRIOS Y PANELES";
                                table = dataGridViewVPCalculate;
                                Descheader = sheets;
                                break;                           

                            case 6:
                                Range = string.Format("A{0}:E{0}", valueinitial);
                                rangetwo = "A{0}:E{0}";
                                sheets = "PUERTAS CANTIDAD";
                                Descheader = sheets;
                                table = dataGridViewP2Calculate;
                                wrapTextDefault = false;
                                break;
                            case 7:
                                Range = string.Format("A{0}:E{0}", valueinitial);
                                sheets = "TUBO METALICOS";
                                Descheader = sheets;
                                table = dataGridViewTMCalculate;
                                break;
                            case 8:
                                Range = string.Format("A{0}:H{0}", valueinitial);
                                rangetwo = "A{0}:E{0}";
                                sheets = "MAMPARAS";
                                Descheader = sheets;
                                table = dataGridViewMCalculate;
                                wrapTextDefault = false;
                                break;
                            case 9:
                                Range = string.Format("A{0}:H{0}", valueinitial);
                                rangetwo = "A{0}:E{0}";
                                sheets = "ALBARAN";
                                Descheader = sheets;
                                table = dataGridViewPMCalculate;
                                wrapTextDefault = false;
                                break;

                        }

                        if (table.Rows.Count > 0 && Datagrid != 9)
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
                                if (Generals.Global.SwSegmentadoUbi == "0")
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
                                            row["Mecanizado"] = g.Min(r => r.Field<string>("Mecanizado"));
                                            row["Se_Calcula_Por"] = g.Key.cal;
                                            return row;

                                        })
                                        .CopyToDataTable();
                                }
                                else
                                {
                                    dt = dt.AsEnumerable()
                                        .GroupBy(r => new { Cod = r["Codigo"], med = r["medida"], cal = r["Se_Calcula_Por"], ubi = r["Ubicación"] })
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
                                            row["Ubicación"] = g.Min(r => r.Field<string>("Ubicación"));
                                            row["Mecanizado"] = g.Min(r => r.Field<string>("Mecanizado"));
                                            row["Se_Calcula_Por"] = g.Key.cal;
                                            return row;

                                        })
                                        .CopyToDataTable();
                                }

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
                                if (PMHValueFinish == 0)
                                {
                                    DataTable dtnew = new DataTable();
                                    var wsDoorOutPm = wb.Worksheets.Add(dtnew, sheets);
                                    wsDoorOutPm.Name = sheets;
                                    wsDoorOutPm.Row(1).InsertRowsAbove(7);
                                    wb.Worksheet(sheets).AddPicture(path + imagePath)
                                      .MoveTo(150, 25)
                                      .Scale(.3); // optional: resize picture
                                }

                                //dt.Columns.Remove("Ubicación");

                                var ws = wb.Worksheets.Add(dt, sheets + "PHer");
                                string RangeSrcDoor = string.Format("A{0}:H{1}", 1, dt.Rows.Count + 1);
                                var rangeDoor = wb.Worksheet(sheets + "PHer").Range(RangeSrcDoor);

                                var wsPM = wb.Worksheet(1);
                                string RangeDstDoor = string.Format("A{0}:H{1}", valueinitial, valueinitial + dt.Rows.Count);
                                rangeDoor.CopyTo(wb.Worksheet(sheets).Range(RangeDstDoor));

                                valueinitialFoot = valueinitial + dt.Rows.Count + 2;

                                wb.Worksheet(sheets + "PHer").Delete();
                            }
                            else if (Datagrid == 9)
                            {
                                var ws = wb.Worksheets.Add(dt, sheets);
                                ws.Row(1).InsertRowsAbove(7);
                                wb.Worksheet(sheets).AddPicture(path + imagePath)
                                  .MoveTo(150, 25)
                                  .Scale(.3); // optional: resize picture

                                

                            }
                            else
                            {
                                if (Datagrid == 3)
                                {
                                    dt.Columns.Remove("Ubicación");
                                }
                                   

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
                                if (Engine.FilaPuerta.EsCabecera(valueP))
                                {
                                    // Enunciado de la puerta: negrita y fondo propio (igual que
                                    // en ExcelExporter), para separarlo de sus Items al imprimir.
                                    var cabecera = wb.Worksheet(sheets).Cells(cellRange).Style;
                                    cabecera.Fill.BackgroundColor = XLColor.FromArgb(0xFC, 0xE4, 0xD6);
                                    cabecera.Font.Bold = true;
                                    cabecera.Font.FontColor = XLColor.FromArgb(0x7B, 0x33, 0x1C);
                                    cabecera.Border.TopBorder = XLBorderStyleValues.Thin;
                                    cabecera.Border.TopBorderColor = XLColor.FromArgb(0xC2, 0x5A, 0x38);
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
                        else if (Datagrid == 9)
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
                                        table2 = dataGridViewPMHerrajeCalculate;
                                        foreach (DataGridViewRow row in table1.Rows)
                                        {
                                            dt1.Rows.Add(row.Cells[1].Value, row.Cells[2].Value, row.Cells[3].Value, row.Cells[4].Value, row.Cells[5].Value, "Perfil Metalico"); 
                                        }

                                        foreach (DataGridViewRow row in table2.Rows)
                                        {
                                            dt1.Rows.Add(row.Cells[1].Value, row.Cells[2].Value, row.Cells[3].Value, row.Cells[4].Value, row.Cells[5].Value, "Perfil Metalico Herraje");
                                        }

                                        break;
                                    case "1":
                                       
                                        table1 = dataGridViewVPCalculate;
                                        foreach (DataGridViewRow row in table1.Rows)
                                        {
                                            dt1.Rows.Add(row.Cells[1].Value, row.Cells[2].Value, row.Cells[3].Value, row.Cells[6].Value, row.Cells[4].Value, "Vidrios y Paneles");
                                        }

                                        break;
                                    case "2":
                                        table1 = dataGridViewPCalculate;
                                        table2 = dataGridViewPHerrajeCalculate;
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
            for (int Datagrid = 1; Datagrid <= 10; Datagrid++)
            {
               
                switch (Datagrid)
                {
                    case 1:                        
                        table = dataGridViewPMCalculate;                        
                        break;
                    case 2:                        
                        table = dataGridViewPCalculate;
                        break;
                    case 3:
                        table = dataGridViewPMHerrajeCalculate;
                        break;
                    case 4:                       
                        table = dataGridViewPHerrajeCalculate;
                        break;
                    case 5:
                        table = dataGridViewVPCalculate;
                        break;
                    case 6:                        
                        table = dataGridViewP2Calculate;
                        break;
                    case 7:
                        table = dataGridViewTMCalculate;
                        break;
                    case 8:
                        table = dataGridViewMCalculate;
                        break;
                    case 9:
                        table = dataGridViewPMVCalculate;
                        break;
                    case 10:
                        table = dataGridViewPMVHerrajeCalculate;
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

                        if (valuezero == "")
                            continue;
                        
                        if ((Datagrid == 2) && !Engine.FilaPuerta.EsCabecera(valuezero))
                        {
                            // Proteger contra códigos sin guion (antes reventaba con
                            // IndexOutOfRange en Split('-')[1] y dejaba el cambio a medias).
                            string[] codeParts = row.Cells[1].Value.ToString().Split('-');
                            if (codeParts.Length > 1)
                            {
                                string codAcabado = codeParts[1].Trim();
                                string codAcabadoNew = param[0].ToString().Split('-')[0].Trim();

                                if (codAcabado == codAcabadoNew)
                                {
                                    string acabadocodNew = (param[1].ToString().Contains("-")) ? param[1].ToString().Split('-')[0].Trim() : "XX";
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
                            if (((Datagrid == 2 || Datagrid == 4) && Engine.FilaPuerta.EsCabecera(valuezero)) || Datagrid == 6)
                            {
                                string AcabadoFinal = (param[1].ToString().Contains("-")) ? param[1].ToString().Split('-')[1].Trim() : param[1].ToString().Trim();
                                AcabadoDesc = AcabadoDesc.Substring(0, posini + 1) + AcabadoFinal + AcabadoDesc.Substring(posfin, AcabadoDesc.Length - posfin);
                                string acabadocodNew = (param[1].ToString().Contains("-")) ? param[1].ToString().Split('-')[0].Trim() : "XX";
                                if (Datagrid == 6)
                                {
                                    row.Cells[0].Value = row.Cells[0].Value.ToString().Split('-')[0].Trim() + "-" + acabadocodNew;
                                    row.Cells[1].Value = AcabadoDesc;
                                }
                                else
                                {
                                    row.Cells[1].Value = row.Cells[1].Value.ToString().Split('-')[0].Trim() + "-" + acabadocodNew; 
                                    row.Cells[2].Value = AcabadoDesc;
                                }
                                
                            }
                            else if (Datagrid == 8)
                            {
                                string acabadocodNew = (param[1].ToString().Contains("-")) ? param[1].ToString().Split('-')[0].Trim() : "XX";

                                row.Cells[0].Value = row.Cells[0].Value.ToString().Split('-')[0].Trim() + "-" + acabadocodNew;
                                row.Cells[2].Value = (param[1].ToString().Contains("-")) ? param[1].ToString().Split('-')[1].Trim() : param[1].ToString().Trim(); 
                            }
                            else
                            {
                                string acabadocodNew = (param[1].ToString().Contains("-")) ? param[1].ToString().Split('-')[0].Trim() : "XX";                                

                                row.Cells[1].Value = row.Cells[1].Value.ToString().Split('-')[0].Trim()+"-"+ acabadocodNew;
                                row.Cells[3].Value = (param[1].ToString().Contains("-")) ? param[1].ToString().Split('-')[1].Trim() : param[1].ToString().Trim();
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

        private void dataGridViewPMCalculate_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                // Seleccionar la celda
                dataGridViewPMCalculate.ClearSelection();
                dataGridViewPMCalculate.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;
                selectedRowIndex = e.RowIndex;

                // Mostrar el menú contextual
                dataGridViewPMCalculate.ContextMenuStrip.Show(Cursor.Position);
            }
        }

        private void dataGridViewPMVCalculate_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                // Seleccionar la celda
                dataGridViewPMVCalculate.ClearSelection();
                dataGridViewPMVCalculate.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;
                selectedRowIndex = e.RowIndex;

                // Mostrar el menú contextual
                dataGridViewPMVCalculate.ContextMenuStrip.Show(Cursor.Position);
            }
        }

        private void Item1_PM_Click(object sender, EventArgs e)
        {
            if (selectedRowIndex >= 0)
            {
                int value = selectedRowIndex;
                FrmBuscar bsc = new FrmBuscar();
                bsc.Consulta = "SubComp";
                bsc.ShowDialog();
                if (bsc.ReturnItem1 == null)
                {
                    return;
                }

                string codigo = bsc.ReturnItem1.Trim();
                string descripcion = bsc.ReturnItem2.Split('(')[0].Trim();
                string acabadosplit = bsc.ReturnItem2.Split('(')[1].Trim().Replace(")","");
                string acabado = bsc.ReturnItem5;
                dataGridViewPMCalculate.Rows[selectedRowIndex].Cells[1].Value = codigo;
                dataGridViewPMCalculate.Rows[selectedRowIndex].Cells[2].Value = descripcion;
                dataGridViewPMCalculate.Rows[selectedRowIndex].Cells[3].Value = acabado;
            }
        }

        private void Item1_PMV_Click(object sender, EventArgs e)
        {
            if (selectedRowIndex >= 0)
            {
                int value = selectedRowIndex;
                FrmBuscar bsc = new FrmBuscar();
                bsc.Consulta = "SubComp";
                bsc.ShowDialog();
                if (bsc.ReturnItem1 == null)
                {
                    return;
                }

                string codigo = bsc.ReturnItem1.Trim();
                string descripcion = bsc.ReturnItem2.Split('(')[0].Trim();
                string acabadosplit = bsc.ReturnItem2.Split('(')[1].Trim().Replace(")", "");
                string acabado = bsc.ReturnItem5;
                dataGridViewPMVCalculate.Rows[selectedRowIndex].Cells[1].Value = codigo;
                dataGridViewPMVCalculate.Rows[selectedRowIndex].Cells[2].Value = descripcion;
                dataGridViewPMVCalculate.Rows[selectedRowIndex].Cells[3].Value = acabado;
            }
        }

        private void Item2_PM_Click(object sender, EventArgs e)
        {
            if (selectedRowIndex >= 0)
            {
                FrmChange bsc = new FrmChange();
                bsc.ShowDialog();
                if (bsc.Acabado1 == null)
                {
                    return;
                }

                string acabadocodNew = (bsc.Acabado2.ToString().ToUpper().Contains("-")) ? bsc.Acabado2.ToString().Split('-')[0] : "XX"; //bsc.Acabado2.ToString().Split('-')[0];
                string acabadoDescNew = (bsc.Acabado2.ToString().ToUpper().Contains("-")) ? bsc.Acabado2.ToString().Split('-')[1] : bsc.Acabado2.ToString().ToUpper();  //.Split('-')[1]

                string AcabadoDesc = dataGridViewPMCalculate.Rows[selectedRowIndex].Cells[3].Value.ToString();
                string codigo = dataGridViewPMCalculate.Rows[selectedRowIndex].Cells[1].Value.ToString().Split('-')[0] + "-"+ acabadocodNew;

               // string AcabadoOld = AcabadoDesc.Substring(AcabadoDesc.IndexOf("(") + 1, AcabadoDesc.IndexOf(")") - (AcabadoDesc.IndexOf(")") + 1));

                dataGridViewPMCalculate.Rows[selectedRowIndex].Cells[1].Value = codigo;
                dataGridViewPMCalculate.Rows[selectedRowIndex].Cells[3].Value = acabadoDescNew;
                
                
            }
        }

        private void Item2_PMV_Click(object sender, EventArgs e)
        {
            if (selectedRowIndex >= 0)
            {
                FrmChange bsc = new FrmChange();
                bsc.ShowDialog();
                if (bsc.Acabado1 == null)
                {
                    return;
                }

                string acabadocodNew = (bsc.Acabado2.ToString().ToUpper().Contains("-")) ? bsc.Acabado2.ToString().Split('-')[0] : "XX"; //bsc.Acabado2.ToString().Split('-')[0];
                string acabadoDescNew = (bsc.Acabado2.ToString().ToUpper().Contains("-")) ? bsc.Acabado2.ToString().Split('-')[1] : bsc.Acabado2.ToString().ToUpper();  //.Split('-')[1]

                string AcabadoDesc = dataGridViewPMVCalculate.Rows[selectedRowIndex].Cells[3].Value.ToString();
                string codigo = dataGridViewPMVCalculate.Rows[selectedRowIndex].Cells[1].Value.ToString().Split('-')[0] + "-" + acabadocodNew;

                // string AcabadoOld = AcabadoDesc.Substring(AcabadoDesc.IndexOf("(") + 1, AcabadoDesc.IndexOf(")") - (AcabadoDesc.IndexOf(")") + 1));

                dataGridViewPMVCalculate.Rows[selectedRowIndex].Cells[1].Value = codigo;
                dataGridViewPMVCalculate.Rows[selectedRowIndex].Cells[3].Value = acabadoDescNew;


            }
        }

        private void Cancelar()
        {
            foreach (Control c in this.tabPrincipal.Controls)
            {
                if (c is TabPage)
                {
                    foreach (Control d in c.Controls)
                    {
                        if (d is TabControl)
                        {
                            foreach (Control h in d.Controls)
                            {
                                if (h is TabPage)
                                {
                                    foreach (Control i in h.Controls)
                                    {
                                        if (i is DataGridView)
                                        {
                                            DataGridView dgv = (DataGridView)i;
                                            dgv.DataSource = "";
                                        }
                                    }
                                }
                            }
                        }

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
            dtVidrio.Rows.Clear();
            dtTubos.Rows.Clear();
            dtPerfilCalculate.Rows.Clear();
            dtPerfilCalculate.Rows.Clear();
            dtPerfilHRCalculate.Rows.Clear();
            dtPerfilOfVidrioPanel.Rows.Clear();
            dtPerfilOfTubos.Rows.Clear();
            lblestadosAnalitica.Text = "";
            lblDirectoryName.Text = "";
            BtnChange.Visible = false;
            dtPerfil.Clear();
            dtPerfilCalculate.Clear();
        }

        private void BtnMaximizar_Click(object sender, EventArgs e)
        {
            int incremento = 150;

            if (this.WindowState != FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Maximized;
                tabPrincipal.Height = (tabPrincipal.Height + incremento);
                tabControl1.Height = (tabControl1.Height + incremento);

                dataGridViewPMCalculate.Height = (dataGridViewPMCalculate.Height + incremento);
                dataGridViewPMHerrajeCalculate.Height = (dataGridViewPMHerrajeCalculate.Height + incremento);
                dataGridViewVPCalculate.Height = (dataGridViewVPCalculate.Height + incremento);
                dataGridViewPCalculate.Height = (dataGridViewPCalculate.Height + incremento);
                dataGridViewPHerrajeCalculate.Height = (dataGridViewPHerrajeCalculate.Height + incremento);
                dataGridViewP2Calculate.Height =(dataGridViewP2Calculate.Height + incremento);
                dataGridViewTMCalculate.Height= (dataGridViewTMCalculate.Height + incremento);
                dataGridViewMCalculate.Height =(dataGridViewMCalculate.Height + incremento);



            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                tabPrincipal.Height = 592;
                tabControl1.Height = 557;
                dataGridViewPMCalculate.Height = 276;
                dataGridViewPMHerrajeCalculate.Height = 276;
                dataGridViewVPCalculate.Height = 276;
                dataGridViewPCalculate.Height = 276;
                dataGridViewPHerrajeCalculate.Height = 276;
                dataGridViewP2Calculate.Height = 276;
                dataGridViewTMCalculate.Height = 276;
                dataGridViewMCalculate.Height = 276;
                dataGridViewPHerrajeCalculate.Height = 276;
                dataGridViewMCalculate.Height = 276;

            }
        }
    }
}
