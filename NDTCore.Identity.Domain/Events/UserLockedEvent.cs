namespace NDTCore.Identity.Domain.Events;

/// <summary>
/// Domain event raised when a user is locked
/// </summary>
public sealed class UserLockedEvent
{
    public Guid UserId { get; }
    public string Reason { get; }
    public DateTime LockedAt { get; }
    public DateTime? LockoutEnd { get; }

    public UserLockedEvent(Guid userId, string reason, DateTime lockedAt, DateTime? lockoutEnd)
    {
        UserId = userId;
        Reason = reason;
        LockedAt = lockedAt;
        LockoutEnd = lockoutEnd;
    }
}

