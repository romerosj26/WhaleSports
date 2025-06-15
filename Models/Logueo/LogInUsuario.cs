using System.Data;
using Microsoft.Data.SqlClient;

namespace WS_2_0.Models.Logueo
{
    public class LogInUsuario
    {
        //Valida el correo y la contraseña para iniciar sesion//
        public static Usuario Ingresar(Usuario usuario, string StringdeConexion)
        {
            try
            {
                using var conn = new SqlConnection(StringdeConexion);   //Conexion a la base de datos//
                using var cmd = new SqlCommand("Val", conn);            //Manda a llamar al procedimiento almacenado Val el cual hace la validacion del usuario//
                cmd.CommandType = CommandType.StoredProcedure;          //Indica que se va a usar un procedimiento almacenado//
                cmd.Parameters.AddWithValue("@Correo", usuario.Correo); //Obtiene el dato ingresado en el correo//

                conn.Open();
                using var reader = cmd.ExecuteReader();                 //la variable reader ejecuta el comando para leer los datos obtenidos del procedimiento almacenado Val//
                if (reader.Read())
                {
                    
                    var hash = (byte[])reader["PasswordHash"];
                    var salt = (byte[])reader["PasswordSalt"];

                    if (PasswordHasher.VerificarContraseña(usuario.Contraseña, hash, salt))
                    {
                        usuario.id_usu = Convert.ToInt32(reader["id_usu"]);
                        usuario.EmailConfirmed = Convert.ToBoolean(reader["EmailConfirmed"]);
                    }
                    else
                    {
                        usuario.id_usu = 0;
                        usuario.EmailConfirmed = false;
                    }
                }
                else
                {
                    usuario.id_usu = 0;
                    usuario.EmailConfirmed = false;
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error de SQL: {ex.Message}");
            }
            return usuario;
        }
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
                        oContacto.Fecha_Reg = Convert.ToDateTime(dr["Fecha_Reg"]);
                        oContacto.EmailConfirmed = Convert.ToBoolean(dr["EmailConfirmed"]);

                        // if (dr["FotoPerfil"] != DBNull.Value)
                        //     oContacto.FotoPerfil = (byte[])dr["FotoPerfil"];

                        // if (dr["FotoPerfilExtension"] != DBNull.Value)
                        //     oContacto.FotoPerfilExtension = dr["FotoPerfilExtension"].ToString(); 
                        //                            // oContacto.ImagenPerfil = dr["ImagenPerfil"] != DBNull.Value ? dr["ImagenPerfil"].ToString() : null;
                        // if (dr["PasswordHash"] != DBNull.Value)
                        //     oContacto.PasswordHash = (byte[])dr["PasswordHash"];

                        // if (dr["PasswordSalt"] != DBNull.Value)
                        // oContacto.PasswordSalt = (byte[])dr["PasswordSalt"];
                    }
                }
            }
            return oContacto;
        }
    }
}