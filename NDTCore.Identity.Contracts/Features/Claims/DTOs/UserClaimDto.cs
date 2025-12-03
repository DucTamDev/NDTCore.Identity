namespace NDTCore.Identity.Contracts.Features.Claims.DTOs;

/// <summary>
/// User claim data transfer object
/// </summary>
public class UserClaimDto
{
    /// <summary>
    /// Claim ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// User ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// User name
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Claim type
    /// </summary>
    public string ClaimType { get; set; } = string.Empty;

    /// <summary>
    /// Claim value
    /// </summary>
    public string ClaimValue { get; set; } = string.Empty;
}

