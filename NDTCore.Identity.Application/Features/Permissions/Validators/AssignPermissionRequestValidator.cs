using FluentValidation;
using NDTCore.Identity.Contracts.Authorization.Permissions;
using NDTCore.Identity.Contracts.Features.Permissions.Requests;

namespace NDTCore.Identity.Application.Features.Permissions.Validators;

/// <summary>
/// Validator for assign permission request
/// </summary>
public class AssignPermissionRequestValidator : AbstractValidator<AssignPermissionRequest>
{
    private readonly IPermissionRegistry _permissionRegistry;

    public AssignPermissionRequestValidator(IPermissionRegistry permissionRegistry)
    {
        _permissionRegistry = permissionRegistry;

        RuleFor(x => x.RoleId)
          .NotEmpty()
          .WithMessage("Role ID is required.");

        RuleFor(x => x.Permissions)
            .NotEmpty()
            .WithMessage("At least one permission is required.")
            .Must(NoDuplicates)
            .WithMessage("Duplicate permissions are not allowed.");

        RuleForEach(x => x.Permissions)
            .NotEmpty().WithMessage("Permission cannot be empty.")
            .Must(BeValidPermission)
            .WithMessage("Permission '{PropertyValue}' is not recognized.");
    }

    private bool BeValidPermission(string permission)
    {
        return _permissionRegistry.IsValidPermission(permission);
    }

    private static bool NoDuplicates(IList<string> permissions)
    {
        return permissions.Count == permissions.Distinct().Count();
    }
}