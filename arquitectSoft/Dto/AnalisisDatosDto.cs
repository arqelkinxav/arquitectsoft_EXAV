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


        public DataTable CalculateTab(int index, DataTable dtmodel)
        {
            DataTable dt = new DataTable();
            switch (index)
            {
                case 1:

                    dt.Columns.Add("Codigo");
                    dt.Columns.Add("Descripcion");
                    dt.Columns.Add("Longitud");
                    dt.Columns.Add("Cantidad");

                    var groupedData = from g in dtmodel.AsEnumerable()
                                      group g by new { cod = g.Field<string>("Codigo"), lon = g.Field<string>("Longitud") } into grp
                                      select new
                                      {
                                          Codigo = grp.Key.cod,
                                          Longitud = grp.Key.lon,
                                          Count = grp.Count()
                                      };
                    foreach (var row in groupedData)
                    {

                        dt.Rows.Add(row.Codigo.ToString().Trim().Replace("\"", ""), "", row.Longitud.ToString().Trim().Replace("\"", ""), row.Count);
                    }
                    break;

                case 2:
                    SetAuxAnchura(dtmodel);
                    List<List<object[]>> list = getSubComponenteEspecialCalc(dtmodel);
                    //dt = GetCalculateVidriosPaneles(dtmodel);

                    DataTable dtModelVidriosPaneles = new DataTable();
                    dtModelVidriosPaneles.Columns.Add("id_subcomponente");
                    dtModelVidriosPaneles.Columns.Add("Id_Unidad_Medida");
                    dtModelVidriosPaneles.Columns.Add("codigo");
                    dtModelVidriosPaneles.Columns.Add("descripcion");
                    dtModelVidriosPaneles.Columns.Add("altura");
                    dtModelVidriosPaneles.Columns.Add("anchura");
                    dtModelVidriosPaneles.Columns.Add("cantidad");
                    dtModelVidriosPaneles.Columns.Add("medida");
                    dtModelVidriosPaneles.Columns.Add("cantidadAdicional");
                    dtModelVidriosPaneles.Columns.Add("Se_Calcula_Por");
                    list.ForEach(delegate (List<object[]> list1)
                    {
                        list1.ForEach(delegate (object[] data)
                        {
                            dtModelVidriosPaneles.Rows.Add(data);
                        });
                    });

                    List<object[]> listEnd = getComponenteVidrioPanelCalc(dtModelVidriosPaneles);

                    dtModelVidriosPaneles.Rows.Clear();
                    dtModelVidriosPaneles.Columns.RemoveAt(1);
                    listEnd.ForEach(delegate (object[] data)
                    {
                        dtModelVidriosPaneles.Rows.Add(data);
                    });
                    dt = dtModelVidriosPaneles;
                    break;
            }

            return dt;
        }


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

        private List<List<object[]>> getSubComponenteEspecialCalc(DataTable dtmodel)
        {
            DataTable dt = new DataTable();
            List<List<object[]>> list = new List<List<object[]>>();

            string codigo;
            string auxAltura;

            foreach (DataRow row in dtmodel.Rows)
            {
                codigo = row["Codigo"].ToString().Replace("\"", "").Trim();
                auxAltura = row["Altura"].ToString().Replace("\"", "").Trim();


                Generals.Conexion con = new Generals.Conexion();
                string fail = "";
                string[] param = { "pCodigo-" + codigo, "pAltura-" + auxAltura };
                con.Open(out fail);
                DataTable dtResult = con.ExecuteDataSetSPparam(Generals.Constantes.QUERY_GET_CALCULATE_VIDRIOPANEL, out fail, param);
                con.Close();

                List<object[]> listDta = new List<object[]>();

                foreach (DataRow rowResult in dtResult.Rows)
                {
                    object[] data = new object[9];
                    data[0] = rowResult[0].ToString();
                    data[1] = rowResult[1].ToString();
                    data[2] = rowResult[2].ToString();
                    data[3] = rowResult[3].ToString();
                    data[4] = rowResult[4].ToString();
                    data[5] = rowResult[5].ToString();
                    data[6] = rowResult[6].ToString();
                    data[7] = rowResult[7].ToString();
                    data[8] = rowResult[8].ToString();
                    listDta.Add(data);
                }

                list.Add(listDta);

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
                                    row["Anchura"].ToString(), row["cantidad"].ToString(), row["Id_Unidad_Medida"].ToString(), 
                                    row["medida"].ToString(), row["cantidadAdicional"].ToString() };
                con.Open(out fail);
                MySqlDataReader drResult = con.ExecuteReader(Generals.Constantes.QUERY_INSERT_PROYECTO_VIDRIO_PANEL, out fail, param);
                con.Close();
            }

            fail = "";
            string[] paramGet = { };
            con.Open(out fail);
            DataTable dtResult = con.ExecuteDataSetSPparam(Generals.Constantes.QUERY_GET_PROYECTO_VIDRIO_PANEL, out fail, paramGet);
            con.Close();

            foreach (DataRow rowResult in dtResult.Rows)
            {
                object[] data = new object[9];
                data[0] = Int32.Parse(rowResult[0].ToString());
                data[1] = rowResult[1].ToString();
                data[2] = rowResult[2].ToString();
                data[3] = Int32.Parse(rowResult[3].ToString());
                data[4] = Int32.Parse(rowResult[4].ToString());
                data[5] = Int32.Parse(rowResult[5].ToString());
                data[6] = Int32.Parse(rowResult[6].ToString());
                data[7] = decimal.Parse(rowResult[7].ToString());
                data[8] = rowResult[8].ToString();

                list.Add(data);
            }

            return list;
        }

        private void SetAuxAnchura(DataTable dtmodel)
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

                string[] param = { row["Codigo"].ToString().Trim().Replace("\"",""),row["Altura"].ToString().Trim().Replace("\"",""), row["Anchura"].ToString().Trim().Replace("\"",""),
                                    Anchura2,Anchura3,Anchura4,Anchura5         };

                con.Open(out fail);
                con.ExecuteNonQuery(Generals.Constantes.QUERY_INSERT_AUXANCHURA, out fail, param, 1);
                con.Close();
            }
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







    }
}
