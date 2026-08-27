using Microsoft.AspNetCore.Mvc;
using TiendaHeaderDemo.Data;
using TiendaHeaderDemo.Models;

namespace TiendaHeaderDemo.ViewComponents;

/// <summary>
/// Renders the full header: top utility bar, main bar (logo/search/cart)
/// and the 3-level category navigation (desktop hover flyout + mobile
/// stacked drawer). Kept as a ViewComponent so _Layout can just do
/// @await Component.InvokeAsync("HeaderMenu") and the header stays
/// swappable/testable on its own.
/// </summary>
public class HeaderMenuViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        List<MenuItem> menu = MenuDataProvider.GetMenu();
        return View(menu);
    }
}
