using System.Data;
using Microsoft.Data.SqlClient;
using System;

namespace WS_2_0.Models.Logueo
{
    public class LogInUsuario
    {
        //Valida el correo y la contraseña para iniciar sesion//
        public static Usuario Ingresar(Usuario usuario, string StringdeConexion)
        {
            int id;
            using (SqlConnection conn = new SqlConnection(StringdeConexion)) //Conexion a la base de datos//
            {
                SqlCommand cmd = new SqlCommand("Val", conn); //Manda a llamar al procedimiento almacenado de Validacion//
                cmd.Parameters.AddWithValue("@Correo", usuario.Correo); //Obtiene el dato ingresado en el correo//
                cmd.Parameters.AddWithValue("@Contraseña", usuario.Contraseña);//Obtiene el dato ingresado en la contraseña//
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                cmd.ExecuteScalar();
                id = usuario.id_usu;
                usuario.id_usu = Convert.ToInt32(cmd.ExecuteScalar().ToString());
            }
            return usuario;
        }
        //Obtiene los datos del correo registrado//
        public Usuario Obtener(Usuario usuario, string StringdeConexion)
        {
            var oContacto = new Usuario();
            using (SqlConnection conn = new SqlConnection(StringdeConexion)) //Conexion a la Base de Datos//
            {
                conn.Open(); //Abre la Base de Datos//
                SqlCommand cmd = new SqlCommand("ObtenerID", conn); //Manda a llamar al procedimiento almacenado de ObtenerID//
                cmd.Parameters.AddWithValue("@id_usu", usuario.id_usu);
                cmd.CommandType = CommandType.StoredProcedure;
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        //Guarda los datos obtenidos dentro de una variable//
                        oContacto.id_usu = Convert.ToInt32(dr["id_usu"]);
                        oContacto.Nombre = dr["Nombre"].ToString();
                        oContacto.Apellidos = dr["Apellidos"].ToString();
                        oContacto.Correo = dr["Correo"].ToString();
                        oContacto.Telefono = dr["Telefono"].ToString();
                        oContacto.Contraseña = dr["Contraseña"].ToString();
                    }
                }
            }
            return oContacto;
        }
    }
}