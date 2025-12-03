using FluentValidation;
using NDTCore.Identity.Contracts.Features.Roles.Requests;

namespace NDTCore.Identity.Application.Features.Roles.Validators;

/// <summary>
/// Validator for update role request
/// </summary>
public class UpdateRoleRequestValidator : AbstractValidator<UpdateRoleRequest>
{
    public UpdateRoleRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Role name is required")
            .MinimumLength(3).WithMessage("Role name must be at least 3 characters")
            .MaximumLength(50).WithMessage("Role name must not exceed 50 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}

