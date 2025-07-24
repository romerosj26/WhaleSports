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
    public class Administrador
    {
        public int idAdministrador { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string Apellido { get; set; }
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Correo no válido")]
        public string Correo { get; set; }
        public string Telefono { get; set; }
        [Required(ErrorMessage = "Debe ingresar una contraseña")]
        [DataType(DataType.Password)]
        public string Contraseña { get; set; }
        
        [Required(ErrorMessage = "Debe seleccionar un rol")]
        public int RolAdminId { get; set; }
        public string? RolNombre { get; set; } 
        public DateTime FechaRegistro { get; set; }
        public bool Activo { get; set; }
    }
    public class AdministradorViewModel
    {
        [Required(ErrorMessage = "El Nombre es obligatorio")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El Apellido es obligatorio")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress]
        public string Correo { get; set; }

        [Required(ErrorMessage = "Debe ingresar una contraseña")]
        [DataType(DataType.Password)]
        public string Contraseña { get; set; }

        [Required(ErrorMessage = "Debe ingresar un Telefono")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un rol")]
        public int RolAdminId { get; set; }

        // Lista de roles para el select
        public List<SelectListItem> RolesDisponibles { get; set; }
    }
    public class RolAdmin
    {
        public int Id { get; set; }
        public string RolNombre { get; set; } = null;
        public string? Descripcion { get; set; }
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}