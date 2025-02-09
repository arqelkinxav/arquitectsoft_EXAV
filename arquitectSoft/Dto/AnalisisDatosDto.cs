using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arquitectSoft.Dto
{
    class AnalisisDatosDto
    {

        public char ValidationSplit(string file)
        {
            char resul;
            using (var sr = new StreamReader(file))
            {

                int colcoma = sr.ReadLine().Split(',').Length;
                int coltab = sr.ReadLine().Split('\t').Length;

                resul = colcoma >= coltab ? ',' : '\t';
            }

            return resul;
        }

        public List<String> setCreateColumns(int index)
        {
            List<String> listColumns = new List<String>();
            switch (index)
            {

                case 1:
                    //-----------------perfil 
                    listColumns.Add("Codigo");
                    listColumns.Add("Item");
                    listColumns.Add("Longitud");
                    listColumns.Add("Ubicacion");
                    listColumns.Add("Comentario");
                    //-----------------
                    break;
                case 2:
                    //-----------------vidrios y paneles
                    listColumns.Add("Codigo");
                    listColumns.Add("Altura");
                    listColumns.Add("Anchura");
                    listColumns.Add("Anchura2");
                    listColumns.Add("Anchura3");
                    listColumns.Add("Anchura4");
                    listColumns.Add("Anchura5");
                    listColumns.Add("Ubicación");
                    //-----------------
                    break;
                case 3:
                    //-----------------Puertas
                    listColumns.Add("Codigo");
                    listColumns.Add("Apertura de Puerta");
                    listColumns.Add("Acabado Perfileria Puertas");
                    listColumns.Add("Family");
                    listColumns.Add("Item");
                    listColumns.Add("Altura");
                    listColumns.Add("Anchura");
                    listColumns.Add("Conectado/pared Tubo L1");
                    listColumns.Add("Conectado/pared Tubo L2");
                    listColumns.Add("Cantidad");
                    listColumns.Add("Ubicación");
                    listColumns.Add("Area");
                    listColumns.Add("Nomenclatura");
                    //-----------------
                    break;
                case 4:
                    //-----------------Tubos Metalicos
                    listColumns.Add("Codigo");
                    listColumns.Add("Tipo");
                    listColumns.Add("Altura");
                    listColumns.Add("Largo");
                    listColumns.Add("Count");
                    listColumns.Add("Acabado");
                    //-----------------
                    break;
                case 5:
                    //-----------------Mamparas
                    listColumns.Add("Codigo");
                    listColumns.Add("Tipo");
                    listColumns.Add("Area");
                    listColumns.Add("Ubicacion");
                    //-----------------
                    break;
                case 6:
                    //-----------------Perfiles Metalicos                   
                    listColumns.Add("id_subcomponente");
                    listColumns.Add("Id_Unidad_Medida");
                    listColumns.Add("codigo");
                    listColumns.Add("descripcion");
                    listColumns.Add("acabado");
                    listColumns.Add("cantidad");
                    listColumns.Add("medida");
                    listColumns.Add("Medidida Calculada");
                    listColumns.Add("Se_Calcula_Por");
                    listColumns.Add("Corte");
                    listColumns.Add("Ubicación");
                    //-----------------
                    break;
                case 7:
                    //-----------------vidrios y paneles                   
                    listColumns.Add("id_subcomponente");
                    listColumns.Add("codigo");
                    listColumns.Add("descripcion");
                    listColumns.Add("acabado");
                    listColumns.Add("altura");
                    listColumns.Add("anchura");
                    listColumns.Add("cantidad");
                    listColumns.Add("Ubicacion");
                    //-----------------
                    break;
                case 8:
                    //-----------------Mamparas
                    listColumns.Add("Codigo");
                    listColumns.Add("Descripción");
                    listColumns.Add("acabado");
                    listColumns.Add("Medida Calculada");
                    listColumns.Add("Cantidad Puertas");
                    listColumns.Add("Area Puertas");
                    //-----------------
                    break;
                case 9:
                    //-----------------Puertas
                    listColumns.Add("Nomenclatura");
                    listColumns.Add("Codigo");
                    listColumns.Add("Descripción");
                    listColumns.Add("cantidad");
                    listColumns.Add("Longitud (Altura)");
                    listColumns.Add("Anchura");
                    listColumns.Add("Corte");
                    listColumns.Add("Mecanizado");
                    //-----------------
                    break;
                case 10:
                    //-----------------PuertasCantidad                    
                    listColumns.Add("Codigo");
                    listColumns.Add("Descripcion");
                    listColumns.Add("Altura");
                    listColumns.Add("Anchura");
                    listColumns.Add("cantidad");
                    //-----------------
                    break;
                case 11:
                    //-----------------Puertas
                    listColumns.Add("Nomenclatura");
                    listColumns.Add("Codigo");
                    listColumns.Add("Descripción");
                    listColumns.Add("acabado");
                    listColumns.Add("cantidad");
                    listColumns.Add("Longitud (Altura)");
                    listColumns.Add("Anchura");
                    listColumns.Add("Corte");
                    listColumns.Add("Mecanizado");
                    //-----------------
                    break;
                case 12:
                    //-----------------Albaran
                    
                    listColumns.Add("codigo");
                    listColumns.Add("descripcion");
                    listColumns.Add("acabado");
                    listColumns.Add("cantidad");
                    listColumns.Add("medida");
                    listColumns.Add("categoria");
                    //-----------------
                    break;
                case 13:
                    //-----------------Puertas

                    listColumns.Add("Nomenclatura");
                    listColumns.Add("Codigo");
                    listColumns.Add("Apertura de Puerta");
                    listColumns.Add("Item");
                    listColumns.Add("Altura");
                    listColumns.Add("Anchura");
                    //-----------------
                    break;
            }

            return listColumns;
        }

        public List<Object[]> readFileTxt(string file, char delimeter)
        {
            List<Object[]> list = new List<Object[]>();
            Object[] array;

            using (var sr = new StreamReader(file))
            {
                string s = "";
                int row = 0;
                while ((s = sr.ReadLine()) != null)
                {
                    row += 1;
                    if (row > 3)
                    {
                        string[] separado = s.Split(delimeter);

                        array = new String[separado.Length];

                        Array.Copy(separado, array, separado.Length);

                        list.Add(array);
                    }
                }
                // Read the stream as a string, and write the string to the console.
                return list;
            }
        }


        public DataTable showTab(int index, List<String> ListColumns, List<Object[]> ListData)
        {
            DataTable dt = new DataTable();


            ListColumns.ForEach(delegate (string s)
            {
                dt.Columns.Add(s);
            });

            ListData.ForEach(delegate (Object[] s)
            {
                dt.Rows.Add(s);
            });

            return dt;
        }


        public DataTable CalculateTab(int index, DataTable dtmodel, DataTable dtmodelPuerta, bool VidrioPanel, Int32 MedidaBase, decimal Desperdicio, bool swmergePM,int pSwAP)
        {
            DataTable dt = new DataTable();
            switch (index)
            {
                case 1:
                case 4:
                case 8:
                    string StrCant = (index == 1 || index == 8) && !VidrioPanel ? "Longitud" : "Altura";
                    int herraje = index == 8 ? 1 : 0;

                    dt = GetDataFinal(dtmodel, StrCant, herraje, MedidaBase, Desperdicio, swmergePM);
                    break;
                case 2:
                    DataTable dtmodelDistinct = SetAuxAnchura(dtmodel);
                    List<List<object[]>> listEsp = getSubComponenteEspecialCalc(dtmodelDistinct);

                    List<string> listColumnsCompEsp = setCreateColumns(7);
                    DataTable dtModelVidriosPaneles = new DataTable();
                    listColumnsCompEsp.ForEach(delegate (string s)
                    {
                        dtModelVidriosPaneles.Columns.Add(s);
                    });

                    listEsp.ForEach(delegate (List<object[]> list1)
                    {
                        list1.ForEach(delegate (object[] data)
                        {
                            dtModelVidriosPaneles.Rows.Add(data);
                        });
                    });

                    List<object[]> listEndCompEsp = getComponenteVidrioPanelCalc(dtModelVidriosPaneles);

                    dtModelVidriosPaneles.Rows.Clear();
                    listEndCompEsp.ForEach(delegate (object[] data)
                    {
                        dtModelVidriosPaneles.Rows.Add(data);
                    });
                    dt = dtModelVidriosPaneles;
                    break;
                case 3:                    
                    dt = getComponentePuertas(dtmodel, getComponentePuertasAgrupar(dtmodel, 0, pSwAP),0, pSwAP);

                    break;
                case 5:
                    List<string> listColumnsMamp = setCreateColumns(8);
                    DataTable dtModelMampara = new DataTable();
                    listColumnsMamp.ForEach(delegate (string s)
                    {
                        dtModelMampara.Columns.Add(s);
                    });


                    List<object[]> listMampara = getComponenteMamparasCalc(getComponenteMampara(dtmodel, dtmodelPuerta));
                    listMampara.ForEach(delegate (object[] data)
                    {
                        dtModelMampara.Rows.Add(data);
                    });


                    dt = dtModelMampara;
                    break;
                case 6:
                    dt = getComponentePuertasCantidad(dtmodel);
                    break;
                case 7:
                    
                    dt = getComponentePuertas(dtmodel, getComponentePuertasAgrupar(dtmodel, 1, pSwAP),1, pSwAP);
                    break;
                
            }

            return dt;
        }

  


        #region Puertas

        public DataTable getComponentePuertas(DataTable dtmodelDoor, DataTable dtmodelDoorGroup, int SwHerraje, int pSwAP)
        {
            DataTable dtresulPuerta = new DataTable();
            List<string> listColumnsCompPuerta = setCreateColumns(SwHerraje == 0 ? 9 : 11);
            listColumnsCompPuerta.ForEach(delegate (string s)
            {
                dtresulPuerta.Columns.Add(s);
            });

            if (SwHerraje == 1)
            {
                Generals.Conexion con = new Generals.Conexion();
                string failH = "";
                string[] paramH = { "pSwHerraje|" + SwHerraje, "pSwAP|" + pSwAP };
                con.Open(out failH);
                DataTable dtResult = con.ExecuteDataSetSPparam(Generals.Constantes.QUERY_GET_CALCULATE_PUERTAS_AGRUPAR, out failH, paramH);
                con.Close();

                if (dtResult.Rows.Count > 0)
                {
                    foreach (DataRow rowP in dtResult.Rows)
                    {
                        dtresulPuerta.Rows.Add("", rowP["codigo"].ToString(), rowP["Descripcion"].ToString(), rowP["AcabadoDesc"].ToString(), rowP["cantidad"].ToString(), rowP["medidaC"].ToString().Replace(".00", ""), rowP["medidaCalculada"].ToString().Replace(".00", ""), rowP["corte"].ToString(), rowP["mecanizado"].ToString());
                    }
                }
            }
            else
            {
                List<object[]> listDta = new List<object[]>();

                string Codigo, Nomenclatura, Nomenclatura2;
                int altura, anchura;        
                int rowinitial = 0;
                foreach (DataRow rowM in dtmodelDoorGroup.Rows)
                {

                    Codigo = rowM["Codigo"].ToString();
                    if (Codigo != "")
                    {   
                        altura = rowM["Altura"].ToString().Replace("\"", "").Trim() != "" ? Int32.Parse(rowM["Altura"].ToString().Replace("\"", "").Trim()) : 0;
                        anchura = rowM["Anchura"].ToString().Replace("\"", "").Trim() != "" ? Int32.Parse(rowM["Anchura"].ToString().Replace("\"", "").Trim()) : 0;

                        var dtExtra = dtmodelDoor.AsEnumerable()
                                                 .Where(x =>
                                                 x.Field<string>("Codigo").ToString().Replace("\"", "").Trim() + 
                                                 x.Field<string>("Apertura de Puerta").ToString().Replace("\"", "").Trim() + "-" + 
                                                 x.Field<string>("Acabado Perfileria Puertas").ToString().Replace("\"", "").Trim().Split('-')[0].Trim() == Codigo && 
                                                 x.Field<string>("Altura").ToString().Replace("\"", "").Trim() == altura.ToString() &&
                                                 x.Field<string>("Anchura").ToString().Replace("\"", "").Trim() == anchura.ToString())
                                                 .Select(t => new
                                                 {
                                                     L1 = t.Field<string>("Conectado/pared Tubo L1").ToString().Replace("\"", "").Trim() == "No" ? false : true,
                                                     L2 = t.Field<string>("Conectado/pared Tubo L2").ToString().Replace("\"", "").Trim() == "No" ? false : true
                                                 })
                                                 .ToList();


                        int rowaddExtraCant = 0;
                        foreach (var x in dtExtra)
                        {
                            if (x.L1)
                                rowaddExtraCant++;

                            if (x.L2)
                                rowaddExtraCant++;
                        }

                        bool rowaddExtra = rowaddExtraCant > 0 ? true : false;
                    

                        Nomenclatura = rowM["Nomenclatura"].ToString().Replace("\"", "").Trim().Replace("P", "Puerta ");
                        Nomenclatura2 = rowM["Nomenclatura"].ToString().Replace("\"", "").Trim();

                        if (dtresulPuerta.Rows.Count > 0)
                        {
                            if (dtresulPuerta.Select("Codigo = '" + Codigo + "' and Nomenclatura = '" + Nomenclatura2 + "'").Length > 0)
                            {
                                break;
                            }
                        }

                        
                        Generals.Conexion con = new Generals.Conexion();
                        string fail = "";
                        string[] paramGeneral = { "pCodigo|" + Codigo };
                        con.Open(out fail);
                        DataTable dtResultG = con.ExecuteDataSetSPparam(Generals.Constantes.QUERY_GET_CALCULATE_PUERTAS_GENERAL, out fail, paramGeneral);
                        con.Close();

                        if (dtResultG != null)
                        {
                            if (dtResultG.Rows.Count > 0)
                            {
                                string descripcionGeneral = dtResultG.Rows[0]["Descripcion"].ToString() + "Altura: (" + altura + ") Anchura: (" + anchura + ")";

                                string[] param = { "pSwHerraje|" + SwHerraje, "pSwAP|" + pSwAP };
                                con.Open(out fail);
                                DataTable dtResult = con.ExecuteDataSetSPparam(Generals.Constantes.QUERY_GET_CALCULATE_PUERTAS_AGRUPAR, out fail, param);
                                con.Close();



                                if (dtResult.Rows.Count > 0)
                                {

                                    if (rowinitial > 0)
                                    {
                                        dtresulPuerta.Rows.Add("", "", "", "", "", "");
                                    }

                                    dtresulPuerta.Rows.Add(Nomenclatura, Codigo, descripcionGeneral, "", "", "");

                                    foreach (DataRow rowP in dtResult.Select("CodigoComponente = '" + Codigo + "' and puerta = '" + Nomenclatura2 + "'"))
                                    {

                                        string uniDes = rowP["unidaMedida"].ToString();

                                
                                        if (rowaddExtra & Int32.Parse(rowP["Extra"].ToString()) == 1)
                                        {
                                            dtresulPuerta.Rows.Add("Item-Extra (" + uniDes + ")", rowP["codigo"].ToString(), rowP["Descripcion"].ToString(), Int32.Parse(rowP["cantidad"].ToString()) * rowaddExtraCant, rowP["medidaC"].ToString().Replace(".00", ""), rowP["medidaCalculada"].ToString().Replace(".00", ""), rowP["corte"].ToString(), rowP["mecanizado"].ToString());
                                        }
                                        else if (Int32.Parse(rowP["Extra"].ToString()) == 0)
                                        {
                                            int AP = Int32.Parse(rowP["Asignacion_puertas"].ToString());
                                            string item = AP == 0 ? "Item (" + uniDes + ")" : "Item (" + uniDes + ")~";
                                            string medidaCNew = rowP["medidaC"].ToString().Replace(".00", "");
                                            string medidaCalculadaNew = rowP["medidaCalculada"].ToString().Replace(".00", "");
                                            if (pSwAP == 0 && AP == 1){ medidaCNew = "0"; medidaCalculadaNew = "0"; }
                                            dtresulPuerta.Rows.Add(item, rowP["codigo"].ToString(), rowP["Descripcion"].ToString(), rowP["cantidad"].ToString(), medidaCNew, medidaCalculadaNew, rowP["corte"].ToString(), rowP["mecanizado"].ToString());

                                        }
                                    }
                                }

                                rowinitial += 1;
                            }
                        }
                   
                    }
                }
            }
         
            Generals.Conexion condelete = new Generals.Conexion();
            string faildelete = "";
            string[] paramdelete = { };
            condelete.Open(out faildelete);
            condelete.ExecuteNonQuery("SET SQL_SAFE_UPDATES = 0; DELETE FROM proyecto_Pt;", out faildelete, paramdelete, 0);
            condelete.Close();

            return dtresulPuerta;
        }

        public DataTable getComponentePuertasAgrupar(DataTable dtmodelDoor, int SwHerraje, int pSwAP)
        {
            DataTable dtresulPuerta = new DataTable();
            DataTable dtresulPuertaFin = new DataTable();

            string Codigo, Apertura, Acabado, Nomenclatura;
            int altura, anchura;
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";

            var ListGroupby = dtmodelDoor.AsEnumerable()
               .GroupBy(t => new
               {
                   Codigo = t.Field<string>("Codigo"),
                   Apertura = t.Field<string>("Apertura de Puerta"),
                   Acabado = t.Field<string>("Acabado Perfileria Puertas"),
                   Altura = t.Field<string>("Altura"),
                   Anchura = t.Field<string>("Anchura")
               }, (key, g) => new { key, g })
               .Select(t => new
               {
                   Codigo = t.g.First().Field<string>("Codigo").ToString().Replace("\"", "").Trim() + t.g.First().Field<string>("Apertura de Puerta").ToString().Replace("\"", "").Trim() + "-" + t.g.First().Field<string>("Acabado Perfileria Puertas").ToString().Replace("\"", "").Trim().Split('-')[0].Trim(),
                   Altura = t.g.First().Field<string>("Altura").ToString().Replace("\"", "").Trim() != "" ? Int32.Parse(t.g.First().Field<string>("Altura").ToString().Replace("\"", "").Trim()) : 0,
                   Achura = t.g.First().Field<string>("Anchura").ToString().Replace("\"", "").Trim() != "" ? Int32.Parse(t.g.First().Field<string>("Anchura").ToString().Replace("\"", "").Trim()) : 0,
                   Reference = $"{string.Join(",", t.g.Select(z => z.Field<string>("Nomenclatura").ToString().Replace("\"", "").Trim()))}", //.Replace("P-", "")
               }).ToList();

            dtresulPuerta.Columns.Add("Codigo");
            dtresulPuerta.Columns.Add("Altura");
            dtresulPuerta.Columns.Add("Anchura");
            dtresulPuerta.Columns.Add("Nomenclatura");

            foreach (var rowM in ListGroupby)
            {
                dtresulPuerta.Rows.Add(rowM.Codigo,rowM.Altura,rowM.Achura,rowM.Reference);
            }



            foreach (DataRow rowM in dtmodelDoor.Rows)
            { 
                Codigo = rowM["Codigo"].ToString().Replace("\"", "").Trim();
                if (Codigo != "")
                {
                    Apertura = rowM["Apertura de Puerta"].ToString().Replace("\"", "").Trim();
                    Acabado = rowM["Acabado Perfileria Puertas"].ToString().Replace("\"", "").Trim();
                    Codigo = Codigo + Apertura + "-" + Acabado.Split('-')[0].Trim();
                    altura = rowM["Altura"].ToString().Replace("\"", "").Trim() != "" ? Int32.Parse(rowM["Altura"].ToString().Replace("\"", "").Trim()) : 0;
                    anchura = rowM["Anchura"].ToString().Replace("\"", "").Trim() != "" ? Int32.Parse(rowM["Anchura"].ToString().Replace("\"", "").Trim()) : 0;
                    
                    var nomen = ListGroupby.Where(x => x.Codigo == Codigo && x.Altura == altura && x.Achura == anchura).ToList();

                    Nomenclatura = nomen[0].Reference; //pSwAP != 1 ? nomen[0].Reference : rowM["Nomenclatura"].ToString().Replace("\"", "").Trim();

                    fail = "";
                    string[] paramGeneral = { "pCodigo|" + Codigo };
                    con.Open(out fail);
                    DataTable dtResultG = con.ExecuteDataSetSPparam(Generals.Constantes.QUERY_GET_CALCULATE_PUERTAS_GENERAL, out fail, paramGeneral);
                    con.Close();

                    if (dtResultG != null)
                    {
                        if (dtResultG.Rows.Count > 0)
                        {
                            string descripcionGeneral = dtResultG.Rows[0]["Descripcion"].ToString() + "Altura: (" + altura + ") Anchura: (" + anchura + ")";

                            string[] param = { "pCodigo|" + Codigo, "plogitud|" + altura, "pAnchura|" + anchura, "pPuerta|" + Nomenclatura, "pSwHerraje|" + SwHerraje, "pSwAP|" + pSwAP };
                            con.Open(out fail);
                            //con.ExecuteNonQuery(Generals.Constantes.QUERY_GET_CALCULATE_PUERTAS, out fail, param, 1);
                            DataTable dtResult = con.ExecuteDataSetSPparam(Generals.Constantes.QUERY_GET_CALCULATE_PUERTAS, out fail, param);
                            con.Close();
                        }
                    }

                }        
            }
            return dtresulPuerta;
        }

        public DataTable getComponentePuertasCantidad(DataTable dtmodelDoor)
        {
            DataTable dtresulPuerta = new DataTable();
            DataTable dtresulPuertaF = new DataTable();
            List<string> listColumnsCompPuerta = setCreateColumns(10);
            listColumnsCompPuerta.ForEach(delegate (string s)
            {
                dtresulPuerta.Columns.Add(s);
                dtresulPuertaF.Columns.Add(s);
            });



            List<object[]> listDta = new List<object[]>();

            string Codigo, Apertura, Acabado;
            int altura, anchura;
            foreach (DataRow rowM in dtmodelDoor.Rows)
            {
                Codigo = rowM["Codigo"].ToString().Replace("\"", "").Trim();
                if (Codigo != "")
                {
                    Apertura = rowM["Apertura de Puerta"].ToString().Replace("\"", "").Trim();
                    Acabado = rowM["Acabado Perfileria Puertas"].ToString().Replace("\"", "").Trim();
                    Codigo = Codigo + Apertura + "-" + Acabado.Split('-')[0].Trim();
                    altura = rowM["Altura"].ToString().Replace("\"", "").Trim() != "" ? Int32.Parse(rowM["Altura"].ToString().Replace("\"", "").Trim()) : 0;
                    anchura = rowM["Anchura"].ToString().Replace("\"", "").Trim() != "" ? Int32.Parse(rowM["Anchura"].ToString().Replace("\"", "").Trim()) : 0;

                    Generals.Conexion con = new Generals.Conexion();
                    string fail = "";
                    string[] paramGeneral = { "pCodigo|" + Codigo };
                    con.Open(out fail);
                    DataTable dtResultG = con.ExecuteDataSetSPparam(Generals.Constantes.QUERY_GET_CALCULATE_PUERTAS_GENERAL, out fail, paramGeneral);
                    con.Close();

                    if (dtResultG != null)
                    {
                        if (dtResultG.Rows.Count > 0)
                        {
                            string descripcionGeneral = dtResultG.Rows[0]["Descripcion"].ToString();
                            dtresulPuerta.Rows.Add(Codigo, descripcionGeneral, altura, anchura);
                        }
                    }

                }
            }

            var groupedData = from g in dtresulPuerta.AsEnumerable()
                              group g by new { cod = g.Field<string>("Codigo"), desc = g.Field<string>("Descripcion"), alt = g.Field<string>("Altura"), anc = g.Field<string>("Anchura") } into grp
                              select new
                              {
                                  Codigo = grp.Key.cod,
                                  Descripcion = grp.Key.desc,
                                  Altura = grp.Key.alt,
                                  Anchura = grp.Key.anc,
                                  Count = grp.Count()
                              };



            foreach (var row in groupedData)
            {
                string codigoF = row.Codigo.ToString();
                string descripcionF = row.Descripcion.ToString();
                string alturaF = row.Altura.ToString();
                string anchuraF = row.Anchura.ToString();
                string cantidad = row.Count.ToString();

                dtresulPuertaF.Rows.Add(codigoF, descripcionF, alturaF, anchuraF, cantidad);
            }

            return dtresulPuertaF;
        }

        public DataTable getComponentePuertasHerraje(DataTable dtmodelDoor)
        {
            DataTable dtresulPuerta = new DataTable();


            return dtresulPuerta;
        }

         #endregion

        #region Perfiles
        private string getComponenteCodigoAcabado(string pcodigo)
        {
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            string codigo = "";
            string[] param = { "pCodigo|" + pcodigo };
            con.Open(out fail);
            DataTable dtResult = con.ExecuteDataSetSPparam(Generals.Constantes.QUERY_GET_COMPONENTE_CODIGOACABADO, out fail, param);
            con.Close();
            if (dtResult != null)
            {
                codigo = dtResult.Rows.Count > 0 ? dtResult.Rows[0][0].ToString() : "";
            }
            

            return codigo;
        }

        private List<List<object[]>> getSubComponenteCalc(DataTable dtmodel, string StrCantidad, int pSwHerraje,Int32 MedidaBase)
        {
            DataTable dt = new DataTable();
            List<List<object[]>> list = new List<List<object[]>>();

            string codigo;
            string ubicacion;
            string Longitud;
            string Anchura = "";            
            float pAnchura = 0;
            float pLongitud = 0;
            float pMedidaBase = MedidaBase;
            bool anchurasw = false;

            foreach (DataColumn columns in dtmodel.Columns)
            {
                if (columns.ToString() != "Anchura")
                {
                    continue;
                }else
                {
                    anchurasw = true;
                }
            }
            
            string colUbicacionName = dtmodel.Columns
                                        .Cast<DataColumn>()
                                        .FirstOrDefault(c => c.ColumnName.Contains("Ubica"))?.ColumnName;
      

            foreach (DataRow row in dtmodel.Rows)
            {
                codigo = row["Codigo"].ToString().Replace("\"", "").Trim();
                ubicacion = row[colUbicacionName].ToString().Replace("\"", "").Trim();

                Longitud = row[StrCantidad].ToString().Replace("\"", "").Trim();
                
                if (StrCantidad == "Altura" && anchurasw)
                {
                    Anchura = row["Anchura"].ToString().Replace("\"", "").Trim();
                    pAnchura = Anchura != "" ? float.Parse(Anchura) : pLongitud;
                    pMedidaBase = pAnchura;
                }



                pLongitud = Longitud != "" ? float.Parse(Longitud) : pLongitud;

                if (!codigo.Contains('-') && codigo != "")
                {
                    codigo = getComponenteCodigoAcabado(codigo);
                }

                Generals.Conexion con = new Generals.Conexion();
                string fail = "";
                string[] param = { "pCodigo|" + codigo, "plogitud|" + pLongitud.ToString(), "pSwHerraje|" + pSwHerraje.ToString(), "pMedidaBase|" + pMedidaBase.ToString(), "pAnchura|" + pAnchura.ToString() };
                con.Open(out fail);
                DataTable dtResult = con.ExecuteDataSetSPparam(Generals.Constantes.QUERY_GET_CALCULATE_PERFILES, out fail, param);
                con.Close();

                List<object[]> listDta = new List<object[]>();

                if (dtResult != null)
                {
                    foreach (DataRow rowResult in dtResult.Rows)
                    {
                        object[] data = new object[11];
                        data[0] = rowResult[0].ToString();
                        data[1] = rowResult[1].ToString();
                        data[2] = rowResult[2].ToString();
                        data[3] = "";
                        data[4] = rowResult[3].ToString();
                        data[5] = rowResult[4].ToString();
                        data[6] = rowResult[5].ToString();
                        data[7] = rowResult[6].ToString();
                        data[9] = rowResult[7].ToString();
                        data[10] = ubicacion;
                        listDta.Add(data);
                    }

                    list.Add(listDta);
                }
                

            }


            return list;
        }

        public List<object[]> getComponenteCalc(DataTable dtmodel, decimal Desperdicio, bool swmergePM, int pSwHerraje)
        {
            List<object[]> list = new List<object[]>();
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            foreach (DataRow row in dtmodel.Rows)
            {
                fail = "";
                string[] param = { row["id_subcomponente"].ToString(), row["Id_Unidad_Medida"].ToString(),
                                    row["cantidad"].ToString(), row["medida"].ToString(),
                                    row["Medidida Calculada"].ToString(),row["Corte"].ToString(),row["Ubicación"].ToString() };
                con.Open(out fail);
                MySqlDataReader drResult = con.ExecuteReader(Generals.Constantes.QUERY_INSERT_PROYECTO, out fail, param);
                con.Close();
            }

            if (swmergePM)
            {
                fail = "";
                string[] paramGet = { "pDesperdicio|" + Desperdicio, "pSwHerraje|" + pSwHerraje.ToString() };
                con.Open(out fail);
                DataTable dtResult = con.ExecuteDataSetSPparam(Generals.Constantes.QUERY_GET_PROYECTO, out fail, paramGet);
                con.Close();

                if (dtResult != null)
                {
                    foreach (DataRow rowResult in dtResult.Rows)
                    {
                        object[] data = new object[10];
                        data[0] = Int32.Parse(rowResult[0].ToString());
                        data[1] = rowResult[1].ToString();
                        data[2] = rowResult[2].ToString();
                        data[3] = rowResult[3].ToString();
                        data[4] = float.Parse(rowResult[4].ToString());
                        data[5] = Int32.Parse(rowResult[5].ToString());
                        data[6] = float.Parse(rowResult[6].ToString());
                        data[7] = rowResult[7].ToString();
                        data[8] = rowResult[8].ToString();
                        data[9] = rowResult[9].ToString();

                        list.Add(data);
                    }
                }

            }

            return list;
        }

        public DataTable GetDataFinal(DataTable dtmodel, string Strcantidad, int pSwHerraje, Int32 MedidaBase, decimal Desperdicio, bool swmergePM)
        {
            List<List<object[]>> listComp = getSubComponenteCalc(dtmodel, Strcantidad, pSwHerraje, MedidaBase);

            List<string> listColumnsComp = setCreateColumns(6);
            DataTable dtModelPerfiles = new DataTable();
            listColumnsComp.ForEach(delegate (string s)
            {
                dtModelPerfiles.Columns.Add(s, s == "cantidad" ? typeof(float) : typeof(string));
            });

            listComp.ForEach(delegate (List<object[]> list1)
            {
                list1.ForEach(delegate (object[] data)
                {
                    dtModelPerfiles.Rows.Add(data);
                });
            });

            List<object[]> listEndComp = getComponenteCalc(dtModelPerfiles, Desperdicio, swmergePM, pSwHerraje);

            dtModelPerfiles.Rows.Clear();
            dtModelPerfiles.Columns.RemoveAt(1);
            listEndComp.ForEach(delegate (object[] data)
            {
                dtModelPerfiles.Rows.Add(data);
            });
            return dtModelPerfiles;
        }

        #endregion

        #region VidriosPanles
        private List<List<object[]>> getSubComponenteEspecialCalc(DataTable dtmodel)
        {
            DataTable dt = new DataTable();
            List<List<object[]>> list = new List<List<object[]>>();

            string codigo;
            string auxAltura;
            string Ubicacion;

            foreach (DataRow row in dtmodel.Rows)
            {
                codigo = row["Codigo"].ToString().Replace("\"", "").Split('-')[0].Trim();
                auxAltura = row["Altura"].ToString().Replace("\"", "").Trim();
                Ubicacion = row["Ubicacion"].ToString().Replace("\"", "").Trim();

                Generals.Conexion con = new Generals.Conexion();
                string fail = "";
                string[] param = { "pCodigo|" + codigo, "pAltura|" + auxAltura, "pUbicacion|" + Ubicacion };
                con.Open(out fail);
                DataTable dtResult = con.ExecuteDataSetSPparam(Generals.Constantes.QUERY_GET_CALCULATE_VIDRIOPANEL, out fail, param);
                con.Close();

                List<object[]> listDta = new List<object[]>();

                if (dtResult != null)
                {
                    foreach (DataRow rowResult in dtResult.Rows)
                    {
                        object[] data = new object[8];
                        data[0] = rowResult[0].ToString();
                        data[1] = "";
                        data[2] = "";
                        data[3] = "";
                        data[4] = rowResult[1].ToString();
                        data[5] = rowResult[2].ToString();
                        data[6] = rowResult[3].ToString();
                        data[7] = rowResult[4].ToString();

                        listDta.Add(data);
                    }
                    if (listDta.Count > 0)
                    {
                        list.Add(listDta);
                    }
                }
                


            }


            return list;
        }

        public List<object[]> getComponenteVidrioPanelCalc(DataTable dtmodel)
        {
            List<object[]> list = new List<object[]>();
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            foreach (DataRow row in dtmodel.Rows)
            {
                fail = "";
                string[] param = { row["id_subcomponente"].ToString(), row["Altura"].ToString(),
                                    row["Anchura"].ToString(), row["cantidad"].ToString(), row["Ubicacion"].ToString() };
                con.Open(out fail);
                MySqlDataReader drResult = con.ExecuteReader(Generals.Constantes.QUERY_INSERT_PROYECTO_VIDRIO_PANEL, out fail, param);
                con.Close();
            }

            fail = "";
            string[] paramGet = { };
            con.Open(out fail);
            DataTable dtResult = con.ExecuteDataSetSPparam(Generals.Constantes.QUERY_GET_PROYECTO_VIDRIO_PANEL, out fail, paramGet);
            con.Close();

            if (dtResult != null)
            {
                foreach (DataRow rowResult in dtResult.Rows)
                {
                    object[] data = new object[8];
                    data[0] = Int32.Parse(rowResult[0].ToString());
                    data[1] = rowResult[1].ToString();
                    data[2] = rowResult[2].ToString();
                    data[3] = rowResult[3].ToString();
                    data[4] = Int32.Parse(rowResult[4].ToString());
                    data[5] = Int32.Parse(rowResult[5].ToString());
                    data[6] = Int32.Parse(rowResult[6].ToString());
                    data[7] = rowResult[7].ToString();
                    list.Add(data);
                }
            }
            
            return list;
        }

        private DataTable SetAuxAnchura(DataTable dtmodel)
        {
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            string[] paramTruncate = { };
            con.Open(out fail);
            con.ExecuteNonQuery("truncate tbauxanchura;", out fail, paramTruncate, 1);
            con.Close();


            foreach (DataRow row in dtmodel.Rows)
            {
                string Anchura2 = row["Anchura2"].ToString().Trim().Replace("\"", "") == "" ? "0" : row["Anchura2"].ToString().Trim().Replace("\"", "");
                string Anchura3 = row["Anchura3"].ToString().Trim().Replace("\"", "") == "" ? "0" : row["Anchura3"].ToString().Trim().Replace("\"", "");
                string Anchura4 = row["Anchura4"].ToString().Trim().Replace("\"", "") == "" ? "0" : row["Anchura4"].ToString().Trim().Replace("\"", "");
                string Anchura5 = row["Anchura5"].ToString().Trim().Replace("\"", "") == "" ? "0" : row["Anchura5"].ToString().Trim().Replace("\"", "");
                string ubicacion = row["ubicación"].ToString().Trim().Replace("\"", "");

                string[] param = { row["Codigo"].ToString().Trim().Replace("\"","").Split('-')[0],row["Altura"].ToString().Trim().Replace("\"",""), row["Anchura"].ToString().Trim().Replace("\"",""),
                                    Anchura2,Anchura3,Anchura4,Anchura5 ,ubicacion        };

                con.Open(out fail);
                con.ExecuteNonQuery(Generals.Constantes.QUERY_INSERT_AUXANCHURA, out fail, param, 1);
                con.Close();
            }


            DataTable table = new DataTable();
            string[] paramGetAnchuraDistinct = { };
            con.Open(out fail);
            MySqlDataReader reader;
            reader = con.ExecuteReader(Generals.Constantes.QUERY_GET_AUXANCHURA, out fail, paramGetAnchuraDistinct);
            table.Load(reader);

            return table;
        }
        #endregion

        #region Mampara
        public List<object[]> getComponenteMampara(DataTable dtmodelMampara, DataTable dtmodelPuerta)
        {
            DataTable dtresul = new DataTable();
            List<object[]> listDta = new List<object[]>();
            object[] listMamp;

            float areaMampara, areaPuerta = 0;
            String UbicacionMampara, UbicacionPuerta;
            int puertasCount = dtmodelPuerta.Rows.Count;
            foreach (DataRow rowM in dtmodelMampara.Rows)
            {
                UbicacionMampara = rowM["Ubicacion"].ToString().Replace("\"", "").Trim();
                if (!"".Equals(UbicacionMampara))
                {
                    areaMampara = float.Parse(rowM["area"].ToString().Replace("\"", "").Trim());
                    if (puertasCount == 0)
                    {
                        listMamp = new Object[7];
                        listMamp[0] = rowM["Codigo"].ToString();
                        listMamp[1] = rowM["Tipo"].ToString();
                        listMamp[2] = "";
                        listMamp[3] = areaMampara;
                        listMamp[4] = UbicacionMampara;
                        listMamp[5] = puertasCount;
                        listMamp[6] = 0;
                        listDta.Add(listMamp);
                    }
                    else
                    {
                        if (dtmodelPuerta.Select("Ubicación = '" + rowM["Ubicacion"].ToString() + "'").Length > 0)
                        {
                            areaPuerta = 0;
                            foreach (DataRow rowP in dtmodelPuerta.Select("Ubicación = '" + rowM["Ubicacion"].ToString() + "'"))
                            {


                                UbicacionPuerta = rowP["Ubicación"].ToString().Replace("\"", "").Trim();
                                if (!"".Equals(UbicacionPuerta))
                                {

                                    if (UbicacionMampara == null ? UbicacionPuerta == null : UbicacionMampara.Equals(UbicacionPuerta))
                                    {
                                        areaPuerta += float.Parse(rowP["area"].ToString().Replace("\"", "").Trim());
                                    }
                                }
                            }

                            listMamp = new Object[7];
                            listMamp[0] = rowM["Codigo"].ToString();
                            listMamp[1] = rowM["Tipo"].ToString();
                            listMamp[2] = "";
                            listMamp[3] = areaMampara - areaPuerta;
                            listMamp[4] = UbicacionMampara;
                            listMamp[5] = 1;
                            listMamp[6] = areaPuerta;
                            listDta.Add(listMamp);
                        }
                        else
                        {
                            listMamp = new Object[7];
                            listMamp[0] = rowM["Codigo"].ToString();
                            listMamp[1] = rowM["Tipo"].ToString();
                            listMamp[2] = "";
                            listMamp[3] = areaMampara;
                            listMamp[4] = UbicacionMampara;
                            listMamp[5] = 0;
                            listMamp[6] = 0;
                            listDta.Add(listMamp);
                        }


                    }
                }


            }



            return listDta;
        }

        public List<object[]> getComponenteMamparasCalc(List<object[]> ltmodel)
        {
            DataTable dtdata = new DataTable();
            List<object[]> list = new List<object[]>();
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";

            ltmodel.ForEach(delegate (object[] data)
            {
                fail = "";
                string codigo = data[0].ToString().Replace("\"", "").Trim();
                string descripcion = data[1].ToString().Replace("\"", "").Trim();
                string cantidad = data[3].ToString();
                string puertas = data[5].ToString();
                string areaPuertas = data[6].ToString();

                string[] param = { codigo, descripcion, cantidad, puertas, areaPuertas };
                con.Open(out fail);
                MySqlDataReader drResult = con.ExecuteReader(Generals.Constantes.QUERY_INSERT_PROYECTO_MAMPARAS, out fail, param);
                con.Close();

            });


            fail = "";
            string[] paramGet = { };
            con.Open(out fail);
            DataTable dtResult = con.ExecuteDataSetSPparam(Generals.Constantes.QUERY_GET_CALCULATE_MAMPARAS, out fail, paramGet);
            con.Close();

            if (dtResult != null)
            {
                foreach (DataRow rowResult in dtResult.Rows)
                {
                    object[] data = new object[6];
                    data[0] = rowResult[0].ToString();
                    data[1] = rowResult[1].ToString();
                    data[2] = rowResult[2].ToString();
                    data[3] = decimal.Parse(rowResult[3].ToString());
                    data[4] = Int32.Parse(rowResult[4].ToString());
                    data[5] = decimal.Parse(rowResult[5].ToString());
                    list.Add(data);
                }
            }



            return list;
        }
        #endregion

        #region SinUsar
        private DataTable GetCalculateVidriosPaneles(DataTable dtmodel)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Codigo");
            dt.Columns.Add("Descripcion");
            dt.Columns.Add("Altura");
            dt.Columns.Add("Anchura");
            dt.Columns.Add("Cantidad");
            dt.Columns.Add("Columna");


            //Inserción de Anchuras.
            //  SetAuxAnchura(dtmodel);
            DataTable dtcolumns = GetComponentesColumnas(dtmodel);

            var groupedData = from g in dtmodel.AsEnumerable()
                              group g by new { cod = g.Field<string>("Codigo"), alt = g.Field<string>("Altura"), anc = g.Field<string>("Anchura") } into grp
                              select new
                              {
                                  Codigo = grp.Key.cod,
                                  Altura = grp.Key.alt,
                                  Anchura = grp.Key.anc,
                                  Count = grp.Count()
                              };


            foreach (var row in groupedData)
            {
                string codigo = row.Codigo.ToString().Trim().Replace("\"", "");
                string altura = row.Altura.ToString().Trim().Replace("\"", "");
                string anchura = row.Anchura.ToString().Trim().Replace("\"", "");



                var results = from myRow in dtcolumns.AsEnumerable()
                              where myRow.Field<string>("Codigo") == codigo
                              select new
                              {
                                  select_Columna = myRow.Field<int>("select_Columna"),
                                  descripcion = myRow.Field<string>("descripcion"),
                                  Columns = myRow.Field<string>("Columns")
                              };

                dt.Rows.Add(codigo, results.First().descripcion, altura, anchura, row.Count, "Columna #1");
                if (results.Count() > 1)
                {

                    foreach (var col in results)
                    {
                        var valorcol = from myRow in dtmodel.AsEnumerable()
                                       where myRow.Field<string>("Codigo") == row.Codigo & myRow.Field<string>("altura") == row.Altura
                                            & myRow.Field<string>("anchura") == row.Anchura
                                       select new
                                       {
                                           AnchuraNum = myRow.Field<string>("anchura" + col.select_Columna.ToString()).Trim().Replace("\"", "")
                                       };

                        dt.Rows.Add(codigo, col.descripcion, altura, valorcol.First().AnchuraNum, row.Count, col.Columns);
                    }
                }



            }

            return dt;
        }

        private DataTable GetComponentesColumnas(DataTable dtmodel)
        {
            DataTable dt = new DataTable();

            var groupedData = from g in dtmodel.AsEnumerable()
                              group g by new { cod = g.Field<string>("Codigo") } into grp
                              select new
                              {
                                  Codigo = grp.Key.cod,
                                  Count = grp.Count()
                              };

            foreach (var row in groupedData)
            {
                string codigo = row.Codigo.ToString().Trim().Replace("\"", "");
                string[] param = { codigo };

                Generals.Conexion con = new Generals.Conexion();
                string fail = "";
                DataTable table = new DataTable();
                con.Open(out fail);
                MySqlDataReader reader;
                reader = con.ExecuteReader(Generals.Constantes.QUERY_GET_AUXANCHURA, out fail, param);
                table.Load(reader);
                dt.Merge(table);

                con.Close();
            }



            return dt;
        }
        #endregion








    }
}
