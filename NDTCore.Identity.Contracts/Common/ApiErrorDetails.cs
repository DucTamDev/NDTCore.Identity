namespace NDTCore.Identity.Contracts.Common;

/// <summary>
/// Detailed error information for API responses
/// </summary>
public class ApiErrorDetails
{
    public string Message { get; init; } = string.Empty;
    public Dictionary<string, List<string>>? ValidationErrors { get; init; }
    public string? StackTrace { get; init; }
}