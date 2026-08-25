namespace WarehouseRequisition.Common;

public interface ICurrentUserService
{
    int UserId { get; }

    string DisplayName { get; }
}

/// <summary>
/// Prototype stand-in for authentication. Replace with ASP.NET Core Identity
/// or the corporate directory when real authentication is introduced.
/// </summary>
public class MockCurrentUser : ICurrentUserService
{
    public int UserId => 5;

    public string DisplayName => "María Fernanda López";
}
