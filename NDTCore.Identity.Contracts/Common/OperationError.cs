using NDTCore.Identity.Domain.Enums;

namespace NDTCore.Identity.Contracts.Common;

/// <summary>
/// Error details for operation results
/// </summary>
public class OperationError
{
    public string Message { get; init; } = string.Empty;
    public OperationErrorType ErrorType { get; init; }
    public Dictionary<string, string[]> ValidationErrors { get; init; } = new();
}
