using System.Diagnostics;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using WS_2_0.Models;
using WS_2_0.Models.Logueo;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Options;
using WS_2_0.Services;

namespace WS_2_0.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly EmailSettings _emailSettings;
        private readonly EmailService _emailService;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, IOptions<EmailSettings> emailOptions, EmailService emailService)
        {
            _logger = logger;
            _configuration = configuration;
            _emailSettings = emailOptions.Value;
            _emailService = emailService;
        }

        public static Usuario prueba = new Usuario();
        public static Usuario pruebaX = new Usuario();
        LogInUsuario _log = new LogInUsuario();


        public IActionResult Index(Usuario usuario)
        {
            string connStr = _configuration.GetConnectionString("StringCONSQLlocal");
            var ocontact = _log.Obtener(usuario, connStr);
            if (prueba.ban == 1)
            {
                ViewBag.Nombre = pruebaX.Nombre;
                ViewBag.id_usu = pruebaX.id_usu;
            }
            else
            {
                ViewBag.Nombre = prueba.Nombre;
                ViewBag.id_usu = prueba.id_usu;
            }
            return View(ocontact);
        }
        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SignUp(Usuario usuario)
        {
            //Validación de la confirmación de contraseña
            if (usuario.Contraseña != usuario.ConfirmContra)
            {
                ModelState.AddModelError("ConfirmContra", "Las contraseñas no coinciden.");
                return View("SignUp", usuario);
            }

            string connStr = _configuration.GetConnectionString("StringCONSQLlocal");
            bool registro;
            string asunto = "Bienvenido a WhaleSports";
            string html = $"<!DOCTYPE html> <html lang='es'> <body> <div style='width:600px;padding:20px;border:1px solid #DBDBDB;border-radius:12px;font-family:Sans-serif'> <h1 style='color:#C76F61'>¡WhaleSports Te da la bienvenida!</h1> <p style='margin-bottom:25px'>Estimado/a&nbsp;<b>{usuario.Nombre}</b>:</p> <p style='margin-bottom:25px'>Gracias por unirte a la familia de WhaleSports.</p>  <p style='margin-top:25px'>Gracias.</p> </div> </body> </html>";
            string para = usuario.Correo;
            string mensaje = usuario.Nombre;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                usuario.Fecha_Reg = DateTime.Now;
                SqlCommand cmd = new SqlCommand("RegistroUsuario", conn);
                cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                cmd.Parameters.AddWithValue("@Apellidos", usuario.Apellidos);
                cmd.Parameters.AddWithValue("@Correo", usuario.Correo);
                cmd.Parameters.AddWithValue("@Telefono", usuario.Telefono);
                cmd.Parameters.AddWithValue("@Contraseña", usuario.Contraseña);
                cmd.Parameters.AddWithValue("@Fecha_Reg", usuario.Fecha_Reg);
                cmd.Parameters.Add("registro", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                cmd.ExecuteNonQuery();
                registro = Convert.ToBoolean(cmd.Parameters["registro"].Value);

                if (registro)
                {
                    try
                    {
                        _emailService.SendEmail(usuario.Correo, asunto, html);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al enviar el correo" + ex.Message);
                        throw;
                    }
                    return RedirectToAction("LogIn", "Home");
                }
                else
                {
                    ModelState.AddModelError("Correo", "El correo ya está registrado.");
                    return View("SignUp", usuario);
                }
            }
        }
        [HttpGet]
        public IActionResult LogIn()
        {
            return View();
        }
        [HttpPost]
        public IActionResult LogIn(Usuario usuario)
        {
            string connStr = _configuration.GetConnectionString("StringCONSQLlocal");
            LogInUsuario.Ingresar(usuario, connStr);
            var usuarios = _log.Obtener(usuario, connStr);
            LogInAdministrador.Entrar(usuario, connStr);
            prueba = usuarios;
            if (usuario.id_usu != 0)
            {
                return RedirectToAction("Index", "Home", usuarios);
            }
            else if (usuario.id_adm != 0)
            {
                return RedirectToAction("administrador", "Home");
            }
            else
            {
                ViewBag.Vali = "Correo y/o Contraseña incorrecto";
                Captcha captcha = new Captcha();
                ViewBag.Cap = captcha.CrearCaptcha();
                return View();
            }
        }
        [HttpGet]
        public IActionResult Rescon()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Rescon(Usuario usuario, string email)
        {
            string para = usuario.Correo;
            // string html = $"<!DOCTYPE html><html html lang='es'><body body > <div style='width:600px;padding:20px;border:1px solid #DBDBDB;border-radius:12px;font-family:Sans-serif'> <h1 style='color:#C76F61'>Restablecer contraseña</h1> <p style='margin-bottom:25px'></p> <p style='margin-bottom:25px'>Se solicitó un restablecimiento de contraseña para tu cuenta, haz clic en el botón que aparece a continuación para cambiar tu contraseña.</p> <a href='{callbackUrl}' target='_blank' style='padding:12px;border-radius:12px;background-color:#6181C7;color:#fff;text-decoration:none'>Cambiar contraseña</a> <p style='margin-top:25px'>Gracias.</p> </div> </body> </html>";
            string mensaje = usuario.Nombre;
            try
            {
                 string connStr = _configuration.GetConnectionString("StringCONSQLlocal");
                Guid token = Guid.NewGuid();
                DateTime expiration = DateTime.Now.AddMinutes(10); //Tiempo de expiración del token

                using SqlConnection conn = new SqlConnection(connStr);
                conn.Open();

                // Eliminar tokens anteriores
                string deleteQuery = "DELETE FROM PasswordResetTokens WHERE Email = @Email";
                using SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn);
                deleteCmd.Parameters.AddWithValue("@Email", email);
                deleteCmd.ExecuteNonQuery();

                // Insertar nuevo token
                string insertQuery = @"
                INSERT INTO PasswordResetTokens (Email, Token, Expiration)
                VALUES (@Email, @Token, @Expiration)";
                using SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                insertCmd.Parameters.AddWithValue("@Email", email);
                insertCmd.Parameters.AddWithValue("@Token", token);
                insertCmd.Parameters.AddWithValue("@Expiration", expiration);
                insertCmd.ExecuteNonQuery();

                // Creación del link con el token
                string resetUrl = Url.Action("Formulariorescon", "Home", new { token = token }, Request.Scheme);
                string asunto = "Recuperación de contraseña";
                //HTML del correo
                string html = $@"
                <!DOCTYPE html>
                    <html html lang='es'>
                        <body body > 
                            <div style='width:600px;padding:20px;border:1px solid #DBDBDB;border-radius:12px;font-family:Sans-serif'> 
                                <h1 style='color:#C76F61'>Restablecer contraseña</h1> 
                                <p style='margin-bottom:25px'></p> 
                                <p style='margin-bottom:25px'>Se solicitó un restablecimiento de contraseña para tu cuenta, haz clic en el botón que aparece a continuación para cambiar tu contraseña.</p> 
                                <a href='{resetUrl}' target='_blank' style='padding:12px;border-radius:12px;background-color:#6181C7;color:#fff;text-decoration:none'>Cambiar contraseña</a> 
                                <p style='margin-top:25px'>Gracias.</p> 
                            </div> 
                        </body> 
                    </html>";
                _emailService.SendEmail(email, asunto, html);
                
                ViewBag.Mensaje = "Correo enviado correctamente";
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View();
        }
        [HttpGet]
        public IActionResult Formulariorescon(Guid token)
        {
            string connStr = _configuration.GetConnectionString("StringCONSQLlocal");
            using SqlConnection conn = new SqlConnection(connStr);
            conn.Open();
            string query = @"SELECT Email, Expiration FROM PasswordResetTokens 
                     WHERE Token = @Token";
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Token", token);
            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                DateTime expiration = (DateTime)reader["Expiration"];
                if (expiration >= DateTime.Now)
                {
                    // Token válido, mostrar vista para establecer nueva contraseña
                    ViewBag.Token = token;
                    return View();
                }
            }
            return RedirectToAction("TokenInvalido","Home");
        }
        [HttpPost]
        public IActionResult Formulariorescon(Guid token, string nuevaContraseña, string confirmarContraseña)
        {
            string connStr = _configuration.GetConnectionString("StringCONSQLlocal");
            using SqlConnection conn = new SqlConnection(connStr);
            conn.Open();

            string query = "SELECT Email, Expiration FROM PasswordResetTokens WHERE Token = @Token";
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Token", token);

            string email = null;
            DateTime expiration = DateTime.MinValue;

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    email = reader["Email"].ToString();
                    expiration = (DateTime)reader["Expiration"];
                }
            }
            if (email == null || expiration < DateTime.Now)
            {
                return RedirectToAction("TokenInvalido","Home");
            }
            if (nuevaContraseña != confirmarContraseña)
            {
                ViewBag.Token = token;
                ViewBag.Correcto = "Las contraseñas no coinciden.";
                return View(); // Vuelve a mostrar el formulario con el mensaje
            }
            // Aquí puedes actualizar la contraseña del usuario en la base de datos
            using SqlCommand updatecmd = new SqlCommand("ActualizarContraseña", conn);
            updatecmd.CommandType = CommandType.StoredProcedure;
            updatecmd.Parameters.AddWithValue("@Correo", email);
            updatecmd.Parameters.AddWithValue("@newcontraseña", nuevaContraseña); // En texto plano, será cifrada en SQL
            updatecmd.ExecuteNonQuery();

            string deleteQuery = "DELETE FROM PasswordResetTokens WHERE Token = @Token";
            using SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn);
            deleteCmd.Parameters.AddWithValue("@Token", token);
            deleteCmd.ExecuteNonQuery();

            ViewBag.Token = token;
            ViewBag.Correcto = "Contraseña restablecida correctamente.";
            return View();
            // return RedirectToAction("ResetCompleto");
        }
        public IActionResult TokenInvalido()
        {
            return View();
        }
        public IActionResult Perfil()
        {
            string connStr = _configuration.GetConnectionString("StringCONSQLlocal");
            var ocontact = _log.Obtener(prueba, connStr);
            ViewBag.Nombre = prueba.Nombre;
            ViewBag.id_usu = prueba.id_usu;
            return View(ocontact);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
