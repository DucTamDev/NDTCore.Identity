using FluentValidation;
using NDTCore.Identity.Contracts.Features.Authentication.Requests;

namespace NDTCore.Identity.Application.Features.Authentication.Validators;

/// <summary>
/// Validator for forgot password request
/// </summary>
public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
{
    public ForgotPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");
    }
}

