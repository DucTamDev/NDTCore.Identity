using NDTCore.Identity.Contracts.Common;
using NDTCore.Identity.Contracts.Features.Authentication.Requests;
using NDTCore.Identity.Contracts.Features.Authentication.Responses;

namespace NDTCore.Identity.Contracts.Interfaces.Services;

public interface IAuthService
{
    Task<ApiResponse<AuthenticationResponse>> LoginAsync(LoginRequest request, string ipAddress, CancellationToken cancellationToken = default);
    Task<ApiResponse<AuthenticationResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<AuthenticationResponse>> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress, CancellationToken cancellationToken = default);
    Task<ApiResponse> ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse> LogoutAsync(Guid userId, CancellationToken cancellationToken = default);
}