using FluentValidation;
using NDTCore.Identity.Contracts.Features.Roles.Requests;

namespace NDTCore.Identity.Application.Features.Roles.Validators;

/// <summary>
/// Validator for assign role request
/// </summary>
public class AssignRoleRequestValidator : AbstractValidator<AssignRoleRequest>
{
    public AssignRoleRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Role ID is required");
    }
}

