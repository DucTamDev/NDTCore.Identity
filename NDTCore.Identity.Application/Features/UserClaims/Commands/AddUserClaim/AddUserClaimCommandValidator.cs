using FluentValidation;

namespace NDTCore.Identity.Application.Features.UserClaims.Commands.AddUserClaim;

/// <summary>
/// Validator for AddUserClaimCommand
/// </summary>
public class AddUserClaimCommandValidator : AbstractValidator<AddUserClaimCommand>
{
    public AddUserClaimCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.ClaimType)
            .NotEmpty()
            .WithMessage("Claim type is required")
            .MaximumLength(256)
            .WithMessage("Claim type must not exceed 256 characters");

        RuleFor(x => x.ClaimValue)
            .NotEmpty()
            .WithMessage("Claim value is required")
            .MaximumLength(512)
            .WithMessage("Claim value must not exceed 512 characters");
    }
}

