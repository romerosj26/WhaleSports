using System;
using System.Data;
using System.Transactions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;

namespace WS_2_0.Models
{
    public class cruProductos
    {
        public List<Producto> ObtenerProductos(string conexion)
        {
            List<Producto> productos = new List<Producto>();
            using (SqlConnection conn = new SqlConnection(conexion))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Productos WHERE Activo = 1", conn);
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
                            ImagenUrl = dr["Imagen"].ToString(),
                            Categoria = dr["Categoria"].ToString()
                        });
                    }
                }
            }
            return productos;
        }
    }
}