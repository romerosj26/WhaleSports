using System.Diagnostics;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using WS_2_0.Models;
using WS_2_0.Models.Logueo;
using WS_2_0.Models.SignUp;
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
        LogInUsuario _log = new LogInUsuario();
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SignUp(Usuario usuario)
        {
            //Valida si las contraseñas ingresadas coinciden//
            if (usuario.Contraseña != usuario.ConfirmContra)
            {
                ModelState.AddModelError("ConfirmContra", "Las contraseñas no coinciden.");
                return View("SignUp", usuario);
            }
            try
            {
                string connStr = _configuration.GetConnectionString("StringCONSQLlocal");
                string baseUrl = $"{Request.Scheme}://{Request.Host}"; // para el enlace de confirmación

                var resultado = SignupUsuario.Registro(usuario, connStr, baseUrl, _emailService);

                if (resultado.Exito)
                {
                    TempData["Mensaje"] = resultado.Mensaje;
                    ViewBag.RegistroCorrecto = TempData["Mensaje"];
                    return View();
                }
                else
                {
                    if (resultado.Mensaje == "El correo ya existe, por favor inicie sesión o ingrese otro correo.")
                    {
                        ModelState.AddModelError("Correo", "El correo ya existe, por favor inicie sesión o ingrese otro correo.");
                        return View("SignUp", usuario);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.RegistroError = "Error al enviar el correo electrónico";
                return View("SignUp", usuario);
            }
            return View();
            }   

        [HttpGet]
        public IActionResult ConfirmarCorreo(Guid token)
        {
            string connStr = _configuration.GetConnectionString("StringCONSQLlocal");
            using SqlConnection conn = new SqlConnection(connStr);
            conn.Open();
            // Verifica el correo y la expiración relacionado con el token //
            string query = "SELECT Email, Expiration FROM EmailConfirmTokens WHERE Token = @Token";
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
                ViewBag.Invalido = "TokenInvalido";
                return View("ConfirmarCorreo");
            }

            // Confirma el Correo mediante el token // 
            string update = "UPDATE Usuario SET EmailConfirmed = 1 WHERE Correo = @Email";
            using (SqlCommand updateCmd = new SqlCommand(update, conn))
            {
                updateCmd.Parameters.AddWithValue("@Email", email);
                // updateCmd.ExecuteNonQuery();
                int rowsAffected = updateCmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    // Borra el token del correo relacionado //
                    string delete = "DELETE FROM EmailConfirmTokens WHERE Token = @Token";
                    using (SqlCommand deleteCmd = new SqlCommand(delete, conn))
                    {
                        deleteCmd.Parameters.AddWithValue("@Token", token);
                        deleteCmd.ExecuteNonQuery();
                    }
                }
            }
            
            ViewBag.Email = email;
            ViewBag.Token = token;
            return View("ConfirmarCorreo");
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
            var usuarioValido = LogInUsuario.Ingresar(usuario, connStr);
            var usuarios = _log.Obtener(usuario, connStr);
            LogInAdministrador.Entrar(usuario, connStr);

            if (usuario.id_usu != 0)
            {
                if (!usuario.EmailConfirmed)
                {
                    ViewBag.Vali = "Debes confirmar tu correo antes de iniciar sesión.";
                    Captcha captcha = new Captcha();
                    ViewBag.Cap = captcha.CrearCaptcha();
                    return View();
                }
                HttpContext.Session.SetInt32("id_usu", usuarioValido.id_usu); // Guarda el id del usuario en la sesión
                HttpContext.Session.SetString("Nombre", usuarios.Nombre); // Guarda el nombre del usuario en la sesión
                return RedirectToAction("Index", "Home", usuarios);
            }
            else if (usuario.id_adm != 0)
            {
                return RedirectToAction("Index", "Administrador");
            }
            ViewBag.Validacion = "Correo y/o Contraseña incorrecto";
            return View("LogIn", usuario);  
        }
        //Enviar correo para restablecer contraseña//
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
                
                ViewBag.CorreoRestablecer = "Correo enviado correctamente";
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
            byte[] salt = PasswordHasher.GenerateSalt();
            byte[] hash = PasswordHasher.HashPassword(nuevaContraseña, salt);

            using SqlCommand updatecmd = new SqlCommand("ActualizarContraseña", conn);
            updatecmd.CommandType = CommandType.StoredProcedure;
            updatecmd.Parameters.AddWithValue("@Correo", email);
            updatecmd.Parameters.Add("@PasswordHash", SqlDbType.VarBinary, hash.Length).Value = hash;
            updatecmd.Parameters.Add("@PasswordSalt", SqlDbType.VarBinary, salt.Length).Value = salt;
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