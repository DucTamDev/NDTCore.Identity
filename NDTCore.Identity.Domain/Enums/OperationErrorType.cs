namespace NDTCore.Identity.Domain.Enums;

/// <summary>
/// Types of operation errors
/// </summary>
public enum OperationErrorType
{
    /// <summary>
    /// Requested entity was not found (404)
    /// </summary>
    NotFound,

    /// <summary>
    /// Validation error (400)
    /// </summary>
    ValidationError,

    /// <summary>
    /// Business rule violation (400)
    /// </summary>
    BusinessRuleViolation,

    /// <summary>
    /// Unauthorized access (401)
    /// </summary>
    Unauthorized,

    /// <summary>
    /// Forbidden access (403)
    /// </summary>
    Forbidden,

    /// <summary>
    /// Conflict error, e.g., duplicate (409)
    /// </summary>
    Conflict,

    /// <summary>
    /// Internal system error (500)
    /// </summary>
    InternalError
}