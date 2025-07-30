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
                            ImagenUrl = dr["Imagen"].ToString(),
                            Stock = (int)dr["Stock"],
                            Activo = (bool)dr["Activo"],
                            Categoria = dr["Categoria"].ToString()
                        });
                    }
                }
            }
            return productos;
        }
    }
}