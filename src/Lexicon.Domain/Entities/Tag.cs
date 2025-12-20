using Lexicon.Domain.Common;

namespace Lexicon.Domain.Entities;

public class Tag : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    // Navigation
    public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
}
