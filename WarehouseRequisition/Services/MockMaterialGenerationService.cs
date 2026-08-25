using WarehouseRequisition.ViewModels;

namespace WarehouseRequisition.Services;

/// <summary>
/// Deterministic mock: derives pseudo-random picks from the order number + line so
/// the same order always produces the same material list during demos.
/// </summary>
public class MockMaterialGenerationService : IMaterialGenerationService
{
    private readonly IPartService _partService;

    public MockMaterialGenerationService(IPartService partService)
    {
        _partService = partService;
    }

    public List<RequisitionItemInputViewModel> Generate(AutoGenerateMaterialsRequest request)
    {
        var catalog = _partService.Search(null, maxResults: 50);
        if (catalog.Count == 0)
        {
            return [];
        }

        var seed = HashSeed($"{request.OrderNumber}|{request.Line}");
        var random = new Random(seed);
        var usedIndexes = new HashSet<int>();

        var items = new List<RequisitionItemInputViewModel>();
        for (var i = 0; i < request.Quantity; i++)
        {
            int index;
            do
            {
                index = random.Next(catalog.Count);
            } while (!usedIndexes.Add(index));

            var part = catalog[index];
            items.Add(new RequisitionItemInputViewModel
            {
                PartNumber = part.PartNumber,
                Description = part.Description,
                RequestedQuantity = random.Next(1, 13),
                QuantityDescription = null,
                UnitOfMeasure = part.UnitOfMeasure,
                Location = part.DefaultLocation,
                Observations = $"Orden {request.OrderNumber} · Línea {request.Line}"
            });
        }

        return items;
    }

    private static int HashSeed(string value)
    {
        unchecked
        {
            var hash = 17;
            foreach (var character in value)
            {
                hash = hash * 31 + character;
            }

            return hash;
        }
    }
}
