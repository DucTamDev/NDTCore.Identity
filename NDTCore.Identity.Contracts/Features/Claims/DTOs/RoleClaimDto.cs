namespace NDTCore.Identity.Contracts.Features.Claims.DTOs;

/// <summary>
/// Role claim data transfer object
/// </summary>
public class RoleClaimDto
{
    /// <summary>
    /// Claim ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Role ID
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// Role name
    /// </summary>
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// Claim type
    /// </summary>
    public string ClaimType { get; set; } = string.Empty;

    /// <summary>
    /// Claim value
    /// </summary>
    public string ClaimValue { get; set; } = string.Empty;
}

