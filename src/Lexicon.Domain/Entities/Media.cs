using Lexicon.Domain.Common;

namespace Lexicon.Domain.Entities;

public class Media : BaseEntity
{
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }
    public string? AltText { get; set; }
}
