using Lexicon.Application.DTOs;

namespace Lexicon.Application.Services;

public interface IAuthorService
{
    Task<IEnumerable<AuthorDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AuthorDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AuthorDto> CreateAsync(CreateAuthorDto dto, CancellationToken cancellationToken = default);
    Task<AuthorDto?> UpdateAsync(Guid id, UpdateAuthorDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
