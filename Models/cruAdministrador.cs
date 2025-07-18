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
            using (SqlConnection conn = new SqlConnection(StringdeConexion))
            {
                using (SqlCommand cmd = new SqlCommand("AdmUsua", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();

                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        oTabla.Add(new Administrador
                        {
                            idAdministrador = (int)dr["id_adm"],
                            Nombre = dr["Nombre"].ToString(),
                            Apellido = dr["Apellidos"].ToString(),
                            Correo = dr["Correo"].ToString(),
                            Telefono = dr["Telefono"].ToString(),
                            Contraseña = dr["Contraseña"].ToString(),
                        });
                    }
                }
                return oTabla;
            }
        }

        public Administrador Obtener(int id_adm, string StringdeConexion)
        {
            var oContacto = new Administrador();
            using (SqlConnection conn = new SqlConnection(StringdeConexion))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("AdmObtener", conn);
                cmd.Parameters.AddWithValue("@id_adm", id_adm);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        oContacto.idAdministrador = Convert.ToInt32(dr["id_adm"]);
                        oContacto.Nombre = dr["Nombre"].ToString();
                        oContacto.Apellido = dr["Apellidos"].ToString();
                        oContacto.Correo = dr["Correo"].ToString();
                        oContacto.Telefono = dr["Telefono"].ToString();
                        oContacto.Contraseña = dr["Contraseña"].ToString();
                    }
                }
            }
            return oContacto;
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
                    SqlCommand cmd = new SqlCommand("AdmGuardar", connStr);
                    cmd.Parameters.AddWithValue("Nombre", ocontacto.Nombre);
                    cmd.Parameters.AddWithValue("Apellidos", ocontacto.Apellido);
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
                using (SqlConnection conn = new SqlConnection(StringdeConexion))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("AdmEditar", conn);
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
        public bool Eliminar(int id_adm, string StringdeConexion)
        {
            bool rpta;
            try
            {
                using (SqlConnection conn = new SqlConnection(StringdeConexion))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("AdmEli", conn);
                    cmd.Parameters.AddWithValue("id_adm", id_adm);
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
        public List<RolAdmin> ObtenerRoles(string connectionString)
        {
            var roles = new List<RolAdmin>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Id, Nombre FROM RolesAdmin", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    roles.Add(new RolAdmin
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Nombre = reader["Nombre"].ToString()
                    });
                }
            }
            return roles;
        }
    }
}