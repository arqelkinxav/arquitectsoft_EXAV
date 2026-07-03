using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace arquitectSoft.View.Wpf.Panels
{
    /// <summary>
    /// Versión "panel" de Cortes para hospedarse dentro del escritorio (MdiChild).
    /// Patrón CRUD-tabla igual que Mecanizados; el registro tiene 4 campos: Código (auto =
    /// Id_Corte, solo lectura), Descripción y dos enteros Corte Derecho / Corte Izquierdo.
    /// Sin chrome ni liquid glass: lo aporta la ventana hija. Reutiliza Dto.CorteDto.
    ///
    /// OJO con el Dto: Dto.CorteDto.SaveCorte mapea corteizq/corteder a las columnas
    /// Corte_Derecho/Corte_Izquierdo de forma DISTINTA en INSERT vs UPDATE; lo compensamos
    /// en Persistir() para que "Corte Derecho" escriba en Corte_Derecho y viceversa.
    /// </summary>
    public partial class CortePanel : UserControl
    {
        private DataTable _tabla;
        private DataView _vista;
        private bool _recargando;

        public CortePanel()
        {
            InitializeComponent();
            Loaded += (s, e) => { if (_tabla == null) CargarLista(); };
        }

        private Window Owner { get { return Window.GetWindow(this); } }

        // ===== Carga / refresco =====
        private void CargarLista()
        {
            _recargando = true;
            try
            {
                DataTable dt = new Dto.CorteDto().GetCortes();   // trae fila sintética (Id 0)
                for (int i = dt.Rows.Count - 1; i >= 0; i--)
                {
                    int id;
                    int.TryParse(Convert.ToString(dt.Rows[i]["Id_Corte"]), out id);
                    if (id == 0) dt.Rows[i].Delete();
                }
                dt.AcceptChanges();

                _tabla = dt;
                _vista = dt.DefaultView;
                AplicarFiltro();
                GridDatos.ItemsSource = _vista;
            }
            catch (Exception ex)
            {
                LblEstado.Text = "No se pudo cargar la lista: " + ex.Message;
            }
            finally { _recargando = false; }
        }

        private void AplicarFiltro()
        {
            if (_vista == null) return;
            string txt = (TxtFiltro.Text ?? "").Replace("'", "").Trim();
            if (txt == "") { _vista.RowFilter = ""; return; }

            var partes = txt.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string filtro = "";
            for (int i = 0; i < partes.Length; i++)
            {
                if (i > 0) filtro += " AND ";
                filtro += "(Descripcion LIKE '%" + partes[i] + "%'"
                        + " OR Convert(Id_Corte, 'System.String') LIKE '%" + partes[i] + "%')";
            }
            _vista.RowFilter = filtro;
        }

        private void Filtro_Changed(object sender, TextChangedEventArgs e)
        {
            if (_recargando) return;
            AplicarFiltro();
        }

        private void GridDatos_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Id_Corte":
                    e.Column.Header = "Código";
                    e.Column.Width = 110;
                    e.Column.IsReadOnly = true;   // el código se auto-genera, no se edita
                    break;
                case "Descripcion":
                    e.Column.Header = "Descripción";
                    e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                    break;
                case "Corte_Derecho":
                    e.Column.Header = "Corte Derecho";
                    e.Column.Width = 140;
                    break;
                case "Corte_Izquierdo":
                    e.Column.Header = "Corte Izquierdo";
                    e.Column.Width = 140;
                    break;
                default:
                    e.Cancel = true;
                    break;
            }
        }

        // ===== Nuevo: línea editable con código auto-generado =====
        private void Nuevo_Click(object sender, RoutedEventArgs e)
        {
            if (_tabla == null) return;
            TxtFiltro.Text = "";

            string codigo;
            try { codigo = new Dto.CorteDto().MaximoCorte(); }
            catch (Exception ex) { LblEstado.Text = "No se pudo generar el código: " + ex.Message; return; }

            DataRow fila = _tabla.NewRow();
            fila["Id_Corte"] = 0;
            fila["Descripcion"] = "";
            fila["Corte_Derecho"] = 0;
            fila["Corte_Izquierdo"] = 0;
            _tabla.Rows.Add(fila);

            var drv = BuscarDrv(fila);
            if (drv != null)
            {
                GridDatos.SelectedItem = drv;
                GridDatos.ScrollIntoView(drv);
                // Edita directamente la columna Descripción (índice 1: Código va en 0).
                if (GridDatos.Columns.Count > 1)
                {
                    GridDatos.CurrentCell = new DataGridCellInfo(drv, GridDatos.Columns[1]);
                    GridDatos.BeginEdit();
                }
            }
            LblEstado.Text = "Código " + codigo + " (estimado). Escribe la descripción y los cortes; al salir de la fila se guarda.";
        }

        private DataRowView BuscarDrv(DataRow fila)
        {
            foreach (DataRowView drv in _vista)
                if (drv.Row == fila) return drv;
            return null;
        }

        // ===== Eliminar =====
        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            var drv = GridDatos.SelectedItem as DataRowView;
            if (drv == null) { LblEstado.Text = "Selecciona primero una fila para eliminar."; return; }

            int id;
            int.TryParse(Convert.ToString(drv.Row["Id_Corte"]), out id);
            string desc = Convert.ToString(drv.Row["Descripcion"]);

            if (id == 0)
            {
                drv.Row.Delete();
                _tabla.AcceptChanges();
                return;
            }

            if (!GlassDialog.Pregunta(Owner, "Cortes",
                "¿Seguro que quieres eliminar el corte \"" + desc + "\"?")) return;

            string resul = new Dto.CorteDto().DeleteCorte(id);
            CargarLista();
            LblEstado.Text = resul;
        }

        // ===== Persistencia al terminar de editar =====
        private void GridDatos_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (_recargando || e.EditAction != DataGridEditAction.Commit) return;
            var drv = e.Row.Item as DataRowView;
            if (drv == null) return;
            Dispatcher.BeginInvoke(new Action(() => Persistir(drv)), DispatcherPriority.Background);
        }

        private void Persistir(DataRowView drv)
        {
            if (_recargando || drv == null || drv.Row.RowState == DataRowState.Detached) return;

            DataRow row = drv.Row;
            string desc = Convert.ToString(row["Descripcion"]).Trim();
            int id, der, izq;
            int.TryParse(Convert.ToString(row["Id_Corte"]), out id);
            int.TryParse(Convert.ToString(row["Corte_Derecho"]), out der);
            int.TryParse(Convert.ToString(row["Corte_Izquierdo"]), out izq);

            if (id == 0 && desc == "") return;   // fila nueva sin tocar

            if (desc == "")
            {
                LblEstado.Text = "La descripción es obligatoria.";
                return;
            }

            var dto = new Dto.CorteDto();
            string resul;
            if (id == 0)
            {
                // INSERT del Dto: Corte_Derecho <- corteizq, Corte_Izquierdo <- corteder.
                resul = dto.SaveCorte("0", desc, "Nuevo", corteizq: der, corteder: izq);
            }
            else
            {
                // UPDATE del Dto: Corte_Derecho <- corteder, Corte_Izquierdo <- corteizq.
                resul = dto.SaveCorte(id.ToString(), desc, "Editar", corteizq: izq, corteder: der);
            }

            CargarLista();
            LblEstado.Text = resul;
        }
    }
}
