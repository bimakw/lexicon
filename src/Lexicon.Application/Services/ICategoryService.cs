using Lexicon.Application.DTOs;
using Lexicon.Domain.Common;

namespace Lexicon.Application.Services;

public interface ICategoryService
{
    Task<Result<IEnumerable<CategoryDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<CategoryDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<CategoryDto>> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<CategoryTreeDto>>> GetTreeAsync(CancellationToken cancellationToken = default);
    Task<Result<CategoryDto>> CreateAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default);
    Task<Result<CategoryDto>> UpdateAsync(Guid id, UpdateCategoryDto dto, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
