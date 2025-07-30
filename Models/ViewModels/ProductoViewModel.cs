using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
}