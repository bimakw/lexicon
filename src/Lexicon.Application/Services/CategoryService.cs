using Lexicon.Application.DTOs;
using Lexicon.Domain.Entities;
using Lexicon.Domain.Interfaces;

namespace Lexicon.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _unitOfWork.Categories.GetAllAsync(cancellationToken);
        return categories.Select(c => MapToDto(c));
    }

    public async Task<CategoryDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id, cancellationToken);
        return category != null ? MapToDto(category) : null;
    }

    public async Task<CategoryDto?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var category = await _unitOfWork.Categories.GetBySlugAsync(slug, cancellationToken);
        return category != null ? MapToDto(category) : null;
    }

    public async Task<IEnumerable<CategoryTreeDto>> GetTreeAsync(CancellationToken cancellationToken = default)
    {
        var rootCategories = await _unitOfWork.Categories.GetRootCategoriesAsync(cancellationToken);
        var result = new List<CategoryTreeDto>();

        foreach (var category in rootCategories)
        {
            result.Add(await BuildTreeAsync(category, cancellationToken));
        }

        return result;
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Slug = GenerateSlug(dto.Name),
            Description = dto.Description,
            ParentId = dto.ParentId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Categories.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(category);
    }

    public async Task<CategoryDto?> UpdateAsync(Guid id, UpdateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id, cancellationToken);
        if (category == null) return null;

        category.Name = dto.Name;
        category.Slug = GenerateSlug(dto.Name);
        category.Description = dto.Description;
        category.ParentId = dto.ParentId;
        category.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Categories.UpdateAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(category);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id, cancellationToken);
        if (category == null) return false;

        await _unitOfWork.Categories.DeleteAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task<CategoryTreeDto> BuildTreeAsync(Category category, CancellationToken cancellationToken)
    {
        var children = await _unitOfWork.Categories.GetChildrenAsync(category.Id, cancellationToken);
        var childDtos = new List<CategoryTreeDto>();

        foreach (var child in children)
        {
            childDtos.Add(await BuildTreeAsync(child, cancellationToken));
        }

        return new CategoryTreeDto(
            category.Id,
            category.Name,
            category.Slug,
            category.Description,
            childDtos
        );
    }

    private static CategoryDto MapToDto(Category category)
    {
        return new CategoryDto(
            category.Id,
            category.Name,
            category.Slug,
            category.Description,
            category.ParentId,
            category.Parent?.Name,
            category.Posts.Count,
            category.CreatedAt,
            category.UpdatedAt
        );
    }

    private static string GenerateSlug(string name)
    {
        return name.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("--", "-")
            .Trim('-');
    }
}
