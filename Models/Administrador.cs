using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WS_2_0.Models
{ 
    public class Administrador
    {
        public int idAdministrador { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public string Contraseña { get; set; }
        public int RolAdminId { get; set; }
        public string? RolNombre { get; set; } 
        public DateTime FechaRegistro { get; set; }
        public bool Activo { get; set; }
    }
}
