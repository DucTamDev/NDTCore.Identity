using Microsoft.AspNetCore.Http;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Helpers;
using NDTCore.Identity.Domain.Constants;

namespace NDTCore.Identity.Contracts.Common.Responses;

/// <summary>
/// Non-generic API response (for operations without data)
/// </summary>
public class ApiResponse : ApiResponse<object?>
{
    // Factory methods for success responses
    public static ApiResponse Success(string message = "Operation completed successfully")
    {
        return new ApiResponse
        {
            IsSuccess = true,
            Message = message,
            StatusCode = StatusCodes.Status200OK
        };
    }

    public static ApiResponse Created(string message = "Resource created successfully")
    {
        return new ApiResponse
        {
            IsSuccess = true,
            Message = message,
            StatusCode = StatusCodes.Status201Created
        };
    }

    public new static ApiResponse NoContent(string message = "Operation completed successfully")
    {
        return new ApiResponse
        {
            IsSuccess = true,
            Message = message,
            StatusCode = StatusCodes.Status204NoContent
        };
    }

    // Factory methods for error responses
    public static ApiResponse Failure(
        int statusCode,
        string message,
        string? errorCode = null,
        Dictionary<string, List<string>>? validationErrors = null)
    {
        return new ApiResponse
        {
            IsSuccess = false,
            Message = message,
            StatusCode = statusCode,
            ErrorCode = errorCode ?? ErrorCodeToHttpStatusMapper.ToErrorCode(statusCode),
            Error = new ApiErrorDetails
            {
                Message = message,
                ValidationErrors = validationErrors
            }
        };
    }

    public static ApiResponse Failure(
        string errorCode,
        string message,
        Dictionary<string, List<string>>? validationErrors = null)
    {
        var statusCode = ErrorCodeToHttpStatusMapper.ToHttpStatusCode(errorCode);
        return Failure(statusCode, message, errorCode, validationErrors);
    }

    public new static ApiResponse BadRequest(
        string message,
        Dictionary<string, List<string>>? validationErrors = null)
    {
        return Failure(
            StatusCodes.Status400BadRequest,
            message,
            ErrorCodes.ValidationError,
            validationErrors);
    }

    public new static ApiResponse NotFound(string message = "Resource not found")
    {
        return Failure(
            StatusCodes.Status404NotFound,
            message,
            ErrorCodes.NotFound);
    }

    public new static ApiResponse Unauthorized(string message = "Unauthorized access")
    {
        return Failure(
            StatusCodes.Status401Unauthorized,
            message,
            ErrorCodes.Unauthorized);
    }

    public new static ApiResponse Forbidden(string message = "Access forbidden")
    {
        return Failure(
            StatusCodes.Status403Forbidden,
            message,
            ErrorCodes.Forbidden);
    }

    public new static ApiResponse Conflict(string message = "Resource conflict")
    {
        return Failure(
            StatusCodes.Status409Conflict,
            message,
            ErrorCodes.Conflict);
    }

    public new static ApiResponse InternalError(
        string message = "An internal server error occurred",
        string? errorCode = null)
    {
        return Failure(
            StatusCodes.Status500InternalServerError,
            message,
            errorCode ?? ErrorCodes.InternalError);
    }

    // Convert from Result (non-generic)
    public static ApiResponse FromResult(Result result)
    {
        if (result.IsSuccess)
        {
            return Success(result.Message);
        }

        return new ApiResponse
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
