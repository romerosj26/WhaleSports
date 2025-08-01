using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WS_2_0.Models
{
    public class CrearProductoViewModel
    {
        [Required(ErrorMessage = "El Nombre es obligatorio")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "La Descripcion es obligatoria")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El Precio es obligatorio")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "Debe ingresar una Imagen")]
        public string ImagenUrl { get; set; }

        [Required(ErrorMessage = "Debe ingresar el Stock")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "Debe seleccionar si el producto esta activo")]
        public bool Activo { get; set; }
        public string? Categoria { get; set; }
    }
    public class EditarProductoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El Nombre es obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La Descripcion es obligatoria")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El Precio es obligatorio")]
        public decimal Precio { get; set; }
        public string? ImagenUrl { get; set; }

        [NotMapped]
        public IFormFile? ImagenArchivo { get; set; } // Para subir una nueva imagen

        [Required(ErrorMessage = "Debe ingresar el Stock")]
        public int Stock { get; set; }
        public bool Activo { get; set; }
        public string? Categoria { get; set; }
    }
}