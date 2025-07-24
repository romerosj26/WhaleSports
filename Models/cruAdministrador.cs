using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Data;
using System;

namespace WS_2_0.Models
{
    public class cruAdministrador
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
        public bool Editar(Administrador ocontac, string StringdeConexion)
        {
            bool rptas;
            try
            {
                using (SqlConnection connStr = new SqlConnection(StringdeConexion))
                {
                    connStr.Open();
                    SqlCommand cmd = new SqlCommand("AdmEditar", connStr);
                    cmd.Parameters.AddWithValue("id_adm", ocontac.idAdministrador);
                    cmd.Parameters.AddWithValue("Nombre", ocontac.Nombre);
                    cmd.Parameters.AddWithValue("Apellidos", ocontac.Apellido);
                    cmd.Parameters.AddWithValue("Telefono", ocontac.Telefono);
                    cmd.Parameters.AddWithValue("Correo", ocontac.Correo);
                    cmd.Parameters.AddWithValue("Contraseña", ocontac.Contraseña);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
                rptas = true;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                rptas = false;
            }

            return rptas;
        }
        public Administrador Eliminar(Administrador administrador, string PasswordConfirm, string StringdeConexion)
        {
            byte[] hash = null, salt = null;
            bool contraseñaCorrecta = false;
            using (var conn = new SqlConnection(StringdeConexion))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT ContrasenaHash, ContrasenaSalt FROM Administradores WHERE idAdministrador = @idAdministrador", conn);
                cmd.Parameters.AddWithValue("@idAdministrador", administrador.idAdministrador);
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    if (reader["ContrasenaHash"] == DBNull.Value || reader["ContrasenaSalt"] == DBNull.Value)
                    {
                        hash = (byte[])reader["ContrasenaHash"];
                        salt = (byte[])reader["ContrasenaSalt"];

                        contraseñaCorrecta = PasswordHasher.VerificarContraseña(PasswordConfirm, hash, salt);
                    }
                }
            }
            if (contraseñaCorrecta)
            {
                using (SqlConnection conn = new SqlConnection(StringdeConexion))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("EliminarAdministrador", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idAdministrador", administrador.idAdministrador);
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                administrador.Contraseña = "Contraseña incorrecta.";
            }
            return administrador;
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