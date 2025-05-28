using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WS_2_0.Models;

namespace WS_2_0.Controllers
{
    public class PerfilController : Controller
    {
        private readonly IConfiguration _configuration;

        public PerfilController(IConfiguration configuration)
        {
            _configuration = configuration;
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
            string connStr = _configuration.GetConnectionString("StringCONSQLlocal");
            var actualizado = _cru.Editar(ocontact, connStr);
            if (actualizado)
            {
                
                HttpContext.Session.SetString("Nombre", ocontact.Nombre);
                HttpContext.Session.SetString("Correo", ocontact.Correo);
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