using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace arquitectSoft.Generals
{
    public class Conexion
    {

   
        MySqlConnection conn;
        static string host = "localhost";
        static string database = "arquitectdb";
        static string userDB = "root";
        static string password = "poseidon";
        public static string strProvider = "server=" + host + ";Database=" + database + ";User ID=" + userDB + ";Password=" + password;

        public bool Open(out string fail)
        {            
            try
            {                
                conn = new MySqlConnection(strProvider);
                conn.Open();
                fail = "";
                return true;
            }
            catch (Exception er)
            {
                fail = "Conexion Error ! " + er.Message;
            }
            return false;
        }

        public void Close()
        {
            conn.Close();
            conn.Dispose();
        }

        public DataSet ExecuteDataSet(string sql,out string fail)
        {
            try
            {
                DataSet ds = new DataSet();
                MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);
                
                da.Fill(ds, "result");
                fail = "";
                return ds;
            }
            catch (Exception ex)
            {
                fail = "Conexion Error ! " + ex.Message;
            }
            return null;
        }

        public DataTable ExecuteDataSetSP(string NameSP, out string fail,string Cadena)
        {
            try
            {
                DataSet ds = new DataSet();

                MySqlCommand cmd = new MySqlCommand(NameSP, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new MySqlParameter("idComponente", Cadena));
                MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                var table = new DataTable("OrdersQuery");
                table.Load(dr);                

                fail = "";
                return table;
            }
            catch (Exception ex)
            {
                fail = "Conexion Error ! " + ex.Message;
            }
            return null;
        }

        public MySqlDataReader ExecuteReader(string sql, out string fail, string[] param)
        {
            try
            {
                MySqlDataReader reader;
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                foreach (string i in param)
                {
                    cmd.Parameters.Add(new MySqlParameter("", i));
                }
                reader = cmd.ExecuteReader();
                fail = "";
                return reader;
            }
            catch (Exception ex)
            {
                fail = "Conexion Error ! " + ex.Message;
            }
            return null;
        }

        public int ExecuteNonQuery(string sql, out string fail, string[] param)
        {
            try
            {
                int affected;               

                MySqlTransaction mytransaction = conn.BeginTransaction();
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                foreach (string i in param)
                {
                    cmd.Parameters.Add(new MySqlParameter("", i));
                }
                cmd.ExecuteNonQuery();

                affected = (int)cmd.LastInsertedId;
                mytransaction.Commit();
                fail = "";
                return affected;
            }
            catch (Exception ex)
            {
                fail = "Conexion Error ! " + ex.Message;
            }
            return -1;
        }

    }
}
