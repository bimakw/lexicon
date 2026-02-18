using Lexicon.Application.DTOs;
using Lexicon.Domain.Common;

namespace Lexicon.Application.Services;

public interface ITagService
{
    Task<Result<IEnumerable<TagDto>>> GetAllAsync(CancellationToken ct = default);
    Task<Result<TagDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<TagDto>> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<Result<TagDto>> CreateAsync(CreateTagDto dto, CancellationToken ct = default);
    Task<Result<TagDto>> UpdateAsync(Guid id, UpdateTagDto dto, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken ct = default);
}
