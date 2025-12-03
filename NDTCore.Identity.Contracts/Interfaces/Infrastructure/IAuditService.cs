namespace NDTCore.Identity.Contracts.Interfaces.Infrastructure;

/// <summary>
/// Service interface for audit logging
/// </summary>
public interface IAuditService
{
    /// <summary>
    /// Logs an audit entry for an entity operation
    /// </summary>
    /// <param name="entityType">Type of entity (e.g., "User", "Role")</param>
    /// <param name="entityId">ID of the entity</param>
    /// <param name="action">Action performed (Create, Update, Delete, etc.)</param>
    /// <param name="oldValues">Old values before the change (will be serialized to JSON)</param>
    /// <param name="newValues">New values after the change (will be serialized to JSON)</param>
    /// <param name="userId">ID of the user who performed the action</param>
    /// <param name="userName">Username of the user who performed the action</param>
    /// <param name="ipAddress">IP address of the client</param>
    /// <param name="userAgent">User agent of the client</param>
    /// <param name="httpMethod">HTTP method used</param>
    /// <param name="requestPath">Request path/URL</param>
    /// <param name="notes">Additional notes or comments</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task LogAsync(
        string entityType,
        Guid entityId,
        string action,
        object? oldValues = null,
        object? newValues = null,
        Guid? userId = null,
        string? userName = null,
        string? ipAddress = null,
        string? userAgent = null,
        string? httpMethod = null,
        string? requestPath = null,
        string? notes = null,
        CancellationToken cancellationToken = default);
}

