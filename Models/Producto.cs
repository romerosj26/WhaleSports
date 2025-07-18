using System;
using System.ComponentModel.DataAnnotations;
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
        public string ImagenUrl { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public string Categoria { get; set; }
    }
}
