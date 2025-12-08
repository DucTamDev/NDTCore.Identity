using NDTCore.Identity.Domain.Constants;
using NDTCore.Identity.Domain.Enums;
using NDTCore.Identity.Domain.Extensions;

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
    public string? ErrorCode { get; protected set; }
    public Dictionary<string, List<string>>? ValidationErrors { get; protected set; }
    public ErrorType ErrorType { get; protected set; }

    // Success factory
    public static Result<TData> Success(TData data, string message = "Operation completed successfully")
    {
        return new Result<TData>
        {
            IsSuccess = true,
            Data = data,
            Message = message,
            ErrorType = ErrorType.None
        };
    }

    // Failure factories for different business error types
    public static Result<TData> NotFound(string message)
    {
        return new Result<TData>
        {
            IsSuccess = false,
            Message = message,
            ErrorCode = ErrorCodes.NotFound,
            ErrorType = ErrorType.NotFound
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
            ValidationErrors = validationErrors,
            ErrorType = ErrorType.ValidationError
        };
    }

    public static Result<TData> Unauthorized(string message)
    {
        return new Result<TData>
        {
            IsSuccess = false,
            Message = message,
            ErrorCode = ErrorCodes.Unauthorized,
            ErrorType = ErrorType.Unauthorized
        };
    }

    public static Result<TData> Forbidden(string message)
    {
        return new Result<TData>
        {
            IsSuccess = false,
            Message = message,
            ErrorCode = ErrorCodes.Forbidden,
            ErrorType = ErrorType.Forbidden
        };
    }

    public static Result<TData> Conflict(string message)
    {
        return new Result<TData>
        {
            IsSuccess = false,
            Message = message,
            ErrorCode = ErrorCodes.Conflict,
            ErrorType = ErrorType.Conflict
        };
    }

    public static Result<TData> Failure(string message, string? errorCode = null)
    {
        return new Result<TData>
        {
            IsSuccess = false,
            Message = message,
            ErrorCode = errorCode ?? ErrorType.BusinessError.ToErrorCode(),
            ErrorType = ErrorType.BusinessError
        };
    }

    // For unexpected exceptions
    public static Result<TData> Exception(Exception ex, string? errorCode = null)
    {
        return new Result<TData>
        {
            IsSuccess = false,
            Message = "An unexpected error occurred",
            ErrorCode = errorCode ?? ErrorType.InternalError.ToErrorCode(),
            ErrorType = ErrorType.InternalError
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
            ErrorType = ErrorType.None
        };
    }

    public static new Result NotFound(string message)
    {
        return new Result
        {
            IsSuccess = false,
            Message = message,
            ErrorCode = ErrorCodes.NotFound,
            ErrorType = ErrorType.NotFound
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
            ValidationErrors = validationErrors,
            ErrorType = ErrorType.ValidationError
        };
    }

    public static new Result Unauthorized(string message)
    {
        return new Result
        {
            IsSuccess = false,
            Message = message,
            ErrorCode = ErrorCodes.Unauthorized,
            ErrorType = ErrorType.Unauthorized
        };
    }

    public static new Result Forbidden(string message)
    {
        return new Result
        {
            IsSuccess = false,
            Message = message,
            ErrorCode = ErrorCodes.Forbidden,
            ErrorType = ErrorType.Forbidden
        };
    }

    public static new Result Conflict(string message)
    {
        return new Result
        {
            IsSuccess = false,
            Message = message,
            ErrorCode = ErrorCodes.Conflict,
            ErrorType = ErrorType.Conflict
        };
    }

    public static new Result Failure(string message, string? errorCode = null)
    {
        return new Result
        {
            IsSuccess = false,
            Message = message,
            ErrorCode = errorCode ?? ErrorType.BusinessError.ToErrorCode(),
            ErrorType = ErrorType.BusinessError
        };
    }

    public static new Result Exception(Exception ex, string? errorCode = null)
    {
        return new Result
        {
            IsSuccess = false,
            Message = "An unexpected error occurred",
            ErrorCode = errorCode ?? ErrorCodes.InternalError,
            ErrorType = ErrorType.InternalError
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
