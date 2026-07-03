using arquitectSoft.Class;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace arquitectSoft.View.Wpf.Panels
{
    /// <summary>
    /// Versión "panel" de Subcomponente para hospedarse dentro del escritorio (MdiChild):
    /// editor de UN registro (Código, Descripción, Acabado, Vidrios/Paneles) con la misma
    /// máquina de estados de botones del WinForms, alta MULTI-ACABADO, grilla de relación
    /// y reemplazo global. Sin chrome ni liquid glass: lo aporta la ventana hija.
    /// Reutiliza Dto.SubComponenteDto / Dto.AcabadoDto y los diálogos BuscarDialog / GlassDialog.
    /// </summary>
    public partial class SubComponentePanel : UserControl
    {
        private string _opc;
        private string _idSubComponente = "";
        private readonly ObservableCollection<MultiAcabado> _multi = new ObservableCollection<MultiAcabado>();

        public SubComponentePanel()
        {
            InitializeComponent();
            DgMulti.ItemsSource = _multi;
            KeyDown += Panel_KeyDown;
            Loaded += (s, e) =>
            {
                if (CmbAcabado.ItemsSource == null)
                {
                    CargarAcabados();
                    EstadoInicial();
                }
            };
        }

        // Ventana que hospeda el panel (para que los diálogos cristal tengan owner).
        private Window Owner { get { return Window.GetWindow(this); } }

        // Atajos: Ctrl+Z = cancelar, Supr = quitar acabado seleccionado de la lista multi.
        private void Panel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Z && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (BtnCancelar.IsEnabled) { Cancelar_Click(null, null); e.Handled = true; }
            }
            else if (e.Key == Key.Delete)
            {
                var sel = DgMulti.SelectedItem as MultiAcabado;
                if (sel != null) { _multi.Remove(sel); e.Handled = true; }
            }
        }

        // ===== Carga inicial =====
        private void CargarAcabados()
        {
            DataTable dt = new Dto.AcabadoDto().GetAcabado();
            CmbAcabado.ItemsSource = dt.DefaultView;
            CmbAcabado.SelectedIndex = 0;
        }

        private string AcabadoSeleccionado()
        {
            return CmbAcabado.SelectedValue != null ? CmbAcabado.SelectedValue.ToString() : "0";
        }

        // ===== Máquina de estados (igual que WinForms) =====
        private void EstadoInicial()
        {
            TxtCodigo.IsEnabled = false;
            TxtDescripcion.IsEnabled = false;
            ChkVidrios.IsEnabled = false;
            CmbAcabado.IsEnabled = false;
            MultiPanel.Visibility = Visibility.Collapsed;

            BtnGuardar.IsEnabled = false;
            BtnCancelar.IsEnabled = false;
            BtnEditar.IsEnabled = false;
            BtnEliminar.IsEnabled = false;
            BtnReemplazar.IsEnabled = false;
            BtnNuevo.IsEnabled = true;
            BtnBuscar.IsEnabled = true;
        }

        private void HabilitarNuevo(string opcion)
        {
            BtnGuardar.IsEnabled = true;
            BtnCancelar.IsEnabled = true;

            TxtCodigo.IsEnabled = opcion != "Editar";   // en edición el código no se toca
            TxtDescripcion.IsEnabled = true;
            ChkVidrios.IsEnabled = true;
            CmbAcabado.IsEnabled = true;

            BtnNuevo.IsEnabled = false;
            BtnEditar.IsEnabled = false;
            BtnEliminar.IsEnabled = false;
            BtnBuscar.IsEnabled = false;

            MultiPanel.Visibility = Visibility.Visible;
        }

        private void BloquearCancelar()
        {
            BtnCancelar.IsEnabled = false;
            BtnGuardar.IsEnabled = false;
            TxtCodigo.IsEnabled = false;
            TxtDescripcion.IsEnabled = false;
            ChkVidrios.IsEnabled = false;
            CmbAcabado.IsEnabled = false;

            BtnNuevo.IsEnabled = true;
            BtnEditar.IsEnabled = false;
            BtnEliminar.IsEnabled = false;
            BtnReemplazar.IsEnabled = false;
            BtnBuscar.IsEnabled = true;

            MultiPanel.Visibility = Visibility.Collapsed;
        }

        private void ClearComponent()
        {
            TxtCodigo.Text = "";
            TxtDescripcion.Text = "";
            ChkVidrios.IsChecked = false;
            CmbAcabado.SelectedIndex = 0;
            _idSubComponente = "";
            _multi.Clear();
            DgRelacion.ItemsSource = null;
        }

        // ===== Botones =====
        private void Nuevo_Click(object sender, RoutedEventArgs e)
        {
            _opc = "Nuevo";
            ClearComponent();
            HabilitarNuevo(null);
            LblEstado.Text = "Nuevo subcomponente. Completa los datos y guarda.";
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            ClearComponent();
            BloquearCancelar();
            LblEstado.Text = "";
        }

        private void Buscar_Click(object sender, RoutedEventArgs e)
        {
            var bsc = new BuscarDialog { Consulta = "SubComp", Owner = Owner };
            bsc.ShowDialog();
            if (bsc.ReturnItem1 == null) return;

            _idSubComponente = bsc.ReturnItem0;
            TxtCodigo.Text = bsc.ReturnItem1.Split('-')[0].Trim();
            TxtDescripcion.Text = bsc.ReturnItem2.Split('(')[0].Trim();
            int idAc;
            int.TryParse(bsc.ReturnItem3, out idAc);
            CmbAcabado.SelectedValue = idAc;
            ChkVidrios.IsChecked = bsc.ReturnItem4 == "1";

            TxtCodigo.IsEnabled = false;
            TxtDescripcion.IsEnabled = false;
            ChkVidrios.IsEnabled = false;
            CmbAcabado.IsEnabled = false;

            BtnCancelar.IsEnabled = true;
            BtnEditar.IsEnabled = true;
            BtnEliminar.IsEnabled = true;
            BtnReemplazar.IsEnabled = true;

            CargarRelacion(TxtCodigo.Text);
            LblEstado.Text = "Subcomponente cargado.";
        }

        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            if (!GlassDialog.Pregunta(Owner, "Subcomponente", "¿Seguro que quieres editar el registro?")) return;
            _opc = "Editar";
            HabilitarNuevo("Editar");
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            var dto = new Dto.SubComponenteDto();
            string fail;
            int acabado;
            int.TryParse(AcabadoSeleccionado(), out acabado);

            if (!dto.ValilidationSaveSubComponenet(TxtCodigo.Text, TxtDescripcion.Text, acabado, out fail))
            {
                GlassDialog.Informar(Owner, "Subcomponente", fail);
                return;
            }

            GuardarSubComponente(dto);
            BloquearCancelar();
        }

        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            if (!GlassDialog.Pregunta(Owner, "Subcomponente", "¿Seguro que quieres eliminar el registro?")) return;

            var dto = new Dto.SubComponenteDto();
            string codSplit = TxtCodigo.Text.Split('-')[0].Trim();
            string desSplit = TxtDescripcion.Text.Split('(')[0].Trim();
            string resul = dto.ExistSubComponent(codSplit, desSplit, AcabadoSeleccionado(), "Eliminar");

            int id;
            if (!int.TryParse(resul, out id))
            {
                GlassDialog.Informar(Owner, "Subcomponente", "No se pudo identificar el registro: " + resul);
                return;
            }

            resul = dto.DeleteComponent(id);
            BloquearCancelar();
            ClearComponent();
            GlassDialog.Informar(Owner, "Subcomponente", resul);
        }

        private void GuardarSubComponente(Dto.SubComponenteDto dto)
        {
            var dtoAcabado = new Dto.AcabadoDto();
            string resul = dto.ExistSubComponent(TxtCodigo.Text, TxtDescripcion.Text, AcabadoSeleccionado(), _opc);

            if (resul == "0" || _opc == "Editar")
            {
                bool chk = ChkVidrios.IsChecked == true;
                resul = dto.SaveSubComponent(TxtCodigo.Text, TxtDescripcion.Text, AcabadoSeleccionado(), chk, _opc, resul);

                // Multi-acabado: replica el subcomponente para cada acabado seleccionado.
                if (_multi.Count > 0)
                {
                    _opc = "Nuevo";
                    foreach (MultiAcabado m in _multi)
                    {
                        string codigoAcabado = dtoAcabado.ExistAcabado(m.Codigo, m.Descripcion);
                        resul = dto.SaveSubComponent(TxtCodigo.Text, TxtDescripcion.Text, codigoAcabado, chk, _opc, resul);
                    }
                }

                ClearComponent();
                GlassDialog.Informar(Owner, "Subcomponente", resul);
            }
            else
            {
                GlassDialog.Informar(Owner, "Subcomponente", "El código ya se encuentra registrado en el sistema.");
            }
        }

        private void CargarRelacion(string codigo)
        {
            try
            {
                DataTable dt = new Dto.SubComponenteDto().GetComponentRelation(codigo);
                DgRelacion.ItemsSource = dt != null ? dt.DefaultView : null;
            }
            catch (Exception ex)
            {
                LblEstado.Text = "No se pudo cargar la relación: " + ex.Message;
            }
        }

        // ===== Multi-acabado =====
        private void AgregarAcabados_Click(object sender, RoutedEventArgs e)
        {
            var bsc = new BuscarDialog { Consulta = "Acaba-Multi", Owner = Owner };
            bsc.ShowDialog();
            if (bsc.ReturnItem1 == null && (bsc.ArrayMultiSelect == null || bsc.ArrayMultiSelect.Rows.Count == 0))
                return;

            if (bsc.ArrayMultiSelect != null && bsc.ArrayMultiSelect.Rows.Count > 0)
            {
                foreach (DataRow r in bsc.ArrayMultiSelect.Rows)
                    _multi.Add(new MultiAcabado(r.ItemArray[1].ToString(), r.ItemArray[2].ToString()));
            }
            else
            {
                _multi.Add(new MultiAcabado(bsc.ReturnItem1, bsc.ReturnItem2));
            }
        }

        private void LimpiarMulti_Click(object sender, RoutedEventArgs e) => _multi.Clear();

        private void DgMulti_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var sel = DgMulti.SelectedItem as MultiAcabado;
            if (sel == null) return;
            if (GlassDialog.Pregunta(Owner, "Multi-Acabado", "¿Quitar este acabado de la lista?"))
                _multi.Remove(sel);
        }

        // ===== Reemplazo del subcomponente SOLO en los componentes seleccionados =====
        private void Reemplazar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_idSubComponente))
            {
                GlassDialog.Informar(Owner, "Subcomponente", "Primero busca el subcomponente a reemplazar.");
                return;
            }

            // Códigos de los componentes seleccionados en la grilla de relación (columna 0 = Código).
            var codigos = new List<string>();
            foreach (var it in DgRelacion.SelectedItems)
            {
                var drv = it as DataRowView;
                if (drv == null) continue;
                string cod = Convert.ToString(drv.Row[0]).Trim();
                if (cod != "" && !codigos.Contains(cod)) codigos.Add(cod);
            }
            if (codigos.Count == 0)
            {
                GlassDialog.Informar(Owner, "Subcomponente",
                    "Selecciona en la lista los componentes donde quieres reemplazar (Ctrl/Shift, o Ctrl+A para todos).");
                return;
            }

            // Elegir el subcomponente de destino.
            var bsc = new BuscarDialog { Consulta = "SubComp", Owner = Owner };
            bsc.ShowDialog();
            if (bsc.ReturnItem1 == null) return;

            string nuevoCod = bsc.ReturnItem1.Split('-')[0].Trim();
            string nuevoDesc = bsc.ReturnItem2.Split('(')[0].Trim();
            string msg = "¿Reemplazar el subcomponente en " + codigos.Count + " componente(s) seleccionado(s)?\n" +
                         "- De:  " + TxtCodigo.Text + " | " + TxtDescripcion.Text + "\n" +
                         "- Por: " + nuevoCod + " | " + nuevoDesc;
            if (!GlassDialog.Pregunta(Owner, "Subcomponente", msg)) return;

            // IN con los códigos seleccionados (se limpian comillas; son códigos de catálogo).
            string inList = string.Join(",", codigos.Select(c => "'" + c.Replace("'", "") + "'"));
            string filtroComp = " AND Id_Componente IN (SELECT Id_Componente FROM componentes WHERE Codigo IN (" + inList + "))";
            string filtroEsp = " AND Id_Componente_especial IN (SELECT Id_Componente FROM componentes WHERE Codigo IN (" + inList + "))";

            string fail = "";
            string[] param = { bsc.ReturnItem0, _idSubComponente };   // ? = nuevoId , ? = oldId
            var con = new Generals.Conexion();
            con.Open(out fail);
            // Detalle genérico
            con.ExecuteNonQuery(
                "UPDATE componentes_detalle SET Id_Subcomponente = ? WHERE Id_Subcomponente = ?" + filtroComp,
                out fail, param, 0);
            // Detalle especial (mismo subcomponente puede estar en componentes especiales)
            if (fail == "")
            {
                con.ExecuteNonQuery(
                    "UPDATE componentes_especial_detalle SET Id_Subcomponente = ? WHERE Id_Subcomponente = ?" + filtroEsp,
                    out fail, param, 0);
            }
            con.Close();

            if (fail == "")
            {
                GlassDialog.Informar(Owner, "Subcomponente", "Reemplazo realizado en los componentes seleccionados.");
                CargarRelacion(TxtCodigo.Text);   // refresca la relación
            }
            else
            {
                GlassDialog.Informar(Owner, "Subcomponente", fail);
            }
        }
    }
}
