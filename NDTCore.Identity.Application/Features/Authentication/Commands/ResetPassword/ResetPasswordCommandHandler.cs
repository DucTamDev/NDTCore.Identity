using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Domain.Constants;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Application.Features.Authentication.Commands.ResetPassword;

/// <summary>
/// Handler for resetting user password
/// </summary>
public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<ResetPasswordCommandHandler> _logger;

    public ResetPasswordCommandHandler(
        IUserRepository userRepository,
        UserManager<AppUser> userManager,
        ILogger<ResetPasswordCommandHandler> logger)
    {
        _userRepository = userRepository;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Find user by email
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (user == null)
                return Result.NotFound("User not found");

            // Reset password using the token
            var resetResult = await _userManager.ResetPasswordAsync(user, request.ResetToken, request.NewPassword);

            if (!resetResult.Succeeded)
            {
                var validationErrors = resetResult.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToList());

                _logger.LogWarning("Password reset failed for user {UserId}: {Errors}",
                    user.Id,
                    string.Join(", ", resetResult.Errors.Select(e => e.Description)));

                return Result.BadRequest(
                    "Password reset failed",
                    ErrorCodes.ValidationError,
                    validationErrors);
            }

            _logger.LogInformation("Password successfully reset for user {UserId}", user.Id);
            return Result.Success("Password has been reset successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password for email: {Email}", request.Email);
            return Result.InternalError("An error occurred while resetting your password");
        }
    }
}

