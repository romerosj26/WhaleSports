using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WS_2_0.Models;
using WS_2_0.Models.ViewModels;
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
    crudAdministrador _crudadm = new crudAdministrador();
    crudClientes _crudclientes = new crudClientes();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Index()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Administradores()
    {
        int? id = HttpContext.Session.GetInt32("idAdministrador");
        if (id == null)
        {
            return RedirectToAction("LogIn", "Home");
        }
        string connStr = _configuration.GetConnectionString("StringCONSQLlocal");
        var olista = _crudadm.Tabla(connStr);
        return View(olista);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [HttpGet]
    public IActionResult CrearAdministrador()
    {
        var viewModel = new CrearAdministradorViewModel
        {
            RolesDisponibles = ObtenerRolesParaSelect()
        };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CrearAdministrador(CrearAdministradorViewModel model)
    {
        string connStr = _configuration.GetConnectionString("StringCONSQLlocal");

        if (!ModelState.IsValid)
        {
            model.RolesDisponibles = ObtenerRolesParaSelect(); // importante: recargar el select
            return View(model);
        }
        var nuevoAdmin = new Administrador
        {
            Nombre = model.Nombre,
            Apellido = model.Apellido,
            Correo = model.Correo,
            Contraseña = model.Contraseña,
            Telefono = model.Telefono,
            RolAdminId = model.RolAdminId
        };
        if (_crudadm.ValidarExistenciaAdministrador(nuevoAdmin, connStr))
        {
            model.RolesDisponibles = ObtenerRolesParaSelect();
            ModelState.AddModelError("", "No se pudo guardar el administrador. El administrador ya existe.");
            return View(model);
        }
        var respuesta = _crudadm.Guardar(nuevoAdmin, connStr);
        if (respuesta)
            return RedirectToAction("Administradores");
        else
        {
            model.RolesDisponibles = ObtenerRolesParaSelect();
            ModelState.AddModelError("", "No se pudo guardar el administrador. Verifique los datos o intente más tarde.");
            return View(model);
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [HttpGet]
    public IActionResult EditarAdministrador(int idAdministrador)
    {
        string connStr = _configuration.GetConnectionString("StringCONSQLlocal");
        var ocontacto = _crudadm.Obtener(idAdministrador, connStr);
        if (ocontacto == null)
            return NotFound();

        var viewModel = new EditarAdministradorViewModel
        {
            idAdministrador = ocontacto.idAdministrador,
            Nombre = ocontacto.Nombre,
            Apellido = ocontacto.Apellido,
            Correo = ocontacto.Correo,
            Telefono = ocontacto.Telefono,
            RolAdminId = ocontacto.RolAdminId,
            RolesDisponibles = ObtenerRolesParaSelect()
        };

        return View(viewModel);
    }
    [HttpPost]
    public IActionResult EditarAdministrador(EditarAdministradorViewModel model)
    {
        string connStr = _configuration.GetConnectionString("StringCONSQLlocal");
        if (!ModelState.IsValid)
        {
            ViewBag.RolesDisponibles = ObtenerRolesParaSelect();
            return View(model);
        }
        var ocontacto = new Administrador
        {
            idAdministrador = model.idAdministrador,
            Nombre = model.Nombre,
            Apellido = model.Apellido,
            Correo = model.Correo,
            Telefono = model.Telefono,
            RolAdminId = model.RolAdminId
        };
        var respuestas = _crudadm.Editar(ocontacto, connStr);
        if (respuestas)
            return RedirectToAction("Administradores");
        else
        {
            ViewBag.RolesDisponibles = ObtenerRolesParaSelect();
            ModelState.AddModelError("", "Error al actualizar el administrador.");
            return View();
        }

    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Eliminar(int idAdministrador, string PasswordConfirm)
    {
        string connStr = _configuration.GetConnectionString("StringCONSQLlocal");
        var respuesta = _crudadm.Eliminar(idAdministrador, PasswordConfirm, connStr);

        if (!respuesta.Exito)
        {
            ModelState.AddModelError(string.Empty, respuesta.Mensaje);
            return RedirectToAction("Administradores", "Administrador");
        }
        return RedirectToAction("Administradores", "Administrador");
    }
    private List<SelectListItem> ObtenerRolesParaSelect()
    {
        string connStr = _configuration.GetConnectionString("StringCONSQLlocal");
        var roles = _crudadm.ObtenerRoles(connStr);
        return roles.Select(r => new SelectListItem
        {
            Value = r.Id.ToString(),
            Text = r.RolNombre
        }).ToList();
    }

    public IActionResult Clientes()
    {
        string connStr = _configuration.GetConnectionString("StringCONSQLlocal");
        var listaClientes = _crudclientes.Tabla(connStr);
        var viewModel = new TablaClientesViewModel
        {
            Usuarios = listaClientes
        };
        return View(viewModel);
    }
    [HttpGet]
    public IActionResult CrearCliente()
    {
        return View();
    }
    [HttpPost]
    public IActionResult CrearCliente(CrearClienteViewModel model)
    {
        string connStr = _configuration.GetConnectionString("StringCONSQLlocal");
        var nuevoCliente = new Usuario
        {
            Nombre = model.Nombre,
            Apellidos = model.Apellidos,
            Correo = model.Correo,
            Contraseña = model.Contraseña,
            Telefono = model.Telefono
        };
        if (_crudclientes.ValidarExistenciaCliente(nuevoCliente, connStr))
        {
            ModelState.AddModelError("", "No se pudo crear al cliente. El cliente ya existe.");
            return View(model);
        }
        var respuesta = _crudclientes.Guardar(nuevoCliente, connStr);
        if (respuesta)
            return RedirectToAction("Clientes");
        else
        {
            ModelState.AddModelError("", "No se pudo guardar al cliente. Verifique los datos o intente más tarde.");
            return View(model);
        }
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [HttpGet]
    public IActionResult EditarCliente(int id_usu)
    {
        string connStr = _configuration.GetConnectionString("StringCONSQLlocal");
        var ocontacto = _crudclientes.ObtenerCliente(id_usu, connStr);
        if (ocontacto == null)
            return NotFound();

        var viewModel = new EditarClienteViewModel
        {
            id_usu = ocontacto.id_usu,
            Nombre = ocontacto.Nombre,
            Apellidos = ocontacto.Apellidos,
            Correo = ocontacto.Correo,
            Telefono = ocontacto.Telefono,
            Activo = ocontacto.Activo
        };

        return View(viewModel);
    }
    [HttpPost]
    public IActionResult EditarCliente(EditarClienteViewModel model)
    {
        string connStr = _configuration.GetConnectionString("StringCONSQLlocal");
        if (!ModelState.IsValid)
        {
            ViewBag.RolesDisponibles = ObtenerRolesParaSelect();
            return View(model);
        }
        var ocontacto = new Usuario
        {
            id_usu = model.id_usu,
            Nombre = model.Nombre,
            Apellidos = model.Apellidos,
            Correo = model.Correo,
            Telefono = model.Telefono,
            Activo = model.Activo
        };
        var respuestas = _crudclientes.EditarCliente(ocontacto, connStr);
        if (respuestas)
            return RedirectToAction("Clientes");
        else
        {
            ModelState.AddModelError("", "Error al actualizar al cliente.");
            return View();
        }

    }
}