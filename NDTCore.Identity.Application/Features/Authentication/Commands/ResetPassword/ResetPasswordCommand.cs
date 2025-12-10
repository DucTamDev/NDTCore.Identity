using NDTCore.Identity.Application.Common.Interfaces;

namespace NDTCore.Identity.Application.Features.Authentication.Commands.ResetPassword;

/// <summary>
/// Command to reset user password with a reset token
/// </summary>
public record ResetPasswordCommand : ICommand
{
    public string Email { get; init; } = string.Empty;
    public string ResetToken { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
}

