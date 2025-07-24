using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using WS_2_0.Models;
using Microsoft.Extensions.Options;
using WS_2_0.Services;
using System.Data;

public class AdministradorController : Controller
{
    private readonly IConfiguration _config;
    private readonly ILogger<AdministradorController> _logger;
    private readonly IConfiguration _configuration;
    public AdministradorController(ILogger<AdministradorController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }
    cruAdministrador _cruadm = new cruAdministrador();
    public IActionResult Index()
    {
        return View();
    }
    public IActionResult Empleados()
    {
        string connStr = _configuration.GetConnectionString("StringCONSQLlocal");
        var olista = _cruadm.Tabla(connStr);
        return View(olista);
    }
    [HttpGet]
    public IActionResult CrearAdministrador()
    {
        CargarRoles();
        return View(new Administrador());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CrearAdministrador(Administrador ocontacto)
    {
        string connStr = _configuration.GetConnectionString("StringCONSQLlocal");

        if (!ModelState.IsValid)
        {
            CargarRoles(); // recarga la lista si el modelo es inválido
            return View(ocontacto);
        }
        if (_cruadm.ValidarExistenciaAdministrador(ocontacto, connStr))
        {
            CargarRoles();
            ModelState.AddModelError("", "No se pudo guardar el administrador. El administrador ya existe.");
            return View(ocontacto);
        }
        var respuesta = _cruadm.Guardar(ocontacto, connStr);
        if (respuesta)
            return RedirectToAction("Empleados");
        else
        {
            CargarRoles();
            ModelState.AddModelError("", "No se pudo guardar el administrador. Verifique los datos o intente más tarde.");
            return View(ocontacto);
        }
    }
    [HttpGet]
    public IActionResult Editar(int idAdministrador)
    {
        string connStr = _configuration.GetConnectionString("StringCONSQLlocal");
        var ocontacto = _cruadm.Obtener(idAdministrador, connStr);
        return View(ocontacto);
    }
    [HttpPost]
    public IActionResult Editar(Administrador ocontacto)
    {
        string connStr = _configuration.GetConnectionString("StringCONSQLlocal");
        var respuestas = _cruadm.Editar(ocontacto, connStr);
        if (respuestas)
            return RedirectToAction("Admadmin");
        else
            return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Eliminar(Administrador administrador, string PasswordConfirm)
    {
        string connStr = _configuration.GetConnectionString("StringCONSQLlocal");
        var respuesta = _cruadm.Eliminar(administrador, PasswordConfirm, connStr);

        if (respuesta.Contraseña == "Contraseña incorrecta.")
        {
            TempData["Error"] = "La contraseña ingresada es incorrecta";
            return RedirectToAction("Empleados", "Administrador");

        }
        return RedirectToAction("Empleados", "Administrador");
    }
    private void CargarRoles()
    {
        string connStr = _configuration.GetConnectionString("StringCONSQLlocal");
        ViewBag.Roles = new SelectList(_cruadm.ObtenerRoles(connStr), "Id", "RolNombre");
    }
}