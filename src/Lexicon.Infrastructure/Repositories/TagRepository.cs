using Lexicon.Domain.Entities;
using Lexicon.Domain.Interfaces;
using Lexicon.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lexicon.Infrastructure.Repositories;

public class TagRepository : Repository<Tag>, ITagRepository
{
    public TagRepository(LexiconDbContext context) : base(context)
    {
    }

    public override async Task<Tag?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.PostTags)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<Tag?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.PostTags)
            .FirstOrDefaultAsync(t => t.Slug == slug, cancellationToken);
    }

    public async Task<IEnumerable<Tag>> GetByPostIdAsync(Guid postId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.PostTags)
            .Where(t => t.PostTags.Any(pt => pt.PostId == postId))
            .ToListAsync(cancellationToken);
    }
}
