using System;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace arquitectSoft.View.Wpf
{
    /// <summary>
    /// Versión WPF (tema cristal) de FrmBuscar: buscador del catálogo (acabados,
    /// subcomponentes, componentes, etc.) contra MySQL. Reutiliza la misma capa de
    /// datos (Generals.Conexion + Generals.Constantes) y expone las MISMAS
    /// propiedades de retorno (ReturnItem0..5) y la propiedad Consulta de entrada.
    /// </summary>
    public partial class BuscarDialog : Window
    {
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int val, int size);
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        private const int DWMWA_BORDER_COLOR = 34;
        private const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;
        private const int DWMWA_SYSTEMBACKDROP_TYPE = 38;
        private const int DWMWCP_ROUND = 2;
        private const int DWMSBT_TRANSIENTWINDOW = 3;
        private const uint DWMWA_COLOR_NONE = 0xFFFFFFFE;

        public string Consulta { get; set; }
        public string ReturnItem0 { get; private set; }
        public string ReturnItem1 { get; private set; }
        public string ReturnItem2 { get; private set; }
        public string ReturnItem3 { get; private set; }
        public string ReturnItem4 { get; private set; }
        public string ReturnItem5 { get; private set; }
        public DataTable ArrayMultiSelect { get; private set; }

        private readonly DispatcherTimer _filtroTimer;

        public BuscarDialog()
        {
            InitializeComponent();
            SourceInitialized += OnSourceInitialized;
            LiquidGlass.PrepararOculto(FrameRim, WinScale);
            Loaded += (s, e) => LiquidGlass.Apertura(FrameRim, WinScale);
            Loaded += BuscarDialog_Loaded;

            // Filtra en vivo mientras se escribe (debounce para no golpear la BD en cada tecla).
            _filtroTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
            _filtroTimer.Tick += (s, e) => { _filtroTimer.Stop(); Buscar(); };
        }

        private void OnSourceInitialized(object sender, EventArgs e)
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            try
            {
                int round = DWMWCP_ROUND;
                DwmSetWindowAttribute(hwnd, DWMWA_WINDOW_CORNER_PREFERENCE, ref round, sizeof(int));
                int dark = 1;
                DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref dark, sizeof(int));
                int backdrop = DWMSBT_TRANSIENTWINDOW;
                DwmSetWindowAttribute(hwnd, DWMWA_SYSTEMBACKDROP_TYPE, ref backdrop, sizeof(int));
                int sinBorde = unchecked((int)DWMWA_COLOR_NONE);
                DwmSetWindowAttribute(hwnd, DWMWA_BORDER_COLOR, ref sinBorde, sizeof(int));
            }
            catch { /* Windows 10 o anterior */ }

            HwndSource src = HwndSource.FromHwnd(hwnd);
            if (src != null && src.CompositionTarget != null)
                src.CompositionTarget.BackgroundColor = Colors.Transparent;
        }

        private void BuscarDialog_Loaded(object sender, RoutedEventArgs e)
        {
            switch (Consulta)
            {
                case "Acaba": LblTitulo.Text = "Buscar acabado"; break;
                case "Acaba-Multi":
                    LblTitulo.Text = "Buscar acabados";
                    BtnMulti.Visibility = Visibility.Visible;
                    Grid.SelectionMode = DataGridSelectionMode.Extended;
                    break;
                case "SubComp":
                    LblTitulo.Text = "Buscar subcomponente";
                    ChkEspecial.Visibility = Visibility.Visible;
                    break;
                case "Comp-Puerta":
                    LblTitulo.Text = "Buscar puerta";
                    ChkEspecial.Visibility = Visibility.Visible;
                    break;
                case "Mecan": LblTitulo.Text = "Buscar mecanizado"; break;
                case "Corte": LblTitulo.Text = "Buscar corte"; break;
                case "Umed": LblTitulo.Text = "Buscar unidad de medida"; break;
                default:
                    LblTitulo.Text = "Buscar componente";
                    ChkEspecial.Visibility = Visibility.Visible;
                    break;
            }

            Buscar();
            TxtBuscar.Focus();
        }

        // ===== Búsqueda (port de FrmBuscar.Buscar) =====
        private void Buscar()
        {
            var con = new Generals.Conexion();
            string fail = "";
            try
            {
                if (!con.Open(out fail))
                {
                    Informar("No se pudo conectar a la base de datos.\n" + fail);
                    return;
                }

                string fil = (ChkEspecial.IsChecked == true) ? "1" : "0";
                string[] terminos = TxtBuscar.Text.Split(' ');
                string condicion;
                string sql;

                switch (Consulta)
                {
                    case "Umed":
                        condicion = ConstruirLike("Descripcion", terminos, " WHERE ");
                        sql = Generals.Constantes.QUERY_UNIDADMEDIDA + condicion;
                        break;
                    case "Corte":
                        condicion = ConstruirLike("Descripcion", terminos, " WHERE ");
                        sql = Generals.Constantes.QUERY_CORTE + condicion;
                        break;
                    case "Acaba-Multi":
                    case "Acaba":
                        condicion = ConstruirLike("CONCAT(Codigo_Homologacion,' - ',Descripcion)", terminos, " WHERE ");
                        sql = Generals.Constantes.QUERY_ACABADO + condicion;
                        break;
                    case "Mecan":
                        condicion = ConstruirLike("CONCAT(Codigo_Homologacion,' - ',Descripcion)", terminos, " WHERE ");
                        sql = Generals.Constantes.QUERY_MECANIZADO + condicion;
                        break;
                    case "SubComp":
                        condicion = ConstruirLike("CONCAT(subcomponentes.Codigo_Homologacion,' - ',subcomponentes.Descripcion)",
                                                  terminos, " WHERE Especial = " + fil + " AND ");
                        sql = Generals.Constantes.QUERY_SUBCOMPONENTES + condicion;
                        break;
                    // Componentes, pero solo los que SON una puerta: los que empiezan por esa
                    // palabra. Deja fuera los perfiles y módulos de mampara del tipo
                    // "PERFIL DE IT UNION A PUERTA", que no se agregan al análisis de puertas.
                    case "Comp-Puerta":
                        condicion = ConstruirLike(
                            "CONCAT(CONCAT(Codigo , IFNULL(concat('-',acabados.Codigo_Homologacion),'')),' - ',componentes.Descripcion)",
                            terminos, " WHERE Especial = " + fil +
                                      " AND componentes.Descripcion LIKE 'PUERTA%' AND ");
                        sql = Generals.Constantes.QUERY_COMPONENTES + condicion;
                        break;
                    default:
                        condicion = ConstruirLike(
                            "CONCAT(CONCAT(Codigo , IFNULL(concat('-',acabados.Codigo_Homologacion),'')),' - ',componentes.Descripcion)",
                            terminos, " WHERE Especial = " + fil + " AND ");
                        sql = Generals.Constantes.QUERY_COMPONENTES + condicion;
                        break;
                }

                Grid.AutoGenerateColumns = true;
                Grid.ItemsSource = con.ExecuteDataSet(sql, out fail).Tables[0].DefaultView;
                con.Close();
            }
            catch (Exception ex)
            {
                con.Close();
                Informar("Error en la búsqueda:\n" + ex.Message);
            }
        }

        // Construye " campo LIKE '%t1%' AND campo LIKE '%t2%' ..." con el prefijo dado.
        private static string ConstruirLike(string campo, string[] terminos, string prefijo)
        {
            string cond = prefijo;
            for (int i = 0; i < terminos.Length; i++)
            {
                if (i > 0) cond += " AND ";
                cond += " " + campo + " LIKE '%" + terminos[i] + "%'";
            }
            return cond;
        }

        // ===== Selección =====
        private void Grid_DoubleClick(object sender, MouseButtonEventArgs e) => Confirmar();
        private void Seleccionar_Click(object sender, RoutedEventArgs e) => Confirmar();

        private void Confirmar()
        {
            var drv = Grid.SelectedItem as DataRowView;
            if (drv == null) return;
            DataRow row = drv.Row;

            // Mapeo por índice de columna (idéntico a los Cells[0..5] del FrmBuscar).
            ReturnItem0 = Celda(row, 0);
            ReturnItem1 = Celda(row, 1);
            ReturnItem2 = Celda(row, 2);
            ReturnItem3 = Celda(row, 3);
            ReturnItem4 = Celda(row, 4);
            ReturnItem5 = Celda(row, 5);

            DialogResult = true;
        }

        private void MultiSelect_Click(object sender, RoutedEventArgs e)
        {
            var dt = new DataTable();
            dt.Columns.Add("Item1");
            dt.Columns.Add("Item2");
            dt.Columns.Add("Item3");
            foreach (var item in Grid.SelectedItems)
            {
                var drv = item as DataRowView;
                if (drv == null) continue;
                dt.Rows.Add(Celda(drv.Row, 0), Celda(drv.Row, 1), Celda(drv.Row, 2));
            }
            ArrayMultiSelect = dt;
            DialogResult = true;
        }

        private static string Celda(DataRow row, int i)
        {
            if (i >= row.Table.Columns.Count) return null;
            object v = row[i];
            return v == null || v == DBNull.Value ? "" : v.ToString();
        }

        private void TxtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { _filtroTimer.Stop(); Buscar(); }
        }

        private void TxtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded) return;
            _filtroTimer.Stop();
            _filtroTimer.Start();
        }

        private void Buscar_Click(object sender, RoutedEventArgs e) => Buscar();
        private void Filtro_Changed(object sender, RoutedEventArgs e) { if (IsLoaded) Buscar(); }
        private void Cerrar_Click(object sender, RoutedEventArgs e) => DialogResult = false;
        private void Cabecera_Drag(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed) DragMove();
        }

        private void Informar(string msg) => GlassDialog.Informar(this, "Buscar", msg);
    }
}
