namespace NDTCore.Identity.Contracts.Features.Users.DTOs;

/// <summary>
/// User statistics data transfer object
/// </summary>
public class UserStatisticsDto
{
    /// <summary>
    /// Total number of users
    /// </summary>
    public int TotalUsers { get; set; }

    /// <summary>
    /// Number of active users
    /// </summary>
    public int ActiveUsers { get; set; }

    /// <summary>
    /// Number of locked users
    /// </summary>
    public int LockedUsers { get; set; }

    /// <summary>
    /// Number of deleted users
    /// </summary>
    public int DeletedUsers { get; set; }

    /// <summary>
    /// Number of users created today
    /// </summary>
    public int UsersCreatedToday { get; set; }

    /// <summary>
    /// Number of users created this month
    /// </summary>
    public int UsersCreatedThisMonth { get; set; }
}

