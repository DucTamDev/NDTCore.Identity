namespace NDTCore.Identity.Contracts.Common;

/// <summary>
/// Pagination metadata
/// </summary>
public class PaginationMetadata
{
    public int CurrentPage { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }
    public bool HasPrevious { get; init; }
    public bool HasNext { get; init; }
}