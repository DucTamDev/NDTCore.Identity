namespace NDTCore.Identity.Contracts.Common;

/// <summary>
/// API response wrapper for paginated data
/// </summary>
public class PagedApiResponse<TData> : ApiResponse<List<TData>>
{
    /// <summary>
    /// Pagination metadata
    /// </summary>
    public PaginationMetadata Pagination { get; init; } = new();

    public static PagedApiResponse<TData> Ok(
        List<TData> data,
        int pageNumber,
        int pageSize,
        int totalCount,
        string message = "Data retrieved successfully")
    {
        return new PagedApiResponse<TData>
        {
            Success = true,
            Message = message,
            Data = data,
            StatusCode = 200,
            Pagination = new PaginationMetadata
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                HasPrevious = pageNumber > 1,
                HasNext = pageNumber < (int)Math.Ceiling(totalCount / (double)pageSize)
            }
        };
    }
}
