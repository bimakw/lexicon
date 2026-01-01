using Lexicon.Application.Interfaces;
using Lexicon.Domain.Entities;
using Lexicon.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace Lexicon.Infrastructure.Security;

public class AuditService : IAuditService
{
    private readonly LexiconDbContext _context;
    private readonly ILogger<AuditService> _logger;

    public AuditService(LexiconDbContext context, ILogger<AuditService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task LogAsync(AuditLogEntry entry)
    {
        var auditLog = new AuditLog
        {
            Id = Guid.NewGuid(),
            Action = entry.Action,
            EntityType = entry.EntityType,
            EntityId = entry.EntityId,
            UserId = entry.UserId,
            Username = MaskSensitiveData(entry.Username),
            IpAddress = entry.IpAddress,
            UserAgent = TruncateUserAgent(entry.UserAgent),
            OldValues = entry.OldValues,
            NewValues = entry.NewValues,
            AdditionalData = entry.AdditionalData,
            IsSuccess = entry.IsSuccess,
            ErrorMessage = entry.ErrorMessage,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.AuditLogs.Add(auditLog);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log to file/console if database fails
            _logger.LogError(ex, "Failed to save audit log: {Action} {EntityType} {EntityId}",
                entry.Action, entry.EntityType, entry.EntityId);
        }

        // Also log to structured logging for real-time monitoring
        if (entry.IsSuccess)
        {
            _logger.LogInformation(
                "Audit: {Action} on {EntityType} (ID: {EntityId}) by User {UserId} from {IpAddress}",
                entry.Action, entry.EntityType, entry.EntityId, entry.UserId, entry.IpAddress);
        }
        else
        {
            _logger.LogWarning(
                "Audit FAILED: {Action} on {EntityType} (ID: {EntityId}) by User {UserId} from {IpAddress} - {Error}",
                entry.Action, entry.EntityType, entry.EntityId, entry.UserId, entry.IpAddress, entry.ErrorMessage);
        }
    }

    public async Task LogAuthenticationAsync(string action, Guid? userId, string? username, string? ipAddress, bool success, string? errorMessage = null)
    {
        await LogAsync(new AuditLogEntry
        {
            Action = action,
            EntityType = "Authentication",
            UserId = userId,
            Username = username,
            IpAddress = ipAddress,
            IsSuccess = success,
            ErrorMessage = errorMessage
        });
    }

    public async Task LogEntityChangeAsync(string action, string entityType, Guid? entityId, Guid? userId, string? oldValues, string? newValues)
    {
        await LogAsync(new AuditLogEntry
        {
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            UserId = userId,
            OldValues = MaskSensitiveFields(oldValues),
            NewValues = MaskSensitiveFields(newValues),
            IsSuccess = true
        });
    }

    private static string? MaskSensitiveData(string? value)
    {
        if (string.IsNullOrEmpty(value)) return value;

        // Mask email addresses partially
        if (value.Contains('@'))
        {
            var parts = value.Split('@');
            if (parts[0].Length > 2)
            {
                return $"{parts[0][..2]}***@{parts[1]}";
            }
        }

        return value;
    }

    private static string? MaskSensitiveFields(string? json)
    {
        if (string.IsNullOrEmpty(json)) return json;

        // Simple masking for common sensitive fields
        var sensitiveFields = new[] { "password", "passwordHash", "token", "secret", "apiKey" };

        foreach (var field in sensitiveFields)
        {
            // Case insensitive replacement
            json = System.Text.RegularExpressions.Regex.Replace(
                json,
                $@"""{field}""\s*:\s*""[^""]*""",
                $@"""{field}"": ""[REDACTED]""",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        return json;
    }

    private static string? TruncateUserAgent(string? userAgent)
    {
        if (string.IsNullOrEmpty(userAgent)) return userAgent;
        return userAgent.Length > 500 ? userAgent[..500] : userAgent;
    }
}
