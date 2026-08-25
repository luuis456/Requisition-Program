using WarehouseRequisition.Enums;

namespace WarehouseRequisition.Models;

public class User
{
    public int Id { get; set; }

    public string EmployeeNumber { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    public bool IsActive { get; set; }
}
