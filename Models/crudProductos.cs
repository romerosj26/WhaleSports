using System;
using System.Data;
using System.Transactions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;

namespace WS_2_0.Models
{
    public class crudProductos
    {
        public List<Producto> Tabla(string conexion)
        {
            List<Producto> productos = new List<Producto>();
            using (SqlConnection conn = new SqlConnection(conexion))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("MostrarProducto", conn);
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        productos.Add(new Producto
                        {
                            Id = (int)dr["Id"],
                            Nombre = dr["Nombre"].ToString(),
                            Descripcion = dr["Descripcion"].ToString(),
                            Precio = (decimal)dr["Precio"],
                            ImagenUrl = dr["ImagenUrl"].ToString(),
                            Stock = (int)dr["Stock"],
                            Activo = (bool)dr["Activo"],
                            Categoria = dr["Categoria"].ToString()
                        });
                    }
                }
            }
            return productos;
        }
        public bool Guardar(Producto oContacto, string StringdeConexion)
        {
            bool rpta;
            try
            {
                using (SqlConnection connStr = new SqlConnection(StringdeConexion))
                {
                    connStr.Open();
                    SqlCommand cmd = new SqlCommand("GuardarProducto", connStr);
                    oContacto.FechaCreacion = DateTime.Now;
                    cmd.Parameters.AddWithValue("Nombre", oContacto.Nombre);
                    cmd.Parameters.AddWithValue("Descripcion", oContacto.Descripcion);
                    cmd.Parameters.AddWithValue("Precio", oContacto.Precio);
                    cmd.Parameters.AddWithValue("ImagenUrl", oContacto.ImagenUrl);
                    cmd.Parameters.AddWithValue("Stock", oContacto.Stock);
                    cmd.Parameters.AddWithValue("Activo", oContacto.Activo);
                    cmd.Parameters.AddWithValue("FechaCreacion", oContacto.FechaCreacion);
                    cmd.Parameters.AddWithValue("Categoria", oContacto.Categoria);
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
        public Producto Obtener(int Id, string StringdeConexion)
        {
            var oContacto = new Producto();
            using (SqlConnection connStr = new SqlConnection(StringdeConexion))
            {
                connStr.Open();
                SqlCommand cmd = new SqlCommand("ObtenerProducto", connStr);
                cmd.Parameters.AddWithValue("@Id", Id);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        oContacto.Id = Convert.ToInt32(dr["Id"]);
                        oContacto.Nombre = dr["Nombre"].ToString();
                        oContacto.Descripcion = dr["Descripcion"].ToString();
                        oContacto.Precio = Convert.ToDecimal(dr["Precio"]);
                        oContacto.ImagenUrl = dr["ImagenUrl"].ToString();
                        oContacto.Stock = Convert.ToInt32(dr["Stock"]);
                        oContacto.Activo = Convert.ToBoolean(dr["Activo"]);
                        oContacto.Categoria = dr["Categoria"].ToString();
                    }
                }
            }
            return oContacto;
        }
        public bool Editar(Producto oContacto, string StringdeConexion)
        {
            bool rpta;
            try
            {
                using (SqlConnection connStr = new SqlConnection(StringdeConexion))
                {
                    connStr.Open();
                    SqlCommand cmd = new SqlCommand("EditarProducto", connStr);
                    cmd.Parameters.AddWithValue("Id", oContacto.Id);
                    cmd.Parameters.AddWithValue("Nombre", oContacto.Nombre);
                    cmd.Parameters.AddWithValue("Descripcion", oContacto.Descripcion);
                    cmd.Parameters.AddWithValue("Precio", oContacto.Precio);
                    cmd.Parameters.AddWithValue("@ImagenUrl", oContacto.ImagenUrl ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("Stock", oContacto.Stock);
                    cmd.Parameters.AddWithValue("Activo", oContacto.Activo);
                    cmd.Parameters.AddWithValue("Categoria", oContacto.Categoria);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
                rpta = true;
            }
            catch (SqlException ex)
            {
                string error = ex.Message;
                rpta = false;
            }
            return rpta;
        }
    }
}