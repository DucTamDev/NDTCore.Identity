namespace NDTCore.Identity.Contracts.Common.Pagination;

////// <summary>
/// Represents pagination metadata for paginated API responses.
/// Contains information about the current page, total pages, and navigation capabilities.
/// </summary>
public sealed record PaginationMetadata
{
    /// <summary>
    /// The current page number (1-based).
    /// </summary>
    public int CurrentPage { get; init; }

    /// <summary>
    /// The number of items per page.
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// The total number of pages available.
    /// </summary>
    public int TotalPages { get; init; }

    /// <summary>
    /// The total number of records across all pages.
    /// </summary>
    public int TotalRecords { get; init; }

    /// <summary>
    /// Indicates whether there is a previous page available.
    /// </summary>
    public bool HasPrevious => CurrentPage > 1;

    /// <summary>
    /// Indicates whether there is a next page available.
    /// </summary>
    public bool HasNext => CurrentPage < TotalPages;

    /// <summary>
    /// Indicates whether this is the first page.
    /// </summary>
    public bool IsFirstPage => CurrentPage == 1;

    /// <summary>
    /// Indicates whether this is the last page.
    /// </summary>
    public bool IsLastPage => CurrentPage == TotalPages;

    /// <summary>
    /// Creates a new instance of PaginationMetadata with validation.
    /// </summary>
    /// <param name="currentPage">The current page number (must be >= 1).</param>
    /// <param name="pageSize">The number of items per page (must be >= 1).</param>
    /// <param name="totalRecords">The total number of records (must be >= 0).</param>
    /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
    public PaginationMetadata(int currentPage, int pageSize, int totalRecords)
    {
        if (currentPage < 1)
            throw new ArgumentException(
                message: "Current page must be greater than 0",
                paramName: nameof(currentPage));

        if (pageSize < 1)
            throw new ArgumentException(
                message: "Page size must be greater than 0",
                paramName: nameof(pageSize));

        if (totalRecords < 0)
            throw new ArgumentException(
                message: "Total records cannot be negative",
                paramName: nameof(totalRecords));

        CurrentPage = currentPage;
        PageSize = pageSize;
        TotalRecords = totalRecords;
        TotalPages = totalRecords > 0
            ? (int)Math.Ceiling((double)totalRecords / pageSize)
            : 0;
    }

    public static PaginationMetadata Empty => new(currentPage: 1, pageSize: 10, totalRecords: 0);

    public static PaginationMetadata Create(int currentPage, int pageSize, int totalRecords)
        => new(currentPage: currentPage, pageSize: pageSize, totalRecords: totalRecords);

    public int? PreviousPage => HasPrevious ? CurrentPage - 1 : null;

    public int? NextPage => HasNext ? CurrentPage + 1 : null;

    public int StartIndex => (CurrentPage - 1) * PageSize;

    public int EndIndex => Math.Min(StartIndex + PageSize - 1, TotalRecords - 1);

    public int ItemsOnCurrentPage => TotalRecords > 0
        ? Math.Min(PageSize, TotalRecords - StartIndex)
        : 0;

    public bool IsValidPage() => CurrentPage <= TotalPages || TotalPages == 0;
}
