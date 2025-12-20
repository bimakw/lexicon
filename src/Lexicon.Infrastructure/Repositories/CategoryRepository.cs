using Lexicon.Domain.Entities;
using Lexicon.Domain.Interfaces;
using Lexicon.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lexicon.Infrastructure.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(LexiconDbContext context) : base(context)
    {
    }

    public override async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Parent)
            .Include(c => c.Posts)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Parent)
            .Include(c => c.Posts)
            .FirstOrDefaultAsync(c => c.Slug == slug, cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Posts)
            .Where(c => c.ParentId == null)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetChildrenAsync(Guid parentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Posts)
            .Where(c => c.ParentId == parentId)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }
}
