using FluentValidation;
using NDTCore.Identity.Contracts.Features.Claims.Requests;

namespace NDTCore.Identity.Application.Features.Claims.Validators;

/// <summary>
/// Validator for create claim request
/// </summary>
public class CreateClaimRequestValidator : AbstractValidator<CreateClaimRequest>
{
    public CreateClaimRequestValidator()
    {
        RuleFor(x => x.ClaimType)
            .NotEmpty().WithMessage("Claim type is required")
            .MaximumLength(200).WithMessage("Claim type cannot exceed 200 characters");

        RuleFor(x => x.ClaimValue)
            .NotEmpty().WithMessage("Claim value is required")
            .MaximumLength(500).WithMessage("Claim value cannot exceed 500 characters");
    }
}

