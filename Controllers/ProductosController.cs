using Microsoft.AspNetCore.Mvc;
using WS_2_0.Models;

public class ProductosController : Controller
{
    private readonly IConfiguration _config;
    private readonly crudProductos _crudproducto = new crudProductos();

    public ProductosController(IConfiguration config)
    {
        _config = config;
    }

    public IActionResult Index(string busqueda = "")
    {
        string connStr = _config.GetConnectionString("StringCONSQLlocal");
        var productos = _crudproducto.Tabla(connStr);

        if (!string.IsNullOrEmpty(busqueda))
        {
            productos = productos
            .Where(p => p.Nombre.Contains(busqueda, StringComparison.OrdinalIgnoreCase))
            .ToList();
        }
        return View(productos);
    }
    [HttpGet]
    public IActionResult CrearProducto()
    {
        return View();
    }
}