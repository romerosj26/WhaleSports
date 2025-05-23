using System.ComponentModel.DataAnnotations;

namespace WS_2_0.Models
{
    public class Usuario
    {
        public int id_usu { get; set; }
        public int id_adm { get; set; }

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
        public string confirmRescon { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]    
        public DateTime Fecha_Reg { get; set; }
        //Confirmación de correo
        public bool EmailConfirmed { get; set; } = false;
        public string EmailConfirmationToken { get; set; } // Token para confirmar el correo
        public DateTime? EmailConfirmationTokenExpiry { get; set; } // Fecha de expiración del token

        public int ban { get; set; }

        public int id_pedido { get; set; }
        public string nombrePro { get; set; }
        public string Talla { get; set; }
        public string Precio { get; set; }
        public int stock { get; set; }
        public int Total { get; set; }
        public int TP { get; set; }
    }
}