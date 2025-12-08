namespace NDTCore.Identity.Domain.Constants;

/// <summary>
/// Centralized error codes for consistent error handling across the application
/// </summary>
public static class ErrorCodes
{
    public const string None = "NONE";
    public const string Unknown = "UNKNOWN";

    // ========================================
    // General Errors (1000-1999)
    // ========================================
    public const string InternalError = "INTERNAL_ERROR";
    public const string InvalidOperation = "INVALID_OPERATION";
    public const string InvalidArgument = "INVALID_ARGUMENT";
    public const string ConfigurationError = "CONFIGURATION_ERROR";

    // ========================================
    // Validation Errors (2000-2999)
    // ========================================
    public const string ValidationError = "VALIDATION_ERROR";
    public const string RequiredField = "REQUIRED_FIELD";
    public const string InvalidFormat = "INVALID_FORMAT";
    public const string OutOfRange = "OUT_OF_RANGE";

    // ========================================
    // Authentication & Authorization (3000-3999)
    // ========================================
    public const string Unauthorized = "UNAUTHORIZED";
    public const string Forbidden = "FORBIDDEN";
    public const string InvalidToken = "INVALID_TOKEN";
    public const string ExpiredToken = "EXPIRED_TOKEN";
    public const string TokenRevoked = "TOKEN_REVOKED";
    public const string InvalidCredentials = "INVALID_CREDENTIALS";
    public const string AccountLocked = "ACCOUNT_LOCKED";
    public const string AccountDisabled = "ACCOUNT_DISABLED";
    public const string InsufficientPermissions = "INSUFFICIENT_PERMISSIONS";

    // ========================================
    // Resource Errors (4000-4999)
    // ========================================
    public const string NotFound = "NOT_FOUND";
    public const string Conflict = "CONFLICT";
    public const string DuplicateEntry = "DUPLICATE_ENTRY";
    public const string ConcurrencyConflict = "CONCURRENCY_CONFLICT";
    public const string ResourceLocked = "RESOURCE_LOCKED";

    // ========================================
    // Business Rule Violations (5000-5999)
    // ========================================
    public const string BusinessRuleViolation = "BUSINESS_RULE_VIOLATION";
    public const string InvalidState = "INVALID_STATE";
    public const string OperationNotAllowed = "OPERATION_NOT_ALLOWED";
    public const string QuotaExceeded = "QUOTA_EXCEEDED";

    // ========================================
    // Database Errors (6000-6999)
    // ========================================
    public const string DatabaseError = "DATABASE_ERROR";
    public const string ForeignKeyViolation = "FOREIGN_KEY_VIOLATION";
    public const string ConnectionTimeout = "CONNECTION_TIMEOUT";
    public const string Deadlock = "DEADLOCK";

    // ========================================
    // External Service Errors (7000-7999)
    // ========================================
    public const string ServiceUnavailable = "SERVICE_UNAVAILABLE";
    public const string ServiceTimeout = "SERVICE_TIMEOUT";
    public const string ExternalServiceError = "EXTERNAL_SERVICE_ERROR";
    public const string OperationCancelled = "OPERATION_CANCELLED";

    // ========================================
    // Rate Limiting (8000-8999)
    // ========================================
    public const string RateLimitExceeded = "RATE_LIMIT_EXCEEDED";
    public const string TooManyRequests = "TOO_MANY_REQUESTS";

    // ========================================
    // Security Errors (9000-9999)
    // ========================================
    public const string SecurityViolation = "SECURITY_VIOLATION";
    public const string SuspiciousActivity = "SUSPICIOUS_ACTIVITY";
    public const string DataBreachAttempt = "DATA_BREACH_ATTEMPT";
}
