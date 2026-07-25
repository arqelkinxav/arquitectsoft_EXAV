using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using arquitectSoft.Dto;
using ClosedXML.Excel;

namespace arquitectSoft.Engine
{
    /// <summary>
    /// Exporta un <see cref="ResultadoAnalisis"/> a Excel con el MISMO formato que
    /// <c>FrmAnalisisDatos.FnExportar</c> de WinForms. Es un port fiel: la única
    /// diferencia es que lee de los DataTable del resultado en vez de los DataGridView.
    /// </summary>
    public class ExcelExporter
    {
        /// <summary>
        /// Genera el .xlsx en <paramref name="folderPath"/> y devuelve la ruta del archivo.
        /// Lanza excepción si no se puede guardar (p. ej. archivo abierto) → la maneja la UI.
        /// </summary>
        /// <param name="res">Resultados del análisis.</param>
        /// <param name="param">[0]Numero [1]Nombre [2]Tecnico [3]Fecha [4]Acabado1 [5]Acabado2 [6]Albaran [7]Referencia (opcional).</param>
        /// <param name="folderPath">Carpeta destino.</param>
        /// <param name="swSegmentadoUbi">Estado de segmentación por ubicación ("0" = sin segmentar).</param>
        public string Exportar(ResultadoAnalisis res, string[] param, string folderPath, string swSegmentadoUbi)
        {
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // Referencia del cliente: llega en el hueco 7, añadido después que el resto.
            // Se lee defensivamente para que las pantallas antiguas, que aún mandan 7
            // elementos, sigan funcionando sin referencia.
            string referencia = (param != null && param.Length > 7) ? (param[7] ?? "").Trim() : "";
            // Nombre del proyecto tal y como debe verse impreso: con la referencia al lado.
            string nombreConRef = (param != null && param.Length > 1) ? (param[1] ?? "") : "";
            if (referencia.Length > 0) nombreConRef = (nombreConRef + " - " + referencia).Trim(' ', '-');

            string filefinish;

            using (XLWorkbook wb = new XLWorkbook())
            {
                int valueinitial = 8;
                int valuecountDoor = Filas(res.Puertas);
                int valueinitialFoot = 0;
                int PMValueFinish = 0;
                int PMHValueFinish = 0;
                string Range = string.Format("A{0}:G{0}", valueinitial);
                string Descheader = "";
                string Rangeheader = "A2:G4";
                string RangeSubheader = "A5:J6";
                string Rangetopfooter;
                string RangeSubfooter = "H5:J6";
                string rangetwo = "A{0}:G{0}";
                string sheets = "";

                string path = Directory.GetCurrentDirectory();
                var imagePath = @"\LOGO.jpg";

                DataTable tabla = new DataTable();
                for (int Datagrid = 1; Datagrid <= 9; Datagrid++)
                {
                    valueinitial = 8;
                    valueinitialFoot = 0;
                    Rangeheader = "A2:G4";
                    RangeSubheader = "A5:J6";
                    int valuesubheaderDescr = 5;
                    int valuesubheaderValue = 6;
                    bool wrapTextDefault = true;
                    switch (Datagrid)
                    {
                        case 1:
                            sheets = "PERFIL METALICO";
                            tabla = res.PerfilMetalico;
                            PMValueFinish = Filas(res.PerfilMetalico);
                            Range = string.Format("A{0}:H{0}", valueinitial);
                            Descheader = sheets;
                            break;
                        case 2:
                            if (PMValueFinish > 0)
                            {
                                valueinitial = valueinitial + PMValueFinish + 8;
                                Rangeheader = string.Format("A{0}:G{1}", valueinitial - 6, valueinitial - 4);
                                valuesubheaderDescr = valueinitial - 3;
                                valuesubheaderValue = valueinitial - 2;
                            }

                            rangetwo = "A{0}:H{0}";
                            sheets = "PERFIL METALICO";
                            Descheader = "PUERTAS";
                            RangeSubheader = string.Format("A{0}:G{1}", valuesubheaderDescr, valuesubheaderValue);
                            Range = string.Format("A{0}:H{0}", valueinitial);
                            tabla = res.Puertas;
                            break;
                        case 3:
                            sheets = "PERFIL METALICO HERRAJES";
                            tabla = res.PerfilMetalicoHerraje;
                            PMHValueFinish = Filas(res.PerfilMetalicoHerraje);
                            Range = string.Format("A{0}:H{0}", valueinitial);
                            Descheader = sheets;
                            break;
                        case 4:
                            if (PMHValueFinish > 0)
                            {
                                valueinitial = valueinitial + PMHValueFinish + 8;
                                Rangeheader = string.Format("A{0}:G{1}", valueinitial - 6, valueinitial - 4);
                                valuesubheaderDescr = valueinitial - 3;
                                valuesubheaderValue = valueinitial - 2;
                            }

                            rangetwo = "A{0}:H{0}";
                            sheets = "PERFIL METALICO HERRAJES";
                            Descheader = "PUERTAS HERRAJES";
                            Range = string.Format("A{0}:H{0}", valueinitial);
                            RangeSubheader = string.Format("A{0}:G{1}", valuesubheaderDescr, valuesubheaderValue);
                            tabla = res.PuertasHerraje;
                            break;
                        case 5:
                            Range = string.Format("A{0}:H{0}", valueinitial);
                            sheets = "VIDRIOS Y PANELES";
                            tabla = res.VidrioPaneles;
                            Descheader = sheets;
                            break;
                        case 6:
                            Range = string.Format("A{0}:E{0}", valueinitial);
                            rangetwo = "A{0}:E{0}";
                            sheets = "PUERTAS CANTIDAD";
                            Descheader = sheets;
                            tabla = res.PuertasCantidad;
                            wrapTextDefault = false;
                            break;
                        case 7:
                            Range = string.Format("A{0}:E{0}", valueinitial);
                            sheets = "TUBO METALICOS";
                            Descheader = sheets;
                            tabla = res.Tubos;
                            break;
                        case 8:
                            Range = string.Format("A{0}:H{0}", valueinitial);
                            rangetwo = "A{0}:E{0}";
                            sheets = "MAMPARAS";
                            Descheader = sheets;
                            tabla = res.Mamparas;
                            wrapTextDefault = false;
                            break;
                        case 9:
                            Range = string.Format("A{0}:H{0}", valueinitial);
                            rangetwo = "A{0}:E{0}";
                            sheets = "ALBARAN";
                            Descheader = sheets;
                            tabla = res.PerfilMetalico;
                            wrapTextDefault = false;
                            break;
                    }

                    if (Filas(tabla) > 0 && Datagrid != 9)
                    {
                        // Construye una copia "stringificada" de la tabla origen
                        // (equivalente a leer las celdas del DataGridView).
                        DataTable dt = new DataTable();
                        foreach (DataColumn column in tabla.Columns)
                            dt.Columns.Add(column.ColumnName, column.DataType);
                        foreach (DataRow fila in tabla.Rows)
                        {
                            dt.Rows.Add();
                            for (int c = 0; c < tabla.Columns.Count; c++)
                                dt.Rows[dt.Rows.Count - 1][c] = Convert.ToString(fila[c]);
                        }

                        if (Datagrid == 1)
                        {
                            if (swSegmentadoUbi == "0")
                            {
                                dt = dt.AsEnumerable()
                                    .GroupBy(r => new { Cod = r["Codigo"], med = r["medida"], cal = r["Se_Calcula_Por"] })
                                    .Select(g =>
                                    {
                                        var row = dt.NewRow();

                                        row["id_Subcomponente"] = g.Min(r => r.Field<string>("id_Subcomponente"));
                                        row["Codigo"] = g.Key.Cod;
                                        row["descripcion"] = g.Min(r => r.Field<string>("descripcion"));
                                        row["acabado"] = g.Min(r => r.Field<string>("acabado"));
                                        row["cantidad"] = g.Sum(r => r.Field<float>("cantidad"));
                                        row["medida"] = g.Key.med;
                                        row["Medidida Calculada"] = g.Min(r => r.Field<string>("Medidida Calculada"));
                                        row["Corte"] = g.Min(r => r.Field<string>("Corte"));
                                        row["Mecanizado"] = g.Min(r => r.Field<string>("Mecanizado"));
                                        row["Se_Calcula_Por"] = g.Key.cal;
                                        return row;
                                    })
                                    .CopyToDataTable();
                            }
                            else
                            {
                                dt = dt.AsEnumerable()
                                    .GroupBy(r => new { Cod = r["Codigo"], med = r["medida"], cal = r["Se_Calcula_Por"], ubi = r["Ubicación"] })
                                    .Select(g =>
                                    {
                                        var row = dt.NewRow();

                                        row["id_Subcomponente"] = g.Min(r => r.Field<string>("id_Subcomponente"));
                                        row["Codigo"] = g.Key.Cod;
                                        row["descripcion"] = g.Min(r => r.Field<string>("descripcion"));
                                        row["acabado"] = g.Min(r => r.Field<string>("acabado"));
                                        row["cantidad"] = g.Sum(r => r.Field<float>("cantidad"));
                                        row["medida"] = g.Key.med;
                                        row["Medidida Calculada"] = g.Min(r => r.Field<string>("Medidida Calculada"));
                                        row["Corte"] = g.Min(r => r.Field<string>("Corte"));
                                        row["Ubicación"] = g.Min(r => r.Field<string>("Ubicación"));
                                        row["Mecanizado"] = g.Min(r => r.Field<string>("Mecanizado"));
                                        row["Se_Calcula_Por"] = g.Key.cal;
                                        return row;
                                    })
                                    .CopyToDataTable();
                            }

                            // El GroupBy reordena: dejar SIEMPRE alfabético por descripción.
                            dt = AnalisisEngine.OrdenarPorDescripcion(dt);
                        }

                        // Sumar filas idénticas (mismo código, descripción, acabado, medida…)
                        // SOLO en los apartados de HERRAJES: Perfil Metálico Herrajes (dg3) y
                        // Puertas Herrajes (dg4). Perfil Metálico (dg1) ya se agrupa por
                        // código+medida más arriba; Vidrios/Tubos/Mamparas/Puertas se dejan
                        // tal cual estaban.
                        if (Datagrid == 3 || Datagrid == 4)
                            dt = SumarDuplicados(dt);

                        // Cantidades sin decimales, redondeadas SIEMPRE hacia arriba.
                        RedondearCantidad(dt);

                        if (Datagrid == 2)
                        {
                            if (PMValueFinish == 0)
                            {
                                DataTable dtnew = new DataTable();
                                var wsDoorOutPm = wb.Worksheets.Add(dtnew, sheets);
                                wsDoorOutPm.Name = sheets;
                                wsDoorOutPm.Row(1).InsertRowsAbove(7);
                                wb.Worksheet(sheets).AddPicture(path + imagePath)
                                  .MoveTo(150, 25)
                                  .Scale(.3);
                            }

                            var ws = wb.Worksheets.Add(dt, sheets + "Puerta");
                            string RangeSrcDoor = string.Format("A{0}:H{1}", 1, dt.Rows.Count + 1);
                            var rangeDoor = wb.Worksheet(sheets + "Puerta").Range(RangeSrcDoor);

                            var wsPM = wb.Worksheet(1);
                            string RangeDstDoor = string.Format("A{0}:H{1}", valueinitial, valueinitial + dt.Rows.Count);
                            rangeDoor.CopyTo(wb.Worksheet(sheets).Range(RangeDstDoor));

                            valueinitialFoot = valueinitial + dt.Rows.Count + 2;

                            wb.Worksheet(sheets + "Puerta").Delete();
                        }
                        else if (Datagrid == 4)
                        {
                            if (PMHValueFinish == 0)
                            {
                                DataTable dtnew = new DataTable();
                                var wsDoorOutPm = wb.Worksheets.Add(dtnew, sheets);
                                wsDoorOutPm.Name = sheets;
                                wsDoorOutPm.Row(1).InsertRowsAbove(7);
                                wb.Worksheet(sheets).AddPicture(path + imagePath)
                                  .MoveTo(150, 25)
                                  .Scale(.3);
                            }

                            var ws = wb.Worksheets.Add(dt, sheets + "PHer");
                            string RangeSrcDoor = string.Format("A{0}:H{1}", 1, dt.Rows.Count + 1);
                            var rangeDoor = wb.Worksheet(sheets + "PHer").Range(RangeSrcDoor);

                            var wsPM = wb.Worksheet(1);
                            string RangeDstDoor = string.Format("A{0}:H{1}", valueinitial, valueinitial + dt.Rows.Count);
                            rangeDoor.CopyTo(wb.Worksheet(sheets).Range(RangeDstDoor));

                            valueinitialFoot = valueinitial + dt.Rows.Count + 2;

                            wb.Worksheet(sheets + "PHer").Delete();
                        }
                        else
                        {
                            if (Datagrid == 3)
                            {
                                dt.Columns.Remove("Ubicación");
                            }

                            var ws = wb.Worksheets.Add(dt, sheets);
                            ws.Row(1).InsertRowsAbove(7);
                            wb.Worksheet(sheets).AddPicture(path + imagePath)
                              .MoveTo(150, 25)
                              .Scale(.3);

                            if (valuecountDoor == 0 || Datagrid > 4)
                            {
                                valueinitialFoot = dt.Rows.Count + 10;
                            }
                            else
                            {
                                valueinitialFoot = 0;
                            }
                        }

                        if (valueinitialFoot > 0)
                        {
                            //Diseño del footer
                            wb.Worksheet(sheets).Cell(string.Format("H{0}", 5)).Value = "VERIFICACIÓN DE DISEÑO";
                            wb.Worksheet(sheets).Cell(string.Format("I{0}", 5)).Value = "OK";
                            wb.Worksheet(sheets).Cell(string.Format("J{0}", 5)).Value = "FECHA";
                            Rangetopfooter = string.Format("H{0}:J{1}", 5, 6);
                            wb.Worksheet(sheets).Cells(Rangetopfooter).Style.Fill.BackgroundColor = XLColor.LightGray;

                            wb.Worksheet(sheets).Cell(string.Format("H{0}", valueinitialFoot)).Value = "REVISION DE FABRICACIÓN";
                            wb.Worksheet(sheets).Cell(string.Format("I{0}", valueinitialFoot)).Value = "OK";
                            wb.Worksheet(sheets).Cell(string.Format("J{0}", valueinitialFoot)).Value = "FECHA";
                            RangeSubfooter = string.Format("H{0}:J{1}", valueinitialFoot, valueinitialFoot + 1);
                            wb.Worksheet(sheets).Cells(RangeSubfooter).Style.Fill.BackgroundColor = XLColor.LightGray;
                            //Cuadricula footer
                            wb.Worksheet(sheets).Range(RangeSubfooter).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            wb.Worksheet(sheets).Range(RangeSubfooter).Style.Border.InsideBorder = XLBorderStyleValues.Dotted;
                            wb.Worksheet(sheets).Range(RangeSubfooter).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            wb.Worksheet(sheets).Range(RangeSubfooter).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            wb.Worksheet(sheets).Range(RangeSubfooter).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            wb.Worksheet(sheets).Range(RangeSubfooter).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        }

                        //Diseño Header
                        wb.Worksheet(sheets).ShowGridLines = false;

                        var range = wb.Worksheet(sheets).Range(Rangeheader);
                        range.Merge().Style.Font.SetBold().Font.FontSize = 16;
                        range.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                        range.Value = Descheader;

                        wb.Worksheet(sheets).Range(Rangeheader).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        wb.Worksheet(sheets).Range(Rangeheader).Style.Border.InsideBorder = XLBorderStyleValues.Dotted;
                        wb.Worksheet(sheets).Range(Rangeheader).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        wb.Worksheet(sheets).Range(Rangeheader).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                        //Diseño SubHeader
                        wb.Worksheet(sheets).Cell(string.Format("A{0}", valuesubheaderDescr)).Value = "Numero del proyecto:";
                        wb.Worksheet(sheets).Cell(string.Format("A{0}", valuesubheaderDescr)).Style.Font.SetBold();
                        wb.Worksheet(sheets).Cell(string.Format("A{0}", valuesubheaderValue)).Value = "Nombre del proyecto:";
                        wb.Worksheet(sheets).Cell(string.Format("A{0}", valuesubheaderValue)).Style.Font.SetBold();
                        wb.Worksheet(sheets).Cell(string.Format("C{0}", valuesubheaderDescr)).Value = "Tecnico a Cargo:";
                        wb.Worksheet(sheets).Cell(string.Format("C{0}", valuesubheaderDescr)).Style.Font.SetBold();
                        wb.Worksheet(sheets).Cell(string.Format("C{0}", valuesubheaderValue)).Value = "Fecha:";
                        wb.Worksheet(sheets).Cell(string.Format("C{0}", valuesubheaderValue)).Style.Font.SetBold();
                        wb.Worksheet(sheets).Cell(string.Format("E{0}", valuesubheaderDescr)).Value = "Acabado de Perfileria:";
                        wb.Worksheet(sheets).Cell(string.Format("E{0}", valuesubheaderDescr)).Style.Font.SetBold();
                        wb.Worksheet(sheets).Cell(string.Format("E{0}", valuesubheaderValue)).Value = "Acabado de Melamina:";
                        wb.Worksheet(sheets).Cell(string.Format("E{0}", valuesubheaderValue)).Style.Font.SetBold();

                        wb.Worksheet(sheets).Cell(string.Format("B{0}", valuesubheaderDescr)).Value = param[0];
                        wb.Worksheet(sheets).Cell(string.Format("B{0}", valuesubheaderValue)).Value = nombreConRef;
                        wb.Worksheet(sheets).Cell(string.Format("D{0}", valuesubheaderDescr)).Value = param[2];
                        wb.Worksheet(sheets).Cell(string.Format("D{0}", valuesubheaderValue)).Value = param[3];
                        wb.Worksheet(sheets).Cell(string.Format("F{0}", valuesubheaderDescr)).Value = param[4];
                        wb.Worksheet(sheets).Cell(string.Format("F{0}", valuesubheaderValue)).Value = param[5];
                        wb.Worksheet(sheets).Range(string.Format("F{0}:G{0}", valuesubheaderDescr)).Merge();
                        wb.Worksheet(sheets).Range(string.Format("F{0}:G{0}", valuesubheaderValue)).Merge();

                        wb.Worksheet(sheets).Range(RangeSubheader).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        wb.Worksheet(sheets).Range(RangeSubheader).Style.Border.InsideBorder = XLBorderStyleValues.Dotted;
                        wb.Worksheet(sheets).Range(RangeSubheader).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        wb.Worksheet(sheets).Range(RangeSubheader).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        wb.Worksheet(sheets).Range(RangeSubheader).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        wb.Worksheet(sheets).Range(RangeSubheader).Style.Border.TopBorder = XLBorderStyleValues.Thin;

                        //Set the color of Header Row.
                        wb.Worksheet(sheets).Cells(Range).Style.Fill.BackgroundColor = XLColor.DarkCoral;
                        for (int i = 1; i <= dt.Rows.Count; i++)
                        {
                            string cellRange = string.Format(rangetwo, i + valueinitial);
                            string cellIniPuertas = string.Format("A{0}", i + valueinitial);
                            string valueP = wb.Worksheet(sheets).Cell(cellIniPuertas).Value.ToString();
                            if (valueP.Contains("Puerta"))
                            {
                                wb.Worksheet(sheets).Cells(cellRange).Style.Fill.BackgroundColor = XLColor.LightGreen;
                            }
                            else
                            {
                                if (i % 2 != 0)
                                    wb.Worksheet(sheets).Cells(cellRange).Style.Fill.BackgroundColor = XLColor.White;
                                else
                                    wb.Worksheet(sheets).Cells(cellRange).Style.Fill.BackgroundColor = XLColor.LightGray;
                            }

                            if (wrapTextDefault)
                                wb.Worksheet(sheets).Cell(string.Format("C{0}", i + valueinitial)).Style.Alignment.WrapText = true;
                            else
                                wb.Worksheet(sheets).Cell(string.Format("B{0}", i + valueinitial)).Style.Alignment.WrapText = true;
                        }

                        //Adjust widths of Columns.
                        wb.Worksheet(sheets).Columns().AdjustToContents();
                        wb.Worksheet(sheets).Column(wrapTextDefault ? 3 : 2).Width = 57;

                        // Todas las celdas centradas (horizontal y vertical).
                        CentrarTodo(wb.Worksheet(sheets));
                    }
                    else if (Datagrid == 9)
                    {
                        // Albaran. Si no se seleccionó nada, se omite (en WinForms petaba).
                        if (string.IsNullOrEmpty(param[6]))
                            continue;

                        AnalisisDatosDto dto = new AnalisisDatosDto();
                        DataTable dt = new DataTable();
                        dto.setCreateColumns(12).ForEach(delegate (string s)
                        {
                            dt.Columns.Add(s, s == "cantidad" ? typeof(float) : typeof(string));
                        });

                        DataTable dt1 = new DataTable();
                        foreach (DataColumn column in dt.Columns)
                            dt1.Columns.Add(column.ColumnName, column.DataType);

                        foreach (string item in param[6].Split('|'))
                        {
                            switch (item)
                            {
                                case "0":
                                    if (res.PerfilMetalico != null)
                                        foreach (DataRow row in res.PerfilMetalico.Rows)
                                            dt1.Rows.Add(row[1], row[2], row[3], row[4], row[5], "Perfil Metalico");
                                    if (res.PerfilMetalicoHerraje != null)
                                        foreach (DataRow row in res.PerfilMetalicoHerraje.Rows)
                                            dt1.Rows.Add(row[1], row[2], row[3], row[4], row[5], "Perfil Metalico Herraje");
                                    break;
                                case "1":
                                    if (res.VidrioPaneles != null)
                                        foreach (DataRow row in res.VidrioPaneles.Rows)
                                            dt1.Rows.Add(row[1], row[2], row[3], row[6], row[4], "Vidrios y Paneles");
                                    break;
                                case "2":
                                    string acabadopuertas = "";
                                    string medidaPuertas = "";
                                    if (res.Puertas != null)
                                        foreach (DataRow row in res.Puertas.Rows)
                                        {
                                            if (Convert.ToString(row[3]) != "")
                                            {
                                                medidaPuertas = Convert.ToString(row[4]);
                                                if (Convert.ToString(row[4]) == "0")
                                                    medidaPuertas = Convert.ToString(row[5]);

                                                dt1.Rows.Add(row[1], row[2], acabadopuertas, row[3], medidaPuertas, "Puertas");
                                            }
                                            else if (Convert.ToString(row[2]) != "")
                                            {
                                                acabadopuertas = Convert.ToString(row[2]);
                                                int pos1 = acabadopuertas.IndexOf("(");
                                                int pos2 = acabadopuertas.IndexOf(")");
                                                int cantacab = pos2 - pos1;
                                                acabadopuertas = acabadopuertas.Substring(pos1 + 1, cantacab - 1);
                                            }
                                        }

                                    if (res.PuertasHerraje != null)
                                        foreach (DataRow row in res.PuertasHerraje.Rows)
                                            dt1.Rows.Add(row[1], row[2], row[3], row[4], row[5], "Puertas Herrajes");
                                    break;
                            }
                        }

                        dt1 = dt1.AsEnumerable()
                                    .GroupBy(r => new { Cod = r["Codigo"], med = r["medida"], cal = r["acabado"] })
                                    .Select(g =>
                                    {
                                        var row = dt1.NewRow();

                                        row["CODIGO"] = g.Key.Cod;
                                        row["categoria"] = g.Min(r => r.Field<string>("categoria"));
                                        row["descripcion"] = g.Min(r => r.Field<string>("descripcion"));
                                        row["cantidad"] = g.Sum(r => r.Field<float>("cantidad"));
                                        row["medida"] = g.Key.med;
                                        row["acabado"] = g.Min(r => r.Field<string>("acabado"));
                                        return row;
                                    })
                                    .CopyToDataTable();

                        // Cantidades del albarán también sin decimales (techo).
                        RedondearCantidad(dt1);

                        var ws = wb.Worksheets.Add(dt1, sheets);
                        ws.Row(1).InsertRowsAbove(18);
                        wb.Worksheet(sheets).AddPicture(path + imagePath)
                          .MoveTo(100, 25)
                          .Scale(.5);

                        // BEGIN HEADER
                        wb.Worksheet(sheets).Cell(string.Format("B{0}", 7)).Value = "SISTEMAS ARQUIMART S.L.";
                        wb.Worksheet(sheets).Cell(string.Format("B{0}", 8)).Value = "c/ Aitzgorri 6-Pol.Ind.Ansoleta";
                        wb.Worksheet(sheets).Cell(string.Format("B{0}", 9)).Value = "01006 Vitoria-Gasteiz";
                        wb.Worksheet(sheets).Cell(string.Format("B{0}", 10)).Value = "Tfno 945 29 14 89";
                        wb.Worksheet(sheets).Cell(string.Format("B{0}", 11)).Value = "CIF B01472216";

                        wb.Worksheet(sheets).Cell(string.Format("D{0}", 2)).Value = "ENTREGA EN:";
                        wb.Worksheet(sheets).Cell(string.Format("D{0}", 5)).Value = "CLIENTE:";
                        wb.Worksheet(sheets).Cell(string.Format("D{0}", 6)).Value = "REFERENCIA OBRA:";
                        wb.Worksheet(sheets).Cell(string.Format("D{0}", 9)).Value = "HORARIO ENTREGA:";
                        wb.Worksheet(sheets).Cell(string.Format("D{0}", 12)).Value = "PERSONA CONTACTO:";
                        wb.Worksheet(sheets).Cell(string.Format("D{0}", 13)).Value = "TELEFONO DE CONTACTO:";

                        wb.Worksheet(sheets).Cell(string.Format("A{0}", 15)).Value = "ALBARAN:";
                        wb.Worksheet(sheets).Cell(string.Format("C{0}", 15)).Value = "FECHA: " + param[3];

                        wb.Worksheet(sheets).Cell(string.Format("A{0}", 16)).Value = param[0] + " - " + nombreConRef;
                        wb.Worksheet(sheets).Cell(string.Format("C{0}", 16)).Value = "PEDIDO:";

                        wb.Worksheet(sheets).Cell(string.Format("A{0}", 17)).Value = "N CAJAS:";
                        wb.Worksheet(sheets).Cell(string.Format("A{0}", 18)).Value = "N PALETS:";

                        wb.Worksheet(sheets).Range(string.Format("A{0}:B{0}", 15)).Merge();
                        wb.Worksheet(sheets).Range(string.Format("A{0}:B{0}", 16)).Merge();

                        wb.Worksheet(sheets).Range(string.Format("A{0}:E{0}", 17)).Merge().Style.Font.SetBold().Font.FontSize = 16;
                        wb.Worksheet(sheets).Range(string.Format("A{0}:E{0}", 17)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                        wb.Worksheet(sheets).Range(string.Format("A{0}:E{0}", 18)).Merge().Style.Font.SetBold().Font.FontSize = 16;
                        wb.Worksheet(sheets).Range(string.Format("A{0}:E{0}", 18)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                        wb.Worksheet(sheets).Columns().AdjustToContents();

                        string Rangeheader1 = string.Format("D{0}:E{1}", 2, 7);
                        string Rangeheader2 = string.Format("D{0}:E{1}", 9, 10);
                        string Rangeheader3 = string.Format("D{0}:E{1}", 12, 13);
                        string Rangeheader4 = string.Format("A{0}:E{1}", 17, 18);
                        wb.Worksheet(sheets).Range(Rangeheader1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        wb.Worksheet(sheets).Range(Rangeheader2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        wb.Worksheet(sheets).Range(Rangeheader3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        wb.Worksheet(sheets).Range(Rangeheader4).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        wb.Worksheet(sheets).Range(Rangeheader4).Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                        wb.Worksheet(sheets).Cell(string.Format("A{0}", 15)).Style.Font.SetBold();
                        wb.Worksheet(sheets).Cell(string.Format("C{0}", 15)).Style.Font.SetBold();
                        wb.Worksheet(sheets).Cell(string.Format("C{0}", 16)).Style.Font.SetBold();
                        wb.Worksheet(sheets).Cell(string.Format("D{0}", 2)).Style.Font.SetBold();
                        wb.Worksheet(sheets).Cell(string.Format("D{0}", 5)).Style.Font.SetBold();
                        wb.Worksheet(sheets).Cell(string.Format("D{0}", 6)).Style.Font.SetBold();
                        wb.Worksheet(sheets).Cell(string.Format("D{0}", 9)).Style.Font.SetBold();
                        wb.Worksheet(sheets).Cell(string.Format("D{0}", 12)).Style.Font.SetBold();
                        wb.Worksheet(sheets).Cell(string.Format("D{0}", 13)).Style.Font.SetBold();
                        // END HEADER

                        //BEGIN FOOTER
                        int indexfooter = 21 + dt1.Rows.Count;
                        wb.Worksheet(sheets).Cell(string.Format("A{0}", indexfooter)).Value = "Transportista:";
                        wb.Worksheet(sheets).Cell(string.Format("A{0}", indexfooter + 1)).Value = "Pagador Portes:";
                        wb.Worksheet(sheets).Cell(string.Format("D{0}", indexfooter)).Value = "F:";
                        wb.Worksheet(sheets).Cell(string.Format("D{0}", indexfooter + 1)).Value = "C:";

                        wb.Worksheet(sheets).Range(string.Format("B{0}:C{0}", indexfooter)).Merge();
                        wb.Worksheet(sheets).Range(string.Format("B{0}:C{0}", indexfooter + 1)).Merge();

                        string Rangeheaderfooter = string.Format("A{0}:E{1}", indexfooter, indexfooter + 1);
                        wb.Worksheet(sheets).Range(Rangeheaderfooter).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        wb.Worksheet(sheets).Range(Rangeheaderfooter).Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                        // Cuadro "REVISADO Y CONFORME" + espacio de firma (petición de
                        // administración jul 2026: documentan la salida de fábrica por
                        // los palets golpeados en logística).
                        int indexRevisado = indexfooter + 3;
                        wb.Worksheet(sheets).Cell(string.Format("A{0}", indexRevisado)).Value = "REVISADO Y CONFORME CON EL TRANSPORTISTA";
                        wb.Worksheet(sheets).Range(string.Format("A{0}:E{0}", indexRevisado)).Merge().Style.Font.SetBold();
                        wb.Worksheet(sheets).Range(string.Format("A{0}:E{0}", indexRevisado)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                        wb.Worksheet(sheets).Cell(string.Format("A{0}", indexRevisado + 1)).Value = "Firma:";
                        wb.Worksheet(sheets).Range(string.Format("A{0}:E{1}", indexRevisado + 1, indexRevisado + 3)).Merge();
                        wb.Worksheet(sheets).Range(string.Format("A{0}:E{1}", indexRevisado + 1, indexRevisado + 3)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left).Alignment.SetVertical(XLAlignmentVerticalValues.Top);
                        for (int fila = indexRevisado + 1; fila <= indexRevisado + 3; fila++)
                            wb.Worksheet(sheets).Row(fila).Height = 22; // hueco cómodo para firmar

                        string RangeRevisado = string.Format("A{0}:E{1}", indexRevisado, indexRevisado + 3);
                        wb.Worksheet(sheets).Range(RangeRevisado).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        wb.Worksheet(sheets).Range(string.Format("A{0}:E{0}", indexRevisado)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                        wb.Worksheet(sheets).Cell(string.Format("A{0}", indexRevisado + 5)).Value = "SISTEMAS ARQUIMART S.L. c/ Aitzgorri 6 - Pol.Ind. Ansoleta 01006 Vitoria-Gasteiz";
                        wb.Worksheet(sheets).Cell(string.Format("A{0}", indexRevisado + 6)).Value = "Tfno:945 29 14 89  e-mail: arquimart@arquimart.es CIF B 01472216";

                        wb.Worksheet(sheets).Range(string.Format("A{0}:E{0}", indexRevisado + 5)).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                        wb.Worksheet(sheets).Range(string.Format("A{0}:E{0}", indexRevisado + 6)).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center).Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                        //END FOOTER

                        wb.Worksheet(sheets).ShowGridLines = false;

                        // Todas las celdas centradas (horizontal y vertical).
                        CentrarTodo(wb.Worksheet(sheets));

                        // El "Firma:" va arriba-izquierda (CentrarTodo lo centraría en
                        // mitad del hueco de firma).
                        wb.Worksheet(sheets).Range(string.Format("A{0}:E{1}", indexRevisado + 1, indexRevisado + 3))
                          .Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                          .Alignment.SetVertical(XLAlignmentVerticalValues.Top);
                    }
                }

                // Mismo formato "código nombre referencia" que el título de la ventana de análisis.
                string FileNameStr = AnalisisEngine.NombreProyecto(param[0], param[1], referencia);
                if (string.IsNullOrWhiteSpace(FileNameStr)) FileNameStr = "Presupuesto";
                foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                    FileNameStr = FileNameStr.Replace(c, '_');
                filefinish = folderPath + "\\" + FileNameStr + ".xlsx";
                GuardarXlsx(wb, filefinish);
            }

            return filefinish;
        }

        // Guarda el libro de forma robusta frente a carpetas sincronizadas (Google Drive /
        // OneDrive): ClosedXML falla al escribir DIRECTO sobre esas unidades porque el
        // cliente de sincronización bloquea el archivo (IOException). Estrategia: guardar
        // en un temporal LOCAL en C: y copiar al destino con reintentos (absorbe el bloqueo
        // momentáneo del sync). Si el destino está realmente abierto en Excel, tras los
        // reintentos relanza la IOException para que la UI avise de cerrarlo.
        private static void GuardarXlsx(XLWorkbook wb, string destino)
        {
            string temp = Path.Combine(Path.GetTempPath(),
                "arqsoft_" + Guid.NewGuid().ToString("N") + ".xlsx");
            wb.SaveAs(temp);
            try
            {
                IOException ultimo = null;
                for (int intento = 0; intento < 5; intento++)
                {
                    try { File.Copy(temp, destino, true); ultimo = null; break; }
                    catch (IOException ex) { ultimo = ex; Thread.Sleep(400); }
                }
                if (ultimo != null) throw ultimo;   // destino bloqueado de verdad (Excel abierto)
            }
            finally
            {
                try { File.Delete(temp); } catch { /* el temp se limpia solo tarde o temprano */ }
            }
        }

        private static int Filas(DataTable t)
        {
            return t == null ? 0 : t.Rows.Count;
        }

        /// <summary>
        /// Suma las filas que son idénticas en todas sus columnas EXCEPTO la cantidad
        /// (y las columnas de id, que cambian por fila). Resuelve el caso en que un mismo
        /// código/descripción/acabado aparecía repetido y separado en vez de sumado.
        /// </summary>
        private static DataTable SumarDuplicados(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return dt;

            DataColumn cant = null;
            foreach (DataColumn c in dt.Columns)
                if (string.Equals(c.ColumnName, "cantidad", StringComparison.OrdinalIgnoreCase)) { cant = c; break; }
            if (cant == null) return dt;   // sin columna de cantidad no hay nada que sumar

            var keyCols = dt.Columns.Cast<DataColumn>()
                .Where(c => c != cant && !c.ColumnName.StartsWith("id", StringComparison.OrdinalIgnoreCase))
                .ToList();

            DataTable result = dt.Clone();
            DataColumn cantRes = result.Columns[cant.ColumnName];   // columna del CLON (no la de dt)
            var map = new System.Collections.Generic.Dictionary<string, DataRow>();
            foreach (DataRow row in dt.Rows)
            {
                string key = string.Join("", keyCols.Select(c => Convert.ToString(row[c])));
                DataRow outRow;
                if (map.TryGetValue(key, out outRow))
                {
                    outRow[cantRes] = AsignarNumero(cantRes, ToDouble(outRow[cantRes]) + ToDouble(row[cant]));
                }
                else
                {
                    outRow = result.NewRow();
                    outRow.ItemArray = (object[])row.ItemArray.Clone();
                    result.Rows.Add(outRow);
                    map[key] = outRow;
                }
            }
            return result;
        }

        /// <summary>Redondea SIEMPRE hacia arriba (techo) la columna "cantidad", sin decimales.</summary>
        private static void RedondearCantidad(DataTable dt)
        {
            if (dt == null) return;
            foreach (DataColumn c in dt.Columns)
            {
                if (!string.Equals(c.ColumnName, "cantidad", StringComparison.OrdinalIgnoreCase)) continue;
                foreach (DataRow row in dt.Rows)
                {
                    if (row[c] == null || row[c] == DBNull.Value) continue;
                    row[c] = AsignarNumero(c, Math.Ceiling(ToDouble(row[c])));
                }
            }
        }

        /// <summary>Centra todas las celdas con contenido de la hoja (horizontal y vertical).</summary>
        private static void CentrarTodo(IXLWorksheet ws)
        {
            var usado = ws.RangeUsed();
            if (usado == null) return;
            usado.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                       .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
        }

        private static double ToDouble(object v)
        {
            if (v == null || v == DBNull.Value) return 0;
            double d;
            if (double.TryParse(Convert.ToString(v, System.Globalization.CultureInfo.InvariantCulture),
                    System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out d))
                return d;
            try { return Convert.ToDouble(v); } catch { return 0; }
        }

        // Devuelve el número convertido al tipo de la columna (float/decimal/int/string).
        private static object AsignarNumero(DataColumn col, double valor)
        {
            try { return Convert.ChangeType(valor, col.DataType, System.Globalization.CultureInfo.InvariantCulture); }
            catch { return valor; }
        }
    }
}
