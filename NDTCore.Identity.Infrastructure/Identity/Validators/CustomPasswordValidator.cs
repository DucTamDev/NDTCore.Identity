using Microsoft.AspNetCore.Identity;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Infrastructure.Identity.Validators;

/// <summary>
/// Custom password validation rules
/// </summary>
public class CustomPasswordValidator : IPasswordValidator<AppUser>
{
    public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string? password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return Task.FromResult(IdentityResult.Failed(new IdentityError
            {
                Code = "PasswordRequired",
                Description = "Password is required"
            }));
        }

        // Add custom validation rules here
        var errors = new List<IdentityError>();

        // Example: Check if password contains username
        if (password.Contains(user.UserName ?? "", StringComparison.OrdinalIgnoreCase))
        {
            errors.Add(new IdentityError
            {
                Code = "PasswordContainsUsername",
                Description = "Password cannot contain username"
            });
        }

        return Task.FromResult(errors.Count == 0
            ? IdentityResult.Success
            : IdentityResult.Failed(errors.ToArray()));
    }
}

