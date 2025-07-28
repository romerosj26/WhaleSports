using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WS_2_0.Models
{
    public class Usuario
    {
        public int id_usu { get; set; }
        public int idAdministrador { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public string Apellidos { get; set; }
        
        [Required(ErrorMessage = "Campo requerido")]
        [EmailAddress(ErrorMessage = "Correo no valido")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
         ErrorMessage = "La contraseña debe tener al menos 8 caracteres, una mayúscula, una minúscula y un número.")]
        public string Contraseña { get; set; }
        [Required(ErrorMessage = "Debe confirmar la contraseña.")]
        [Compare("Contraseña", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmContra { get; set; }
        public string newContraseña { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime Fecha_Reg { get; set; }
        //Confirmación de correo
        public bool EmailConfirmed { get; set; } = false;
        public bool Activo { get; set; }
        public byte[]? FotoPerfil { get; set; } // nombre del archivo original
        public string? FotoPerfilExtension { get; set; } // Ej: ".jpg", ".png", ".webp"
    }
    public class ResultadoEdicionUsuario
    {
        public bool Exito { get; set; }
        public bool CorreoModificado { get; set; }
        public string Token { get; set; }
        public string CorreoNuevo { get; set; }
    }
}