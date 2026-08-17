using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using arquitectSoft.Engine;

namespace arquitectSoft.View.Wpf.Panels
{
    /// <summary>
    /// Versión "panel" de la ventana "Análisis de Mamparas" para hospedarse dentro del
    /// escritorio (MdiChild). Mismo comportamiento (cálculo en 2º plano, spinner, drag-drop,
    /// previsualizador, exportar, cambiar acabado) SIN chrome de ventana ni liquid glass:
    /// de eso se encarga la ventana hija que lo contiene. Reutiliza AnalisisEngine.
    /// </summary>
    public partial class AnalisisPanel : UserControl
    {
        private const int MedidaMin = 0;
        private const int MedidaMax = 99999;
        private const int DesperdicioMin = 0;
        private const int DesperdicioMax = 100;

        // ---- Motor de cálculo (sin UI) + control de recálculo ----
        private readonly AnalisisEngine _engine = new AnalisisEngine();
        private DispatcherTimer _recalcTimer;
        private int _generacion = 0;        // descarta resultados de cálculos obsoletos

        public AnalisisPanel()
        {
            InitializeComponent();

            // Debounce: recalcula 0,25 s después del último cambio de Medida/Desperdicio.
            _recalcTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(250) };
            _recalcTimer.Tick += RecalcTimer_Tick;
            TxtMedidaBase.TextChanged += Valor_Changed;
            TxtDesperdicio.TextChanged += Valor_Changed;

            // Ajuste de columnas (centrado + wrap de descripción) en todas las grillas.
            foreach (var dg in TodasLasGrillas())
                dg.AutoGeneratingColumn += Dg_AutoGeneratingColumn;
            DgPreview.AutoGeneratingColumn += Dg_AutoGeneratingColumn;
        }

        // Ventana que hospeda el panel (para que los diálogos cristal tengan owner).
        private Window Owner { get { return Window.GetWindow(this); } }

        // Último resultado, para el previsualizador (acceso a tablas crudas por pestaña).
        private ResultadoAnalisis _ultimo;

        // Acabado de perfilería que tienen HOY las cantidades (el "01" por defecto, o el
        // último aplicado con "Cambiar Acabado"). Sirve para amarrar el campo "Acabado
        // Perfilería" del export en los dos sentidos. Se resiembra desde los datos.
        private string _acabadoPerfil = "";

        // Copia intacta del último análisis fresco (con los placeholders MOD… y la perfilería
        // por defecto). Al cambiar la perfilería se reconstruye desde aquí para re-resolver
        // las dependencias. _resolver trae las reglas MOD… → acabado real de la base.
        private ResultadoAnalisis _base;
        private DependenciaResolver _resolver;

        private DataGrid[] TodasLasGrillas() => new[]
        {
            DgPerfilMetalico, DgPerfilMetalicoHerraje, DgVidrioPaneles, DgPuertas,
            DgPuertasHerrajes, DgPuertasCantidad, DgTubos, DgMamparas
        };

        // ===== Validación: solo dígitos =====
        private void SoloEnteros(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }

        // ===== Spinners =====
        private void MedidaUp_Click(object sender, RoutedEventArgs e) =>
            TxtMedidaBase.Text = Clamp(LeerEntero(TxtMedidaBase.Text) + 1, MedidaMin, MedidaMax).ToString();
        private void MedidaDown_Click(object sender, RoutedEventArgs e) =>
            TxtMedidaBase.Text = Clamp(LeerEntero(TxtMedidaBase.Text) - 1, MedidaMin, MedidaMax).ToString();
        private void DesperdicioUp_Click(object sender, RoutedEventArgs e) =>
            TxtDesperdicio.Text = Clamp(LeerEntero(TxtDesperdicio.Text) + 1, DesperdicioMin, DesperdicioMax).ToString();
        private void DesperdicioDown_Click(object sender, RoutedEventArgs e) =>
            TxtDesperdicio.Text = Clamp(LeerEntero(TxtDesperdicio.Text) - 1, DesperdicioMin, DesperdicioMax).ToString();

