using WarehouseRequisition.Models;

namespace WarehouseRequisition.Repositories;

public interface IRequisitionRepository
{
    IEnumerable<Requisition> GetAll();

    Requisition? GetById(int id);

    Requisition? GetByNumber(string requisitionNumber);

    void Add(Requisition requisition);

    void Update(Requisition requisition);

    void Delete(int id);

    /// <summary>Generates the next unique REQ-YYYYMMDD-XXXX number for the given date.</summary>
    string GetNextRequisitionNumber(DateTime requestedDate);
}
