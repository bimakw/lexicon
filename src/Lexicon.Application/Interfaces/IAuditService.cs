namespace Lexicon.Application.Interfaces;

public interface IAuditService
{
    Task LogAsync(AuditLogEntry entry);
    Task LogAuthenticationAsync(string action, Guid? userId, string? username, string? ipAddress, bool success, string? errorMessage = null);
    Task LogEntityChangeAsync(string action, string entityType, Guid? entityId, Guid? userId, string? oldValues, string? newValues);
}

public record AuditLogEntry
{
    public string Action { get; init; } = string.Empty;
    public string EntityType { get; init; } = string.Empty;
    public Guid? EntityId { get; init; }
    public Guid? UserId { get; init; }
    public string? Username { get; init; }
    public string? IpAddress { get; init; }
    public string? UserAgent { get; init; }
    public string? OldValues { get; init; }
    public string? NewValues { get; init; }
    public string? AdditionalData { get; init; }
    public bool IsSuccess { get; init; } = true;
    public string? ErrorMessage { get; init; }
}
