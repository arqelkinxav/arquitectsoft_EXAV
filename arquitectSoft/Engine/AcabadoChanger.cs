using System;
using System.Data;

namespace arquitectSoft.Engine
{
    /// <summary>
    /// Cambio de acabado sobre un resultado ya calculado, SIN tocar la base de datos.
    /// Es el port fiel de <c>FrmAnalisisDatos.FnChangeInfo</c>: recorre todas las
    /// tablas calculadas y, donde el acabado coincide con <paramref name="acabado1"/>,
    /// lo sustituye por <paramref name="acabado2"/> (cambiando tanto el sufijo del
    /// código como el texto del acabado), respetando las reglas por pestaña.
    ///
    /// En WinForms se trabajaba sobre las celdas del DataGridView por índice de
    /// columna; aquí se trabaja sobre las mismas columnas de la DataTable (mismo
    /// orden, porque el motor produce exactamente esas tablas), por lo que los
    /// índices 0/1/2/3 equivalen a Cells[0]/[1]/[2]/[3] del formulario.
    /// </summary>
    public static class AcabadoChanger
    {
        /// <summary>Aplica el cambio de acabado global a todas las tablas del resultado.</summary>
        public static void Aplicar(ResultadoAnalisis r, string acabado1, string acabado2)
        {
            if (r == null) return;

            // Mismo recorrido y numeración que FnChangeInfo (Datagrid 1..8).
            // Las pestañas 9/10 (PMV) del WinForms no existen en la ventana WPF.
            Procesar(r.PerfilMetalico,        1, acabado1, acabado2);
            Procesar(r.Puertas,               2, acabado1, acabado2);
            Procesar(r.PerfilMetalicoHerraje, 3, acabado1, acabado2);
            Procesar(r.PuertasHerraje,        4, acabado1, acabado2);
            Procesar(r.VidrioPaneles,         5, acabado1, acabado2);
            Procesar(r.PuertasCantidad,       6, acabado1, acabado2);
            Procesar(r.Tubos,                 7, acabado1, acabado2);
            Procesar(r.Mamparas,              8, acabado1, acabado2);
        }

        /// <summary>
        /// Cambia el acabado SÓLO en una fila concreta de Perfil Metálico
        /// (equivale a "Cambiar Acabado Temporal" del menú clic-derecho).
        /// </summary>
        public static void CambiarFilaPerfil(DataRow row, string acabado2)
        {
            if (row == null) return;
            string acabadocodNew = acabado2.Contains("-") ? acabado2.Split('-')[0].Trim() : "XX";
            string acabadoDescNew = acabado2.Contains("-") ? acabado2.Split('-')[1].Trim() : acabado2.Trim();

            // Cells[1] = Codigo, Cells[3] = acabado (igual que Item2_PM_Click).
            row[1] = Texto(row[1]).Split('-')[0].Trim() + "-" + acabadocodNew;
            row[3] = acabadoDescNew;
        }

        private static void Procesar(DataTable table, int datagrid, string acabado1, string acabado2)
        {
            if (table == null || table.Rows.Count == 0) return;

            foreach (DataRow row in table.Rows)
            {
                int posini = 0;
                int posfin = 0;
                string acabadoDesc = "";
                string valuezero = Texto(row[0]);
                string acabado = Texto(row[3]);

                if (valuezero == "") continue;

                // Datagrid 2 (Puertas) en filas que NO son cabecera de "Puerta":
                // sólo se ajusta el sufijo del código si coincide el código de acabado.
                if (datagrid == 2 && !FilaPuerta.EsCabecera(valuezero))
                {
                    string[] partes = Texto(row[1]).Split('-');
                    if (partes.Length > 1)
                    {
                        string codAcabado = partes[1].Trim();
                        string codAcabadoNew = acabado1.Split('-')[0].Trim();
                        if (codAcabado == codAcabadoNew)
                        {
                            string nuevoCod = acabado2.Contains("-") ? acabado2.Split('-')[0].Trim() : "XX";
                            row[1] = Texto(row[1]).Split('-')[0].Trim() + "-" + nuevoCod;
                        }
                    }
                    continue;
                }

                // Para Puertas (2) y Puertas Cantidad (6) el acabado viene embebido
                // dentro de la descripción, entre paréntesis: "DESC (ACABADO)".
                if (datagrid == 2 || datagrid == 6)
                {
                    acabadoDesc = (datagrid == 6) ? Texto(row[1]) : Texto(row[2]);
                    posini = acabadoDesc.IndexOf("(");
                    posfin = acabadoDesc.IndexOf(")");
                    if (posini < 0 || posfin < 0 || posfin <= posini) continue;
                    acabado = acabadoDesc.Substring(posini + 1, posfin - (posini + 1));
                }

                // Mamparas (8): el acabado está en Cells[2].
                if (datagrid == 8)
                {
                    acabado = Texto(row[2]);
                }

                if (!acabado1.Contains(acabado) || acabado == "") continue;

                if (((datagrid == 2 || datagrid == 4) && FilaPuerta.EsCabecera(valuezero)) || datagrid == 6)
                {
                    string acabadoFinal = acabado2.Contains("-") ? acabado2.Split('-')[1].Trim() : acabado2.Trim();
                    acabadoDesc = acabadoDesc.Substring(0, posini + 1) + acabadoFinal
                                + acabadoDesc.Substring(posfin, acabadoDesc.Length - posfin);
                    string nuevoCod = acabado2.Contains("-") ? acabado2.Split('-')[0].Trim() : "XX";

                    if (datagrid == 6)
                    {
                        row[0] = Texto(row[0]).Split('-')[0].Trim() + "-" + nuevoCod;
                        row[1] = acabadoDesc;
                    }
                    else
                    {
                        row[1] = Texto(row[1]).Split('-')[0].Trim() + "-" + nuevoCod;
                        row[2] = acabadoDesc;
                    }
                }
                else if (datagrid == 8)
                {
                    string nuevoCod = acabado2.Contains("-") ? acabado2.Split('-')[0].Trim() : "XX";
                    row[0] = Texto(row[0]).Split('-')[0].Trim() + "-" + nuevoCod;
                    row[2] = acabado2.Contains("-") ? acabado2.Split('-')[1].Trim() : acabado2.Trim();
                }
                else
                {
                    string nuevoCod = acabado2.Contains("-") ? acabado2.Split('-')[0].Trim() : "XX";
                    row[1] = Texto(row[1]).Split('-')[0].Trim() + "-" + nuevoCod;
                    row[3] = acabado2.Contains("-") ? acabado2.Split('-')[1].Trim() : acabado2.Trim();
                }
            }
        }

        private static string Texto(object cell)
        {
            return cell == null || cell == DBNull.Value ? "" : cell.ToString();
        }
    }
}
