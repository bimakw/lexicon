using Lexicon.Domain.Entities;
using Lexicon.Domain.Interfaces;
using Lexicon.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lexicon.Infrastructure.Repositories;

public class CommentRepository : Repository<Comment>, ICommentRepository
{
    public CommentRepository(LexiconDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Comment>> GetByPostIdAsync(Guid postId, bool? isApproved = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(c => c.PostId == postId);

        if (isApproved.HasValue)
            query = query.Where(c => c.IsApproved == isApproved.Value);

        return await query
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Comment>> GetPendingCommentsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Post)
            .Where(c => !c.IsApproved)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
