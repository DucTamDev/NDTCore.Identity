using NDTCore.Identity.Application.Common.Interfaces;
using NDTCore.Identity.Contracts.Features.Authentication.Responses;

namespace NDTCore.Identity.Application.Features.Authentication.Commands.Login;

/// <summary>
/// Command to authenticate a user
/// </summary>
public record LoginCommand : ICommand<AuthenticationResponse>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string IpAddress { get; init; } = string.Empty;
}

