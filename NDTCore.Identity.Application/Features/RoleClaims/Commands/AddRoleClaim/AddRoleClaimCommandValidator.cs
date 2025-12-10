using FluentValidation;

namespace NDTCore.Identity.Application.Features.RoleClaims.Commands.AddRoleClaim;

/// <summary>
/// Validator for AddRoleClaimCommand
/// </summary>
public class AddRoleClaimCommandValidator : AbstractValidator<AddRoleClaimCommand>
{
    public AddRoleClaimCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty()
            .WithMessage("Role ID is required");

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

