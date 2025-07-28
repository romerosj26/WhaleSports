using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Data;
using WS_2_0.Models.ViewModels;
using System;

namespace WS_2_0.Models
{
    public class crudAdministrador
    {
        public List<Administrador> Tabla(string StringdeConexion)
        {
            var oTabla = new List<Administrador>();
            using (SqlConnection connStr = new SqlConnection(StringdeConexion))
            {
                using (SqlCommand cmd = new SqlCommand("MostrarAdministrador", connStr))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    connStr.Open();

                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        oTabla.Add(new Administrador
                        {
                            idAdministrador = (int)dr["idAdministrador"],
                            Nombre = dr["Nombre"].ToString(),
                            Apellido = dr["Apellido"].ToString(),
                            Correo = dr["Correo"].ToString(),
                            Telefono = dr["Telefono"].ToString(),
                            Contraseña = dr["ContrasenaHash"].ToString(),
                            RolAdminId = (int)dr["RolAdminId"],
                            RolNombre = dr["RolNombre"].ToString(),
                            Activo = (bool)dr["Activo"],
                            FechaRegistro = (DateTime)dr["FechaRegistro"]
                        });
                    }
                }
                return oTabla;
            }
        }

        public Administrador Obtener(int idAdministrador, string StringdeConexion)
        {
            var oContacto = new Administrador();
            using (SqlConnection connStr = new SqlConnection(StringdeConexion))
            {
                connStr.Open();
                SqlCommand cmd = new SqlCommand("ObtenerAdministrador", connStr);
                cmd.Parameters.AddWithValue("@idAdministrador", idAdministrador);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        oContacto.idAdministrador = Convert.ToInt32(dr["idAdministrador"]);
                        oContacto.Nombre = dr["Nombre"].ToString();
                        oContacto.Apellido = dr["Apellido"].ToString();
                        oContacto.Correo = dr["Correo"].ToString();
                        oContacto.Telefono = dr["Telefono"].ToString();
                        oContacto.Contraseña = dr["ContrasenaHash"].ToString();
                        oContacto.RolAdminId = (int)dr["RolAdminId"];
                        oContacto.RolNombre = dr["RolNombre"].ToString();
                        oContacto.Activo = (bool)dr["Activo"];
                        oContacto.FechaRegistro = (DateTime)dr["FechaRegistro"];
                    }
                }
            }
            return oContacto;
        }
        public bool ValidarExistenciaAdministrador(Administrador administrador, string StringdeConexion)
        {
            using (SqlConnection connStr = new SqlConnection(StringdeConexion))
            {
                connStr.Open();
                var cmd = new SqlCommand("SELECT COUNT(*) FROM Administradores WHERE Correo = @Correo", connStr);
                cmd.Parameters.AddWithValue("@Correo", administrador.Correo);
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }
        public bool Guardar(Administrador ocontacto, string StringdeConexion)
        {
            bool rpta;
            byte[] salt = PasswordHasher.GenerateSalt();
            byte[] hash = PasswordHasher.HashPassword(ocontacto.Contraseña, salt);

            try
            {
                using (SqlConnection connStr = new SqlConnection(StringdeConexion))
                {
                    connStr.Open();
                    SqlCommand cmd = new SqlCommand("GuardarAdministrador", connStr);
                    ocontacto.FechaRegistro = DateTime.Now;
                    cmd.Parameters.AddWithValue("Nombre", ocontacto.Nombre);
                    cmd.Parameters.AddWithValue("Apellido", ocontacto.Apellido);
                    cmd.Parameters.AddWithValue("Correo", ocontacto.Correo);
                    cmd.Parameters.AddWithValue("Telefono", ocontacto.Telefono);
                    cmd.Parameters.Add("@ContrasenaHash", SqlDbType.VarBinary, hash.Length).Value = hash;
                    cmd.Parameters.Add("@ContrasenaSalt", SqlDbType.VarBinary, salt.Length).Value = salt;
                    cmd.Parameters.AddWithValue("@RolAdminId", ocontacto.RolAdminId);
                    cmd.Parameters.AddWithValue("@FechaRegistro", ocontacto.FechaRegistro);
                    cmd.CommandType = CommandType.StoredProcedure;
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
        public bool Editar(Administrador ocontacto, string StringdeConexion)
        {
            bool rptas;
            try
            {
                using (SqlConnection connStr = new SqlConnection(StringdeConexion))
                {
                    connStr.Open();
                    SqlCommand cmd = new SqlCommand("EditarAdministrador", connStr);
                    cmd.Parameters.AddWithValue("idAdministrador", ocontacto.idAdministrador);
                    cmd.Parameters.AddWithValue("Nombre", ocontacto.Nombre);
                    cmd.Parameters.AddWithValue("Apellido", ocontacto.Apellido);
                    cmd.Parameters.AddWithValue("Telefono", ocontacto.Telefono);
                    cmd.Parameters.AddWithValue("Correo", ocontacto.Correo);
                    cmd.Parameters.AddWithValue("@RolAdminId", ocontacto.RolAdminId);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
                rptas = true;
            }
            catch (SqlException  ex)
            {
                string error = ex.Message;
                rptas = false;
            }

            return rptas;
        }
        public ResultadoEliminacion Eliminar(int idAdministrador, string PasswordConfirm, string StringdeConexion)
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
                    Mensaje = "Contraseña incorrecta. No se puede eliminar el administrador."
                };
            }
            
            // Eliminación de administrador //
            using (SqlConnection connStr = new SqlConnection(StringdeConexion))
            {
                connStr.Open();
                SqlCommand cmd = new SqlCommand("EliminarAdministrador", connStr);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idAdministrador", idAdministrador);
                cmd.ExecuteNonQuery();
            }
            return new ResultadoEliminacion
            {
                Exito = true,
                Mensaje = "Administrador eliminado correctamente."
            };
        }
        public List<RolAdmin> ObtenerRoles(string connectionString)
        {
            var roles = new List<RolAdmin>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Id, RolNombre FROM RolesAdmin", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    roles.Add(new RolAdmin
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        RolNombre = reader["RolNombre"].ToString()
                    });
                }
            }
            return roles;
        }
    }
}