using NDTCore.Identity.Domain.Constants;

namespace NDTCore.Identity.Contracts.Common.Results;

/// <summary>
/// Non-generic Result for operations without return data
/// </summary>
public class Result : Result<object>
{
    public static Result Success(string message = "Operation completed successfully")
    {
        return new Result
        {
            IsSuccess = true,
            Message = message,
            ErrorCode = ErrorCodes.None
        };
    }

    public static new Result NotFound(string message)
    {
        return new Result
        {
            IsSuccess = false,
            Message = message,
            ErrorCode = ErrorCodes.NotFound
        };
    }

    public static new Result ValidationError(
        string message,
        Dictionary<string, List<string>>? validationErrors = null)
    {
        return new Result
        {
            IsSuccess = false,
            Message = message,
            ErrorCode = ErrorCodes.ValidationError,
            ValidationErrors = validationErrors
        };
    }

    public static new Result Unauthorized(string message)
    {
        return new Result
        {
            IsSuccess = false,
            Message = message,
            ErrorCode = ErrorCodes.Unauthorized
        };
    }

    public static new Result Forbidden(string message)
    {
        return new Result
        {
            IsSuccess = false,
            Message = message,
            ErrorCode = ErrorCodes.Forbidden
        };
    }

    public static new Result Conflict(string message)
    {
        return new Result
        {
            IsSuccess = false,
            Message = message,
            ErrorCode = ErrorCodes.Conflict
        };
    }

    public static new Result Failure(string message, string? errorCode = null)
    {
        return new Result
        {
            IsSuccess = false,
            Message = message,
            ErrorCode = errorCode ?? ErrorCodes.InternalError
        };
    }

    public static new Result Exception(Exception ex, string? errorCode = null)
    {
        return new Result
        {
            IsSuccess = false,
            Message = "An unexpected error occurred",
            ErrorCode = errorCode ?? ErrorCodes.InternalError
        };
    }

    // Convenience methods for compatibility (domain-friendly, no HTTP status codes)
    public static new Result BadRequest(
        string message,
        string? errorCode = null,
        Dictionary<string, List<string>>? validationErrors = null)
    {
        return ValidationError(message, validationErrors);
    }

    public static new Result InternalError(string message, string? errorCode = null)
    {
        return Failure(message, errorCode);
    }
}
