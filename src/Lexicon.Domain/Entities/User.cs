using Lexicon.Domain.Common;

namespace Lexicon.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public bool EmailConfirmed { get; set; } = false;
    public int FailedLoginAttempts { get; set; } = 0;
    public DateTime? LockoutEnd { get; set; }
    public DateTime? LastLoginAt { get; set; }

    // Navigation
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;
    public Guid? AuthorId { get; set; }
    public Author? Author { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
