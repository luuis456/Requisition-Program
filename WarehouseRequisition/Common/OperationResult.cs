namespace WarehouseRequisition.Common;

public class OperationResult
{
    public bool Success { get; init; }

    public string? ErrorMessage { get; init; }

    public static OperationResult Ok() => new() { Success = true };

    public static OperationResult Fail(string message) => new() { Success = false, ErrorMessage = message };
}

public class OperationResult<T> : OperationResult
{
    public T? Value { get; init; }

    public static OperationResult<T> Ok(T value) => new() { Success = true, Value = value };

    public new static OperationResult<T> Fail(string message) => new() { Success = false, ErrorMessage = message };
}
