using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WS_2_0.Models
{
    public class Usuario
    {
        public int id_usu { get; set; }
        public int id_adm { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
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
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public string Contraseña { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int RolAdminId { get; set; }
        public RolAdmin Rol { get; set; } = null!;
    }
    public class EmpleadoViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress]
        public string Correo { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un rol")]
        public int RolAdminId { get; set; }

        // Lista de roles para el select
        public List<SelectListItem> RolesDisponibles { get; set; }
    }
    public class RolAdmin
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null;
        public string? Descripcion { get; set; }
        public ICollection<RolPermiso> RolPermisos { get; set; } = new List<RolPermiso>();
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
    public class Permiso
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }

        public ICollection<RolPermiso> RolPermisos { get; set; } = new List<RolPermiso>();
    }
    public class RolPermiso
    {
        public int RolId { get; set; }
        public RolAdmin Rol { get; set; } = null!;

        public int PermisoId { get; set; }
        public Permiso Permiso { get; set; } = null!;
    }
}