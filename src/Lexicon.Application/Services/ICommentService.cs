using Lexicon.Application.DTOs;
using Lexicon.Domain.Common;

namespace Lexicon.Application.Services;

public interface ICommentService
{
    Task<Result<IEnumerable<CommentDto>>> GetByPostIdAsync(Guid postId, bool? isApproved = null, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<CommentDto>>> GetPendingAsync(CancellationToken cancellationToken = default);
    Task<Result<CommentDto>> CreateAsync(Guid postId, CreateCommentDto dto, CancellationToken cancellationToken = default);
    Task<Result<CommentDto>> ApproveAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
