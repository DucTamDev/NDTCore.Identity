using Microsoft.AspNetCore.Identity;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Infrastructure.Identity.Validators;

/// <summary>
/// Custom user validation rules
/// </summary>
public class CustomUserValidator : IUserValidator<AppUser>
{
    public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
    {
        var errors = new List<IdentityError>();

        // Add custom validation rules here

        // Example: Validate first name and last name
        if (string.IsNullOrWhiteSpace(user.FirstName))
        {
            errors.Add(new IdentityError
            {
                Code = "FirstNameRequired",
                Description = "First name is required"
            });
        }

        if (string.IsNullOrWhiteSpace(user.LastName))
        {
            errors.Add(new IdentityError
            {
                Code = "LastNameRequired",
                Description = "Last name is required"
            });
        }

        return Task.FromResult(errors.Count == 0
            ? IdentityResult.Success
            : IdentityResult.Failed(errors.ToArray()));
    }
}

