using System.ComponentModel.DataAnnotations;

namespace NDTCore.Identity.Contracts.Features.Claims.Requests;

/// <summary>
/// Request model for updating a claim
/// </summary>
public class UpdateClaimRequest
{
    /// <summary>
    /// Claim type
    /// </summary>
    [Required(ErrorMessage = "Claim type is required")]
    [StringLength(200, ErrorMessage = "Claim type cannot exceed 200 characters")]
    public string ClaimType { get; set; } = string.Empty;

    /// <summary>
    /// Claim value
    /// </summary>
    [Required(ErrorMessage = "Claim value is required")]
    [StringLength(500, ErrorMessage = "Claim value cannot exceed 500 characters")]
    public string ClaimValue { get; set; } = string.Empty;
}

