using FluentValidation;
using NDTCore.Identity.Contracts.Features.Authentication.Requests;

namespace NDTCore.Identity.Application.Features.Authentication.Validators;

/// <summary>
/// Validator for refresh token request
/// </summary>
public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty().WithMessage("Access token is required");

        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required");
    }
}

