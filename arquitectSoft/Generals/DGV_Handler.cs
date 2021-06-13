using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace arquitectSoft.Generals
{
    public static class DGV_Handler
    {
        public static DataGridViewComboBoxColumn CreateUnidadCalculadaComboBox()
        {

            Dto.UnidadCalculadaDto dto = new Dto.UnidadCalculadaDto();
            DataGridViewComboBoxColumn combo = new DataGridViewComboBoxColumn();

            combo.DataSource = dto.GetUnidadCalculada();
            combo.DisplayMember = "Descripcion";
            combo.ValueMember = "Id_Unidad_Calculada";
            combo.Name = "UnidadCalculada";
            combo.HeaderText = "Unidad Calculada";
            combo.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            return combo;
        }

        public static DataGridViewComboBoxColumn CreateCorteComboBox()
        {

            Dto.CorteDto dto = new Dto.CorteDto();
            DataGridViewComboBoxColumn combo = new DataGridViewComboBoxColumn();

            combo.DataSource = dto.GetCortes();
            combo.DisplayMember = "Descripcion";
            combo.ValueMember = "Id_Corte";
            combo.Name = "Cortes";
            combo.HeaderText = "Cortes";
            combo.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            return combo;
        }

        public static DataGridViewComboBoxColumn CreateColumnasComboBox()
        {

            DataTable dto = new DataTable();
            dto.Columns.Add("Id_Columna");
            dto.Columns.Add("Descripcion");

            dto.Rows.Add("1", "Columna #1");
            dto.Rows.Add("2", "Columna #2");
            dto.Rows.Add("3", "Columna #3");
            dto.Rows.Add("4", "Columna #4");
            dto.Rows.Add("5", "Columna #5");

            dto.AcceptChanges();

            DataGridViewComboBoxColumn combo = new DataGridViewComboBoxColumn();

            combo.DataSource = dto;
            combo.DisplayMember = "Descripcion";
            combo.ValueMember = "Id_Columna";
            combo.Name = "Columna";
            combo.HeaderText = "Sel. Columna";
            combo.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            return combo;
        }

        public static DataGridViewTextBoxColumn CreateTextBox(string dataproperty, string headertext, string name, bool is_numbers)
        {
            DataGridViewTextBoxColumn textbox = new DataGridViewTextBoxColumn();
            textbox.DataPropertyName = dataproperty;
            textbox.HeaderText = headertext;
            textbox.Name = name;

            if (is_numbers)
            {
                textbox.DefaultCellStyle.Format = "0.00";
                textbox.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            return textbox;
        }

        public static DataGridViewCheckBoxColumn CreateCheckBox(string dataproperty, string headertext, string name)
        {
            DataGridViewCheckBoxColumn checkbox = new DataGridViewCheckBoxColumn();
            checkbox.DataPropertyName = dataproperty;
            checkbox.HeaderText = headertext;
            checkbox.Name = name;
            
            return checkbox;
        }
    }
}
