namespace NDTCore.Identity.Domain.Events;

/// <summary>
/// Domain event raised when a role is assigned to a user
/// </summary>
public sealed class RoleAssignedEvent
{
    public Guid UserId { get; }
    public Guid RoleId { get; }
    public string RoleName { get; }
    public DateTime AssignedAt { get; }
    public string? AssignedBy { get; }

    public RoleAssignedEvent(Guid userId, Guid roleId, string roleName, DateTime assignedAt, string? assignedBy)
    {
        UserId = userId;
        RoleId = roleId;
        RoleName = roleName;
        AssignedAt = assignedAt;
        AssignedBy = assignedBy;
    }
}

