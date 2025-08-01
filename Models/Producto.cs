using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WS_2_0.Models
{
    public class Producto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Precio { get; set; }

        [StringLength(300)]
        public string? ImagenUrl { get; set; }

        [NotMapped]
        public IFormFile? ImagenArchivo { get; set; }
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public string Categoria { get; set; }
    }
    public class Tallas
    {
        public int Id { get; set; }
        public string Talla { get; set; } // Ej: "S", "M", "L", "XL"
    }
    public class Colores
    {
        public int Id { get; set; }
        public string Color { get; set; } // Ej: "Negro", "Rojo", "Azul"
    }
    public class VariantesProducto
    {
        public int Id { get; set; }

        public int ProductoId { get; set; }
        public Producto Producto { get; set; }

        public int TallaId { get; set; }
        public Tallas Talla { get; set; }

        public int ColorId { get; set; }
        public Colores Color { get; set; }

        public int Stock { get; set; }
    }
}
