namespace WS_2_0.Models
{
    public class PasswordResetToken
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public Guid Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}