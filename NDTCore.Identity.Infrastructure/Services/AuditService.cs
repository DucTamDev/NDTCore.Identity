using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Interfaces.Infrastructure;
using NDTCore.Identity.Domain.Entities;
using NDTCore.Identity.Infrastructure.Persistence.Context;
using System.Text.Json;

namespace NDTCore.Identity.Infrastructure.Services;

/// <summary>
/// Service implementation for audit logging
/// </summary>
public class AuditService : IAuditService
{
    private readonly IdentityDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<AuditService> _logger;

    public AuditService(
        IdentityDbContext context,
        ICurrentUserService currentUserService,
        ILogger<AuditService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task LogAsync(
        string entityType,
        Guid entityId,
        string action,
        object? oldValues = null,
        object? newValues = null,
        Guid? userId = null,
        string? userName = null,
        string? ipAddress = null,
        string? userAgent = null,
        string? httpMethod = null,
        string? requestPath = null,
        string? notes = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid(),
                EntityType = entityType,
                EntityId = entityId,
                Action = action,
                OldValues = oldValues != null ? JsonSerializer.Serialize(oldValues, new JsonSerializerOptions { WriteIndented = false }) : null,
                NewValues = newValues != null ? JsonSerializer.Serialize(newValues, new JsonSerializerOptions { WriteIndented = false }) : null,
                UserId = userId ?? _currentUserService.UserId,
                UserName = userName ?? _currentUserService.UserName,
                IpAddress = ipAddress ?? _currentUserService.IpAddress,
                UserAgent = userAgent ?? _currentUserService.UserAgent,
                HttpMethod = httpMethod,
                RequestPath = requestPath,
                Notes = notes,
                CreatedAt = DateTime.UtcNow
            };

            _context.Set<AuditLog>().Add(auditLog);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogDebug(
                "Audit log created: {EntityType} {EntityId} - {Action} by {UserName}",
                entityType, entityId, action, auditLog.UserName);
        }
        catch (Exception ex)
        {
            // Log error but don't throw - audit logging should not break the main operation
            _logger.LogError(ex,
                "Failed to create audit log: {EntityType} {EntityId} - {Action}",
                entityType, entityId, action);
        }
    }
}

