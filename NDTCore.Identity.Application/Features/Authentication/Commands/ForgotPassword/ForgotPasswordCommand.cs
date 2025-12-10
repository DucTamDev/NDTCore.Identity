using NDTCore.Identity.Application.Common.Interfaces;

namespace NDTCore.Identity.Application.Features.Authentication.Commands.ForgotPassword;

/// <summary>
/// Command to initiate password reset process
/// </summary>
public record ForgotPasswordCommand : ICommand
{
    public string Email { get; init; } = string.Empty;
}

