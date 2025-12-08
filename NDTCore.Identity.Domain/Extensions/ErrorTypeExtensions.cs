using NDTCore.Identity.Domain.Constants;
using NDTCore.Identity.Domain.Enums;

namespace NDTCore.Identity.Domain.Extensions;

/// <summary>
/// Extension methods for ErrorType
/// </summary>
public static class ErrorTypeExtensions
{
    /// <summary>
    /// Convert ErrorType to HTTP status code
    /// </summary>
    public static int ToHttpStatusCode(this ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.None => 200,
            ErrorType.NotFound => 404,
            ErrorType.ValidationError => 422,
            ErrorType.Unauthorized => 401,
            ErrorType.Forbidden => 403,
            ErrorType.Conflict => 409,
            ErrorType.BusinessError => 400,
            ErrorType.InternalError => 500,
            ErrorType.ServiceUnavailable => 503,
            ErrorType.Timeout => 504,
            _ => 500
        };
    }

    /// <summary>
    /// Check if error is client error (4xx)
    /// </summary>
    public static bool IsClientError(this ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.NotFound or
            ErrorType.ValidationError or
            ErrorType.Unauthorized or
            ErrorType.Forbidden or
            ErrorType.Conflict or
            ErrorType.BusinessError => true,
            _ => false
        };
    }

    /// <summary>
    /// Check if error is server error (5xx)
    /// </summary>
    public static bool IsServerError(this ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.InternalError or
            ErrorType.ServiceUnavailable or
            ErrorType.Timeout => true,
            _ => false
        };
    }

    /// <summary>
    /// Convert ErrorType to standardized error code string (ErrorCodes)
    /// </summary>
    public static string ToErrorCode(this ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.None => ErrorCodes.None,

            // 4xx
            ErrorType.NotFound => ErrorCodes.NotFound,
            ErrorType.ValidationError => ErrorCodes.ValidationError,
            ErrorType.Unauthorized => ErrorCodes.Unauthorized,
            ErrorType.Forbidden => ErrorCodes.Forbidden,
            ErrorType.Conflict => ErrorCodes.Conflict,
            ErrorType.BusinessError => ErrorCodes.BusinessRuleViolation,

            // 5xx
            ErrorType.InternalError => ErrorCodes.InternalError,
            ErrorType.ServiceUnavailable => ErrorCodes.ServiceUnavailable,
            ErrorType.Timeout => ErrorCodes.ServiceTimeout,

            // fallback
            _ => ErrorCodes.Unknown
        };
    }
}