namespace NDTCore.Identity.Contracts.Features.Authentication.DTOs;

/// <summary>
/// User information data transfer object for authentication responses
/// </summary>
public class UserInfoDto
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
    /// Full name
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Avatar URL
    /// </summary>
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// Phone number
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Email confirmation status
    /// </summary>
    public bool EmailConfirmed { get; set; }

    /// <summary>
    /// Phone number confirmation status
    /// </summary>
    public bool PhoneNumberConfirmed { get; set; }

    /// <summary>
    /// User roles
    /// </summary>
    public List<string> Roles { get; set; } = new();

    /// <summary>
    /// Tenant ID (for multi-tenant scenarios)
    /// </summary>
    public string? TenantId { get; set; }
}

