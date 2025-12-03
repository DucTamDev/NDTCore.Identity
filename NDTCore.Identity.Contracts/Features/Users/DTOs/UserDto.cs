namespace NDTCore.Identity.Contracts.Features.Users.DTOs;

/// <summary>
/// User data transfer object
/// </summary>
public class UserDto
{
    /// <summary>
    /// User unique identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// User email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Username
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// First name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Full name (computed)
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// Phone number
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Avatar URL
    /// </summary>
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// Email confirmation status
    /// </summary>
    public bool EmailConfirmed { get; set; }

    /// <summary>
    /// Phone number confirmation status
    /// </summary>
    public bool PhoneNumberConfirmed { get; set; }

    /// <summary>
    /// Account active status
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Last login timestamp
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Account creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last update timestamp
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// User roles
    /// </summary>
    public List<string> Roles { get; set; } = new();
}

