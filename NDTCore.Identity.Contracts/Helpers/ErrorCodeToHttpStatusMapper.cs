using Microsoft.AspNetCore.Http;
using NDTCore.Identity.Domain.Constants;

namespace NDTCore.Identity.Contracts.Helpers;

/// <summary>
/// Maps error codes to HTTP status codes
/// </summary>
public static class ErrorCodeToHttpStatusMapper
{
    private static readonly Dictionary<string, int> _errorCodeMappings = new()
    {
        // 400 - Bad Request
        [ErrorCodes.ValidationError] = StatusCodes.Status400BadRequest,
        [ErrorCodes.InvalidArgument] = StatusCodes.Status400BadRequest,
        [ErrorCodes.InvalidOperation] = StatusCodes.Status400BadRequest,
        [ErrorCodes.InvalidFormat] = StatusCodes.Status400BadRequest,
        [ErrorCodes.InvalidCredentials] = StatusCodes.Status400BadRequest,

        // 401 - Unauthorized
        [ErrorCodes.Unauthorized] = StatusCodes.Status401Unauthorized,
        [ErrorCodes.InvalidToken] = StatusCodes.Status401Unauthorized,
        [ErrorCodes.AccountLocked] = StatusCodes.Status401Unauthorized,
        [ErrorCodes.AccountDisabled] = StatusCodes.Status401Unauthorized,

        // 403 - Forbidden
        [ErrorCodes.Forbidden] = StatusCodes.Status403Forbidden,
        [ErrorCodes.InsufficientPermissions] = StatusCodes.Status403Forbidden,

        // 404 - Not Found
        [ErrorCodes.NotFound] = StatusCodes.Status404NotFound,

        // 409 - Conflict
        [ErrorCodes.Conflict] = StatusCodes.Status409Conflict,
        [ErrorCodes.DuplicateEntry] = StatusCodes.Status409Conflict,
        [ErrorCodes.ConcurrencyConflict] = StatusCodes.Status409Conflict,

        // 422 - Unprocessable Entity
        [ErrorCodes.BusinessRuleViolation] = StatusCodes.Status422UnprocessableEntity,
        [ErrorCodes.ForeignKeyViolation] = StatusCodes.Status422UnprocessableEntity,

        // 429 - Too Many Requests
        [ErrorCodes.RateLimitExceeded] = StatusCodes.Status429TooManyRequests,

        // 500 - Internal Server Error
        [ErrorCodes.InternalError] = StatusCodes.Status500InternalServerError,
        [ErrorCodes.DatabaseError] = StatusCodes.Status500InternalServerError,
        [ErrorCodes.Deadlock] = StatusCodes.Status500InternalServerError,

        // 502 - Bad Gateway
        [ErrorCodes.ExternalServiceError] = StatusCodes.Status502BadGateway,

        // 503 - Service Unavailable
        [ErrorCodes.ServiceUnavailable] = StatusCodes.Status503ServiceUnavailable,

        // 504 - Gateway Timeout
        [ErrorCodes.ServiceTimeout] = StatusCodes.Status504GatewayTimeout,
        [ErrorCodes.ConnectionTimeout] = StatusCodes.Status504GatewayTimeout,
        [ErrorCodes.OperationCancelled] = StatusCodes.Status504GatewayTimeout,
    };

    /// <summary>
    /// Converts an error code to HTTP status code
    /// </summary>
    public static int ToHttpStatusCode(string errorCode)
    {
        if (string.IsNullOrWhiteSpace(errorCode))
        {
            return StatusCodes.Status500InternalServerError;
        }

        return _errorCodeMappings.TryGetValue(errorCode, out var statusCode)
            ? statusCode
            : StatusCodes.Status500InternalServerError;
    }

    /// <summary>
    /// Converts HTTP status code to error code (reverse mapping)
    /// </summary>
    public static string ToErrorCode(int statusCode)
    {
        return statusCode switch
        {
            StatusCodes.Status400BadRequest => ErrorCodes.ValidationError,
            StatusCodes.Status401Unauthorized => ErrorCodes.Unauthorized,
            StatusCodes.Status403Forbidden => ErrorCodes.Forbidden,
            StatusCodes.Status404NotFound => ErrorCodes.NotFound,
            StatusCodes.Status409Conflict => ErrorCodes.Conflict,
            StatusCodes.Status422UnprocessableEntity => ErrorCodes.BusinessRuleViolation,
            StatusCodes.Status429TooManyRequests => ErrorCodes.RateLimitExceeded,
            StatusCodes.Status500InternalServerError => ErrorCodes.InternalError,
            StatusCodes.Status502BadGateway => ErrorCodes.ExternalServiceError,
            StatusCodes.Status503ServiceUnavailable => ErrorCodes.ServiceUnavailable,
            StatusCodes.Status504GatewayTimeout => ErrorCodes.ServiceTimeout,
            _ => ErrorCodes.InternalError
        };
    }

    /// <summary>
    /// Checks if status code indicates client error (4xx)
    /// </summary>
    public static bool IsClientError(int statusCode)
    {
        return statusCode >= 400 && statusCode < 500;
    }

    /// <summary>
    /// Checks if status code indicates server error (5xx)
    /// </summary>
    public static bool IsServerError(int statusCode)
    {
        return statusCode >= 500 && statusCode < 600;
    }
}
