using FluentValidation;
using NDTCore.Identity.Contracts.Authorization;
using NDTCore.Identity.Contracts.Features.Permissions.Requests;

namespace NDTCore.Identity.Application.Features.Permissions.Validators;

/// <summary>
/// Validator for assign permission request
/// </summary>
public class AssignPermissionRequestValidator : AbstractValidator<AssignPermissionRequest>
{
    public AssignPermissionRequestValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Role ID is required");

        RuleFor(x => x.Permissions)
            .NotEmpty().WithMessage("At least one permission is required")
            .Must(p => p != null && p.Count > 0).WithMessage("At least one permission must be specified");

        RuleForEach(x => x.Permissions)
            .Must(BeValidPermission).WithMessage("Invalid permission: {PropertyValue}");
    }

    private bool BeValidPermission(string permission)
    {
        return NDTCore.Identity.Contracts.Authorization.Permissions.IsValidPermission(permission);
    }
}

