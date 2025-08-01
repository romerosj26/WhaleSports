using Microsoft.AspNetCore.Mvc;
using WS_2_0.Models;

public class ProductosController : Controller
{
    private readonly IConfiguration _config;
    private readonly IConfiguration _configuration;
    public ProductosController(IConfiguration config, IConfiguration configuration)
    {
        _config = config;
        _configuration = configuration;
    }
    crudProductos _crudproductos = new crudProductos();
    public IActionResult Index(string busqueda = "")
    {
        string connStr = _config.GetConnectionString("StringCONSQLlocal");
        var productos = _crudproductos.Tabla(connStr);

        if (!string.IsNullOrEmpty(busqueda))
        {
            productos = productos
            .Where(p => p.Nombre.Contains(busqueda, StringComparison.OrdinalIgnoreCase))
            .ToList();
        }
        return View(productos);
    }
    public IActionResult VistaProducto()
    {
        return View();
    }
    [HttpGet]
    public IActionResult CrearProducto()
    {
        return View(new CrearProductoViewModel
        {
            Activo = true
        });
    }
    [HttpPost]
    public IActionResult CrearProducto(CrearProductoViewModel model)
    {
        string connStr = _configuration.GetConnectionString("StringCONSQLlocal");
        var nuevoProducto = new Producto
        {
            Nombre = model.Nombre,
            Descripcion = model.Descripcion,
            Precio = model.Precio,
            ImagenUrl = model.ImagenUrl,
            Stock = model.Stock,
            Categoria = model.Categoria,
            Activo = model.Activo
        };
        var respuesta = _crudproductos.Guardar(nuevoProducto, connStr);
        if (respuesta)
            return RedirectToAction("Index", "Productos");
        else
        {
            ModelState.AddModelError("", "No se pudo guardar al cliente. Verifique los datos o intente más tarde.");
            return View(model);
        }
    }
    [HttpGet]
    public IActionResult EditarProducto(int Id)
    {
        string connStr = _configuration.GetConnectionString("StringCONSQLlocal");
        var oContacto = _crudproductos.Obtener(Id, connStr);
        if (oContacto == null)
            return NotFound();

        var viewModel = new EditarProductoViewModel
        {
            Id = oContacto.Id,
            Nombre = oContacto.Nombre,
            Descripcion = oContacto.Descripcion,
            Precio = oContacto.Precio,
            ImagenUrl = oContacto.ImagenUrl,
            Stock = oContacto.Stock,
            Activo = oContacto.Activo,
            Categoria = oContacto.Categoria
        };
        return View(viewModel);
    }
    [HttpPost]
    public IActionResult EditarProducto(EditarProductoViewModel model)
    {
        string connStr = _configuration.GetConnectionString("StringCONSQLlocal");
        ModelState.Remove("ImagenArchivo");
        if (!ModelState.IsValid)
        {
            var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            foreach(var err in errores)
                Console.WriteLine("Error: " + err);
        
            return View(model);
        }
        string imagenFinal = model.ImagenUrl;
        if (model.ImagenArchivo != null && model.ImagenArchivo.Length > 0)
        {
            string carpetaDestino = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "css", "images", "productos");

            if (!Directory.Exists(carpetaDestino))
                Directory.CreateDirectory(carpetaDestino);

            string nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(model.ImagenArchivo.FileName);
            string rutaArchivo = Path.Combine(carpetaDestino, nombreArchivo);

            using (var stream = new FileStream(rutaArchivo, FileMode.Create))
            {
                model.ImagenArchivo.CopyTo(stream);
            }

            imagenFinal = nombreArchivo;
        }

        var oContacto = new Producto
        {
            Id = model.Id,
            Nombre = model.Nombre,
            Descripcion = model.Descripcion,
            Precio = model.Precio,
            ImagenUrl = imagenFinal,
            Stock = model.Stock,
            Activo = model.Activo,
            Categoria = model.Categoria
        };

        var respuesta = _crudproductos.Editar(oContacto, connStr);

        if (respuesta)
            return RedirectToAction("Index", "Productos");
        else
        {
            ModelState.AddModelError("", "Error al actualizar el producto.");
            return View(model);
        }
    }
}