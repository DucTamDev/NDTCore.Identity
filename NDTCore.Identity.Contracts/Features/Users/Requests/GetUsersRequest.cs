using System.ComponentModel.DataAnnotations;

namespace NDTCore.Identity.Contracts.Features.Users.Requests;

/// <summary>
/// Request model for getting paginated list of users
/// </summary>
public class GetUsersRequest
{
    /// <summary>
    /// Page number (1-based)
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Page size
    /// </summary>
    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Search term for filtering users
    /// </summary>
    [StringLength(200)]
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Field to sort by
    /// </summary>
    [StringLength(50)]
    public string? SortBy { get; set; }

    /// <summary>
    /// Sort direction (true = descending, false = ascending)
    /// </summary>
    public bool SortDescending { get; set; }
}

