using System;
using System.Linq;

namespace WS_2_0.Models.Logueo
{
    public class Captcha
    {
        public string CrearCaptcha()
        {
            string Captcha = "";
            for (int i = 0; i <= 2; i++)
            {
                var guid = Guid.NewGuid();
                var justNumbers = new string(guid.ToString().Where(char.IsDigit).ToArray());
                var seed = int.Parse(justNumbers.Substring(0, 4));

                var random = new Random(seed);
                var value = random.Next(0, 9);

                Captcha = Captcha + value.ToString();

                int numero = random.Next(26);
                char letra = (char)('a' + numero);

                Captcha = Captcha + letra;
            }

            return Captcha;
        }
    }
}