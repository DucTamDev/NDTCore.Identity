namespace NDTCore.Identity.Contracts.Common;

/// <summary>
/// Standard API response wrapper
/// </summary>
/// <typeparam name="T">Type of the response data</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Indicates if the request was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Response message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Response data
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// List of errors (if any)
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// HTTP status code
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Request trace identifier
    /// </summary>
    public string? TraceId { get; set; }

    /// <summary>
    /// Response timestamp
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Creates a successful response
    /// </summary>
    /// <param name="data">The response data</param>
    /// <param name="message">Success message</param>
    /// <param name="statusCode">HTTP status code</param>
    /// <returns>ApiResponse instance</returns>
    public static ApiResponse<T> SuccessResponse(T data, string message = "Success", int statusCode = 200)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            StatusCode = statusCode
        };
    }

    /// <summary>
    /// Creates a failure response
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="statusCode">HTTP status code</param>
    /// <param name="errors">List of errors</param>
    /// <returns>ApiResponse instance</returns>
    public static ApiResponse<T> FailureResponse(string message, int statusCode = 400, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            StatusCode = statusCode,
            Errors = errors ?? new List<string>()
        };
    }

    /// <summary>
    /// Creates an error response
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="errors">List of errors</param>
    /// <param name="statusCode">HTTP status code</param>
    /// <returns>ApiResponse instance</returns>
    public static ApiResponse<T> ErrorResponse(string message, List<string>? errors = null, int statusCode = 500)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            StatusCode = statusCode,
            Errors = errors ?? new List<string>()
        };
    }
}

/// <summary>
/// Standard API response wrapper (non-generic)
/// </summary>
public class ApiResponse : ApiResponse<object>
{
    /// <summary>
    /// Creates a successful response
    /// </summary>
    /// <param name="message">Success message</param>
    /// <param name="statusCode">HTTP status code</param>
    /// <returns>ApiResponse instance</returns>
    public static ApiResponse SuccessResponse(string message = "Success", int statusCode = 200)
    {
        return new ApiResponse
        {
            Success = true,
            Message = message,
            StatusCode = statusCode
        };
    }

    /// <summary>
    /// Creates a failure response
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="statusCode">HTTP status code</param>
    /// <param name="errors">List of errors</param>
    /// <returns>ApiResponse instance</returns>
    public static new ApiResponse FailureResponse(string message, int statusCode = 400, List<string>? errors = null)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            StatusCode = statusCode,
            Errors = errors ?? new List<string>()
        };
    }
}

