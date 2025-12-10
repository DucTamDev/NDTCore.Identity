using FluentValidation;

namespace NDTCore.Identity.Application.Features.UserRoles.Commands.AssignRoleToUser;

/// <summary>
/// Validator for AssignRoleToUserCommand
/// </summary>
public class AssignRoleToUserCommandValidator : AbstractValidator<AssignRoleToUserCommand>
{
    public AssignRoleToUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.RoleId)
            .NotEmpty()
            .WithMessage("Role ID is required");
    }
}

