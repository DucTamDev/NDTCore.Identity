using System.ComponentModel.DataAnnotations;

namespace NDTCore.Identity.Contracts.Features.Users.Requests;

/// <summary>
/// Request model for updating user profile
/// </summary>
public class UpdateUserRequest
{
    /// <summary>
    /// First name
    /// </summary>
    [Required(ErrorMessage = "First name is required")]
    [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last name
    /// </summary>
    [Required(ErrorMessage = "Last name is required")]
    [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Phone number (optional)
    /// </summary>
    [Phone(ErrorMessage = "Invalid phone number format")]
    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Address (optional)
    /// </summary>
    [StringLength(500)]
    public string? Address { get; set; }

    /// <summary>
    /// City (optional)
    /// </summary>
    [StringLength(100)]
    public string? City { get; set; }

    /// <summary>
    /// State (optional)
    /// </summary>
    [StringLength(100)]
    public string? State { get; set; }

    /// <summary>
    /// Zip code (optional)
    /// </summary>
    [StringLength(20)]
    public string? ZipCode { get; set; }

    /// <summary>
    /// Country (optional)
    /// </summary>
    [StringLength(100)]
    public string? Country { get; set; }

    /// <summary>
    /// Avatar URL (optional)
    /// </summary>
    [Url(ErrorMessage = "Invalid URL format")]
    [StringLength(500)]
    public string? AvatarUrl { get; set; }
}

