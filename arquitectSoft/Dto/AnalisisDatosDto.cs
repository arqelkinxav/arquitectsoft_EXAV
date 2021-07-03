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


            ListColumns.ForEach(delegate (string s) {
                dt.Columns.Add(s);
            });

            ListData.ForEach(delegate (Object[] s) {
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

                        dt.Rows.Add(row.Codigo.ToString().Trim().Replace("\"", ""), "",row.Longitud.ToString().Trim().Replace("\"", ""), row.Count);
                    }
                    break;

                case 2:
                    dt = GetCalculateVidriosPaneles(dtmodel);
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
                string[] param = { row["Codigo"].ToString().Trim().Replace("\"",""),row["Altura"].ToString().Trim().Replace("\"",""), row["Anchura"].ToString().Trim().Replace("\"",""),
                                            row["Anchura2"].ToString().Trim().Replace("\"",""), row["Anchura3"].ToString().Trim().Replace("\"",""), row["Anchura4"].ToString().Trim().Replace("\"",""),
                                            row["Anchura5"].ToString().Trim().Replace("\"","") };

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
