namespace TiendaHeaderDemo.Models;

/// <summary>
/// Represents one node of the header's category menu.
/// The same type is reused for all 3 levels (categoría → subcategoría → sub-subcategoría),
/// which keeps both the server-side render and the recursive partial simple.
/// </summary>
public class MenuItem
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = "#";

    /// <summary>Emoji/icon-font glyph shown only for level-1 items in the drawer.</summary>
    public string? Icon { get; set; }

    /// <summary>Optional small tag such as "Nuevo" or "Oferta" shown next to the name.</summary>
    public string? Badge { get; set; }

    public List<MenuItem> Children { get; set; } = new();
}
