using Lexicon.Domain.Entities;

namespace Lexicon.Domain.Interfaces;

public interface ITagRepository : IRepository<Tag>
{
    Task<Tag?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<IEnumerable<Tag>> GetByPostIdAsync(Guid postId, CancellationToken cancellationToken = default);
}
