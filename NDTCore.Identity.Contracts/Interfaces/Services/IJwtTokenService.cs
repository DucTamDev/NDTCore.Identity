using NDTCore.Identity.Domain.Entities;
using System.Security.Claims;

namespace NDTCore.Identity.Contracts.Interfaces.Services;

public interface IJwtTokenService
{
    string GenerateAccessToken(AppUser user, IList<string> roles);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    DateTime GetTokenExpirationTime();
    string GetJwtIdFromToken(string token);
}