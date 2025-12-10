using Microsoft.AspNetCore.Http;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Helpers;
using NDTCore.Identity.Domain.Constants;

namespace NDTCore.Identity.Contracts.Common.Responses;

/// <summary>
/// API response wrapper for HTTP endpoints
/// Used by controllers to return standardized HTTP responses
/// </summary>
public class ApiResponse<TData>
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public TData? Data { get; set; }
    public int StatusCode { get; set; }
    public string? ErrorCode { get; set; }
    public ApiErrorDetails? Error { get; set; }
    public string? TraceId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Factory methods for success responses
    public static ApiResponse<TData> Success(
        TData data,
        string message = "Operation completed successfully")
    {
        return new ApiResponse<TData>
        {
            IsSuccess = true,
            Message = message,
            Data = data,
            StatusCode = StatusCodes.Status200OK
        };
    }

    public static ApiResponse<TData> Created(
        TData data,
        string message = "Resource created successfully")
    {
        return new ApiResponse<TData>
        {
            IsSuccess = true,
            Message = message,
            Data = data,
            StatusCode = StatusCodes.Status201Created
        };
    }

    public static ApiResponse<TData> NoContent(string message = "Operation completed successfully")
    {
        return new ApiResponse<TData>
        {
            IsSuccess = true,
            Message = message,
            StatusCode = StatusCodes.Status204NoContent
        };
    }

    // Factory methods for error responses
    public static ApiResponse<TData> Failure(
        int statusCode,
        string message,
        string? errorCode = null,
        Dictionary<string, List<string>>? validationErrors = null,
        TData? data = default)
    {
        return new ApiResponse<TData>
        {
            IsSuccess = false,
            Message = message,
            Data = data,
            StatusCode = statusCode,
            ErrorCode = errorCode ?? ErrorCodeToHttpStatusMapper.ToErrorCode(statusCode),
            Error = new ApiErrorDetails
            {
                Message = message,
                ValidationErrors = validationErrors
            }
        };
    }

    public static ApiResponse<TData> Failure(
        string errorCode,
        string message,
        Dictionary<string, List<string>>? validationErrors = null,
        TData? data = default)
    {
        var statusCode = ErrorCodeToHttpStatusMapper.ToHttpStatusCode(errorCode);
        return Failure(statusCode, message, errorCode, validationErrors, data);
    }

    public static ApiResponse<TData> BadRequest(
        string message,
        Dictionary<string, List<string>>? validationErrors = null)
    {
        return Failure(
            StatusCodes.Status400BadRequest,
            message,
            ErrorCodes.ValidationError,
            validationErrors);
    }

    public static ApiResponse<TData> NotFound(string message = "Resource not found")
    {
        return Failure(
            StatusCodes.Status404NotFound,
            message,
            ErrorCodes.NotFound);
    }

    public static ApiResponse<TData> Unauthorized(string message = "Unauthorized access")
    {
        return Failure(
            StatusCodes.Status401Unauthorized,
            message,
            ErrorCodes.Unauthorized);
    }

    public static ApiResponse<TData> Forbidden(string message = "Access forbidden")
    {
        return Failure(
            StatusCodes.Status403Forbidden,
            message,
            ErrorCodes.Forbidden);
    }

    public static ApiResponse<TData> Conflict(string message = "Resource conflict")
    {
        return Failure(
            StatusCodes.Status409Conflict,
            message,
            ErrorCodes.Conflict);
    }

    public static ApiResponse<TData> InternalError(
        string message = "An internal server error occurred",
        string? errorCode = null)
    {
        return Failure(
            StatusCodes.Status500InternalServerError,
            message,
            errorCode ?? ErrorCodes.InternalError);
    }

    // Convert from Result<T>
    public static ApiResponse<TData> FromResult(Result<TData> result)
    {
        if (result.IsSuccess)
        {
            return Success(result.Data!, result.Message);
        }

        return new ApiResponse<TData>
        {
            IsSuccess = false,
            Message = result.Message,
            StatusCode = ErrorCodeToHttpStatusMapper.ToHttpStatusCode(result.ErrorCode),
            ErrorCode = result.ErrorCode,
            Error = new ApiErrorDetails
            {
                Message = result.Message,
                ValidationErrors = result.ValidationErrors
            }
        };
    }
}
