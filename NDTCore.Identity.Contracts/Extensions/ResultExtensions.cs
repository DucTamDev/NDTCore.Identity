using NDTCore.Identity.Contracts.Common;
using NDTCore.Identity.Contracts.Common.Pagination;
using NDTCore.Identity.Contracts.Common.Results;

namespace NDTCore.Identity.Contracts.Extensions;

/// <summary>
/// Extension methods provide convenient syntax only
/// Actual conversion logic lives in ApiResponse classes
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Syntactic sugar for ApiResponse.FromResult()
    /// </summary>
    public static ApiResponse<TData> ToApiResponse<TData>(this Result<TData> result)
        => ApiResponse<TData>.FromResult(result);

    /// <summary>
    /// Syntactic sugar for ApiResponse.FromResult()
    /// </summary>
    public static ApiResponse ToApiResponse(this Result result)
        => ApiResponse.FromResult(result);

    /// <summary>
    /// Syntactic sugar for PagedApiResponse.FromResult()
    /// </summary>
    public static PagedApiResponse<TData> ToPagedApiResponse<TData>(
        this Result<PaginatedCollection<TData>> result)
        => PagedApiResponse<TData>.FromResult(result);
}
