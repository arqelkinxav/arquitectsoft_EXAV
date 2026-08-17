using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace arquitectSoft.View.Wpf.Panels
{
    /// <summary>
    /// Pantalla de configuración de las "dependencias de vidrio". En la base cada componente
    /// tiene sus subcomponentes cargados como el tipo de vidrio ESTÁNDAR de su sistema; aquí
    /// se declaran los sistemas (por el prefijo de su código: DV, IT, AV…) y, para cada tipo
    /// alternativo, qué subcomponente pasa a ser cuál — el vidrio y todo lo que arrastra
    /// (calces, gomas, cintas, perfiles).
    ///
    /// Guarda en <c>beta_vidrio_sistema</c> / <c>beta_vidrio_regla</c> vía <see cref="Dto.VidrioDto"/>.
    /// </summary>
    public partial class VidriosPanel : UserControl
    {
        private readonly Dto.VidrioDto _dto = new Dto.VidrioDto();

        private int _idSistema;                 // sistema seleccionado (0 = nuevo)
        private int _idSubOrigen, _idSubDestino;   // par elegido con el buscador
        private bool _cargando;                 // evita recargas mientras se rellenan los combos

        public VidriosPanel()
        {
            InitializeComponent();
            Loaded += (s, e) => { if (CmbTipoDestino.ItemsSource == null) Iniciar(); };
        }

        // Ventana que hospeda el panel (para que los diálogos cristal tengan owner).
        private Window Owner { get { return Window.GetWindow(this); } }

        private void Iniciar()
        {
            _cargando = true;
            try
            {
                DataTable tipos = _dto.GetTipos();
                if (tipos == null)
                {
                    LblEstado.Text = "No se encontró la configuración de vidrios en la base. "
                                   + "Ejecuta db/migrations/005_dependencias_vidrio.sql.";
                    return;
                }
                CmbTipoDestino.ItemsSource = tipos.DefaultView;
                CmbEstandar.ItemsSource = _dto.GetTiposConVacio().DefaultView;
                CmbEstandar.SelectedValue = 0;
                if (tipos.Rows.Count > 0) CmbTipoDestino.SelectedIndex = 0;
            }
            finally { _cargando = false; }

            CargarSistemas();
        }

        // ===== Sistemas =====

        private void CargarSistemas()
        {
            DataTable dt = _dto.GetSistemas();
            if (dt == null) { LblEstado.Text = "No se pudieron cargar los sistemas."; return; }
            GridSistemas.ItemsSource = dt.DefaultView;
        }

        private void GridSistemas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var drv = GridSistemas.SelectedItem as DataRowView;
            if (drv == null) return;

            _cargando = true;
            _idSistema = Convert.ToInt32(drv.Row["Id"]);
            TxtPrefijo.Text = Convert.ToString(drv.Row["Prefijo"]);
            TxtDescripcion.Text = Convert.ToString(drv.Row["Descripcion"]);
            CmbEstandar.SelectedValue = Convert.ToInt32(drv.Row["Id_Tipo_Estandar"]);
            _cargando = false;

            CargarReglas();
        }

        private void NuevoSistema_Click(object sender, RoutedEventArgs e)
        {
            _cargando = true;
            _idSistema = 0;
            GridSistemas.SelectedItem = null;
            TxtPrefijo.Text = "";
            TxtDescripcion.Text = "";
            CmbEstandar.SelectedValue = 0;
            _cargando = false;

            CargarReglas();
            TxtPrefijo.Focus();
            LblEstado.Text = "Sistema nuevo: escribe el prefijo con el que empiezan sus códigos y guarda.";
        }

        private void GuardarSistema_Click(object sender, RoutedEventArgs e)
        {
            string prefijo = (TxtPrefijo.Text ?? "").Trim();
            if (prefijo == "")
            {
                GlassDialog.Informar(Owner, "Dependencias de vidrio",
                    "Escribe el prefijo con el que empiezan los códigos del sistema (DV, IT, AV…).");
                return;
            }

            string fail = _dto.GuardarSistema(_idSistema, prefijo, (TxtDescripcion.Text ?? "").Trim(), TipoEstandar);
            if (!string.IsNullOrEmpty(fail)) { LblEstado.Text = "Error al guardar el sistema: " + fail; return; }

            CargarSistemas();
            SeleccionarSistema(prefijo);
            LblEstado.Text = "Sistema guardado.";
        }

        private void EliminarSistema_Click(object sender, RoutedEventArgs e)
        {
            if (_idSistema == 0)
            {
                GlassDialog.Informar(Owner, "Dependencias de vidrio", "Selecciona un sistema de la lista.");
                return;
            }
            if (!GlassDialog.Pregunta(Owner, "Dependencias de vidrio",
                    "¿Eliminar el sistema \"" + TxtPrefijo.Text + "\" y TODAS sus sustituciones?",
                    si: "Eliminar", no: "Cancelar"))
                return;

            string fail = _dto.EliminarSistema(_idSistema);
            if (!string.IsNullOrEmpty(fail)) { LblEstado.Text = "Error al eliminar: " + fail; return; }

            CargarSistemas();
            NuevoSistema_Click(null, null);
            LblEstado.Text = "Sistema eliminado.";
        }

        // Vuelve a dejar seleccionado el sistema recién guardado (la lista se recarga entera).
        private void SeleccionarSistema(string prefijo)
        {
            var vista = GridSistemas.ItemsSource as DataView;
            if (vista == null) return;
            foreach (DataRowView drv in vista)
            {
                if (!string.Equals(Convert.ToString(drv.Row["Prefijo"]).Trim(), prefijo,
                                   StringComparison.OrdinalIgnoreCase)) continue;
                GridSistemas.SelectedItem = drv;
                GridSistemas.ScrollIntoView(drv);
                return;
            }
        }

        private int TipoEstandar
        {
            get { return CmbEstandar.SelectedValue == null ? 0 : Convert.ToInt32(CmbEstandar.SelectedValue); }
        }

        private int TipoDestino
        {
            get { return CmbTipoDestino.SelectedValue == null ? 0 : Convert.ToInt32(CmbTipoDestino.SelectedValue); }
        }

        // ===== Sustituciones =====

        private void CmbTipoDestino_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_cargando) return;
            CargarReglas();
        }

        private void CargarReglas()
        {
            if (_idSistema == 0 || TipoDestino == 0)
            {
                GridReglas.ItemsSource = null;
                LblSistema.Text = "SUSTITUCIONES — SELECCIONA UN SISTEMA";
                return;
            }

            DataTable dt = _dto.GetReglas(_idSistema, TipoDestino);
            GridReglas.ItemsSource = dt == null ? null : dt.DefaultView;

            LblSistema.Text = ("SUSTITUCIONES DE " + TxtPrefijo.Text + " CUANDO SEA " + NombreTipoDestino()).ToUpper();

            // Cargar reglas para el propio estándar no tiene sentido: no hay nada que sustituir.
            if (TipoDestino == TipoEstandar && TipoEstandar != 0)
                LblEstado.Text = "Ese es el tipo ESTÁNDAR del sistema: no necesita sustituciones (es lo que ya está en la base).";
        }

        private string NombreTipoDestino()
        {
            var drv = CmbTipoDestino.SelectedItem as DataRowView;
            return drv == null ? "" : Convert.ToString(drv.Row["Nombre"]);
        }

        private void BuscarOrigen_Click(object sender, RoutedEventArgs e)
        {
            int id; string texto;
            if (ElegirSubcomponente(out id, out texto)) { _idSubOrigen = id; TxtOrigen.Text = texto; }
        }

        private void BuscarDestino_Click(object sender, RoutedEventArgs e)
        {
            int id; string texto;
            if (ElegirSubcomponente(out id, out texto)) { _idSubDestino = id; TxtDestino.Text = texto; }
        }

        // Buscador del catálogo de subcomponentes: ReturnItem0 = Id, 1 = código, 2 = descripción.
        // La casilla "Sólo especiales" del propio buscador da acceso a los vidrios y paneles.
        private bool ElegirSubcomponente(out int id, out string texto)
        {
            id = 0; texto = "";
            var bsc = new BuscarDialog { Consulta = "SubComp", Owner = Owner };
            if (bsc.ShowDialog() != true) return false;
            if (!int.TryParse((bsc.ReturnItem0 ?? "").Trim(), out id) || id <= 0) return false;
            texto = (bsc.ReturnItem1 ?? "").Trim() + " - " + (bsc.ReturnItem2 ?? "").Trim();
            return true;
        }

        private void AgregarRegla_Click(object sender, RoutedEventArgs e)
        {
            if (_idSistema == 0) { Avisar("Selecciona primero el sistema."); return; }
            if (TipoDestino == 0) { Avisar("Elige el tipo de vidrio al que se cambia."); return; }
            if (_idSubOrigen == 0 || _idSubDestino == 0)
            {
                Avisar("Elige el subcomponente estándar y el que debe sustituirlo.");
                return;
            }
            if (_idSubOrigen == _idSubDestino)
            {
                Avisar("El subcomponente de origen y el de destino son el mismo.");
                return;
            }

            string fail = _dto.GuardarRegla(_idSistema, TipoDestino, _idSubOrigen, _idSubDestino);
            if (!string.IsNullOrEmpty(fail)) { LblEstado.Text = "Error al guardar la sustitución: " + fail; return; }

            CargarReglas();
            Limpiar_Click(null, null);
            LblEstado.Text = "Sustitución guardada.";
        }

        private void EliminarRegla_Click(object sender, RoutedEventArgs e)
        {
            var drv = GridReglas.SelectedItem as DataRowView;
            if (drv == null) { Avisar("Selecciona la sustitución que quieres eliminar."); return; }
            if (!GlassDialog.Pregunta(Owner, "Dependencias de vidrio",
                    "¿Eliminar la sustitución seleccionada?", si: "Eliminar", no: "Cancelar"))
                return;

            string fail = _dto.EliminarRegla(Convert.ToInt32(drv.Row["Id"]));
            if (!string.IsNullOrEmpty(fail)) { LblEstado.Text = "Error al eliminar: " + fail; return; }

            CargarReglas();
            LblEstado.Text = "Sustitución eliminada.";
        }

        private void Limpiar_Click(object sender, RoutedEventArgs e)
        {
            _idSubOrigen = _idSubDestino = 0;
            TxtOrigen.Text = TxtDestino.Text = "";
            GridReglas.SelectedItem = null;
        }

        private void Avisar(string mensaje)
        {
            GlassDialog.Informar(Owner, "Dependencias de vidrio", mensaje);
        }
    }
}
