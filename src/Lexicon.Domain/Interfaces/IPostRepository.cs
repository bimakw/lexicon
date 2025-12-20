using Lexicon.Domain.Common;
using Lexicon.Domain.Entities;

namespace Lexicon.Domain.Interfaces;

public interface IPostRepository : IRepository<Post>
{
    Task<Post?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<IEnumerable<Post>> GetByStatusAsync(PostStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Post>> GetByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Post>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Post>> GetByTagIdAsync(Guid tagId, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Post> Posts, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        PostStatus? status = null,
        Guid? categoryId = null,
        Guid? authorId = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);
}
