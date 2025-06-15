using System.Data;
using Microsoft.Data.SqlClient;
using WS_2_0.Services;

namespace WS_2_0.Models.SignUp
{
    public class SignupUsuario
    {
        public static ResultadoRegistro Registro(Usuario usuario, string StringdeConexion, string baseUrl, EmailService _emailService)
        {
            // Genera un token único para la confirmación del correo//
            string token = Guid.NewGuid().ToString();
            string asunto = "Bienvenido a WhaleSports";
            byte[] salt = PasswordHasher.GenerateSalt();
            byte[] hash = PasswordHasher.HashPassword(usuario.Contraseña, salt);
            string confirmUrl = $"{baseUrl}/Home/ConfirmarCorreo?token={token}";
            string html = @$"
                            <!DOCTYPE html>
                                <html lang='es'>
                                    <body>
                                        <div style='width:600px;padding:20px;border:1px solid #DBDBDB;border-radius:12px;font-family:Sans-serif'>
                                            <h1 style='color:#C76F61'>¡WhaleSports Te da la bienvenida!</h1>
                                            <p style='margin-bottom:25px'>Estimado/a&nbsp;<b>{usuario.Nombre}</b>:</p>
                                            <p style='margin-bottom:25px'>Gracias por unirte a la familia de WhaleSports.</p>  
                                            <p style='margin-top:25px'><a href='{confirmUrl}' style='padding:10px 20px;background-color:#C76F61;color:white;border-radius:5px;text-decoration:none;'>Confirmar Correo</a></p>
                                        </div>
                                    </body>
                                </html>";
            try
            {
                //Conexión a la base de datos//
                using var connStr = new SqlConnection(StringdeConexion);
                connStr.Open();

                // Verificar si el correo ya está registrado//
                string checkConfirm = "SELECT EmailConfirmed, Activo FROM Usuario WHERE Correo = @Correo";
                using SqlCommand checkCmd = new SqlCommand(checkConfirm, connStr);
                checkCmd.Parameters.AddWithValue("@Correo", usuario.Correo);

                object confirmed = null;
                bool usuarioActivo = false;

                using (var reader = checkCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        confirmed = reader["EmailConfirmed"];
                        usuarioActivo = reader["Activo"] != DBNull.Value && Convert.ToBoolean(reader["Activo"]);
                    }
                }
            ;

                if (confirmed != null && confirmed != DBNull.Value)
                {
                    bool confirmado = Convert.ToBoolean(confirmed);
                    if (confirmado)
                    {
                        if (!usuarioActivo)
                        {
                            var reactivarCmd = new SqlCommand(@"
                                UPDATE Usuario 
                                SET Nombre=@Nombre,
                                    Apellidos=@Apellidos,
                                    Correo=@Correo,
                                    Telefono=@Telefono,
                                    Fecha_Reg=@Fecha_Reg,
                                    PasswordHash=@PasswordHash,
                                    PasswordSalt=@PasswordSalt, 
                                    Activo = 1 
                                WHERE Correo = @Correo", connStr);

                            usuario.Fecha_Reg = DateTime.Now;
                            reactivarCmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                            reactivarCmd.Parameters.AddWithValue("@Apellidos", usuario.Apellidos);
                            reactivarCmd.Parameters.AddWithValue("@Telefono", usuario.Telefono);
                            reactivarCmd.Parameters.AddWithValue("@Correo", usuario.Correo);
                            reactivarCmd.Parameters.Add("@PasswordHash", SqlDbType.VarBinary, hash.Length).Value = hash;
                            reactivarCmd.Parameters.Add("@PasswordSalt", SqlDbType.VarBinary, salt.Length).Value = salt;
                            reactivarCmd.Parameters.AddWithValue("@Fecha_Reg", usuario.Fecha_Reg);

                            reactivarCmd.ExecuteNonQuery();

                            // Si el registro fue exitoso, insertar el token de confirmación
                            string insertToken = "INSERT INTO EmailConfirmTokens (Token, Email, Expiration) VALUES (@Token, @Email, @Expiration)";
                            using (SqlCommand tokenCmd = new SqlCommand(insertToken, connStr))
                            {
                                tokenCmd.Parameters.AddWithValue("@Token", Guid.Parse(token));
                                tokenCmd.Parameters.AddWithValue("@Email", usuario.Correo);
                                tokenCmd.Parameters.AddWithValue("@Expiration", DateTime.Now.AddHours(1)); // Expiración del token en 1 hora
                                tokenCmd.ExecuteNonQuery();
                            }

                            _emailService.SendEmail(usuario.Correo, asunto, html);

                            return new ResultadoRegistro
                            {
                                Exito = true,
                                Mensaje = "Registrado correctamente.",
                                UsuarioRegistrado = usuario
                            };
                        }
                        else
                        {
                            return new ResultadoRegistro
                            {
                                Exito = false,
                                Mensaje = "El correo ya existe, por favor inicie sesión o ingrese otro correo.",
                                UsuarioRegistrado = null
                            };
                        }
                    }
                    else
                    {
                        //Correo ya existe, pero no está confirmado, eliminar tokens antiguos
                        string deletOldToken = "DELETE FROM EmailConfirmTokens WHERE Email = @Email";
                        using (SqlCommand deletOldTokenCmd = new SqlCommand(deletOldToken, connStr))
                        {
                            deletOldTokenCmd.Parameters.AddWithValue("@Email", usuario.Correo);
                            deletOldTokenCmd.ExecuteNonQuery();
                        }

                        var reconfiCmd = new SqlCommand(@"
                                UPDATE Usuario 
                                SET Nombre=@Nombre,
                                    Apellidos=@Apellidos,
                                    Correo=@Correo,
                                    Telefono=@Telefono,
                                    Fecha_Reg=@Fecha_Reg,
                                    PasswordHash=@PasswordHash,
                                    PasswordSalt=@PasswordSalt, 
                                    Activo = 1 
                                WHERE Correo = @Correo", connStr);

                            usuario.Fecha_Reg = DateTime.Now;
                            reconfiCmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                            reconfiCmd.Parameters.AddWithValue("@Apellidos", usuario.Apellidos);
                            reconfiCmd.Parameters.AddWithValue("@Telefono", usuario.Telefono);
                            reconfiCmd.Parameters.AddWithValue("@Correo", usuario.Correo);
                            reconfiCmd.Parameters.Add("@PasswordHash", SqlDbType.VarBinary, hash.Length).Value = hash;
                            reconfiCmd.Parameters.Add("@PasswordSalt", SqlDbType.VarBinary, salt.Length).Value = salt;
                            reconfiCmd.Parameters.AddWithValue("@Fecha_Reg", usuario.Fecha_Reg);

                            reconfiCmd.ExecuteNonQuery();

                            // Si el registro fue exitoso, insertar el token de confirmación
                            string insertToken = "INSERT INTO EmailConfirmTokens (Token, Email, Expiration) VALUES (@Token, @Email, @Expiration)";
                            using (SqlCommand tokenCmd = new SqlCommand(insertToken, connStr))
                            {
                                tokenCmd.Parameters.AddWithValue("@Token", Guid.Parse(token));
                                tokenCmd.Parameters.AddWithValue("@Email", usuario.Correo);
                                tokenCmd.Parameters.AddWithValue("@Expiration", DateTime.Now.AddHours(1)); // Expiración del token en 1 hora
                                tokenCmd.ExecuteNonQuery();
                            }

                            _emailService.SendEmail(usuario.Correo, asunto, html);

                            return new ResultadoRegistro
                            {
                                Exito = true,
                                Mensaje = "Registrado correctamente.",
                                UsuarioRegistrado = usuario
                            };
                    }
                }

                //Registra al usuario si no existe
                SqlCommand cmd = new SqlCommand("RegistroUsuario", connStr);
                cmd.CommandType = CommandType.StoredProcedure;
                usuario.Fecha_Reg = DateTime.Now;
                cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                cmd.Parameters.AddWithValue("@Apellidos", usuario.Apellidos);
                cmd.Parameters.AddWithValue("@Correo", usuario.Correo);
                cmd.Parameters.AddWithValue("@Telefono", usuario.Telefono);
                cmd.Parameters.Add("@PasswordHash", SqlDbType.VarBinary, hash.Length).Value = hash;
                cmd.Parameters.Add("@PasswordSalt", SqlDbType.VarBinary, salt.Length).Value = salt;
                cmd.Parameters.AddWithValue("@Fecha_Reg", usuario.Fecha_Reg);
                cmd.Parameters.Add("registro", SqlDbType.Bit).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                bool registro = Convert.ToBoolean(cmd.Parameters["registro"].Value);

                if (registro)
                {
                    // Si el registro fue exitoso, insertar el token de confirmación
                    string insertToken = "INSERT INTO EmailConfirmTokens (Token, Email, Expiration) VALUES (@Token, @Email, @Expiration)";
                    using (SqlCommand tokenCmd = new SqlCommand(insertToken, connStr))
                    {
                        tokenCmd.Parameters.AddWithValue("@Token", Guid.Parse(token));
                        tokenCmd.Parameters.AddWithValue("@Email", usuario.Correo);
                        tokenCmd.Parameters.AddWithValue("@Expiration", DateTime.Now.AddHours(1)); // Expiración del token en 1 hora
                        tokenCmd.ExecuteNonQuery();
                    }


                    _emailService.SendEmail(usuario.Correo, asunto, html);

                    return new ResultadoRegistro
                    {
                        Exito = true,
                        Mensaje = "Registrado correctamente.",
                        UsuarioRegistrado = usuario
                    };
                }
                return new ResultadoRegistro
                {
                    Exito = false,
                    Mensaje = "Error al enviar el correo electrónico",
                    UsuarioRegistrado = null
                };
            } catch (Exception ex) {
                throw;
            }
        }
    }
}