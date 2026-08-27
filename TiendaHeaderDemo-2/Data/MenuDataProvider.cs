using TiendaHeaderDemo.Models;

namespace TiendaHeaderDemo.Data;

/// <summary>
/// Temporary hardcoded source for the header's category tree.
/// Swap this out for an EF Core repository or a JSON/config-backed
/// service later without touching the view/component contract,
/// since both just depend on List&lt;MenuItem&gt;.
/// </summary>
public static class MenuDataProvider
{
    public static List<MenuItem> GetMenu()
    {
        return new List<MenuItem>
        {
            new()
            {
                Id = "electro", Name = "Electrónica", Icon = "📱", Url = "#",
                Children = new()
                {
                    new()
                    {
                        Id = "celulares", Name = "Celulares", Url = "#",
                        Children = new()
                        {
                            new() { Id = "smartphones", Name = "Smartphones", Url = "#" },
                            new() { Id = "celulares-basicos", Name = "Celulares básicos", Url = "#" },
                            new() { Id = "accesorios-cel", Name = "Fundas y protectores", Url = "#" },
                            new() { Id = "audifonos", Name = "Audífonos", Url = "#" },
                        }
                    },
                    new()
                    {
                        Id = "computo", Name = "Cómputo", Url = "#",
                        Children = new()
                        {
                            new() { Id = "laptops", Name = "Laptops", Url = "#" },
                            new() { Id = "escritorio", Name = "PC de escritorio", Url = "#" },
                            new() { Id = "monitores", Name = "Monitores", Url = "#" },
                            new() { Id = "impresoras", Name = "Impresoras", Url = "#" },
                        }
                    },
                    new()
                    {
                        Id = "tv-audio", Name = "TV y audio", Url = "#",
                        Children = new()
                        {
                            new() { Id = "pantallas", Name = "Pantallas", Url = "#", Badge = "Oferta" },
                            new() { Id = "barras-sonido", Name = "Barras de sonido", Url = "#" },
                            new() { Id = "bocinas", Name = "Bocinas portátiles", Url = "#" },
                        }
                    },
                }
            },
            new()
            {
                Id = "muebles", Name = "Muebles", Icon = "🛋️", Url = "#",
                Children = new()
                {
                    new()
                    {
                        Id = "sala", Name = "Sala", Url = "#",
                        Children = new()
                        {
                            new() { Id = "sillones", Name = "Sillones", Url = "#" },
                            new() { Id = "mesas-centro", Name = "Mesas de centro", Url = "#" },
                            new() { Id = "libreros", Name = "Libreros", Url = "#" },
                        }
                    },
                    new()
                    {
                        Id = "recamara", Name = "Recámara", Url = "#",
                        Children = new()
                        {
                            new() { Id = "camas", Name = "Camas", Url = "#" },
                            new() { Id = "colchones", Name = "Colchones", Url = "#", Badge = "Nuevo" },
                            new() { Id = "closets", Name = "Closets", Url = "#" },
                        }
                    },
                    new()
                    {
                        Id = "comedor", Name = "Comedor", Url = "#",
                        Children = new()
                        {
                            new() { Id = "mesas-comedor", Name = "Mesas de comedor", Url = "#" },
                            new() { Id = "sillas", Name = "Sillas", Url = "#" },
                            new() { Id = "vitrinas", Name = "Vitrinas", Url = "#" },
                        }
                    },
                }
            },
            new()
            {
                Id = "ropa", Name = "Ropa", Icon = "👕", Url = "#",
                Children = new()
                {
                    new()
                    {
                        Id = "mujer", Name = "Mujer", Url = "#",
                        Children = new()
                        {
                            new() { Id = "vestidos", Name = "Vestidos", Url = "#" },
                            new() { Id = "blusas", Name = "Blusas", Url = "#" },
                            new() { Id = "pantalones-mujer", Name = "Pantalones", Url = "#" },
                        }
                    },
                    new()
                    {
                        Id = "hombre", Name = "Hombre", Url = "#",
                        Children = new()
                        {
                            new() { Id = "playeras", Name = "Playeras", Url = "#" },
                            new() { Id = "pantalones-hombre", Name = "Pantalones", Url = "#" },
                            new() { Id = "chamarras", Name = "Chamarras", Url = "#" },
                        }
                    },
                    new()
                    {
                        Id = "ninos", Name = "Niños", Url = "#",
                        Children = new()
                        {
                            new() { Id = "ninas", Name = "Niña", Url = "#" },
                            new() { Id = "ninos-ropa", Name = "Niño", Url = "#" },
                            new() { Id = "bebes", Name = "Bebé", Url = "#" },
                        }
                    },
                }
            },
            new()
            {
                Id = "calzado", Name = "Calzado", Icon = "👟", Url = "#",
                Children = new()
                {
                    new()
                    {
                        Id = "deportivo", Name = "Deportivo", Url = "#",
                        Children = new()
                        {
                            new() { Id = "running", Name = "Running", Url = "#" },
                            new() { Id = "casual-dep", Name = "Casual deportivo", Url = "#" },
                        }
                    },
                    new()
                    {
                        Id = "formal", Name = "Formal", Url = "#",
                        Children = new()
                        {
                            new() { Id = "zapatos-vestir", Name = "Zapatos de vestir", Url = "#" },
                            new() { Id = "tacones", Name = "Tacones", Url = "#" },
                        }
                    },
                }
            },
            new()
            {
                Id = "hogar", Name = "Hogar y cocina", Icon = "🍳", Url = "#",
                Children = new()
                {
                    new()
                    {
                        Id = "electrodomesticos", Name = "Electrodomésticos", Url = "#",
                        Children = new()
                        {
                            new() { Id = "refrigeradores", Name = "Refrigeradores", Url = "#" },
                            new() { Id = "lavadoras", Name = "Lavadoras", Url = "#" },
                            new() { Id = "estufas", Name = "Estufas", Url = "#" },
                        }
                    },
                    new()
                    {
                        Id = "cocina", Name = "Cocina", Url = "#",
                        Children = new()
                        {
                            new() { Id = "ollas", Name = "Ollas y sartenes", Url = "#" },
                            new() { Id = "vajillas", Name = "Vajillas", Url = "#" },
                        }
                    },
                }
            },
            new()
            {
                Id = "motos", Name = "Motos", Icon = "🏍️", Url = "#",
                Children = new()
                {
                    new()
                    {
                        Id = "motos-trabajo", Name = "Motos de trabajo", Url = "#",
                        Children = new()
                        {
                            new() { Id = "moto-125", Name = "125cc", Url = "#" },
                            new() { Id = "moto-150", Name = "150cc", Url = "#" },
                        }
                    },
                    new()
                    {
                        Id = "refacciones", Name = "Refacciones", Url = "#",
                        Children = new()
                        {
                            new() { Id = "cascos", Name = "Cascos", Url = "#" },
                            new() { Id = "llantas-moto", Name = "Llantas", Url = "#" },
                        }
                    },
                }
            },
        };
    }
}
