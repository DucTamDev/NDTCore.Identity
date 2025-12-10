using NDTCore.Identity.Domain.Common;

namespace NDTCore.Identity.Domain.Entities;

/// <summary>
/// Audit log entity for tracking all CRUD operations
/// </summary>
public class AuditLog : BaseEntity, IAggregateRoot
{
    /// <summary>
    /// Type of entity being audited (e.g., "User", "Role", "UserRole")
    /// </summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// ID of the entity being audited
    /// </summary>
    public Guid EntityId { get; set; }

    /// <summary>
    /// Action performed (Create, Update, Delete, etc.)
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// Old values before the change (JSON format)
    /// </summary>
    public string? OldValues { get; set; }

    /// <summary>
    /// New values after the change (JSON format)
    /// </summary>
    public string? NewValues { get; set; }

    /// <summary>
    /// ID of the user who performed the action
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Username of the user who performed the action
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// IP address of the client
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// User agent of the client
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// HTTP method used (GET, POST, PUT, DELETE, etc.)
    /// </summary>
    public string? HttpMethod { get; set; }

    /// <summary>
    /// Request path/URL
    /// </summary>
    public string? RequestPath { get; set; }

    /// <summary>
    /// Additional notes or comments
    /// </summary>
    public string? Notes { get; set; }
}

