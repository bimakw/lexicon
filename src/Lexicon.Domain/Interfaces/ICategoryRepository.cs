using Lexicon.Domain.Entities;

namespace Lexicon.Domain.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<IEnumerable<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Category>> GetChildrenAsync(Guid parentId, CancellationToken cancellationToken = default);
}
