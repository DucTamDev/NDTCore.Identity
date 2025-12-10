using NDTCore.Identity.Application.Common.Interfaces;
using NDTCore.Identity.Contracts.Features.Authentication.Responses;

namespace NDTCore.Identity.Application.Features.Authentication.Commands.RefreshToken;

/// <summary>
/// Command to refresh access token
/// </summary>
public record RefreshTokenCommand : ICommand<AuthenticationResponse>
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public string IpAddress { get; init; } = string.Empty;
}

