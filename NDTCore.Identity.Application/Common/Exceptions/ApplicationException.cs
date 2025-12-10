namespace NDTCore.Identity.Application.Common.Exceptions;

/// <summary>
/// Base application exception
/// </summary>
public class ApplicationException : Exception
{
    public ApplicationException() : base()
    {
    }

    public ApplicationException(string message) : base(message)
    {
    }

    public ApplicationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

