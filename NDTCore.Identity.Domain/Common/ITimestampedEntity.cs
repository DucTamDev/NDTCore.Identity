namespace NDTCore.Identity.Domain.Common;

/// <summary>
/// Interface for entities with timestamp tracking
/// </summary>
public interface ITimestampedEntity
{
    DateTime? CreatedAt { get; set; }
    DateTime? UpdatedAt { get; set; }
}

