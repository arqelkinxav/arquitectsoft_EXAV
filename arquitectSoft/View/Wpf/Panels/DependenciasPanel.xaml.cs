using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace arquitectSoft.View.Wpf.Panels
{
    /// <summary>
    /// Pantalla de configuración de las "dependencias de acabado": reglas que dicen, para un
    /// acabado placeholder (MOD… / ELEGIR…) y un valor de perfilería, a qué acabado REAL debe
    /// convertirse el material dependiente. Guarda en la tabla <c>beta_dependencias_acabado</c>
    /// vía <see cref="Dto.DependenciaDto"/> (upsert por placeholder+perfilería).
    /// </summary>
    public partial class DependenciasPanel : UserControl
    {
        private readonly Dto.DependenciaDto _dto = new Dto.DependenciaDto();

        // Códigos (Codigo_Homologacion) elegidos en el formulario.
        private string _codPlaceholder = "";
        private string _codPerfileria = "";
        private string _codResultado = "";

        public DependenciasPanel()
        {
            InitializeComponent();
            Loaded += (s, e) => CargarReglas();
        }

        private Window Owner { get { return Window.GetWindow(this); } }

        private void CargarReglas()
        {
            try { GridReglas.ItemsSource = _dto.GetReglasDetalle().DefaultView; }
            catch (Exception ex) { LblEstado.Text = "No se pudieron cargar las reglas: " + ex.Message; }
        }

        // ===== Selectores (buscador de acabados del catálogo) =====
        private void BuscarPlaceholder_Click(object sender, RoutedEventArgs e)
        {
            string cod, texto;
            if (ElegirAcabado(out cod, out texto)) { _codPlaceholder = cod; TxtPlaceholder.Text = texto; }
        }

        private void BuscarPerfileria_Click(object sender, RoutedEventArgs e)
        {
            string cod, texto;
            if (ElegirAcabado(out cod, out texto)) { _codPerfileria = cod; TxtPerfileria.Text = texto; }
        }

        private void BuscarResultado_Click(object sender, RoutedEventArgs e)
        {
            string cod, texto;
            if (ElegirAcabado(out cod, out texto)) { _codResultado = cod; TxtResultado.Text = texto; }
        }

        private bool ElegirAcabado(out string codigo, out string texto)
        {
            codigo = ""; texto = "";
            var bsc = new BuscarDialog { Consulta = "Acaba", Owner = Owner };
            if (bsc.ShowDialog() != true) return false;
            codigo = (bsc.ReturnItem1 ?? "").Trim();   // Codigo_Homologacion
            texto = bsc.ReturnItem2 ?? "";             // "CÓDIGO - DESCRIPCIÓN"
            return codigo != "";
        }

        // ===== Guardar / Eliminar / Limpiar =====
        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            if (_codPlaceholder == "" || _codPerfileria == "" || _codResultado == "")
            {
                GlassDialog.Informar(Owner, "Dependencias",
                    "Elige el placeholder, el valor de perfilería y el acabado resultante.");
                return;
            }

            string fail = _dto.GuardarRegla(_codPlaceholder, _codPerfileria, _codResultado);
            if (!string.IsNullOrEmpty(fail)) { LblEstado.Text = "Error al guardar: " + fail; return; }

            CargarReglas();
            LblEstado.Text = "Regla guardada.";
        }

        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            if (_codPlaceholder == "" || _codPerfileria == "")
            {
                GlassDialog.Informar(Owner, "Dependencias",
                    "Selecciona una regla de la tabla (o indica placeholder y perfilería) para eliminar.");
                return;
            }
            if (!GlassDialog.Pregunta(Owner, "Dependencias",
                    "¿Eliminar la regla del placeholder para esa perfilería?", si: "Eliminar", no: "Cancelar"))
                return;

            string fail = _dto.EliminarRegla(_codPlaceholder, _codPerfileria);
            if (!string.IsNullOrEmpty(fail)) { LblEstado.Text = "Error al eliminar: " + fail; return; }

            CargarReglas();
            Limpiar_Click(null, null);
            LblEstado.Text = "Regla eliminada.";
        }

        private void Limpiar_Click(object sender, RoutedEventArgs e)
        {
            _codPlaceholder = _codPerfileria = _codResultado = "";
            TxtPlaceholder.Text = TxtPerfileria.Text = TxtResultado.Text = "";
            GridReglas.SelectedItem = null;
        }

        // ===== Grilla =====
        private void GridReglas_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Placeholder": e.Column.Header = "Placeholder";
                    e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star); break;
                case "Perfileria": e.Column.Header = "Si la perfilería es…";
                    e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star); break;
                case "Resultado": e.Column.Header = "El placeholder pasa a…";
                    e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star); break;
                default: e.Cancel = true; break;   // oculta las columnas de código
            }
        }

        private void GridReglas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var drv = GridReglas.SelectedItem as DataRowView;
            if (drv == null) return;

            _codPlaceholder = Convert.ToString(drv.Row["Cod_Placeholder"]).Trim();
            _codPerfileria = Convert.ToString(drv.Row["Cod_Perfileria"]).Trim();
            _codResultado = Convert.ToString(drv.Row["Cod_Resultado"]).Trim();

            TxtPlaceholder.Text = _codPlaceholder + " - " + Convert.ToString(drv.Row["Placeholder"]);
            TxtPerfileria.Text = _codPerfileria + " - " + Convert.ToString(drv.Row["Perfileria"]);
            TxtResultado.Text = _codResultado + " - " + Convert.ToString(drv.Row["Resultado"]);
        }
    }
}
