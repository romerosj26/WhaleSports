namespace WS_2_0.Models
{
    public class ResultadoRegistro
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; }
        public Usuario? UsuarioRegistrado { get; set; }
    }
}