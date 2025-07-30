using System;
using System.Data;
using System.Transactions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;

namespace WS_2_0.Models
{
    public class crudClientes
    {
        public List<Usuario> Tabla(string StringdeConexion)
        {
            var oTabla = new List<Usuario>();
            using (SqlConnection connStr = new SqlConnection(StringdeConexion))
            {
                using (SqlCommand cmd = new SqlCommand("MostrarCliente", connStr))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    connStr.Open();

                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        oTabla.Add(new Usuario
                        {
                            id_usu = (int)dr["id_usu"],
                            Nombre = dr["Nombre"].ToString(),
                            Apellidos = dr["Apellidos"].ToString(),
                            Correo = dr["Correo"].ToString(),
                            Telefono = dr["Telefono"].ToString(),
                            Fecha_Reg = (DateTime)dr["Fecha_Reg"],
                            Contraseña = dr["PasswordHash"].ToString(),
                            EmailConfirmed = (bool)dr["EmailConfirmed"],
                            Activo = (bool)dr["Activo"]
                        });
                    }
                }
                return oTabla;
            }
        }
        public Usuario Obtener(int id_usu, string StringdeConexion)
        {
            var oContacto = new Usuario();
            using (SqlConnection conn = new SqlConnection(StringdeConexion))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("TObtener", conn);
                cmd.Parameters.AddWithValue("@id_usu", id_usu);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        oContacto.id_usu = Convert.ToInt32(dr["id_usu"]);
                        oContacto.Nombre = dr["Nombre"].ToString();
                        oContacto.Apellidos = dr["Apellidos"].ToString();
                        oContacto.Correo = dr["Correo"].ToString();
                        oContacto.Telefono = dr["Telefono"].ToString();
                        oContacto.Fecha_Reg = Convert.ToDateTime(dr["Fecha_Reg"]);
                        oContacto.EmailConfirmed = Convert.ToBoolean(dr["EmailConfirmed"]);
                        oContacto.FotoPerfil = dr["FotoPerfil"] != DBNull.Value ? (byte[])dr["FotoPerfil"] : null;
                    }
                }
            }
            return oContacto;
        }
        public bool ValidarExistenciaCliente(Usuario usuario, string StringdeConexion)
        {
            using (SqlConnection connStr = new SqlConnection(StringdeConexion))
            {
                connStr.Open();
                var cmd = new SqlCommand("SELECT COUNT(*) FROM Usuario WHERE Correo = @Correo", connStr);
                cmd.Parameters.AddWithValue("@Correo", usuario.Correo);
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }
        public bool Guardar(Usuario ocontacto, string StringdeConexion)
        {
            bool rpta;
            byte[] salt = PasswordHasher.GenerateSalt();
            byte[] hash = PasswordHasher.HashPassword(ocontacto.Contraseña, salt);

            try
            {
                using (SqlConnection connStr = new SqlConnection(StringdeConexion))
                {
                    connStr.Open();
                    SqlCommand cmd = new SqlCommand("GuardarCliente", connStr);
                    ocontacto.Fecha_Reg = DateTime.Now;
                    cmd.Parameters.AddWithValue("Nombre", ocontacto.Nombre);
                    cmd.Parameters.AddWithValue("Apellidos", ocontacto.Apellidos);
                    cmd.Parameters.AddWithValue("Correo", ocontacto.Correo);
                    cmd.Parameters.AddWithValue("Telefono", ocontacto.Telefono);
                    cmd.Parameters.AddWithValue("@Fecha_Reg", ocontacto.Fecha_Reg);
                    cmd.Parameters.Add("@PasswordHash", SqlDbType.VarBinary, hash.Length).Value = hash;
                    cmd.Parameters.Add("@PasswordSalt", SqlDbType.VarBinary, salt.Length).Value = salt;
                    cmd.CommandType = CommandType.StoredProcedure; cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
                rpta = true;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                rpta = false;
            }
            return rpta;
        }
        public ResultadoEdicionUsuario Editar(Usuario ocontacto, string StringdeConexion)
        {
            var rpta = new ResultadoEdicionUsuario();

            try
            {
                using (SqlConnection conn = new SqlConnection(StringdeConexion))
                {
                    conn.Open();
                    using var tran = conn.BeginTransaction();

                    string querycorreo = "SELECT Correo FROM Usuario WHERE id_usu = @id_usu";
                    SqlCommand Ccmd = new SqlCommand(querycorreo, conn, tran);
                    Ccmd.Parameters.AddWithValue("@id_usu", ocontacto.id_usu);
                    Ccmd.CommandType = CommandType.Text;

                    var correoAntiguo = Ccmd.ExecuteScalar()?.ToString();

                    using (SqlCommand cmd = new SqlCommand("TEditarUsu", conn, tran))
                    {
                        cmd.Parameters.AddWithValue("id_usu", ocontacto.id_usu);
                        cmd.Parameters.AddWithValue("Nombre", ocontacto.Nombre);
                        cmd.Parameters.AddWithValue("Apellidos", ocontacto.Apellidos);
                        cmd.Parameters.AddWithValue("Correo", ocontacto.Correo);
                        cmd.Parameters.AddWithValue("Telefono", ocontacto.Telefono);
                        //cmd.Parameters.AddWithValue("Contraseña", ocontacto.Contraseña);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.ExecuteNonQuery();
                    }

                    rpta.Exito = true;

                    if (correoAntiguo != ocontacto.Correo)
                    {
                        rpta.CorreoModificado = true;
                        rpta.CorreoNuevo = ocontacto.Correo;
                        using (var cmdDes = new SqlCommand("UPDATE Usuario SET EmailConfirmed = 0 WHERE id_usu = @id_usu", conn, tran))
                        {
                            cmdDes.Parameters.AddWithValue("@id_usu", ocontacto.id_usu);
                            cmdDes.ExecuteNonQuery();
                        }
                        string checkConfirm = "SELECT EmailConfirmed FROM Usuario WHERE Correo = @Correo";
                        using SqlCommand checkCmd = new SqlCommand(checkConfirm, conn, tran);
                        {
                            checkCmd.Parameters.AddWithValue("@Correo", ocontacto.Correo);
                            object confirmed = checkCmd.ExecuteScalar();

                            if (confirmed != null && confirmed != DBNull.Value)
                            {
                                bool confirmado = Convert.ToBoolean(confirmed);
                                if (confirmado)
                                {
                                }
                                else
                                {
                                    //Correo modificado correctamente, pero no está confirmado, eliminar tokens antiguos
                                    string deletOldToken = "DELETE FROM EmailConfirmTokens WHERE Email = @Email";
                                    using (SqlCommand deletOldTokenCmd = new SqlCommand(deletOldToken, conn, tran))
                                    {
                                        deletOldTokenCmd.Parameters.AddWithValue("@Email", ocontacto.Correo);
                                        deletOldTokenCmd.ExecuteNonQuery();
                                    }
                                    // Generar un nuevo token y guardarlo en la base de datos
                                    string token = Guid.NewGuid().ToString();
                                    DateTime expiration = DateTime.UtcNow.AddHours(24);
                                    string insertarToken = @"
                                INSERT INTO EmailConfirmTokens (Token, Email, Expiration)
                                VALUES (@Token, @Email, @Expiration)";
                                    using (var cmdInsertToken = new SqlCommand(insertarToken, conn, tran))
                                    {
                                        cmdInsertToken.Parameters.AddWithValue("@Token", token);
                                        cmdInsertToken.Parameters.AddWithValue("@Email", ocontacto.Correo);
                                        cmdInsertToken.Parameters.AddWithValue("@Expiration", expiration);
                                        cmdInsertToken.ExecuteNonQuery();
                                    }
                                    rpta.Token = token; // Asignar el token generado al resultado
                                }
                            }
                        }
                    }
                    tran.Commit();
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                rpta.Exito = false;
            }

            return rpta;
        }
        // public bool EditarContraseña(Usuario ocontacto, string StringdeConexion)
        // {
        //     bool rpta;
        //     try
        //     {
        //         using (SqlConnection connStr = new SqlConnection(StringdeConexion))
        //         {
        //             connStr.Open();
        //             using var tran = connStr.BeginTransaction();

        //             string query = @"
        //             SELECT 
		//                 PasswordHash,
		//                 PasswordSalt
	    //             FROM Usuario 
	    //             WHERE id_usu = @id_usu
	    //             AND Activo = 1";
        //             using SqlCommand cmd = new SqlCommand(query, connStr);
        //             cmd.Parameters.AddWithValue("@id_usu", ocontacto.id_usu);

        //             using SqlDataReader reader = cmd.ExecuteReader();
        //             if (!reader.Read())
        //             {
                        
        //             }

        //             byte[] hashActual = (byte[])reader["PasswordHash"];
        //             byte[] saltActual = (byte[])reader["PasswordSalt"];

        //             if (!PasswordHasher.VerificarContraseña(model.ContraseñaActual, hashActual, saltActual))
        //             {
        //                 ModelState.AddModelError("ContraseñaActual", "La contraseña actual es incorrecta.");
        //                 return RedirectToAction("Index", "Perfil");
        //             }

        //             string queryContraseña = "SELECT Contraseña FROM Usuario WHERE id_usu = @id_usu";
        //             SqlCommand Ccmd = new SqlCommand(queryContraseña, connStr, tran);
        //             Ccmd.Parameters.AddWithValue("@id_usu", ocontacto.id_usu);
        //             Ccmd.CommandType = CommandType.Text;

        //             var contraseñaAntigua = Ccmd.ExecuteScalar()?.ToString();

        //             // Aquí puedes actualizar la contraseña del usuario en la base de datos
        //             using SqlCommand updatecmd = new SqlCommand("ActualizarContraseña", connStr, tran);
        //             updatecmd.CommandType = CommandType.StoredProcedure;
        //             updatecmd.Parameters.AddWithValue("@Contraseña", ocontacto.Contraseña);
        //             updatecmd.Parameters.AddWithValue("@newcontraseña", ocontacto.newContraseña); // En texto plano, será cifrada en SQL
        //             updatecmd.Parameters.AddWithValue("@id_usu", ocontacto.id_usu);

        //             if (contraseñaAntigua == ocontacto.Contraseña)
        //             {
        //                 updatecmd.ExecuteNonQuery();
        //             }

        //             tran.Commit();
        //         }
        //         rpta = true;
        //     }
        //     catch (Exception ex)
        //     {
        //         string error = ex.Message;
        //         rpta = false;
        //     }
        //     return rpta;
        // }
        public ResultadoEliminacion EliminarCliente(int id_usua, string PasswordConfirm, string StringdeConexion)
        {
            byte[] hash = null, salt = null;

            // Obtener hash y salt del administrador supremo //
            using (var connStr = new SqlConnection(StringdeConexion))
            {
                connStr.Open();
                var cmd = new SqlCommand("SELECT ContrasenaHash, ContrasenaSalt FROM Administradores WHERE RolAdminId = 1", connStr);
                using var reader = cmd.ExecuteReader();

                if (!reader.Read())
                {
                    return new ResultadoEliminacion
                    {
                        Exito = false,
                        Mensaje = "No se encontró el administrador supremo."
                    };
                }
                if (reader.IsDBNull(0) || reader.IsDBNull(1))
                {
                    return new ResultadoEliminacion
                    {
                        Exito = false,
                        Mensaje = "El administrador supremo no tiene contraseña registrada."
                    };
                }
                hash = (byte[])reader["ContrasenaHash"];
                salt = (byte[])reader["ContrasenaSalt"];
            }

            // Verificar la contraseña //
            bool contraseñaCorrecta = PasswordHasher.VerificarContraseña(PasswordConfirm, hash, salt);

            if (!contraseñaCorrecta)
            {
                return new ResultadoEliminacion
                {
                    Exito = false,
                    Mensaje = "Contraseña incorrecta. No se puede eliminar al cliente."
                };
            }

            // Eliminación del cliente //
            using (SqlConnection connStr = new SqlConnection(StringdeConexion))
            {
                connStr.Open();
                SqlCommand cmd = new SqlCommand("EliminarCliente", connStr);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_usu", id_usua);
                cmd.ExecuteNonQuery();
            }
            return new ResultadoEliminacion
            {
                Exito = true,
                Mensaje = "Cliente eliminado correctamente."
            };
        }
        public bool cambioFotoPerfil(int id_usu, byte[] fotoPerfil, string extension, string StringdeConexion)
        {
            using (SqlConnection conn = new SqlConnection(StringdeConexion))
            {
                conn.Open();
                var query = "UPDATE Usuario SET FotoPerfil = @FotoPerfil, FotoPerfilExtension = @Extension WHERE id_usu = @id_usu";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FotoPerfil", fotoPerfil);
                    cmd.Parameters.AddWithValue("@Extension", extension ?? (object)DBNull.Value); // Maneja el caso de extensión nula
                    cmd.Parameters.AddWithValue("@id_usu", id_usu);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
        public Usuario ObtenerCliente(int id_usu, string StringdeConexion)
        {
            var oContacto = new Usuario();
            using (SqlConnection connStr = new SqlConnection(StringdeConexion))
            {
                connStr.Open();
                SqlCommand cmd = new SqlCommand("ObtenerCliente", connStr);
                cmd.Parameters.AddWithValue("@id_usu", id_usu);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        oContacto.id_usu = Convert.ToInt32(dr["id_usu"]);
                        oContacto.Nombre = dr["Nombre"].ToString();
                        oContacto.Apellidos = dr["Apellidos"].ToString();
                        oContacto.Correo = dr["Correo"].ToString();
                        oContacto.Telefono = dr["Telefono"].ToString();
                        oContacto.Activo = (bool)dr["Activo"];
                    }
                }
            }
            return oContacto;
        }
        public bool EditarCliente(Usuario oContacto, string StringdeConexion)
        {
            bool rptas;
            try
            {
                using (SqlConnection connStr = new SqlConnection(StringdeConexion))
                {
                    connStr.Open();
                    SqlCommand cmd = new SqlCommand("EditarCliente", connStr);
                    cmd.Parameters.AddWithValue("id_usu", oContacto.id_usu);
                    cmd.Parameters.AddWithValue("Nombre", oContacto.Nombre);
                    cmd.Parameters.AddWithValue("Apellidos", oContacto.Apellidos);
                    cmd.Parameters.AddWithValue("Telefono", oContacto.Telefono);
                    cmd.Parameters.AddWithValue("Correo", oContacto.Correo);
                    cmd.Parameters.AddWithValue("@Activo", oContacto.Activo);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
                rptas = true;
            }
            catch (SqlException ex)
            {
                string error = ex.Message;
                rptas = false;
            }
            return rptas;
        }
    }
}