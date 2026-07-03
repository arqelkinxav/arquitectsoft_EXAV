using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace arquitectSoft.View.Wpf.Panels
{
    /// <summary>
    /// Versión "panel" de Unidad de Medida para hospedarse dentro del escritorio (MdiChild).
    /// Patrón CRUD-tabla; 3 columnas: Código (Id_Unidad_Medida auto-incremental, solo
    /// lectura), Descripción y Convención. Sin chrome ni liquid glass: lo aporta la ventana
    /// hija. Reutiliza Dto.UnidadMedidaDto.
    /// </summary>
    public partial class UnidadMedidaPanel : UserControl
    {
        private DataTable _tabla;
        private DataView _vista;
        private bool _recargando;

        public UnidadMedidaPanel()
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
                DataTable dt = new Dto.UnidadMedidaDto().GetUnidadMedida();   // trae fila sintética (Id 0)
                for (int i = dt.Rows.Count - 1; i >= 0; i--)
                {
                    int id;
                    int.TryParse(Convert.ToString(dt.Rows[i]["Id_Unidad_Medida"]), out id);
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
                        + " OR Convencion LIKE '%" + partes[i] + "%'"
                        + " OR Convert(Id_Unidad_Medida, 'System.String') LIKE '%" + partes[i] + "%')";
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
                case "Id_Unidad_Medida":
                    e.Column.Header = "Código";
                    e.Column.Width = 110;
                    e.Column.IsReadOnly = true;   // auto-incremental, no se edita
                    break;
                case "Descripcion":
                    e.Column.Header = "Descripción";
                    e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                    break;
                case "Convencion":
                    e.Column.Header = "Convención";
                    e.Column.Width = 150;
                    break;
                default:
                    e.Cancel = true;
                    break;
            }
        }

        // ===== Nuevo: línea editable; el código real lo asigna la BD al guardar =====
        private void Nuevo_Click(object sender, RoutedEventArgs e)
        {
            if (_tabla == null) return;
            TxtFiltro.Text = "";

            string codigo;
            try { codigo = new Dto.UnidadMedidaDto().MaximaUnidadMedida(); }
            catch (Exception ex) { LblEstado.Text = "No se pudo generar el código: " + ex.Message; return; }

            DataRow fila = _tabla.NewRow();
            fila["Id_Unidad_Medida"] = 0;
            fila["Descripcion"] = "";
            fila["Convencion"] = "";
            _tabla.Rows.Add(fila);

            var drv = BuscarDrv(fila);
            if (drv != null)
            {
                GridDatos.SelectedItem = drv;
                GridDatos.ScrollIntoView(drv);
                if (GridDatos.Columns.Count > 1)
                {
                    GridDatos.CurrentCell = new DataGridCellInfo(drv, GridDatos.Columns[1]);
                    GridDatos.BeginEdit();
                }
            }
            LblEstado.Text = "Código " + codigo + " (estimado). Escribe la descripción y la convención; al salir de la fila se guarda.";
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
            int.TryParse(Convert.ToString(drv.Row["Id_Unidad_Medida"]), out id);
            string desc = Convert.ToString(drv.Row["Descripcion"]);

            if (id == 0)
            {
                drv.Row.Delete();
                _tabla.AcceptChanges();
                return;
            }

            if (!GlassDialog.Pregunta(Owner, "Unidades de Medida",
                "¿Seguro que quieres eliminar la unidad \"" + desc + "\"?")) return;

            string resul = new Dto.UnidadMedidaDto().DeleteUnidadMedida(id);
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
            string conv = Convert.ToString(row["Convencion"]).Trim();
            int id;
            int.TryParse(Convert.ToString(row["Id_Unidad_Medida"]), out id);

            if (id == 0 && desc == "" && conv == "") return;   // fila nueva sin tocar

            if (desc == "" || conv == "")
            {
                LblEstado.Text = "La descripción y la convención son obligatorias.";
                return;
            }

            var dto = new Dto.UnidadMedidaDto();
            string resul;
            if (id == 0)
                resul = dto.SaveUnidadMedida("0", desc, conv, "Nuevo", "0");   // la BD asigna el Id
            else
                resul = dto.SaveUnidadMedida(id.ToString(), desc, conv, "Editar", id.ToString());

            CargarLista();
            LblEstado.Text = resul;
        }
    }
}
