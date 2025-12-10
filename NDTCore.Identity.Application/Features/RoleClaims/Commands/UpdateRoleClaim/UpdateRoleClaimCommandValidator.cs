using FluentValidation;

namespace NDTCore.Identity.Application.Features.RoleClaims.Commands.UpdateRoleClaim;

/// <summary>
/// Validator for UpdateRoleClaimCommand
/// </summary>
public class UpdateRoleClaimCommandValidator : AbstractValidator<UpdateRoleClaimCommand>
{
    public UpdateRoleClaimCommandValidator()
    {
        RuleFor(x => x.ClaimId)
            .GreaterThan(0)
            .WithMessage("Claim ID must be greater than 0");

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

