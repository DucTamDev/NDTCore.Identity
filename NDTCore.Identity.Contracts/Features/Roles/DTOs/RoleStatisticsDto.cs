namespace NDTCore.Identity.Contracts.Features.Roles.DTOs;

/// <summary>
/// Role statistics data transfer object
/// </summary>
public class RoleStatisticsDto
{
    /// <summary>
    /// Total number of roles
    /// </summary>
    public int TotalRoles { get; set; }

    /// <summary>
    /// Number of system roles
    /// </summary>
    public int SystemRoles { get; set; }

    /// <summary>
    /// Number of custom roles
    /// </summary>
    public int CustomRoles { get; set; }

    /// <summary>
    /// User count grouped by role name
    /// </summary>
    public Dictionary<string, int> UserCountByRole { get; set; } = new();
}

