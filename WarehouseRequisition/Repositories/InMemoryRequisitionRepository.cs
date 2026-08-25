using WarehouseRequisition.Data;
using WarehouseRequisition.Models;

namespace WarehouseRequisition.Repositories;

public class InMemoryRequisitionRepository : IRequisitionRepository
{
    private readonly InMemoryDataStore _store;
    private readonly IDataStorePersistence _persistence;

    public InMemoryRequisitionRepository(InMemoryDataStore store, IDataStorePersistence persistence)
    {
        _store = store;
        _persistence = persistence;
    }

    public IEnumerable<Requisition> GetAll()
    {
        lock (_store.SyncRoot)
        {
            return _store.Requisitions.OrderByDescending(r => r.CreatedAt).ToList();
        }
    }

    public Requisition? GetById(int id)
    {
        lock (_store.SyncRoot)
        {
            return _store.Requisitions.FirstOrDefault(r => r.Id == id);
        }
    }

    public Requisition? GetByNumber(string requisitionNumber)
    {
        lock (_store.SyncRoot)
        {
            return _store.Requisitions.FirstOrDefault(r =>
                string.Equals(r.RequisitionNumber, requisitionNumber, StringComparison.OrdinalIgnoreCase));
        }
    }

    public void Add(Requisition requisition)
    {
        lock (_store.SyncRoot)
        {
            requisition.Id = _store.Requisitions.Count == 0 ? 1 : _store.Requisitions.Max(r => r.Id) + 1;
            requisition.Items.ForEach(i =>
            {
                i.Id = 0;
                i.RequisitionId = requisition.Id;
            });
            _store.Requisitions.Add(requisition);
            _persistence.Save(_store);
        }
    }

    public void Update(Requisition requisition)
    {
        lock (_store.SyncRoot)
        {
            var index = _store.Requisitions.FindIndex(r => r.Id == requisition.Id);
            if (index < 0)
            {
                throw new InvalidOperationException($"Requisition {requisition.Id} was not found.");
            }

            _store.Requisitions[index] = requisition;
            _persistence.Save(_store);
        }
    }

    public void Delete(int id)
    {
        lock (_store.SyncRoot)
        {
            var removed = _store.Requisitions.RemoveAll(r => r.Id == id);
            if (removed > 0)
            {
                _persistence.Save(_store);
            }
        }
    }

    public string GetNextRequisitionNumber(DateTime requestedDate)
    {
        lock (_store.SyncRoot)
        {
            var prefix = $"REQ-{requestedDate:yyyyMMdd}-";
            var maxSequence = _store.Requisitions
                .Where(r => r.RequisitionNumber.StartsWith(prefix, StringComparison.Ordinal))
                .Select(r => int.TryParse(r.RequisitionNumber[prefix.Length..], out var sequence) ? sequence : 0)
                .DefaultIfEmpty(0)
                .Max();

            return $"{prefix}{maxSequence + 1:D4}";
        }
    }
}
