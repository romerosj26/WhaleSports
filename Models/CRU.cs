using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace WS_2_0.Models
{
    public class CRU
    {
        public List<Usuario> Tabla(string StringdeConexion)
        {
            var oTabla = new List<Usuario>();
            using (SqlConnection conn = new SqlConnection(StringdeConexion))
            {
                using (SqlCommand cmd = new SqlCommand("TUsua", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();

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
                            Contraseña = dr["Contraseña"].ToString(),
                        });
                    }
                }
                return oTabla;
            }
        }
        public Usuario Obtener(int id_usua, string StringdeConexion)
        {
            var oContacto = new Usuario();
            using (SqlConnection conn = new SqlConnection(StringdeConexion))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("TObtener", conn);
                cmd.Parameters.AddWithValue("@id_usu", id_usua);
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
                        oContacto.Contraseña = dr["Contraseña"].ToString();
                        oContacto.Fecha_Reg = Convert.ToDateTime(dr["Fecha_Reg"]);
                        oContacto.EmailConfirmed = Convert.ToBoolean(dr["EmailConfirmed"]);
                    }
                }
            }
            return oContacto;
        }
        public bool Guardar(Usuario ocontacto, string StringdeConexion)
        {
            bool rpta;
            try
            {
                using (SqlConnection conn = new SqlConnection(StringdeConexion))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("TGuardar", conn);
                    cmd.Parameters.AddWithValue("Nombre", ocontacto.Nombre);
                    cmd.Parameters.AddWithValue("Apellidos", ocontacto.Apellidos);
                    cmd.Parameters.AddWithValue("Correo", ocontacto.Correo);
                    cmd.Parameters.AddWithValue("Telefono", ocontacto.Telefono);
                    cmd.Parameters.AddWithValue("Contraseña", ocontacto.Contraseña);
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
        public bool Editar(Usuario ocontacto, string StringdeConexion)
        {
            bool rpta;
            // try
            // {
                using (SqlConnection conn = new SqlConnection(StringdeConexion))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("TEditarUsu", conn);
                    cmd.Parameters.AddWithValue("id_usu", ocontacto.id_usu);
                    cmd.Parameters.AddWithValue("Nombre", ocontacto.Nombre);
                    cmd.Parameters.AddWithValue("Apellidos", ocontacto.Apellidos);
                    cmd.Parameters.AddWithValue("Correo", ocontacto.Correo);
                    cmd.Parameters.AddWithValue("Telefono", ocontacto.Telefono);
                    //cmd.Parameters.AddWithValue("Contraseña", ocontacto.Contraseña);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
                rpta = true;
            // }
            // catch (Exception ex)
            // {
            //     string error = ex.Message;
            //     rpta = false;
            // }

            return rpta;
        }
        public bool Eliminar(int id_usua, string StringdeConexion)
        {
            bool rpta;
            try
            {
                using (SqlConnection conn = new SqlConnection(StringdeConexion))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("TEliUsu", conn);
                    cmd.Parameters.AddWithValue("id_usu", id_usua);
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
    
    }
}