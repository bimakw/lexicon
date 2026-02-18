using Lexicon.Application.DTOs;
using Lexicon.Domain.Common;

namespace Lexicon.Application.Services;

public interface IPostService
{
    Task<Result<PostDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<PostDto>> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<PostListDto>>> GetPagedAsync(
        int page = 1,
        int pageSize = 10,
        PostStatus? status = null,
        Guid? categoryId = null,
        Guid? authorId = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);
    Task<Result<PostDto>> CreateAsync(CreatePostDto dto, CancellationToken cancellationToken = default);
    Task<Result<PostDto>> UpdateAsync(Guid id, UpdatePostDto dto, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<PostDto>> PublishAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<PostDto>> UnpublishAsync(Guid id, CancellationToken cancellationToken = default);
}
