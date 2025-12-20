using Lexicon.Domain.Entities;

namespace Lexicon.Domain.Interfaces;

public interface ICommentRepository : IRepository<Comment>
{
    Task<IEnumerable<Comment>> GetByPostIdAsync(Guid postId, bool? isApproved = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Comment>> GetPendingCommentsAsync(CancellationToken cancellationToken = default);
}
