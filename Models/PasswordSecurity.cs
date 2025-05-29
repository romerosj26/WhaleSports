using System.Security.Cryptography;
using System.Text;

namespace WS_2_0.Models
{
    public static class PasswordHasher
    {
        private const int SaltSize = 16; // 128 bits
        private const int KeySize = 32;  // 256 bits
        private const int Iterations = 100000;

        public static (string hash, string salt) HashPassword(string password)
        {
            using var rng = new RNGCryptoServiceProvider();
            byte[] saltBytes = new byte[16];
            rng.GetBytes(saltBytes);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 100000, HashAlgorithmName.SHA256);
            byte[] hashBytes = pbkdf2.GetBytes(32);

            return (Convert.ToBase64String(hashBytes),Convert.ToBase64String(saltBytes));
        }

        public static bool VerificarContraseña(string password, string hashAlmacenado)
        {
            var partes = hashAlmacenado.Split('.');
            if (partes.Length != 2)
            {
                return false; // Formato incorrecto del hash almacenado
            }

            var salt = Convert.FromBase64String(partes[0]);
            var hashEsperado = partes[1];

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            var hashIngresado = Convert.ToBase64String(pbkdf2.GetBytes(32));

            return hashEsperado == hashIngresado;
        }
    }
}
