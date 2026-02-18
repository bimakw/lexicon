using Lexicon.Application.DTOs;
using Lexicon.Domain.Common;

namespace Lexicon.Application.Services;

public interface IAuthorService
{
    Task<Result<IEnumerable<AuthorDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<AuthorDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<AuthorDto>> CreateAsync(CreateAuthorDto dto, CancellationToken cancellationToken = default);
    Task<Result<AuthorDto>> UpdateAsync(Guid id, UpdateAuthorDto dto, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
