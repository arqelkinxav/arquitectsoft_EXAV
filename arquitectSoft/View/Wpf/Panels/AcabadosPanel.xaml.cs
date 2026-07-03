using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace arquitectSoft.View.Wpf.Panels
{
    /// <summary>
    /// Versión "panel" de Acabados para hospedarse dentro del escritorio (MdiChild).
    /// Mismo comportamiento que AcabadosWindow (tabla editable, alta inline, filtro por
    /// letras en cualquier orden) pero SIN chrome de ventana ni liquid glass: de eso se
    /// encarga la ventana hija que lo contiene. Reutiliza Dto.AcabadoDto intacto.
    /// </summary>
    public partial class AcabadosPanel : UserControl
    {
        private DataTable _tabla;
        private DataView _vista;
        private bool _recargando;   // evita reentradas durante el refresco

        public AcabadosPanel()
        {
            InitializeComponent();
            Loaded += (s, e) => { if (_tabla == null) CargarLista(); };
        }

        // Ventana que hospeda el panel (para que los diálogos cristal tengan owner).
        private Window Owner { get { return Window.GetWindow(this); } }

        // ===== Carga / refresco de la tabla =====
        private void CargarLista()
        {
            _recargando = true;
            try
            {
                DataTable dt = new Dto.AcabadoDto().GetAcabado();   // trae fila sintética "(Seleccione)"
                for (int i = dt.Rows.Count - 1; i >= 0; i--)
                {
                    int id;
                    int.TryParse(Convert.ToString(dt.Rows[i]["Id_Acabado"]), out id);
                    if (id == 0) { dt.Rows[i].Delete(); continue; }   // quita la fila "(Seleccione)"

                    // GetAcabado() devuelve Descripcion = CONCAT(Codigo,' - ',Descripcion).
                    // Quitamos el prefijo "código - " para mostrar/editar SOLO la descripción
                    // real; si no, al guardar se volvería a concatenar y se duplicaría el código.
                    string codigo = Convert.ToString(dt.Rows[i]["Codigo_Homologacion"]);
                    string desc = Convert.ToString(dt.Rows[i]["Descripcion"]);
                    string prefijo = codigo + " - ";
                    if (desc.StartsWith(prefijo)) desc = desc.Substring(prefijo.Length);
                    dt.Rows[i]["Descripcion"] = desc;
                }
                dt.AcceptChanges();

                _tabla = dt;
                _vista = dt.DefaultView;
                AplicarFiltro();
                GridAcabados.ItemsSource = _vista;
            }
            catch (Exception ex)
            {
                LblEstado.Text = "No se pudo cargar la lista: " + ex.Message;
            }
            finally { _recargando = false; }
        }

        // Filtro por LETRAS en cualquier orden: cada palabra debe aparecer (en código
        // o descripción), sin importar el orden en que se escriban.
        private void AplicarFiltro()
        {
            if (_vista == null) return;
            string txt = (TxtFiltro.Text ?? "").Replace("'", "").Trim();
            if (txt == "")
            {
                _vista.RowFilter = "";
                return;
            }

            var partes = txt.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string filtro = "";
            for (int i = 0; i < partes.Length; i++)
            {
                if (i > 0) filtro += " AND ";
                filtro += "(Codigo_Homologacion LIKE '%" + partes[i] + "%' OR Descripcion LIKE '%" + partes[i] + "%')";
            }
            _vista.RowFilter = filtro;
        }

        private void Filtro_Changed(object sender, TextChangedEventArgs e)
        {
            if (_recargando) return;
            AplicarFiltro();
        }

        private void GridAcabados_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Id_Acabado":
                    e.Cancel = true;   // columna interna oculta
                    break;
                case "Codigo_Homologacion":
                    e.Column.Header = "Código";
                    e.Column.Width = 130;
                    break;
                case "Descripcion":
                    e.Column.Header = "Descripción";
                    e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                    break;
                default:
                    e.Cancel = true;
                    break;
            }
        }

        // ===== Nuevo: añade una línea editable en la propia tabla =====
        private void Nuevo_Click(object sender, RoutedEventArgs e)
        {
            if (_tabla == null) return;
            TxtFiltro.Text = "";   // limpia el filtro para que la fila nueva sea visible

            DataRow fila = _tabla.NewRow();
            fila["Id_Acabado"] = 0;            // 0 = aún no guardada
            fila["Codigo_Homologacion"] = "";
            fila["Descripcion"] = "";
            _tabla.Rows.Add(fila);

            var drv = BuscarDrv(fila);
            if (drv != null)
            {
                GridAcabados.SelectedItem = drv;
                GridAcabados.ScrollIntoView(drv);
                if (GridAcabados.Columns.Count > 0)
                {
                    GridAcabados.CurrentCell = new DataGridCellInfo(drv, GridAcabados.Columns[0]);
                    GridAcabados.BeginEdit();
                }
            }
            LblEstado.Text = "Escribe el código y la descripción; al salir de la fila se guarda.";
        }

        private DataRowView BuscarDrv(DataRow fila)
        {
            foreach (DataRowView drv in _vista)
                if (drv.Row == fila) return drv;
            return null;
        }

        // ===== Eliminar la fila seleccionada =====
        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            var drv = GridAcabados.SelectedItem as DataRowView;
            if (drv == null)
            {
                LblEstado.Text = "Selecciona primero una fila para eliminar.";
                return;
            }

            int id;
            int.TryParse(Convert.ToString(drv.Row["Id_Acabado"]), out id);
            string codigo = Convert.ToString(drv.Row["Codigo_Homologacion"]);

            if (id == 0)
            {
                // Fila nueva sin guardar: basta con quitarla.
                drv.Row.Delete();
                _tabla.AcceptChanges();
                return;
            }

            if (!GlassDialog.Pregunta(Owner, "Acabados",
                "¿Seguro que quieres eliminar el acabado \"" + codigo + "\"?")) return;

            string resul = new Dto.AcabadoDto().DeleteAcabado(id);
            CargarLista();
            LblEstado.Text = resul;
        }

        // ===== Persistencia al terminar de editar una fila =====
        private void GridAcabados_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (_recargando || e.EditAction != DataGridEditAction.Commit) return;
            var drv = e.Row.Item as DataRowView;
            if (drv == null) return;

            // Diferido para que el commit de la fila llegue a la DataRow antes de leerla.
            Dispatcher.BeginInvoke(new Action(() => Persistir(drv)), DispatcherPriority.Background);
        }

        private void Persistir(DataRowView drv)
        {
            if (_recargando || drv == null || drv.Row.RowState == DataRowState.Detached) return;

            DataRow row = drv.Row;
            string codigo = Convert.ToString(row["Codigo_Homologacion"]).Trim();
            string desc = Convert.ToString(row["Descripcion"]).Trim();
            int id;
            int.TryParse(Convert.ToString(row["Id_Acabado"]), out id);

            // Fila nueva totalmente vacía: ignorar.
            if (id == 0 && codigo == "" && desc == "") return;

            if (codigo == "" || desc == "")
            {
                LblEstado.Text = "Código y descripción son obligatorios.";
                return;
            }

            var dto = new Dto.AcabadoDto();
            string resul;
            if (id == 0)
            {
                // Alta: comprobar que no exista ya.
                string existe = dto.ExistAcabado(codigo, desc);
                if (existe != "0")
                {
                    GlassDialog.Informar(Owner, "Acabados", "Ese acabado ya existe.");
                    CargarLista();
                    return;
                }
                resul = dto.SaveAcabado(codigo, desc, "Nuevo", "0");
            }
            else
            {
                // Edición: valida que NINGÚN otro acabado use ya ese código.
                if (!dto.CodigoLibre(codigo, id.ToString()))
                {
                    GlassDialog.Informar(Owner, "Acabados", "Ya existe otro acabado con el código \"" + codigo + "\".");
                    CargarLista();
                    return;
                }

                // Código anterior (para propagar a las reglas si cambió).
                string codigoViejo = row.HasVersion(DataRowVersion.Original)
                    ? Convert.ToString(row["Codigo_Homologacion", DataRowVersion.Original]).Trim()
                    : codigo;

                // Actualiza código + descripción del MISMO registro (por Id): no rompe relaciones.
                resul = dto.SaveAcabado(codigo, desc, "Editar", id.ToString());

                // Si el código cambió, propágalo a las reglas de dependencia que lo usen.
                if (codigoViejo != codigo)
                {
                    try { new Dto.DependenciaDto().RenombrarCodigo(codigoViejo, codigo); }
                    catch { /* el acabado ya quedó guardado; las reglas se pueden ajustar aparte */ }
                }
            }

            CargarLista();
            LblEstado.Text = resul;
        }
    }
}
