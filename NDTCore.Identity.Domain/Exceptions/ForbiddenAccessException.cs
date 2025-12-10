namespace NDTCore.Identity.Domain.Exceptions;

/// <summary>
/// Exception thrown when access to a resource is forbidden
/// </summary>
public class ForbiddenAccessException : DomainException
{
    public string? Resource { get; }

    public ForbiddenAccessException() : base("Access to this resource is forbidden")
    {
    }

    public ForbiddenAccessException(string message) : base(message)
    {
    }

    public ForbiddenAccessException(string resource, string message) : base(message)
    {
        Resource = resource;
    }

    public ForbiddenAccessException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

