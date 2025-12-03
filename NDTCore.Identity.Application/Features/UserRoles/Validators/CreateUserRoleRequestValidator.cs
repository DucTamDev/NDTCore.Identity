using FluentValidation;
using NDTCore.Identity.Contracts.Features.UserRoles.Requests;

namespace NDTCore.Identity.Application.Features.UserRoles.Validators;

/// <summary>
/// Validator for create user role request
/// </summary>
public class CreateUserRoleRequestValidator : AbstractValidator<CreateUserRoleRequest>
{
    public CreateUserRoleRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Role ID is required");

        RuleFor(x => x.AssignedBy)
            .MaximumLength(200).WithMessage("AssignedBy cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.AssignedBy));
    }
}

