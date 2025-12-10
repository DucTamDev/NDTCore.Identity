using NDTCore.Identity.Domain.Constants;

namespace NDTCore.Identity.Domain.Exceptions;

/// <summary>
/// Exception thrown when an entity is not found
/// </summary>
public class NotFoundException : BaseDomainException
{
    public string EntityName { get; }
    public object? EntityKey { get; }

    public NotFoundException(string entityName, object? key)
        : base(
            ErrorCodes.NotFound,
            $"Entity '{entityName}' with key '{key}' was not found.",
            $"The requested {entityName} was not found.")
    {
        EntityName = entityName;
        EntityKey = key;
        ErrorDetails = new Dictionary<string, object>
        {
            { "EntityName", entityName },
            { "EntityKey", key?.ToString() ?? "null" }
        };
    }
}
