namespace NDTCore.Identity.Domain.Enums;

/// <summary>
/// Standard error types that can be used across all layers
/// Maps to HTTP status codes at API layer
/// </summary>
public enum ErrorType
{

    /// <summary>
    /// No error - operation successful
    /// </summary>
    None = 0,

    /// <summary>
    /// Unknow error - operation failed
    /// </summary>
    Unknow = 1,

    // Client Errors (4xx)
    /// <summary>
    /// Resource not found (HTTP 404)
    /// </summary>
    NotFound = 2,

    /// <summary>
    /// Validation failed (HTTP 422)
    /// </summary>
    ValidationError = 3,

    /// <summary>
    /// Authentication required (HTTP 401)
    /// </summary>
    Unauthorized = 4,

    /// <summary>
    /// Insufficient permissions (HTTP 403)
    /// </summary>
    Forbidden = 5,

    /// <summary>
    /// Resource conflict (HTTP 409)
    /// </summary>
    Conflict = 6,

    /// <summary>
    /// General business rule violation (HTTP 400)
    /// </summary>
    BusinessError = 7,

    // Server Errors (5xx)
    /// <summary>
    /// Internal system error (HTTP 500)
    /// </summary>
    InternalError = 8,

    /// <summary>
    /// External service unavailable (HTTP 503)
    /// </summary>
    ServiceUnavailable = 9,

    /// <summary>
    /// Operation timeout (HTTP 504)
    /// </summary>
    Timeout = 10
}