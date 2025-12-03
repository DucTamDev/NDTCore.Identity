namespace NDTCore.Identity.Contracts.Interfaces.Infrastructure;

/// <summary>
/// Service interface for accessing current user context
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the current user ID
    /// </summary>
    Guid? UserId { get; }

    /// <summary>
    /// Gets the current username
    /// </summary>
    string? UserName { get; }

    /// <summary>
    /// Gets the current user email
    /// </summary>
    string? Email { get; }

    /// <summary>
    /// Checks if the current user is authenticated
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Gets the IP address of the current request
    /// </summary>
    string? IpAddress { get; }

    /// <summary>
    /// Gets the user agent of the current request
    /// </summary>
    string? UserAgent { get; }
}

