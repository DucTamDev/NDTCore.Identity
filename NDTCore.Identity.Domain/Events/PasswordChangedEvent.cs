namespace NDTCore.Identity.Domain.Events;

/// <summary>
/// Domain event raised when a user's password is changed
/// </summary>
public sealed class PasswordChangedEvent
{
    public Guid UserId { get; }
    public DateTime ChangedAt { get; }
    public string? ChangedBy { get; }
    public bool IsAdminReset { get; }

    public PasswordChangedEvent(Guid userId, DateTime changedAt, string? changedBy, bool isAdminReset)
    {
        UserId = userId;
        ChangedAt = changedAt;
        ChangedBy = changedBy;
        IsAdminReset = isAdminReset;
    }
}

