using NDTCore.Identity.Application.Common.Interfaces;
using NDTCore.Identity.Contracts.Features.UserRoles.DTOs;

namespace NDTCore.Identity.Application.Features.UserRoles.Commands.AssignRoleToUser;

/// <summary>
/// Command to assign a role to a user
/// </summary>
public record AssignRoleToUserCommand : ICommand<UserRoleDto>
{
    public Guid UserId { get; init; }
    public Guid RoleId { get; init; }
}

