using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NDTCore.Identity.Contracts.Configuration;
using NDTCore.Identity.Contracts.Interfaces.Services;
using NDTCore.Identity.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace NDTCore.Identity.Application.Features.Authentication.Services;

/// <summary>
/// Service for JWT token generation and validation
/// </summary>
public class JwtTokenService : IJwtTokenService
{
    private readonly ILogger<JwtTokenService> _logger;
    private readonly JwtSettings _jwtSettings;

    public JwtTokenService(
        ILogger<JwtTokenService> logger,
        IOptions<JwtSettings> jwtOptions)
    {
        _logger = logger;
        _jwtSettings = jwtOptions.Value;
    }

    public string GenerateAccessToken(AppUser user, IList<string> roles)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(nameof(user));

        var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
        var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

        // Add role claims
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = _jwtSettings.Audience,
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token algorithm");
            }

            return principal;
        }
        catch (SecurityTokenExpiredException securityTokenExpiredException)
        {
            // Expected for expired tokens in refresh flow
            _logger.LogError(securityTokenExpiredException,
                "[{ClassName}.{FunctionName}] - Security token expired: Error={Error}",
                nameof(JwtTokenService),
                nameof(GetPrincipalFromExpiredToken),
                securityTokenExpiredException.Message);

            return null;
        }
        catch (SecurityTokenInvalidSignatureException signatureEx)
        {
            // Invalid signature - potential tampering
            _logger.LogError(signatureEx,
                "[{ClassName}.{FunctionName}] - Invalid token signature: Error={Error}",
                nameof(JwtTokenService),
                nameof(GetPrincipalFromExpiredToken),
                signatureEx.Message);

            return null;
        }
        catch (SecurityTokenException securityTokenException)
        {
            // Other security token exceptions
            _logger.LogError(securityTokenException,
                "[{ClassName}.{FunctionName}] - Security token error: Error={Error}",
                nameof(JwtTokenService),
                nameof(GetPrincipalFromExpiredToken),
                securityTokenException.Message);

            return null;
        }
        catch (Exception ex)
        {
            // Unexpected errors
            _logger.LogError(ex,
                "[{ClassName}.{FunctionName}] - Unexpected error: Error={Error}",
                nameof(JwtTokenService),
                nameof(GetPrincipalFromExpiredToken),
                ex.Message);

            return null;
        }
    }

    public DateTime GetTokenExpirationTime()
    {
        return DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);
    }

    public string GetJwtIdFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        return jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value ?? string.Empty;
    }
}