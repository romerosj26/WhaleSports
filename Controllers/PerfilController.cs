using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WS_2_0.Models;
using Microsoft.Extensions.Options;
using WS_2_0.Services;


namespace WS_2_0.Controllers
{
    public class PerfilController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly EmailSettings _emailSettings;
        private readonly EmailService _emailService;

        public PerfilController(IConfiguration configuration,IOptions<EmailSettings> emailOptions, EmailService emailService)
        {
            _configuration = configuration;
            _emailSettings = emailOptions.Value;
            _emailService = emailService;
        }
        CRU _cru = new CRU();
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index()
        {

            int? id = HttpContext.Session.GetInt32("id_usu");
            if (id == null)
            {
                return RedirectToAction("LogIn", "Home");
            }

            string connStr = _configuration.GetConnectionString("StringCONSQLlocal");
            var usuario = _cru.Obtener((int)id, connStr);
            return View(usuario);
        }
        [HttpPost]
        public async Task<IActionResult> ActualizarFotoPerfil(IFormFile ImagenFile)
        {
            if (ImagenFile == null || ImagenFile.Length == 0)
            {
                return RedirectToAction("Index");
            }

            var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(ImagenFile.FileName).ToLower();

            if (!extensionesPermitidas.Contains(extension))
            {
                //mostrar error de tipo de archivo no permitido
                return RedirectToAction("Index");
            }
            int ? id = HttpContext.Session.GetInt32("id_usu");
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            using var ms = new MemoryStream();
            await ImagenFile.CopyToAsync(ms);


            var usuario = _cru.Obtener((int)id, _configuration.GetConnectionString("StringCONSQLlocal"));
            usuario.FotoPerfil = ms.ToArray();
            usuario.FotoPerfilExtension = extension;
    _cru.cambioFotoPerfil((int)id, ms.ToArray(), extension, _configuration.GetConnectionString("StringCONSQLlocal"));

            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult PerfilEdit()
        {
            int? id = HttpContext.Session.GetInt32("id_usu");
            if (id == null)
            {
                return RedirectToAction("LogIn", "Home");
            }
            string connStr = _configuration.GetConnectionString("StringCONSQLlocal");
            Usuario usuario = _cru.Obtener((int)id, connStr);
            return View(usuario);
        }
        [HttpPost]
        public IActionResult PerfilEdit(Usuario ocontact)
        {
            string asunto = "Confirmación de Correo - WhaleSports";
            string connStr = _configuration.GetConnectionString("StringCONSQLlocal");
            var actualizado = _cru.Editar(ocontact, connStr);

            if (actualizado.Exito)
            {
                HttpContext.Session.SetString("Nombre", ocontact.Nombre);
                HttpContext.Session.SetString("Correo", ocontact.Correo);
                if (actualizado.CorreoModificado && !string.IsNullOrEmpty(actualizado.Token))
                {
                    string confirmUrl = Url.Action("ConfirmarCorreo", "Home", new { token = actualizado.Token, email = actualizado.CorreoNuevo }, Request.Scheme);
                    string html = @$"
                            <!DOCTYPE html>
                                <html lang='es'>
                                    <body>
                                        <div style='width:600px;padding:20px;border:1px solid #DBDBDB;border-radius:12px;font-family:Sans-serif'>
                                            <h1 style='color:#C76F61'>¡WhaleSports Te da la bienvenida!</h1>
                                            <p style='margin-bottom:25px'>Estimado/a&nbsp;<b>{ocontact.Nombre}</b>:</p>
                                            <p style='margin-bottom:25px'>Gracias por unirte a la familia de WhaleSports.</p>  
                                            <p style='margin-top:25px'><a href='{confirmUrl}' style='padding:10px 20px;background-color:#C76F61;color:white;border-radius:5px;text-decoration:none;'>Confirmar Correo</a></p>
                                        </div>
                                    </body>
                                </html>";
                    _emailService.SendEmail(actualizado.CorreoNuevo, asunto, html);
                    // Para mostrar SweetAlert en la próxima carga:
                    TempData["CorreoModificado"] = true;
                }
                // Opcionalmente, actualizar sesión o TempData
                return RedirectToAction("Index", "Perfil"); // Redirige a la vista de perfil
            }
            else
            {
                return View(ocontact); // muestra los errores
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}