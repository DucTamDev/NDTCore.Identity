namespace NDTCore.Identity.Domain.Exceptions;

/// <summary>
/// Exception thrown when unauthorized access is attempted
/// </summary>
public class UnauthorizedAccessException : DomainException
{
    public UnauthorizedAccessException() : base("Unauthorized access")
    {
    }

    public UnauthorizedAccessException(string message) : base(message)
    {
    }

    public UnauthorizedAccessException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

