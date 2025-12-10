using Microsoft.AspNetCore.Identity;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Application.Features.Authentication.Services;

/// <summary>
/// Service for password hashing and verification
/// </summary>
public class PasswordHashingService
{
    private readonly IPasswordHasher<AppUser> _passwordHasher;

    public PasswordHashingService(IPasswordHasher<AppUser> passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public string HashPassword(AppUser user, string password)
    {
        return _passwordHasher.HashPassword(user, password);
    }

    public bool VerifyPassword(AppUser user, string hashedPassword, string providedPassword)
    {
        var result = _passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
        return result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}

