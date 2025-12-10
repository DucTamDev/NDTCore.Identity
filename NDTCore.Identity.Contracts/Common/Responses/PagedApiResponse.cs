using Microsoft.AspNetCore.Http;
using NDTCore.Identity.Contracts.Common.Pagination;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Helpers;

namespace NDTCore.Identity.Contracts.Common.Responses;

/// <summary>
/// Paginated API response
/// </summary>
public class PagedApiResponse<TData> : ApiResponse<IEnumerable<TData>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public static PagedApiResponse<TData> Success(
        IEnumerable<TData> data,
        int pageNumber,
        int pageSize,
        int totalCount,
        string message = "Data retrieved successfully")
    {
        return new PagedApiResponse<TData>
        {
            IsSuccess = true,
            Message = message,
            StatusCode = StatusCodes.Status200OK,
            Data = data,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public static PagedApiResponse<TData> Failure(
        int statusCode,
        string message,
        string? errorCode = null)
    {
        return new PagedApiResponse<TData>
        {
            IsSuccess = false,
            Message = message,
            StatusCode = statusCode,
            ErrorCode = errorCode ?? ErrorCodeToHttpStatusMapper.ToErrorCode(statusCode),
            Error = new ApiErrorDetails { Message = message }
        };
    }
    /// <summary>
    /// Converts Result<PaginatedCollection<TData>> to PagedApiResponse<TData>
    /// Factory method - keeps conversion logic in the response class
    /// </summary>
    public static PagedApiResponse<TData> FromResult(
        Result<PaginatedCollection<TData>> result)
    {
        if (result.IsSuccess && result.Data is not null)
        {
            return Success(
                data: result.Data.Items,
                pageNumber: result.Data.Metadata.CurrentPage,
                pageSize: result.Data.Metadata.PageSize,
                totalCount: result.Data.Metadata.TotalRecords,
                message: result.Message
            );
        }

        return new PagedApiResponse<TData>
        {
            IsSuccess = false,
            Message = result.Message,
            StatusCode = ErrorCodeToHttpStatusMapper.ToHttpStatusCode(result.ErrorCode),
            ErrorCode = result.ErrorCode,
            Error = new ApiErrorDetails
            {
                Message = result.Message,
                ValidationErrors = result.ValidationErrors
            },
            Data = Enumerable.Empty<TData>(),
            PageNumber = 1,
            PageSize = 10,
            TotalCount = 0
        };
    }
}
