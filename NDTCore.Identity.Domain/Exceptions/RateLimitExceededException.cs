using NDTCore.Identity.Domain.Constants;

namespace NDTCore.Identity.Domain.Exceptions;

/// <summary>
/// Exception thrown when rate limit is exceeded
/// </summary>
public class RateLimitExceededException : BaseDomainException
{
    public int RetryAfterSeconds { get; }

    public RateLimitExceededException(int retryAfterSeconds = 60)
        : base(
            ErrorCodes.RateLimitExceeded,
            $"Rate limit exceeded. Retry after {retryAfterSeconds} seconds.",
            "Too many requests. Please try again later.")
    {
        RetryAfterSeconds = retryAfterSeconds;
        ErrorDetails = new Dictionary<string, object>
        {
            { "RetryAfterSeconds", retryAfterSeconds }
        };
    }
}

