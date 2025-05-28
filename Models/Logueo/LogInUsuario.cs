using System.Data;
using Microsoft.Data.SqlClient;

namespace WS_2_0.Models.Logueo
{
    public class LogInUsuario
    {
        //Valida el correo y la contraseña para iniciar sesion//
        public static Usuario Ingresar(Usuario usuario, string StringdeConexion)
        {
            using (SqlConnection conn = new SqlConnection(StringdeConexion)) //Conexion a la base de datos//
            {
                SqlCommand cmd = new SqlCommand("Val", conn); //Manda a llamar al procedimiento almacenado de Validacion//
                cmd.Parameters.AddWithValue("@Correo", usuario.Correo); //Obtiene el dato ingresado en el correo//
                cmd.Parameters.AddWithValue("@Contraseña", usuario.Contraseña);//Obtiene el dato ingresado en la contraseña//
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        usuario.id_usu = Convert.ToInt32(reader["id_usu"]);
                        usuario.EmailConfirmed = Convert.ToBoolean(reader["EmailConfirmed"]);
                    }
                    else
                    {
                        usuario.id_usu = 0; //Si no se encuentra el correo, se asigna un valor de 0 al id_usu//
                        usuario.EmailConfirmed = false; //Si no se encuentra el correo, se asigna un valor de false a EmailConfirmed//
                    }
                }
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
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        //Guarda los datos obtenidos dentro de una variable//
                        oContacto.id_usu = Convert.ToInt32(dr["id_usu"]);
                        #pragma warning disable CS8601 // Possible null reference assignment.
                        oContacto.Nombre = dr["Nombre"].ToString();
                        oContacto.Apellidos = dr["Apellidos"].ToString();
                        oContacto.Correo = dr["Correo"].ToString();
                        oContacto.Telefono = dr["Telefono"].ToString();
                        oContacto.Contraseña = dr["Contraseña"].ToString();
                        oContacto.Fecha_Reg = Convert.ToDateTime(dr["Fecha_Reg"]);
                        oContacto.EmailConfirmed = Convert.ToBoolean(dr["EmailConfirmed"]);
                        // oContacto.ImagenPerfil = dr["ImagenPerfil"] != DBNull.Value ? dr["ImagenPerfil"].ToString() : null;
                    }
                }
            }
            return oContacto;
        }
    }
}