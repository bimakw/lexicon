using Lexicon.Domain.Entities;
using Lexicon.Domain.Interfaces;
using Lexicon.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lexicon.Infrastructure.Repositories;

public class MediaRepository : Repository<Media>, IMediaRepository
{
    public MediaRepository(LexiconDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Media>> GetByContentTypeAsync(string contentType, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => m.ContentType.StartsWith(contentType))
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
