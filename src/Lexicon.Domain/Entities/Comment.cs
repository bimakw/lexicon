using Lexicon.Domain.Common;

namespace Lexicon.Domain.Entities;

public class Comment : BaseEntity
{
    public Guid PostId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsApproved { get; set; } = false;

    // Navigation
    public Post Post { get; set; } = null!;
}
