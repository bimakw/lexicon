using Lexicon.Application.DTOs;

namespace Lexicon.Application.Services;

public interface ITagService
{
    Task<IEnumerable<TagDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TagDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TagDto?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<TagDto> CreateAsync(CreateTagDto dto, CancellationToken cancellationToken = default);
    Task<TagDto?> UpdateAsync(Guid id, UpdateTagDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
