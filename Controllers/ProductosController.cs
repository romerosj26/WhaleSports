using Microsoft.AspNetCore.Mvc;
using WS_2_0.Models;

public class ProductosController : Controller
{
    private readonly IConfiguration _config;
    private readonly cruProductos _cruproducto = new cruProductos();

    public ProductosController(IConfiguration config)
    {
        _config = config;
    }

    public IActionResult Index(string busqueda = "")
    {
        string conexion = _config.GetConnectionString("StringCONSQLlocal");
        var productos = _cruproducto.ObtenerProductos(conexion);

        if (!string.IsNullOrEmpty(busqueda))
        {
            productos = productos
            .Where(p => p.Nombre.Contains(busqueda, StringComparison.OrdinalIgnoreCase))
            .ToList();
        }

        return View(productos);
    }
}