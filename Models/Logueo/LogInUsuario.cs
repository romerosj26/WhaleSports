using System.Data;
using Microsoft.Data.SqlClient;
using WS_2_0.Models; // Asegúrate de que esta ruta sea correcta para tu proyecto

namespace WS_2_0.Models.Logueo
{
    public class LogInUsuario
    {
        //Valida el correo y la contraseña para iniciar sesion//
        public static Usuario Ingresar(Usuario usuario, string StringdeConexion)
        {
            try
            {
                using var conn = new SqlConnection(StringdeConexion); //Conexion a la base de datos//
                conn.Open();

                using var cmd = new SqlCommand("Val", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Correo", usuario.Correo); //Obtiene el dato ingresado en el correo//

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    string hashAlmacenado = reader["Contraseña"].ToString();
                    bool confirmado = Convert.ToBoolean(reader["EmailConfirmed"]);

                    if (PasswordHasher.VerificarContraseña(usuario.Contraseña, hashAlmacenado))
                    {
                        return new Usuario
                        {
                            id_usu = Convert.ToInt32(reader["id_usu"]),
                            Correo = reader["Correo"].ToString(),
                            EmailConfirmed = confirmado
                        };
                    }
                }
            }
            catch (SqlException ex)
            { 

            }
            return new Usuario {id_usu = 0, EmailConfirmed = false }; //Retorna un usuario nulo si no se encuentra el correo o la contraseña no es correcta//;
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