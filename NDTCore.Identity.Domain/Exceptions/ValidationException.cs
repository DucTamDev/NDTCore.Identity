using NDTCore.Identity.Domain.Constants;

namespace NDTCore.Identity.Domain.Exceptions;

/// <summary>
/// Exception thrown when validation fails
/// </summary>
public class ValidationException : BaseDomainException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(IDictionary<string, string[]> errors)
        : base(
            ErrorCodes.ValidationError,
            "One or more validation errors occurred.",
            "Please correct the validation errors and try again.")
    {
        Errors = errors;
        ErrorDetails = new Dictionary<string, object>
        {
            { "ValidationErrors", errors }
        };
    }

    public ValidationException(string field, string error)
        : base(
            ErrorCodes.ValidationError,
            $"Validation error for field '{field}': {error}",
            error)
    {
        Errors = new Dictionary<string, string[]>
        {
            { field, new[] { error } }
        };
        ErrorDetails = new Dictionary<string, object>
        {
            { "Field", field },
            { "Error", error }
        };
    }
}
