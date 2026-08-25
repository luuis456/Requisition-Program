using WarehouseRequisition.Data;
using WarehouseRequisition.Models;

namespace WarehouseRequisition.Services;

public class PartService : IPartService
{
    private readonly InMemoryDataStore _store;

    public PartService(InMemoryDataStore store)
    {
        _store = store;
    }

    public Part? FindByPartNumber(string partNumber)
    {
        lock (_store.SyncRoot)
        {
            return _store.Parts.FirstOrDefault(p =>
                string.Equals(p.PartNumber, partNumber.Trim(), StringComparison.OrdinalIgnoreCase));
        }
    }

    public List<Part> Search(string? term, int maxResults = 20)
    {
        lock (_store.SyncRoot)
        {
            var query = _store.Parts.AsQueryable();
            if (!string.IsNullOrWhiteSpace(term))
            {
                term = term.Trim();
                query = query.Where(p =>
                    p.PartNumber.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    p.Description.Contains(term, StringComparison.OrdinalIgnoreCase));
            }

            return query.OrderBy(p => p.PartNumber).Take(maxResults).ToList();
        }
    }
}
