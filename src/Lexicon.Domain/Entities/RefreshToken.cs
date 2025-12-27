using Lexicon.Domain.Common;

namespace Lexicon.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public string Token { get; set; } = string.Empty;
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? RevokedReason { get; set; }
    public string? ReplacedByToken { get; set; }
    public string? CreatedByIp { get; set; }
    public string? RevokedByIp { get; set; }

    // Navigation
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    // Helper properties
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt != null;
    public bool IsActive => !IsRevoked && !IsExpired;
}
