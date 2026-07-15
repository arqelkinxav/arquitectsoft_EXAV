using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace arquitectSoft.Dto
{
    /// <summary>
    /// Acceso a la tabla `usuario` para la gestión de cuentas y permisos.
    /// Los roles se guardan en la columna `rol` (ver Generals.Global.ROL_*).
    /// Las contraseñas se guardan en texto plano (igual que el resto del sistema).
    /// </summary>
    class UsuarioDto
    {
        // ===== Listado para el panel de administración =====
        public DataTable GetUsuarios()
        {
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            con.Open(out fail);
            DataTable dt = con.ExecuteDataSet(Generals.Constantes.QUERY_USUARIOS, out fail).Tables[0];
            con.Close();
            return dt;
        }

        /// <summary>True si YA existe otro usuario con ese login (excluye el id indicado; alta = 0).</summary>
        public bool LoginEnUso(string login, int idActual)
        {
            bool enUso = false;
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            try
            {
                con.Open(out fail);
                string[] param = { login, idActual.ToString() };
                MySqlDataReader row = con.ExecuteReader(Generals.Constantes.QUERY_USUARIO_LOGIN_OTRO, out fail, param);
                if (row != null && row.Read()) enUso = true;
                con.Close();
            }
            catch { try { con.Close(); } catch { } }
            return enUso;
        }

        /// <summary>Alta (id = 0) o edición de un usuario. Devuelve mensaje de resultado.</summary>
        public string GuardarUsuario(int id, string login, string clave, string nombre, int rol)
        {
            string resul = "";
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            try
            {
                con.Open(out fail);
                string sql;
                string[] param;
                string ok;
                if (id == 0)
                {
                    sql = Generals.Constantes.QUERY_INSERT_USUARIO;
                    param = new string[] { login, clave, nombre, rol.ToString() };
                    ok = "Usuario creado correctamente";
                }
                else
                {
                    sql = Generals.Constantes.QUERY_UPDATE_USUARIO;
                    param = new string[] { login, clave, nombre, rol.ToString(), id.ToString() };
                    ok = "Usuario actualizado correctamente";
                }
                con.ExecuteNonQuery(sql, out fail, param, 1);
                con.Close();
                resul = fail == "" ? ok : fail;
            }
            catch (Exception ex)
            {
                resul = ex.Message;
                try { con.Close(); } catch { }
            }
            return resul;
        }

        public string EliminarUsuario(int id)
        {
            string resul = "";
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            try
            {
                con.Open(out fail);
                string[] param = { id.ToString() };
                con.ExecuteNonQuery(Generals.Constantes.QUERY_DELETE_USUARIO, out fail, param, 0);
                con.Close();
                resul = fail == "" ? "Usuario eliminado correctamente" : fail;
            }
            catch (Exception ex)
            {
                resul = ex.Message;
                try { con.Close(); } catch { }
            }
            return resul;
        }

        /// <summary>True si la clave coincide con la almacenada para ese id.</summary>
        public bool VerificarClave(int id, string clave)
        {
            bool ok = false;
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            try
            {
                con.Open(out fail);
                string[] param = { id.ToString(), clave };
                MySqlDataReader row = con.ExecuteReader(Generals.Constantes.QUERY_VERIFICAR_CLAVE, out fail, param);
                if (row != null && row.Read()) ok = true;
                con.Close();
            }
            catch { try { con.Close(); } catch { } }
            return ok;
        }

        /// <summary>Cambia el nombre legible (no el login) del usuario indicado. Devuelve mensaje de resultado.</summary>
        public string CambiarNombre(int id, string nuevoNombre)
        {
            string resul = "";
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            try
            {
                con.Open(out fail);
                string[] param = { nuevoNombre, id.ToString() };
                con.ExecuteNonQuery(Generals.Constantes.QUERY_CAMBIAR_NOMBRE, out fail, param, 1);
                con.Close();
                resul = fail == "" ? "Nombre actualizado correctamente" : fail;
            }
            catch (Exception ex)
            {
                resul = ex.Message;
                try { con.Close(); } catch { }
            }
            return resul;
        }

        /// <summary>Cambia la clave del usuario indicado. Devuelve mensaje de resultado.</summary>
        public string CambiarClave(int id, string nuevaClave)
        {
            string resul = "";
            Generals.Conexion con = new Generals.Conexion();
            string fail = "";
            try
            {
                con.Open(out fail);
                string[] param = { nuevaClave, id.ToString() };
                con.ExecuteNonQuery(Generals.Constantes.QUERY_CAMBIAR_CLAVE, out fail, param, 1);
                con.Close();
                resul = fail == "" ? "Contraseña actualizada correctamente" : fail;
            }
            catch (Exception ex)
            {
                resul = ex.Message;
                try { con.Close(); } catch { }
            }
            return resul;
        }
    }
}