        private static int LeerEntero(string texto)
        {
            int v;
            return int.TryParse(texto, out v) ? v : 0;
        }
        private static int Clamp(int v, int min, int max) => v < min ? min : (v > max ? max : v);

        // ===== Cargar TXT → motor → mostrar =====
        private async void Cargar_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true,
                Filter = "Archivos TXT (*.txt)|*.txt",
                Title = "Seleccionar despieces (.txt)"
            };
            if (dlg.ShowDialog(Owner) != true) return;
            await CargarArchivos(dlg.FileNames);
        }

        /// <summary>
        /// Lógica común de carga (la usan el botón Cargar y el arrastrar-soltar):
        /// filtra los .txt, pregunta por la segmentación y dispara el análisis.
        /// </summary>
        private async Task CargarArchivos(string[] rutas)
        {
            string[] txts = (rutas ?? new string[0])
                .Where(r => !string.IsNullOrEmpty(r) &&
                            r.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (txts.Length == 0)
            {
                LblEstado.Text = "No se encontraron archivos .txt para cargar.";
                return;
            }

            bool segmentar = GlassDialog.Pregunta(Owner, "Análisis de Mamparas",
                "¿Quieres segmentar el análisis de los Perfiles Metálicos por Ubicación?",
                si: "Sí, por ubicación", no: "No");

            try
            {
                _engine.Cargar(txts, segmentar);
            }
            catch (Exception ex)
            {
                LblEstado.Text = "Error al leer los archivos: " + ex.Message;
                return;
            }

            LblRuta.Text = _engine.DirectorioActual ?? "";
            ActualizarTituloVentana();
            await RecalcularAsync(seleccionarPestana: true);
        }

        // Pone el título de la ventana contenedora = "código nombre" del proyecto cargado
        // (mismo formato que el nombre del Excel). Si no hay info, deja el título por defecto.
        private void ActualizarTituloVentana()
        {
            string t = AnalisisEngine.NombreProyecto(_engine.ProyectoCodigo, _engine.ProyectoNombre, _engine.ProyectoReferencia);
            MdiChild host = HostMdi();
            if (host != null)
                host.CambiarTitulo(string.IsNullOrEmpty(t) ? "Análisis de Mamparas" : t);
        }

        // Sube por el árbol visual hasta la ventana hija (MdiChild) que contiene el panel.
        private MdiChild HostMdi()
        {
            DependencyObject d = this;
            while (d != null && !(d is MdiChild))
                d = VisualTreeHelper.GetParent(d);
            return d as MdiChild;
        }

        // ===== Arrastrar y soltar archivos sobre el panel =====
        private static bool TraeArchivos(DragEventArgs e) =>
            e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop);

        private void Ventana_DragOver(object sender, DragEventArgs e)
        {
            bool ok = TraeArchivos(e);
            e.Effects = ok ? DragDropEffects.Copy : DragDropEffects.None;
            if (ok) DropOverlay.Visibility = Visibility.Visible;
            e.Handled = true;
        }

        private void Ventana_DragLeave(object sender, DragEventArgs e)
        {
            // Ocultar sólo cuando el cursor sale de verdad del panel
            // (DragLeave también burbujea desde los controles internos).
            var p = e.GetPosition(this);
            if (p.X <= 0 || p.Y <= 0 || p.X >= ActualWidth || p.Y >= ActualHeight)
                DropOverlay.Visibility = Visibility.Collapsed;
        }

        private async void Ventana_Drop(object sender, DragEventArgs e)
        {
            DropOverlay.Visibility = Visibility.Collapsed;
            if (!TraeArchivos(e)) return;

            var rutas = e.Data.GetData(DataFormats.FileDrop) as string[];
            e.Handled = true;
            await CargarArchivos(rutas);
        }

        // ===== Recálculo en tiempo real (debounce) =====
        private void Valor_Changed(object sender, TextChangedEventArgs e)
        {
            if (!_engine.DatosCargados) return;
            _recalcTimer.Stop();
            _recalcTimer.Start();
        }

        private async void RecalcTimer_Tick(object sender, EventArgs e)
        {
            _recalcTimer.Stop();
            await RecalcularAsync(seleccionarPestana: false);
        }

        /// <summary>
        /// Lee los parámetros, corre el motor en segundo plano y muestra el resultado.
        /// Si llega un recálculo más nuevo mientras este corre, descarta el resultado viejo.
        /// </summary>
        private async Task RecalcularAsync(bool seleccionarPestana)
        {
            if (!_engine.DatosCargados) return;

            int medida = LeerEntero(TxtMedidaBase.Text);
            int pct = LeerEntero(TxtDesperdicio.Text);
            int gen = ++_generacion;

            LblEstado.Text = "Calculando…";
            SpinnerCargando();
            var sw = Stopwatch.StartNew();

            ResultadoAnalisis res = null;
            Exception err = null;
            try
            {
                // El motor NO toca UI → seguro en hilo de fondo.
                res = await Task.Run(() => _engine.Ejecutar(medida, pct));
            }
            catch (Exception ex) { err = ex; }

            sw.Stop();
            if (gen != _generacion) return;   // ya hay un cálculo más reciente: ignorar este

            if (err != null)
            {
                SpinnerOcultar();
                LblEstado.Text = "Error en el cálculo: " + err.Message;
                return;
            }

            // Cálculo fresco: guarda la base intacta (con placeholders MOD… y perfilería 01),
            // carga las reglas de dependencia y muestra ya resuelto para la perfilería 01.
            _base = res.Copiar();
            _resolver = DependenciaResolver.Cargar();
            _acabadoPerfil = AcabadoPorDefecto(res.PerfilMetalico);
            RefrescarDesdeBase(seleccionarPestana);

            LblEstado.Text = res.TieneDatos
                ? string.Format("Listo · {0:0.0} s", sw.ElapsedMilliseconds / 1000.0)
                : "No se encontraron datos para analizar.";

            SpinnerListo();
            await Task.Delay(750);
            if (gen == _generacion) SpinnerOcultar();
        }

        // ===== Animación spinner → check (XAML puro, sin dependencias) =====
        private void IniciarGiro()
        {
            var giro = new DoubleAnimation(0, 360, new Duration(TimeSpan.FromSeconds(0.9)))
            { RepeatBehavior = RepeatBehavior.Forever };
            SpinRotate.BeginAnimation(RotateTransform.AngleProperty, giro);
        }

        private void DetenerGiro() => SpinRotate.BeginAnimation(RotateTransform.AngleProperty, null);

        private void SpinnerCargando()
        {
            SpinnerOverlay.BeginAnimation(OpacityProperty, null);
            SpinnerOverlay.Opacity = 1;
            SpinnerOverlay.Visibility = Visibility.Visible;

            SpinArc.BeginAnimation(OpacityProperty, null);
            SpinArc.Opacity = 1;
            CheckPath.BeginAnimation(OpacityProperty, null);
            CheckPath.Opacity = 0;
            CheckScale.ScaleX = CheckScale.ScaleY = 0.6;
            LblSpinner.Text = "Calculando…";
            IniciarGiro();
        }

        private void SpinnerListo()
        {
            DetenerGiro();
            LblSpinner.Text = "Listo";

            SpinArc.BeginAnimation(OpacityProperty,
                new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.15))));
            CheckPath.BeginAnimation(OpacityProperty,
                new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.15))));

            var pop = new DoubleAnimation(0.6, 1, new Duration(TimeSpan.FromSeconds(0.35)))
            { EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.7 } };
            CheckScale.BeginAnimation(ScaleTransform.ScaleXProperty, pop);
            CheckScale.BeginAnimation(ScaleTransform.ScaleYProperty, pop);
        }

        private void SpinnerOcultar()
        {
            DetenerGiro();
            var fade = new DoubleAnimation(SpinnerOverlay.Opacity, 0,
                new Duration(TimeSpan.FromSeconds(0.2)));
            fade.Completed += (s, e) => SpinnerOverlay.Visibility = Visibility.Collapsed;
            SpinnerOverlay.BeginAnimation(OpacityProperty, fade);
        }

        /// <summary>
        /// Acabado POR DEFECTO de la perfilería = el del CÓDIGO "01" (el sufijo "-01" del
        /// código del perfil), NO por descripción/homologación. Devuelve "CÓDIGO - DESC"
        /// como el buscador. Recorre las filas de perfil (no separador, no cabecera "Puerta"):
        /// el código va en la col 1 ("BASE-ACAB") y la descripción del acabado en la col 3.
        /// Si no hay ninguna fila con código de acabado "01", cae a la primera fila válida.
        /// </summary>
        private static string AcabadoPorDefecto(DataTable perfil)
        {
            if (perfil == null || perfil.Columns.Count < 4) return "";
            string primera = "";
            foreach (DataRow row in perfil.Rows)
            {
                string c0 = Convert.ToString(row[0]);
                if (string.IsNullOrEmpty(c0) || c0.Contains("Puerta")) continue;
                string desc = Convert.ToString(row[3]);
                if (string.IsNullOrEmpty(desc)) continue;
                string cod = Convert.ToString(row[1]);
                string codAcab = cod.Contains("-") ? cod.Split('-')[1].Trim() : "";
                string full = codAcab != "" ? codAcab + " - " + desc : desc;
                if (primera == "") primera = full;
                if (codAcab == "01") return full;
            }
            return primera;
        }

        /// <summary>Código de acabado de una cadena "CÓDIGO - DESCRIPCIÓN".</summary>
        private static string CodigoAcabado(string acabado)
        {
            if (string.IsNullOrEmpty(acabado)) return "";
            return acabado.Contains("-") ? acabado.Split('-')[0].Trim() : acabado.Trim();
        }

        /// <summary>
        /// Reconstruye el resultado mostrado desde la copia base intacta: parte de los
        /// placeholders MOD… y la perfilería por defecto, aplica el cambio a la perfilería
        /// vigente (_acabadoPerfil) y resuelve las dependencias para ese valor. Al partir
        /// SIEMPRE de la base, se puede re-resolver cuantas veces cambie la perfilería.
        /// </summary>
        private void RefrescarDesdeBase(bool seleccionarPestana)
        {
            if (_base == null) return;

            ResultadoAnalisis vista = _base.Copiar();

            // 1) Lleva la perfilería del valor por defecto al vigente (si cambió).
            string defecto = AcabadoPorDefecto(_base.PerfilMetalico);
            if (!string.IsNullOrWhiteSpace(_acabadoPerfil) &&
                CodigoAcabado(_acabadoPerfil) != CodigoAcabado(defecto))
                AcabadoChanger.Aplicar(vista, defecto, _acabadoPerfil);

            // 2) Resuelve los acabados dependientes (MOD…) según la perfilería vigente.
            //    Recarga las reglas cada vez, para tomar cambios hechos en la pantalla de
            //    Dependencias sin tener que re-analizar.
            _resolver = DependenciaResolver.Cargar();
            if (_resolver != null && _resolver.HayReglas)
            {
                var sinRegla = _resolver.Resolver(vista, CodigoAcabado(_acabadoPerfil));
                if (sinRegla.Count > 0)
                    LblEstado.Text = "Aviso: sin regla de dependencia para " + string.Join(", ", sinRegla)
                                   + " con esta perfilería.";
            }

            MostrarResultado(vista, seleccionarPestana);
        }

        private void MostrarResultado(ResultadoAnalisis r, bool seleccionarPestana)
        {
            _ultimo = r;
            DgPerfilMetalico.ItemsSource = Vista(r.PerfilMetalico);
            DgPerfilMetalicoHerraje.ItemsSource = Vista(r.PerfilMetalicoHerraje);
            DgVidrioPaneles.ItemsSource = Vista(r.VidrioPaneles);
            DgPuertas.ItemsSource = Vista(r.Puertas);
            DgPuertasHerrajes.ItemsSource = Vista(r.PuertasHerraje);
            DgPuertasCantidad.ItemsSource = Vista(r.PuertasCantidad);
            DgTubos.ItemsSource = Vista(r.Tubos);
            DgMamparas.ItemsSource = Vista(r.Mamparas);

            BtnChange.Visibility = r.TieneDatos ? Visibility.Visible : Visibility.Collapsed;
            BtnVidrio.Visibility = r.TieneDatos ? Visibility.Visible : Visibility.Collapsed;
            HintVacio.Visibility = r.TieneDatos ? Visibility.Collapsed : Visibility.Visible;

            if (seleccionarPestana && r.PestanaSugerida >= 0 && r.PestanaSugerida < Tabs.Items.Count)
                Tabs.SelectedIndex = r.PestanaSugerida;

            if (PreviewPanel.Visibility == Visibility.Visible)
                RefreshPreview();
        }

        // ===== Previsualizador de datos cargados (crudos) =====
        private void Preview_Click(object sender, RoutedEventArgs e)
        {
            bool mostrar = PreviewPanel.Visibility != Visibility.Visible;
            if (mostrar)
            {
                FilaSplitter.Height = new GridLength(8);
                // Conserva el alto previo si el usuario ya lo había ajustado.
                if (FilaPreview.Height.Value < 60)
                    FilaPreview.Height = new GridLength(240);
                FilaPreview.MinHeight = 90;
                PreviewSplitter.Visibility = Visibility.Visible;
                PreviewPanel.Visibility = Visibility.Visible;
                RefreshPreview();
            }
            else
            {
                FilaSplitter.Height = new GridLength(0);
                FilaPreview.Height = new GridLength(0);
                FilaPreview.MinHeight = 0;
                PreviewSplitter.Visibility = Visibility.Collapsed;
                PreviewPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Ignora eventos de selección que burbujean desde grillas internas.
            if (!ReferenceEquals(e.OriginalSource, Tabs)) return;
            if (PreviewPanel == null || PreviewPanel.Visibility != Visibility.Visible) return;
            RefreshPreview();
        }

        private void RefreshPreview()
        {
            string nombre = (Tabs.SelectedItem as TabItem)?.Header?.ToString() ?? "";
            DataTable raw = _ultimo == null ? null : RawPorPestana(_ultimo, Tabs.SelectedIndex);
            DgPreview.ItemsSource = raw == null ? null : raw.DefaultView;
            LblPreview.Text = "Datos cargados — " + nombre + (raw == null ? "  (sin datos)" : "");
        }

        private static DataTable RawPorPestana(ResultadoAnalisis r, int idx)
        {
            switch (idx)
            {
                case 0:
                case 1: return r.PerfilMetalicoRaw;   // Perfil Metálico (+ Herraje)
                case 2: return r.VidrioRaw;
                case 3:
                case 4:
                case 5: return r.PuertasRaw;          // Puertas (+ Herrajes, Cantidad)
                case 6: return r.TubosRaw;
                case 7: return r.MamparasRaw;
                default: return null;
            }
        }

        private static DataView Vista(DataTable t) => t == null ? null : t.DefaultView;

        // ===== Ajuste de columnas: todo centrado; la "descripción" se ajusta al
        //       ancho disponible y hace wrap (la fila crece) en vez de desbordar. =====
        private void Dg_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            bool esDescripcion = e.PropertyName != null
                && e.PropertyName.ToLowerInvariant().Contains("descrip");

            var estiloCelda = new Style(typeof(TextBlock));
            estiloCelda.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center));
            estiloCelda.Setters.Add(new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center));
            estiloCelda.Setters.Add(new Setter(TextBlock.PaddingProperty, new Thickness(6, 3, 6, 3)));
            if (esDescripcion)
                estiloCelda.Setters.Add(new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap));

            var bound = e.Column as DataGridBoundColumn;
            if (bound != null) bound.ElementStyle = estiloCelda;

            if (esDescripcion)
            {
                e.Column.Width = new DataGridLength(250);
                e.Column.MinWidth = 140;
            }
            else
            {
                e.Column.Width = DataGridLength.Auto;
            }
        }

        // ===== Coloreado de filas del grid de Puertas =====
        private static readonly Brush PuertaSeparador = Congelar(Color.FromRgb(0x2A, 0x2A, 0x2A));
        private static readonly Brush PuertaTitulo    = Congelar(Color.FromArgb(0x4D, 0xE0, 0x7B, 0x5B)); // peach
        private static readonly Brush PuertaVariante  = Congelar(Color.FromArgb(0x4D, 0x53, 0xC5, 0x6E)); // verde

        private static Brush Congelar(Color c)
        {
            var b = new SolidColorBrush(c);
            b.Freeze();
            return b;
        }

        private void DgPuertas_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            var drv = e.Row.Item as DataRowView;
            if (drv == null || drv.Row.ItemArray.Length == 0)
            {
                e.Row.ClearValue(Control.BackgroundProperty);
                return;
            }

            string v = Convert.ToString(drv.Row[0]);
            if (string.IsNullOrEmpty(v))
                e.Row.Background = PuertaSeparador;
            else if (v.Contains("Puerta"))
                e.Row.Background = PuertaTitulo;
            else if (v.Contains("~"))
                e.Row.Background = PuertaVariante;
            else
                e.Row.ClearValue(Control.BackgroundProperty);   // fila normal: estilo por defecto
        }

        // ===== Cancelar: limpia las tablas mostradas =====
        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            _generacion++;   // descarta cualquier cálculo en vuelo
            DgPerfilMetalico.ItemsSource = null;
            DgPerfilMetalicoHerraje.ItemsSource = null;
            DgVidrioPaneles.ItemsSource = null;
            DgPuertas.ItemsSource = null;
            DgPuertasHerrajes.ItemsSource = null;
            DgPuertasCantidad.ItemsSource = null;
            DgTubos.ItemsSource = null;
            DgMamparas.ItemsSource = null;
            _ultimo = null;
            DgPreview.ItemsSource = null;
            BtnChange.Visibility = Visibility.Collapsed;
            BtnVidrio.Visibility = Visibility.Collapsed;
            _engine.SeleccionVidrio = null;
            HintVacio.Visibility = Visibility.Visible;
            LblEstado.Text = "Listo. Carga uno o varios archivos TXT para analizar.";
            LblRuta.Text = "";
        }

        // ===== Exportar a Excel (mismo formato que WinForms) =====
        private async void Exportar_Click(object sender, RoutedEventArgs e)
        {
            if (_ultimo == null || !_ultimo.TieneDatos)
            {
                GlassDialog.Informar(Owner, "Exportar", "No existen datos analizados para exportar.");
                return;
            }

            // Tipo de vidrio: se muestra en el diálogo precargado con lo que ya se está
            // aplicando, y desde ahí se puede cambiar (igual que el acabado de perfilería).
            VidrioResolver vidrio = VidrioResolver.Cargar();
            var sistemas = vidrio.HayConfiguracion ? _engine.SistemasDelProyecto(vidrio) : null;

            var bsc = new ExportDialog
            {
                Owner = Owner,
                PrefillNumero = _engine.ProyectoCodigo,
                PrefillNombre = _engine.ProyectoNombre,
                PrefillReferencia = _engine.ProyectoReferencia,
                PrefillAcabado1 = _acabadoPerfil,  // Sentido A: precarga el acabado ya aplicado
                VidrioSistemas = sistemas,
                VidrioResolver = vidrio,
                VidrioSeleccion = _engine.SeleccionVidrio
            };
            bsc.ShowDialog();
            if (bsc.Numero == null) return;   // canceló

            // Si se cambió el tipo de vidrio hay que REHACER el cálculo antes de exportar: la
            // sustitución se aplica dentro del análisis, no sobre el resultado.
            if (bsc.VidrioCambiado)
            {
                _engine.SeleccionVidrio = bsc.VidrioSeleccion;
                await RecalcularAsync(seleccionarPestana: false);
                if (_ultimo == null || !_ultimo.TieneDatos)
                {
                    GlassDialog.Informar(Owner, "Exportar", "El recálculo no devolvió datos: no se exportó nada.");
                    return;
                }
            }

            // Sentido B: si en el diálogo se eligió un acabado de perfilería distinto al
            // que tienen las cantidades, aplícalo antes de exportar (así se pasa el "01" —o el
            // vigente— al escogido, sin usar "Cambiar Acabado"). Va por el pipeline base para
            // que también se re-resuelvan las dependencias MOD… para la nueva perfilería.
            if (!string.IsNullOrWhiteSpace(bsc.Acabado1) &&
                !string.IsNullOrWhiteSpace(_acabadoPerfil) &&
                bsc.Acabado1.Trim() != _acabadoPerfil.Trim())
            {
                _acabadoPerfil = bsc.Acabado1;   // el vigente pasa a ser el elegido
                RefrescarDesdeBase(false);
            }

            string[] param = { bsc.Numero, bsc.Nombre, bsc.Tecnico, bsc.Fecha,
                               bsc.Acabado1, bsc.Acabado2, bsc.Albaran, bsc.Referencia };

            string folder;
            using (var fb = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (!string.IsNullOrEmpty(_engine.DirectorioActual))
                    fb.SelectedPath = _engine.DirectorioActual;
                if (fb.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
                folder = fb.SelectedPath;
            }
            if (string.IsNullOrEmpty(folder)) return;

            try
            {
                LblEstado.Text = "Exportando…";
                string archivo = new ExcelExporter()
                    .Exportar(_ultimo, param, folder, _ultimo.SwSegmentadoUbiFinal);

                LblEstado.Text = "Exportado: " + archivo;
                if (GlassDialog.Pregunta(Owner, "Exportar",
                        "Se exportó correctamente. ¿Deseas abrirlo ahora?", si: "Abrir", no: "Ahora no"))
                    Process.Start(archivo);
                else
                    GlassDialog.Informar(Owner, "Exportar", "El archivo está en:\n" + archivo);
            }
            catch (IOException)
            {
                GlassDialog.Informar(Owner, "Exportar",
                    "Un archivo se encontraba abierto. Ciérralo e inténtalo nuevamente.");
                LblEstado.Text = "Exportación cancelada (archivo abierto).";
            }
            catch (Exception ex)
            {
                GlassDialog.Informar(Owner, "Exportar", "Error al exportar: " + ex.Message);
                LblEstado.Text = "Error al exportar.";
            }
        }

        // ===== Cambiar Acabado (global) =====
        private void CambiarAcabado_Click(object sender, RoutedEventArgs e)
        {
            if (_ultimo == null || !_ultimo.TieneDatos)
            {
                GlassDialog.Informar(Owner, "Cambiar Acabado", "Primero carga y analiza unos archivos.");
                return;
            }

            string a1, a2;
            if (!GlassDialog.PedirAcabado(Owner, out a1, out a2)) return;
            if (string.IsNullOrWhiteSpace(a1))
            {
                GlassDialog.Informar(Owner, "Cambiar Acabado", "Indica el acabado a buscar (origen).");
                return;
            }

            // ¿Se está cambiando la PERFILERÍA (el acabado del slot por defecto vigente)?
            if (CodigoAcabado(a1) == CodigoAcabado(_acabadoPerfil) && !string.IsNullOrWhiteSpace(a2))
            {
                // Va por el pipeline base: re-aplica perfilería + re-resuelve dependencias MOD…
                _acabadoPerfil = a2;
                RefrescarDesdeBase(false);
            }
            else
            {
                // Cambio de un acabado cualquiera (no perfilería): edición directa, como antes.
                AcabadoChanger.Aplicar(_ultimo, a1, a2 ?? "");
                MostrarResultado(_ultimo, false);
            }
            LblEstado.Text = "Acabado actualizado: \"" + a1 + "\" → \"" + a2 + "\".";
        }

        // ===== Tipo de vidrio por sistema =====
        // A diferencia de "Cambiar Acabado" (que retoca el resultado ya calculado), aquí hay que
        // VOLVER A CALCULAR: la sustitución se aplica dentro del cálculo, componente a componente,
        // que es donde todavía se sabe de qué sistema viene cada pieza.
        private async void TipoVidrio_Click(object sender, RoutedEventArgs e)
        {
            if (!_engine.DatosCargados)
            {
                GlassDialog.Informar(Owner, "Tipo de vidrio", "Primero carga los archivos del proyecto.");
                return;
            }

            VidrioResolver resolver = VidrioResolver.Cargar();
            if (!resolver.HayConfiguracion)
            {
                GlassDialog.Informar(Owner, "Tipo de vidrio",
                    "Todavía no hay sistemas dados de alta. Configúralos en la pantalla Vidrios.");
                return;
            }

            var sistemas = _engine.SistemasDelProyecto(resolver);
            if (sistemas.Count == 0)
            {
                GlassDialog.Informar(Owner, "Tipo de vidrio",
                    "No se pudo identificar ningún sistema en este proyecto. Suele faltar el TXT de mamparas (5-…).");
                return;
            }

            var dlg = new VidrioDialog { Owner = Owner };
            dlg.Cargar(sistemas, resolver, _engine.SeleccionVidrio);
            if (dlg.ShowDialog() != true) return;

            _engine.SeleccionVidrio = dlg.Seleccion;
            await RecalcularAsync(seleccionarPestana: false);

            LblEstado.Text = "Tipo de vidrio aplicado: " + Resumen(dlg.Seleccion, resolver) + ".";
        }

        // "DV → 6+6 · AV → 5+5" (solo lo que se aparta del estándar del sistema).
        private static string Resumen(IDictionary<string, int> seleccion, VidrioResolver resolver)
        {
            var partes = new List<string>();
            foreach (var par in seleccion)
            {
                SistemaVidrio s = resolver.SistemaDeCodigo(par.Key);
                if (s != null && par.Value == s.IdTipoEstandar) continue;   // sigue como en la base
                partes.Add(par.Key + " → " + resolver.NombreTipo(par.Value));
            }
            return partes.Count == 0 ? "todo como está en la base" : string.Join(" · ", partes);
        }

        // ===== Menú clic-derecho sobre Perfil Metálico =====
        private DataRow FilaPerfilSeleccionada()
        {
            var drv = DgPerfilMetalico.SelectedItem as DataRowView;
            return drv == null ? null : drv.Row;
        }

        // "Remplazar SubComponente" → catálogo (reutiliza el buscador WinForms).
        private void RemplazarSub_Click(object sender, RoutedEventArgs e)
        {
            var row = FilaPerfilSeleccionada();
            if (row == null)
            {
                GlassDialog.Informar(Owner, "Remplazar SubComponente", "Selecciona primero una fila de Perfil Metálico.");
                return;
            }

            var bsc = new BuscarDialog { Consulta = "SubComp", Owner = Owner };
            if (bsc.ShowDialog() != true) return;

            string codigo = bsc.ReturnItem1.Trim();
            string descripcion = bsc.ReturnItem2.Split('(')[0].Trim();
            string acabado = bsc.ReturnItem5;

            row[1] = codigo;
            row[2] = descripcion;
            row[3] = acabado;
            MostrarResultado(_ultimo, false);
            LblEstado.Text = "SubComponente remplazado por " + codigo + ".";
        }

        // "Cambiar Acabado Temporal" → sólo la fila seleccionada.
        private void AcabadoTemporal_Click(object sender, RoutedEventArgs e)
        {
            var row = FilaPerfilSeleccionada();
            if (row == null)
            {
                GlassDialog.Informar(Owner, "Cambiar Acabado", "Selecciona primero una fila de Perfil Metálico.");
                return;
            }

            string acabadoActual = row.ItemArray.Length > 3 ? Convert.ToString(row[3]) : "";
            string a1, a2;
            if (!GlassDialog.PedirAcabado(Owner, out a1, out a2, acabadoActual)) return;
            if (string.IsNullOrWhiteSpace(a2)) return;

            AcabadoChanger.CambiarFilaPerfil(row, a2);
            MostrarResultado(_ultimo, false);
            LblEstado.Text = "Acabado de la fila actualizado.";
        }
    }
}
