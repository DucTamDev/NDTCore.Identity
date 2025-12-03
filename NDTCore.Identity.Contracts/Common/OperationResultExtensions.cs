using NDTCore.Identity.Domain.Enums;

namespace NDTCore.Identity.Contracts.Common;

/// <summary>
/// Extension methods to convert OperationResult to ApiResponse
/// </summary>
public static class OperationResultExtensions
{
    /// <summary>
    /// Converts OperationResult<T> to ApiResponse<T>
    /// </summary>
    public static ApiResponse<TData> ToApiResponse<TData>(
        this OperationResult<TData> result,
        string? successMessage = null)
    {
        if (result.IsSuccess && result.Data is not null && !result.Data.Equals(default(TData)))
        {
            return ApiResponse<TData>.Ok(
                result.Data,
                successMessage ?? "Operation completed successfully"
            );
        }

        // Map error type to appropriate HTTP status
        return result.Error!.ErrorType switch
        {
            OperationErrorType.NotFound => ApiResponse<TData>.NotFound(
                result.Error.Message,
                "NOT_FOUND"
            ),

            OperationErrorType.ValidationError => ApiResponse<TData>.BadRequest(
                result.Error.Message,
                "VALIDATION_ERROR",
                result.Error.ValidationErrors
            ),

            OperationErrorType.BusinessRuleViolation => ApiResponse<TData>.BadRequest(
                result.Error.Message,
                "BUSINESS_RULE_VIOLATION"
            ),

            OperationErrorType.Unauthorized => ApiResponse<TData>.Unauthorized(
                result.Error.Message,
                "UNAUTHORIZED"
            ),

            OperationErrorType.Forbidden => ApiResponse<TData>.Forbidden(
                result.Error.Message,
                "FORBIDDEN"
            ),

            OperationErrorType.Conflict => ApiResponse<TData>.Conflict(
                result.Error.Message,
                "CONFLICT"
            ),

            OperationErrorType.InternalError => ApiResponse<TData>.InternalError(
                result.Error.Message,
                "INTERNAL_ERROR"
            ),

            _ => ApiResponse<TData>.InternalError(
                result.Error.Message,
                "UNKNOWN_ERROR"
            )
        };
    }

    /// <summary>
    /// Converts non-generic OperationResult to ApiResponse
    /// </summary>
    public static ApiResponse ToApiResponse(
        this OperationResult result,
        string? successMessage = null)
    {
        if (result.IsSuccess)
        {
            return ApiResponse.Ok(successMessage ?? "Operation completed successfully");
        }

        return result.Error!.ErrorType switch
        {
            OperationErrorType.NotFound => ApiResponse.NotFound(
                result.Error.Message,
                "NOT_FOUND"
            ),

            OperationErrorType.ValidationError => ApiResponse.BadRequest(
                result.Error.Message,
                "VALIDATION_ERROR",
                result.Error.ValidationErrors
            ),

            OperationErrorType.BusinessRuleViolation => ApiResponse.BadRequest(
                result.Error.Message,
                "BUSINESS_RULE_VIOLATION"
            ),

            OperationErrorType.Unauthorized => ApiResponse.Unauthorized(
                result.Error.Message,
                "UNAUTHORIZED"
            ),

            OperationErrorType.Forbidden => ApiResponse.Forbidden(
                result.Error.Message,
                "FORBIDDEN"
            ),

            OperationErrorType.Conflict => ApiResponse.Conflict(
                result.Error.Message,
                "CONFLICT"
            ),

            _ => ApiResponse.InternalError(
                result.Error.Message,
                "INTERNAL_ERROR"
            )
        };
    }

    /// <summary>
    /// Converts PagedResult to PagedApiResponse
    /// </summary>
    public static PagedApiResponse<TData> ToPagedApiResponse<TData>(
        this OperationResult<PagedResult<TData>> result,
        string? successMessage = null)
    {
        if (result.IsSuccess && result.Data != null)
        {
            var pagedData = result.Data;
            return PagedApiResponse<TData>.Ok(
                pagedData.Items,
                pagedData.PageNumber,
                pagedData.PageSize,
                pagedData.TotalCount,
                successMessage ?? "Data retrieved successfully"
            );
        }

        // Return empty paged response on error
        return new PagedApiResponse<TData>
        {
            Success = false,
            Message = result.Error?.Message ?? "Operation failed",
            Data = new List<TData>(),
            StatusCode = MapErrorTypeToStatusCode(result.Error?.ErrorType),
            ErrorCode = result.Error?.ErrorType.ToString(),
            Error = new ApiErrorDetails
            {
                Message = result.Error?.Message ?? "Unknown error",
                ValidationErrors = result.Error?.ValidationErrors
            }
        };
    }

    private static int MapErrorTypeToStatusCode(OperationErrorType? errorType)
    {
        return errorType switch
        {
            OperationErrorType.NotFound => 404,
            OperationErrorType.ValidationError => 400,
            OperationErrorType.BusinessRuleViolation => 400,
            OperationErrorType.Unauthorized => 401,
            OperationErrorType.Forbidden => 403,
            OperationErrorType.Conflict => 409,
            OperationErrorType.InternalError => 500,
            _ => 500
        };
    }
}