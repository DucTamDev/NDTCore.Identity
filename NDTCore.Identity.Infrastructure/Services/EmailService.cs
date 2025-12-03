using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Interfaces.Services;

namespace NDTCore.Identity.Infrastructure.Services;

/// <summary>
/// Stub implementation of email service - logs emails instead of sending them
/// Replace with actual SMTP/SendGrid/AWS SES implementation as needed
/// </summary>
public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task SendPasswordResetEmailAsync(
        string email,
        string resetToken,
        string userName,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Password reset email would be sent to {Email} for user {UserName}. Token: {Token}",
            email, userName, MaskToken(resetToken));

        // TODO: Implement actual email sending
        // Example: await _smtpClient.SendAsync(message);

        return Task.CompletedTask;
    }

    public Task SendEmailConfirmationAsync(
        string email,
        string confirmationToken,
        string userName,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Email confirmation would be sent to {Email} for user {UserName}. Token: {Token}",
            email, userName, MaskToken(confirmationToken));

        return Task.CompletedTask;
    }

    public Task SendPasswordChangedNotificationAsync(
        string email,
        string userName,
        DateTime changedAt,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Password changed notification would be sent to {Email} for user {UserName}. Changed at: {ChangedAt}",
            email, userName, changedAt);

        return Task.CompletedTask;
    }

    public Task SendAccountLockedNotificationAsync(
        string email,
        string userName,
        string reason,
        DateTime lockedAt,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Account locked notification would be sent to {Email} for user {UserName}. Reason: {Reason}, Locked at: {LockedAt}",
            email, userName, reason, lockedAt);

        return Task.CompletedTask;
    }

    public Task SendWelcomeEmailAsync(
        string email,
        string userName,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Welcome email would be sent to {Email} for user {UserName}",
            email, userName);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Masks sensitive token data for logging
    /// </summary>
    private static string MaskToken(string token)
    {
        if (string.IsNullOrEmpty(token) || token.Length < 8)
            return "***";

        return $"{token[..4]}...{token[^4..]}";
    }
}