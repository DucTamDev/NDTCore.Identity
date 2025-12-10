using NDTCore.Identity.Domain.Constants;

namespace NDTCore.Identity.Contracts.Common.Results;

/// <summary>
/// Result pattern for business logic operations
/// Business layer should NOT know about HTTP status codes
/// </summary>
public class Result<TData>
{
    public bool IsSuccess { get; protected set; }
    public TData? Data { get; protected set; }
    public string Message { get; protected set; } = string.Empty;
    public string ErrorCode { get; protected set; } = string.Empty;
    public Dictionary<string, List<string>>? ValidationErrors { get; protected set; }

    // Success factory
    public static Result<TData> Success(TData data, string message = "Operation completed successfully")
    {
        return new Result<TData>
        {
            IsSuccess = true,
            Data = data,
            Message = message,
            ErrorCode = ErrorCodes.None
        };
    }

    // Failure factories for different business error types
    public static Result<TData> NotFound(string message)
    {
        return new Result<TData>
        {
            IsSuccess = false,
            Message = message,
            ErrorCode = ErrorCodes.NotFound
        };
    }

    public static Result<TData> ValidationError(
        string message,
        Dictionary<string, List<string>>? validationErrors = null)
    {
        return new Result<TData>
        {
            IsSuccess = false,
            Message = message,
            ErrorCode = ErrorCodes.ValidationError,
            ValidationErrors = validationErrors
        };
    }

    public static Result<TData> Unauthorized(string message)
    {
        return new Result<TData>
        {
            IsSuccess = false,
            Message = message,
            ErrorCode = ErrorCodes.Unauthorized
        };
    }

    public static Result<TData> Forbidden(string message)
    {
        return new Result<TData>
        {
            IsSuccess = false,
            Message = message,
            ErrorCode = ErrorCodes.Forbidden
        };
    }

    public static Result<TData> Conflict(string message)
    {
        return new Result<TData>
        {
            IsSuccess = false,
            Message = message,
            ErrorCode = ErrorCodes.Conflict
        };
    }

    public static Result<TData> Failure(string message, string? errorCode = null)
    {
        return new Result<TData>
        {
            IsSuccess = false,
            Message = message,
            ErrorCode = errorCode ?? ErrorCodes.InternalError
        };
    }

    // For unexpected exceptions
    public static Result<TData> Exception(Exception ex, string? errorCode = null)
    {
        return new Result<TData>
        {
            IsSuccess = false,
            Message = "An unexpected error occurred",
            ErrorCode = errorCode ?? ErrorCodes.InternalError
        };
    }

    // Convenience methods for compatibility (domain-friendly, no HTTP status codes)
    public static Result<TData> Created(TData data, string message = "Resource created successfully")
    {
        return Success(data, message);
    }

    public static Result<TData> BadRequest(
        string message,
        string? errorCode = null,
        Dictionary<string, List<string>>? validationErrors = null)
    {
        return ValidationError(message, validationErrors);
    }

    public static Result<TData> InternalError(string message, string? errorCode = null)
    {
        return Failure(message, errorCode);
    }
}
