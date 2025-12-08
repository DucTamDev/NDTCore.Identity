using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Features.Authentication.Requests;
using NDTCore.Identity.Contracts.Features.Authentication.Responses;

namespace NDTCore.Identity.Contracts.Interfaces.Services;

public interface IAuthService
{
    Task<Result<AuthenticationResponse>> LoginAsync(LoginRequest request, string ipAddress, CancellationToken cancellationToken = default);
    Task<Result<AuthenticationResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<Result<AuthenticationResponse>> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress, CancellationToken cancellationToken = default);
    Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken = default);
    Task<Result> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default);
    Task<Result> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default);
    Task<Result> LogoutAsync(Guid userId, CancellationToken cancellationToken = default);
}