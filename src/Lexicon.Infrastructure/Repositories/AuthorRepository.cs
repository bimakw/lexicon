using Lexicon.Domain.Entities;
using Lexicon.Domain.Interfaces;
using Lexicon.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lexicon.Infrastructure.Repositories;

public class AuthorRepository : Repository<Author>, IAuthorRepository
{
    public AuthorRepository(LexiconDbContext context) : base(context)
    {
    }

    public override async Task<Author?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Posts)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<Author?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Posts)
            .FirstOrDefaultAsync(a => a.Email == email, cancellationToken);
    }
}
