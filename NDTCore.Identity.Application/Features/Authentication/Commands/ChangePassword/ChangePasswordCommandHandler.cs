using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Domain.Constants;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Application.Features.Authentication.Commands.ChangePassword;

/// <summary>
/// Handler for ChangePasswordCommand
/// </summary>
public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<ChangePasswordCommandHandler> _logger;

    public ChangePasswordCommandHandler(
        UserManager<AppUser> userManager,
        ILogger<ChangePasswordCommandHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result> Handle(
        ChangePasswordCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing password change for user: {UserId}", request.UserId);

        // Get user
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
        {
            _logger.LogWarning("Password change failed - user not found: {UserId}", request.UserId);
            return Result.NotFound($"User with ID '{request.UserId}' was not found");
        }

        // Change password
        var result = await _userManager.ChangePasswordAsync(
            user,
            request.CurrentPassword,
            request.NewPassword);

        if (!result.Succeeded)
        {
            var validationErrors = result.Errors
                .GroupBy(e => e.Code)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.Description).ToList());

            _logger.LogWarning("Password change failed for user: {UserId}", request.UserId);
            return Result.BadRequest(
                message: "One or more validation errors occurred",
                errorCode: ErrorCodes.ValidationError,
                validationErrors: validationErrors);
        }

        _logger.LogInformation("Password changed successfully for user: {UserId}", request.UserId);

        return Result.Success("Password changed successfully");
    }
}

