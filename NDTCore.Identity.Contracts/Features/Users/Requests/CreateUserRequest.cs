using System.ComponentModel.DataAnnotations;

namespace NDTCore.Identity.Contracts.Features.Users.Requests;

/// <summary>
/// Request model for creating a new user
/// </summary>
public class CreateUserRequest
{
    /// <summary>
    /// User email address
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Username
    /// </summary>
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Password
    /// </summary>
    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; } = string.Empty;

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
}

