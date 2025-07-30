using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WS_2_0.Models;
using WS_2_0.Models.ViewModels;
using Microsoft.Extensions.Options;
using WS_2_0.Services;
using System.Data;

namespace WS_2_0.Controllers
{
    public class PerfilController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly EmailSettings _emailSettings;
        private readonly EmailService _emailService;

        public PerfilController(IConfiguration configuration, IOptions<EmailSettings> emailOptions, EmailService emailService)
        {
            _configuration = configuration;
            _emailSettings = emailOptions.Value;
            _emailService = emailService;
        }
        crudClientes _cru = new crudClientes();
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
            int? id = HttpContext.Session.GetInt32("id_usu");
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarCuenta(string PasswordConfirm)
        {
            int? id = HttpContext.Session.GetInt32("id_usu");
            if (id == null)
            {
                return RedirectToAction("LogIn", "Home");
            }
            string connStr = _configuration.GetConnectionString("StringCONSQLlocal");
            byte[] hash = null, salt = null;
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT PasswordHash, PasswordSalt FROM Usuario WHERE id_usu = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    if (reader["PasswordHash"] == DBNull.Value || reader["PasswordSalt"] == DBNull.Value)
                    {
                        ViewBag.Error = "No se puede verificar la contraseña. Contacta al soporte.";
                        return RedirectToAction("Perfil");
                    }
                    hash = (byte[])reader["PasswordHash"];
                    salt = (byte[])reader["PasswordSalt"];
                }
            }
            if (PasswordHasher.VerificarContraseña(PasswordConfirm, hash, salt))
            {
                // Eliminar cuenta lógicamente
                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    var cmd = new SqlCommand("EliminarUsuarioLogico", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdUsuario", id);
                    cmd.Parameters.AddWithValue("@Descripcion", "Usuario solicitó la eliminación de su cuenta.");
                    cmd.ExecuteNonQuery();
                }
                HttpContext.Session.Clear();
                return RedirectToAction("Index", "Perfil");

            }
            TempData["Error"] = "La contraseña ingresada es incorrecta";
            return RedirectToAction("Index", "Perfil");
        }
        [HttpPost]
        public IActionResult EditarContraseña(EditarContraseñaViewModel model)
        {
            int? id = HttpContext.Session.GetInt32("id_usu");
            if (id == null)
            {
                return RedirectToAction("LogIn", "Home");
            }

            if (model.NuevaContraseña != model.ConfirmarContraseña)
            {
                ModelState.AddModelError("ConfirmarContraseña", "Las contraseñas no coinciden.");
                return RedirectToAction("Index", "Perfil");
            }

            string connStr = _configuration.GetConnectionString("StringCONSQLlocal");
            using SqlConnection conn = new SqlConnection(connStr);
            conn.Open();

            string query = @"
                    SELECT 
		                PasswordHash,
		                PasswordSalt
	                FROM Usuario 
	                WHERE id_usu = @id_usu
	                AND Activo = 1";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id_usu",id.Value);

            using SqlDataReader reader = cmd.ExecuteReader();  //la variable reader ejecuta el comando para leer los datos obtenidos del procedimiento almacenado Val//
            if (!reader.Read())
            {
                ModelState.AddModelError("", "Usuario no encontrado");
                return RedirectToAction("Index", "Perfil");
            }

            byte[] hashActual = (byte[])reader["PasswordHash"];
            byte[] saltActual = (byte[])reader["PasswordSalt"];

            if (!PasswordHasher.VerificarContraseña(model.ContraseñaActual, hashActual, saltActual))
            {
                ModelState.AddModelError("ContraseñaActual", "La contraseña actual es incorrecta.");
                return RedirectToAction("Index", "Perfil");
            }
            byte[] nuevaSalt = PasswordHasher.GenerateSalt();
            byte[] nuevoHash = PasswordHasher.HashPassword(model.NuevaContraseña, nuevaSalt);

            reader.Close();

            using (SqlCommand updateCmd = new SqlCommand("UPDATE Usuario SET PasswordHash = @PasswordHash,PasswordSalt=@PasswordSalt  WHERE id_usu = @id_usu", conn))
            {
                updateCmd.Parameters.AddWithValue("@id_usu", id.Value);
                updateCmd.Parameters.Add("@PasswordHash", SqlDbType.VarBinary, nuevoHash.Length).Value = nuevoHash;
                updateCmd.Parameters.Add("@PasswordSalt", SqlDbType.VarBinary, nuevaSalt.Length).Value = nuevaSalt;
                updateCmd.ExecuteNonQuery();
            }
            TempData["MensajeExito"] = "Contraseña actualizada correctamente.";
            return RedirectToAction("Index", "Perfil");
        }

    }
}