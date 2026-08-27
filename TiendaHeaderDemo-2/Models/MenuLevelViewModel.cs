namespace TiendaHeaderDemo.Models;

/// <summary>
/// Wraps a slice of the menu tree plus its depth, so the recursive
/// _MenuLevel partial knows how deep it is (for styling/caret choices)
/// without relying on ViewData.
/// </summary>
public class MenuLevelViewModel
{
    public List<MenuItem> Items { get; set; } = new();
    public int Level { get; set; } = 1;
}
