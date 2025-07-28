using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WS_2_0.Models.ViewModels
{
    public class TablaClientesViewModel
    {
        public List<Usuario> Usuarios { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime Fecha_Reg { get; set; }
        public bool EmailConfirmed { get; set; } = false;

        [Required(ErrorMessage = "Campo requerido")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
         ErrorMessage = "La contraseña debe tener al menos 8 caracteres, una mayúscula, una minúscula y un número.")]
        public string Contraseña { get; set; }
        public bool Activo { get; set; }

    }
    public class CrearClienteViewModel
    {
        [Required(ErrorMessage = "El Nombre es obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El Apellido es obligatorio")]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress]
        public string Correo { get; set; }

        [Required(ErrorMessage = "Debe ingresar una contraseña")]
        [DataType(DataType.Password)]
        public string Contraseña { get; set; }

        [Required(ErrorMessage = "Debe ingresar un Telefono")]
        public string Telefono { get; set; }
    }
    public class EditarClienteViewModel
    {
        public int id_usu { get; set; }
        [Required(ErrorMessage = "El Nombre es obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El Apellido es obligatorio")]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress]
        public string Correo { get; set; }

        [Required(ErrorMessage = "Debe ingresar un Telefono")]
        public string Telefono { get; set; }

        public bool Activo { get; set; }
    }
}
