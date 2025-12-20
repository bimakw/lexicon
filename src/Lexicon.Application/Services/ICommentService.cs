using Lexicon.Application.DTOs;

namespace Lexicon.Application.Services;

public interface ICommentService
{
    Task<IEnumerable<CommentDto>> GetByPostIdAsync(Guid postId, bool? isApproved = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<CommentDto>> GetPendingAsync(CancellationToken cancellationToken = default);
    Task<CommentDto> CreateAsync(Guid postId, CreateCommentDto dto, CancellationToken cancellationToken = default);
    Task<CommentDto?> ApproveAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
