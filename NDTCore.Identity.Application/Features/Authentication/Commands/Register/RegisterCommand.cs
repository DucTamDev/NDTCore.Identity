using NDTCore.Identity.Application.Common.Interfaces;
using NDTCore.Identity.Contracts.Features.Authentication.Responses;

namespace NDTCore.Identity.Application.Features.Authentication.Commands.Register;

/// <summary>
/// Command to register a new user
/// </summary>
public record RegisterCommand : ICommand<AuthenticationResponse>
{
    public string UserName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
}

