using System;
using System.Data;
using System.Linq;
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
                    LblHint.Text = "Doble clic en una puerta para seleccionar";
                    ColumnasPuertas();
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

                DataTable tabla = con.ExecuteDataSet(sql, out fail).Tables[0];
                con.Close();

                if (Consulta == "Comp-Puerta")
                {
                    // Agrupada por familia: cada bloque se pinta con la celda de familia
                    // combinada a la izquierda (ver GroupStyle en el XAML). Sin orden en la
                    // vista: manda el de la tabla, que ya viene puesto.
                    var cvs = new System.Windows.Data.CollectionViewSource
                    {
                        Source = AgruparPorFamilia(tabla).DefaultView
                    };
                    cvs.GroupDescriptions.Add(new System.Windows.Data.PropertyGroupDescription(ColFamilia));
                    Grid.ItemsSource = cvs.View;
                }
                else
                {
                    Grid.AutoGenerateColumns = true;
                    Grid.ItemsSource = tabla.DefaultView;
                }
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

        // ===== Tabla por familias (solo "Buscar puerta") =====

        // Columna añadida al final para pintar la familia. Va AL FINAL a proposito: los
        // ReturnItem0..5 se leen por indice de columna (0=Id, 1=Codigo, 2=Descripcion...)
        // y meterla delante los descuadraria.
        private const string ColFamilia = "_Familia";

        // Columna vacía que solo reserva el hueco de la celda de familia: la familia se
        // pinta encima, una sola vez por bloque, desde la plantilla de grupo del XAML.
        private const string ColHueco = "_Hueco";

        private const double AnchoFamilia = 74;

        /// <summary>
        /// Columnas fijas del buscador de puertas: familia, código y descripción. Se apaga
        /// el autogenerado (que sacaba Id_Componente, Especial y AcabadoPrincipal, que aquí
        /// no dicen nada) y el ordenar por cabecera, porque el orden lo fija AgruparPorFamilia.
        /// </summary>
        private void ColumnasPuertas()
        {
            Grid.AutoGenerateColumns = false;
            Grid.CanUserSortColumns = false;
            Grid.Columns.Clear();

            // Columna vacía: solo abre el hueco y pone la cabecera. El texto lo pinta la
            // plantilla de grupo, centrado sobre todas las filas de la familia.
            Grid.Columns.Add(new DataGridTextColumn
            {
                Header = "Familia",
                Binding = new System.Windows.Data.Binding(ColHueco),
                Width = AnchoFamilia
            });
            Grid.Columns.Add(new DataGridTextColumn
            {
                Header = "Código",
                Binding = new System.Windows.Data.Binding("Codigo"),
                Width = 130
            });
            Grid.Columns.Add(new DataGridTextColumn
            {
                Header = "Descripción",
                Binding = new System.Windows.Data.Binding("Descripcion"),
                Width = new DataGridLength(1, DataGridLengthUnitType.Star)
            });
        }

        /// <summary>
        /// Devuelve la misma tabla ordenada por familia (el prefijo de letras del código:
        /// ITS, AVS, VIS…) y, dentro de cada una, por código en orden natural. La familia
        /// se escribe SOLO en la primera fila de cada bloque, para que se lea como una
        /// celda a la izquierda con todas sus puertas al lado, sin repetir el nombre 148
        /// veces. El esquema y el orden de columnas originales se conservan intactos.
        /// </summary>
        private static DataTable AgruparPorFamilia(DataTable dt)
        {
            DataTable orden = dt.Clone();
            orden.Columns.Add(ColFamilia);
            orden.Columns.Add(ColHueco);
            if (dt == null || dt.Rows.Count == 0) return orden;

            var filas = dt.AsEnumerable()
                          .OrderBy(r => Familia(Convert.ToString(r["Codigo"])), StringComparer.OrdinalIgnoreCase)
                          .ThenBy(r => Bloque(Convert.ToString(r["Codigo"])))
                          .ThenBy(r => ClaveNatural(Convert.ToString(r["Codigo"])));

            foreach (DataRow r in filas)
            {
                DataRow nueva = orden.Rows.Add(r.ItemArray);
                nueva[ColFamilia] = Familia(Convert.ToString(r["Codigo"]));
                nueva[ColHueco] = "";
            }
            return orden;
        }

        /// <summary>Prefijo de letras del código: "ITS0102-01" → "ITS". Sin letras, "OTROS".</summary>
        private static string Familia(string codigo)
        {
            string cod = (codigo ?? "").Trim();
            int i = 0;
            while (i < cod.Length && char.IsLetter(cod[i])) i++;
            return i == 0 ? "OTROS" : cod.Substring(0, i).ToUpperInvariant();
        }

        /// <summary>
        /// El código sin la familia de delante ni el acabado de detrás:
        /// "ITS17B01-01" → "17B01". Es la parte que distingue una puerta de otra.
        /// </summary>
        private static string Cuerpo(string codigo)
        {
            string cod = (codigo ?? "").Trim();
            int g = cod.IndexOf('-');
            if (g >= 0) cod = cod.Substring(0, g);
            int i = 0;
            while (i < cod.Length && char.IsLetter(cod[i])) i++;
            return cod.Substring(i);
        }

        /// <summary>
        /// Dentro de una familia van primero los códigos SOLO numéricos y después los que
        /// llevan letras, así ITS1701 sale antes que ITS17B01. Comparándolos de corrido, el
        /// orden natural metía ITS17B01 antes (17 &lt; 1701) y las variantes se colaban entre
        /// medias de la serie principal.
        /// </summary>
        private static int Bloque(string codigo)
        {
            foreach (char c in Cuerpo(codigo))
                if (char.IsLetter(c)) return 1;
            return 0;
        }

        /// <summary>
        /// Orden natural: los tramos de dígitos se comparan como números, así
        /// ITS0009 va antes que ITS0010 y ITS0010 antes que ITS00105.
        /// </summary>
        private static string ClaveNatural(string codigo)
        {
            string s = (codigo ?? "").Trim().ToUpperInvariant();
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < s.Length; )
            {
                if (char.IsDigit(s[i]))
                {
                    int j = i;
                    while (j < s.Length && char.IsDigit(s[j])) j++;
                    sb.Append(s.Substring(i, j - i).TrimStart('0').PadLeft(8, '0'));
                    i = j;
                }
                else { sb.Append(s[i]); i++; }
            }
            return sb.ToString();
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
