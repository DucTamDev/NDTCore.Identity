using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Domain.Constants;
using NDTCore.Identity.Domain.Exceptions;

namespace NDTCore.Identity.Infrastructure.Exceptions;

/// <summary>
/// Helper class for consistent exception handling in repositories
/// </summary>
public static class ExceptionHandler
{
    /// <summary>
    /// Handles database exceptions and throws appropriate domain exceptions
    /// </summary>
    public static void HandleDatabaseException(
        Exception ex,
        ILogger logger,
        string operation,
        object? entityId = null)
    {
        switch (ex)
        {
            case DbUpdateConcurrencyException:
                HandleConcurrencyException((DbUpdateConcurrencyException)ex, logger, operation, entityId);
                break;

            case DbUpdateException dbEx:
                HandleDbUpdateException(dbEx, logger, operation);
                break;

            default:
                HandleGenericException(ex, logger, operation);
                break;
        }
    }

    private static void HandleConcurrencyException(
        DbUpdateConcurrencyException ex,
        ILogger logger,
        string operation,
        object? entityId)
    {
        logger.LogWarning(ex, "Concurrency conflict during {Operation} for entity {EntityId}", operation, entityId);
        throw new ConflictException(
            $"Concurrency conflict during {operation} for entity {entityId}.",
            "The record was modified by another user. Please refresh and try again.",
            ex);
    }

    private static void HandleDbUpdateException(
        DbUpdateException ex,
        ILogger logger,
        string operation)
    {
        var innerException = ex.InnerException?.Message ?? string.Empty;

        // Unique constraint violation
        if (innerException.Contains("UNIQUE") ||
            innerException.Contains("IX_") ||
            innerException.Contains("PK_") ||
            innerException.Contains("Cannot insert duplicate key"))
        {
            logger.LogWarning(ex, "Unique constraint violation during {Operation}", operation);
            throw new ConflictException(
                $"Unique constraint violation during {operation}.",
                "A record with the same value already exists.",
                ex);
        }

        // Foreign key constraint violation
        if (innerException.Contains("FOREIGN KEY") ||
            innerException.Contains("FK_") ||
            innerException.Contains("The DELETE statement conflicted"))
        {
            logger.LogWarning(ex, "Foreign key constraint violation during {Operation}", operation);
            throw new DomainException(
                $"Foreign key constraint violation during {operation}.",
                "Cannot perform this operation due to related records.",
                ex);
        }

        // Generic database error
        logger.LogError(ex, "Database update error during {Operation}", operation);
        throw new DomainException(
            $"Database error during {operation}: {ex.GetBaseException().Message}",
            "An error occurred while saving data. Please try again.",
            ex);
    }

    private static void HandleGenericException(
        Exception ex,
        ILogger logger,
        string operation)
    {
        logger.LogError(ex, "Unexpected error during {Operation}", operation);
        throw new DomainException(
            $"Unexpected error during {operation}: {ex.GetBaseException().Message}",
            "An unexpected error occurred. Please try again.",
            ex);
    }
}
