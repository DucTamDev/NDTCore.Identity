using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Interfaces.Infrastructure;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Application.Features.Authentication.Commands.ForgotPassword;

/// <summary>
/// Handler for initiating password reset
/// </summary>
public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;

    public ForgotPasswordCommandHandler(
        IUserRepository userRepository,
        UserManager<AppUser> userManager,
        IEmailService emailService,
        ILogger<ForgotPasswordCommandHandler> logger)
    {
        _userRepository = userRepository;
        _userManager = userManager;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Find user by email
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

            // For security reasons, always return success even if user doesn't exist
            if (user == null)
            {
                _logger.LogWarning("Password reset requested for non-existent email: {Email}", request.Email);
                return Result.Success("If the email exists in our system, a password reset link has been sent.");
            }

            // Generate password reset token
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            // TODO: Send email with reset token
            // In production, this would send an email with a link containing the reset token
            // For now, just log it
            _logger.LogInformation("Password reset token generated for user {UserId}: {Token}", user.Id, resetToken);

            // Optionally send email (if email service is configured)
            try
            {
                var resetUrl = $"https://yourdomain.com/reset-password?token={resetToken}&email={user.Email}";
                await _emailService.SendPasswordResetEmailAsync(user.Email!, resetUrl, user.UserName ?? "User");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password reset email to {Email}", request.Email);
                // Don't fail the operation if email sending fails
            }

            return Result.Success("If the email exists in our system, a password reset link has been sent.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing forgot password request for email: {Email}", request.Email);
            return Result.InternalError("An error occurred while processing your request");
        }
    }
}

