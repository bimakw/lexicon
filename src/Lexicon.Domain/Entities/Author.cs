using Lexicon.Domain.Common;

namespace Lexicon.Domain.Entities;

public class Author : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }

    // Navigation
    public ICollection<Post> Posts { get; set; } = new List<Post>();
}
