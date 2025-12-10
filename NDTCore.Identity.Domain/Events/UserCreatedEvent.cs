namespace NDTCore.Identity.Domain.Events;

/// <summary>
/// Domain event raised when a user is created
/// </summary>
public sealed class UserCreatedEvent
{
    public Guid UserId { get; }
    public string Email { get; }
    public string FullName { get; }
    public DateTime CreatedAt { get; }

    public UserCreatedEvent(Guid userId, string email, string fullName, DateTime createdAt)
    {
        UserId = userId;
        Email = email;
        FullName = fullName;
        CreatedAt = createdAt;
    }
}

