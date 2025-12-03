namespace NDTCore.Identity.Contracts.Common;

/// <summary>
/// Standard API response wrapper for all HTTP endpoints
/// Provides consistent structure for success and error responses
/// </summary>
public class ApiResponse<TData>
{
    /// <summary>
    /// Indicates if the operation was successful
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Human-readable message describing the result
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Response data (null if operation failed)
    /// </summary>
    public TData? Data { get; init; }

    /// <summary>
    /// HTTP status code
    /// </summary>
    public int StatusCode { get; init; }

    /// <summary>
    /// Error code for programmatic error handling (null if success)
    /// </summary>
    public string? ErrorCode { get; init; }

    /// <summary>
    /// Detailed error information (null if success)
    /// </summary>
    public ApiErrorDetails? Error { get; init; }

    /// <summary>
    /// Request correlation ID for tracing
    /// </summary>
    public string? TraceId { get; init; }

    /// <summary>
    /// Response timestamp (UTC)
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    // ========================================
    // FACTORY METHODS
    // ========================================

    /// <summary>
    /// Creates a successful response
    /// </summary>
    public static ApiResponse<TData> Ok(
        TData data,
        string message = "Operation completed successfully")
    {
        return new ApiResponse<TData>
        {
            Success = true,
            Message = message,
            Data = data,
            StatusCode = 200,
            ErrorCode = null,
            Error = null
        };
    }

    /// <summary>
    /// Creates a created response (201)
    /// </summary>
    public static ApiResponse<TData> Created(
        TData data,
        string message = "Resource created successfully")
    {
        return new ApiResponse<TData>
        {
            Success = true,
            Message = message,
            Data = data,
            StatusCode = 201,
            ErrorCode = null,
            Error = null
        };
    }

    /// <summary>
    /// Creates a bad request response (400)
    /// </summary>
    public static ApiResponse<TData> BadRequest(
        string message,
        string? errorCode = "BAD_REQUEST",
        Dictionary<string, string[]>? validationErrors = null)
    {
        return new ApiResponse<TData>
        {
            Success = false,
            Message = message,
            Data = default,
            StatusCode = 400,
            ErrorCode = errorCode,
            Error = new ApiErrorDetails
            {
                Message = message,
                ValidationErrors = validationErrors
            }
        };
    }

    /// <summary>
    /// Creates a not found response (404)
    /// </summary>
    public static ApiResponse<TData> NotFound(
        string message = "Resource not found",
        string? errorCode = "NOT_FOUND")
    {
        return new ApiResponse<TData>
        {
            Success = false,
            Message = message,
            Data = default,
            StatusCode = 404,
            ErrorCode = errorCode,
            Error = new ApiErrorDetails { Message = message }
        };
    }

    /// <summary>
    /// Creates an unauthorized response (401)
    /// </summary>
    public static ApiResponse<TData> Unauthorized(
        string message = "Unauthorized access",
        string? errorCode = "UNAUTHORIZED")
    {
        return new ApiResponse<TData>
        {
            Success = false,
            Message = message,
            Data = default,
            StatusCode = 401,
            ErrorCode = errorCode,
            Error = new ApiErrorDetails { Message = message }
        };
    }

    /// <summary>
    /// Creates a forbidden response (403)
    /// </summary>
    public static ApiResponse<TData> Forbidden(
        string message = "Access forbidden",
        string? errorCode = "FORBIDDEN")
    {
        return new ApiResponse<TData>
        {
            Success = false,
            Message = message,
            Data = default,
            StatusCode = 403,
            ErrorCode = errorCode,
            Error = new ApiErrorDetails { Message = message }
        };
    }

    /// <summary>
    /// Creates a conflict response (409)
    /// </summary>
    public static ApiResponse<TData> Conflict(
        string message,
        string? errorCode = "CONFLICT")
    {
        return new ApiResponse<TData>
        {
            Success = false,
            Message = message,
            Data = default,
            StatusCode = 409,
            ErrorCode = errorCode,
            Error = new ApiErrorDetails { Message = message }
        };
    }

    /// <summary>
    /// Creates an internal server error response (500)
    /// </summary>
    public static ApiResponse<TData> InternalError(
        string message = "An internal error occurred",
        string? errorCode = "INTERNAL_ERROR")
    {
        return new ApiResponse<TData>
        {
            Success = false,
            Message = message,
            Data = default,
            StatusCode = 500,
            ErrorCode = errorCode,
            Error = new ApiErrorDetails { Message = message }
        };
    }
}

/// <summary>
/// Non-generic API response for operations that don't return data
/// </summary>
public class ApiResponse : ApiResponse<object>
{
    public static ApiResponse Ok(string message = "Operation completed successfully")
    {
        return new ApiResponse
        {
            Success = true,
            Message = message,
            Data = null,
            StatusCode = 200
        };
    }

    public static new ApiResponse BadRequest(
        string message,
        string? errorCode = "BAD_REQUEST",
        Dictionary<string, string[]>? validationErrors = null)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            StatusCode = 400,
            ErrorCode = errorCode,
            Error = new ApiErrorDetails
            {
                Message = message,
                ValidationErrors = validationErrors
            }
        };
    }

    public static new ApiResponse NotFound(
        string message = "Resource not found",
        string? errorCode = "NOT_FOUND")
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            StatusCode = 404,
            ErrorCode = errorCode,
            Error = new ApiErrorDetails { Message = message }
        };
    }

    /// <summary>
    /// Creates an unauthorized response (401)
    /// </summary>
    public static new ApiResponse Unauthorized(
        string message = "Unauthorized access",
        string? errorCode = "UNAUTHORIZED")
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            StatusCode = 401,
            ErrorCode = errorCode,
            Error = new ApiErrorDetails { Message = message }
        };
    }

    /// <summary>
    /// Creates a forbidden response (403)
    /// </summary>
    public static new ApiResponse Forbidden(
        string message = "Access forbidden",
        string? errorCode = "FORBIDDEN")
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            StatusCode = 403,
            ErrorCode = errorCode,
            Error = new ApiErrorDetails { Message = message }
        };
    }

    /// <summary>
    /// Creates a conflict response (409)
    /// </summary>
    public static new ApiResponse Conflict(
        string message,
        string? errorCode = "CONFLICT")
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            StatusCode = 409,
            ErrorCode = errorCode,
            Error = new ApiErrorDetails { Message = message }
        };
    }

    /// <summary>
    /// Creates an internal server error response (500)
    /// </summary>
    public static new ApiResponse InternalError(
        string message = "An internal error occurred",
        string? errorCode = "INTERNAL_ERROR")
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            StatusCode = 500,
            ErrorCode = errorCode,
            Error = new ApiErrorDetails { Message = message }
        };
    }
}
