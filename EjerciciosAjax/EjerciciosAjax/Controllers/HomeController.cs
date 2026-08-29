using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EjerciciosAjax.Models;

namespace EjerciciosAjax.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
    
    
    
    /*  IMPORTANTE!!!  APLICANDO SERIALIZACION   */
    
    /////// FORMA BASICA DE RECIBIR DATOS MEDIANTE UN FORMULARIO SERIALIZADO
    // public async Task<IActionResult> BasicFormSerializadoConParametrosSueltos(string nombre, int edad)
    // {
    //     Console.WriteLine("informacion que llega de ajax " + nombre + " edad " + edad);
    //     
    //     
    //     // se puede regresar badrequest o ok
    //     return Ok(new { succcess = true, message = "datos recibidos con exito" });      
    // }

    
    // serializado se puedes recibir de cualquiera de las formas, sea por parametros o el modelo, no tienen que haber atributos exactos en cantidad
    public class DatosPersona (){ public string Nombre {get; set;} public int Edad {get; set;} public string Apellido {get; set;}}
    [HttpPost]
    public async Task<IActionResult> BasicFormTestNoSerialize(DatosPersona persona)
    {
            
        Console.WriteLine("informacion que llega de ajax " + persona.Nombre + " edad " + persona.Edad);
        
        // se puede regresar badrequest o ok
        return Ok(new { succcess = true, message = "datos recibidos con exito" });      
    }

    
    /*   SIN APLICAR SERIALIZACION  */
    
    // Sin serializar se usa frombody porque convierte el json 
    [HttpPost]
    public async Task<IActionResult> BasicFormUsingFromBody([FromBody] DatosPersona persona)
    {
        Console.WriteLine("informacion que llega de ajax " + persona.Nombre + " edad " + persona.Edad);
        
        // se puede regresar badrequest o ok
        return Ok(new { succcess = true, message = "datos recibidos con exito" });      
    }
    
    
    
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Departamento { get; set; }
        public bool Activo { get; set; }
    }
    
    public class UsuarioFiltro
    {
        public string Nombre { get; set; }
        public string Departamento { get; set; }
        public bool Activo { get; set; }
    }
    
    [HttpPost]
    public IActionResult ObtenerUsuarios(UsuarioFiltro filtro)
    {
        // Simulamos datos de BD

        Console.WriteLine(filtro.Nombre +  " " + filtro.Departamento);   
        
        var usuarios = new List<Usuario>
        {
            new Usuario
            {
                Id = 1,
                Nombre = "Luis",
                Departamento = "Sistemas",
                Activo = true
            },

            new Usuario
            {
                Id = 2,
                Nombre = "Juan",
                Departamento = "Ventas",
                Activo = true
            },

            new Usuario
            {
                Id = 3,
                Nombre = "Pedro",
                Departamento = "Sistemas",
                Activo = false
            }
        };

        // Aplicamos filtros

        if (!string.IsNullOrEmpty(filtro.Nombre))
        {
            usuarios = usuarios
                .Where(x => x.Nombre.Contains(filtro.Nombre))
                .ToList();
        }

        if (!string.IsNullOrEmpty(filtro.Departamento))
        {
            usuarios = usuarios
                .Where(x => x.Departamento == filtro.Departamento)
                .ToList();
        }

        usuarios = usuarios
            .Where(x => x.Activo == filtro.Activo)
            .ToList();

        return Json(new
        {
            success = true,
            mensaje = "",
            data = usuarios
        });
    }
    

    

    
    
    
  
}