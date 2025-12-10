namespace NDTCore.Identity.Domain.Exceptions;

/// <summary>
/// Exception thrown when an invalid operation is attempted
/// </summary>
public class InvalidOperationException : DomainException
{
    public InvalidOperationException() : base("Invalid operation")
    {
    }

    public InvalidOperationException(string message) : base(message)
    {
    }

    public InvalidOperationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

