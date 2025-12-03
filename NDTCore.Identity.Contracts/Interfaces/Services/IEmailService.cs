namespace NDTCore.Identity.Contracts.Interfaces.Services;

/// <summary>
/// Service for sending emails (password reset, account confirmation, etc.)
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends a password reset email with a reset token
    /// </summary>
    /// <param name="email">Recipient email address</param>
    /// <param name="resetToken">Password reset token</param>
    /// <param name="userName">User's display name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SendPasswordResetEmailAsync(
        string email,
        string resetToken,
        string userName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an email confirmation link
    /// </summary>
    /// <param name="email">Recipient email address</param>
    /// <param name="confirmationToken">Email confirmation token</param>
    /// <param name="userName">User's display name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SendEmailConfirmationAsync(
        string email,
        string confirmationToken,
        string userName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a notification when password is changed
    /// </summary>
    /// <param name="email">Recipient email address</param>
    /// <param name="userName">User's display name</param>
    /// <param name="changedAt">When the password was changed</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SendPasswordChangedNotificationAsync(
        string email,
        string userName,
        DateTime changedAt,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a notification when account is locked
    /// </summary>
    /// <param name="email">Recipient email address</param>
    /// <param name="userName">User's display name</param>
    /// <param name="reason">Lock reason</param>
    /// <param name="lockedAt">When the account was locked</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SendAccountLockedNotificationAsync(
        string email,
        string userName,
        string reason,
        DateTime lockedAt,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a welcome email to new users
    /// </summary>
    /// <param name="email">Recipient email address</param>
    /// <param name="userName">User's display name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SendWelcomeEmailAsync(
        string email,
        string userName,
        CancellationToken cancellationToken = default);
}