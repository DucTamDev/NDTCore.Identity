using NDTCore.Identity.Application.Common.Interfaces;

namespace NDTCore.Identity.Application.Features.Authentication.Commands.ChangePassword;

/// <summary>
/// Command to change user password
/// </summary>
public record ChangePasswordCommand : ICommand
{
    public Guid UserId { get; init; }
    public string CurrentPassword { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
}

