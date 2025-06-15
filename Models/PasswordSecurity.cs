using System.Security.Cryptography;
using System.Text;

namespace WS_2_0.Models
{
    public static class PasswordHasher
    {
        public static byte[] GenerateSalt()
        {
            using var rng = new RNGCryptoServiceProvider();
            byte[] salt = new byte[16];
            rng.GetBytes(salt);
            return salt;
        }
        public static byte[] HashPassword(string password, byte[] salt)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(32); // 32 bytes = 256 bits
        }

        public static bool VerificarContraseña(string inputPassword, byte[] storedHash, byte[] storedSalt)
        {
            var hashOfInput = HashPassword(inputPassword, storedSalt);
            return CryptographicOperations.FixedTimeEquals(hashOfInput, storedHash);
        }
    }
}
