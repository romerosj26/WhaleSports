using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WS_2_0.Models
{
    public class CrearAdministradorViewModel
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
        public List<SelectListItem>? RolesDisponibles { get; set; }
    }
    public class EditarAdministradorViewModel
    {
        public int idAdministrador { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public int RolAdminId { get; set; }
        public List<SelectListItem>? RolesDisponibles { get; set; }
    }
    public class RolAdmin
    {
        public int Id { get; set; }
        public string RolNombre { get; set; } = null;
        public string? Descripcion { get; set; }
    }
    public class ResultadoEliminacion
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; }
    }
}