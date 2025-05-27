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
        public async Task<IActionResult> SubirImagen(IFormFile ImagenFile)
        {
            var idUsuario = HttpContext.Session.GetInt32("id_usu");
            if (idUsuario == null)
            {
                return RedirectToAction("LogIn", "Home");
            }

            if (ImagenFile != null && ImagenFile.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(ImagenFile.FileName)}";
                var carpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/css/Images/perfiles");

                if (!Directory.Exists(carpeta))
                {
                    Directory.CreateDirectory(carpeta);
                }
                string connStr = _configuration.GetConnectionString("StringCONSQLlocal");
                string imagenAnterior = null;

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    SqlCommand getcmd = new SqlCommand("SELECT ImagenPerfil FROM Usuario WHERE id_usu = @id", conn);
                    getcmd.Parameters.AddWithValue("@id", idUsuario);
                    var result = getcmd.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        imagenAnterior = result?.ToString();
                    }
                }
                // Borrar imagen anterior si existe
                if (!string.IsNullOrEmpty(imagenAnterior))
                {
                    var rutaImagenAnterior = Path.Combine(carpeta, imagenAnterior);
                    if (System.IO.File.Exists(rutaImagenAnterior))
                    {
                        System.IO.File.Delete(rutaImagenAnterior);
                    }
                }
                // Guardar nueva imagen
                var path = Path.Combine(carpeta, fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await ImagenFile.CopyToAsync(stream);
                }

                // Actualizar el nombre de la imagen en la BD
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("UPDATE Usuario SET ImagenPerfil = @Imagen WHERE id_usu = @id", conn);
                    cmd.Parameters.AddWithValue("@Imagen", fileName);
                    cmd.Parameters.AddWithValue("@id", idUsuario);
                    cmd.ExecuteNonQuery();
                }
            }

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