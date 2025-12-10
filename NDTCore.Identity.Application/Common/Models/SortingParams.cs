namespace NDTCore.Identity.Application.Common.Models;

/// <summary>
/// Sorting parameters (field, direction)
/// </summary>
public class SortingParams
{
    public string? SortBy { get; set; }
    public SortDirection Direction { get; set; } = SortDirection.Ascending;
}

/// <summary>
/// Sort direction enumeration
/// </summary>
public enum SortDirection
{
    Ascending,
    Descending
}

