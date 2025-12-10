namespace NDTCore.Identity.Infrastructure.Services.Infrastructure;

/// <summary>
/// DateTime abstraction implementation
/// </summary>
public class DateTimeProvider
{
    public virtual DateTime UtcNow => DateTime.UtcNow;
    public virtual DateTime Now => DateTime.Now;
}

