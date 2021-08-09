using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace arquitectSoft
{
    public partial class FrmLogin : Form
    {
    
        public FrmLogin()
        {
            InitializeComponent();
        }

        private void BtnIngresar_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void elipseControl1_Click(object sender, EventArgs e)
        {

        }

        private void BtnIniciarSesion_Click(object sender, EventArgs e)
        {
            Generals.Conexion con = new Generals.Conexion();


            string fail = "";
            string NombreDesc = "";

            if (TxtUser.Text != "" && TxtPass.Text != "")
            {

                try
                {
                    con.Open(out fail);
                    MySqlDataReader row;
                    string[] param = { TxtUser.Text, TxtPass.Text };
                    row = con.ExecuteReader(Generals.Constantes.QUERY_EXITS_USUARIO, out fail, param);
                    if (row.HasRows)
                    {
                        while (row.Read())
                        {
                            NombreDesc = row["Nombre"].ToString();

                        }
                    }
                    else
                    {
                        fail = "No se Encontro Usuario con los datos Ingresados!!!";
                    };
                    con.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    con.Close();
                }

            }

            if (fail == "" && NombreDesc != "")
            {
                Generals.Global.NameConnect = NombreDesc;

                MessageBox.Show("Bienvenido: " + NombreDesc + "!", "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Hide();
                FrmMDIPrincipal fm = new FrmMDIPrincipal();
                fm.ShowDialog();
            }
            else
            {
                MessageBox.Show(fail, "Mensaje Alerta", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {
 
        }
    }
}
