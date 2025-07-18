using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Collections.Generic;
using WS_2_0.Models;
using WS_2_0.viewModels;
using WS_2_0.Data;

public class AdministradorController : Controller
{
    private readonly IConfiguration _config;
    private readonly ILogger<AdministradorController> _logger;
    private readonly IConfiguration _configuration;
     private readonly ApplicationDbContext _context;
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
    public IActionResult GuardarEmpleado()
    {
        // string connStr = _configuration.GetConnectionString("StringCONSQLlocal");
        // ViewBag.Roles = _cruadm.ObtenerRoles(connStr);
        var model = new EmpleadoViewModel
        {
            RolesDisponibles = ObtenerRoles()
        };
        return View(model);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult GuardarEmpleado(Administrador ocontacto)
    {
        if (!ModelState.IsValid)
    {
        // Volver a cargar los roles si hay errores
        model.RolesDisponibles = _context.Roles
            .Where(r => r.Activo)
            .Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = r.Nombre
            }).ToList();

        return View(model);
    }
        string connStr = _configuration.GetConnectionString("StringCONSQLlocal");
        var respuesta = _cruadm.Guardar(ocontacto, connStr);
        if (respuesta)
            return RedirectToAction("Empleados");
        else
            return View();
    }
}