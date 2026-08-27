using Microsoft.AspNetCore.Mvc;

namespace TiendaHeaderDemo.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Error()
    {
        return Problem();
    }
}
