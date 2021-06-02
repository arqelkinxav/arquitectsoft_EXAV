using System;
using System.Collections.Generic;
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
