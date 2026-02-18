using Lexicon.Application.DTOs;
using Lexicon.Domain.Common;

namespace Lexicon.Application.Services;

public interface ITagService
{
    Task<Result<IEnumerable<TagDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<TagDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<TagDto>> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<Result<TagDto>> CreateAsync(CreateTagDto dto, CancellationToken cancellationToken = default);
    Task<Result<TagDto>> UpdateAsync(Guid id, UpdateTagDto dto, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
