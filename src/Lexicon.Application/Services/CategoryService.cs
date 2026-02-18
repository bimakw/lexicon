using AutoMapper;
using Lexicon.Application.DTOs;
using Lexicon.Domain.Common;
using Lexicon.Domain.Entities;
using Lexicon.Domain.Interfaces;

namespace Lexicon.Application.Services;

public class CategoryService(IUnitOfWork unitOfWork, IMapper mapper) : ICategoryService
{
    // Gunakan unitOfWork langsung dari primary constructor agar lebih ringkas
    public async Task<Result<IEnumerable<CategoryDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var categories = await unitOfWork.Categories.GetAllAsync(ct);
        return Result<IEnumerable<CategoryDto>>.Success(mapper.Map<IEnumerable<CategoryDto>>(categories));
    }

    public async Task<Result<CategoryDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var category = await unitOfWork.Categories.GetByIdAsync(id, ct);
        return category != null 
            ? Result<CategoryDto>.Success(mapper.Map<CategoryDto>(category)) 
            : Result<CategoryDto>.Failure("Kategori tidak ditemukan.");
    }

    public async Task<Result<CategoryDto>> GetBySlugAsync(string slug, CancellationToken ct = default)
    {
        var category = await unitOfWork.Categories.GetBySlugAsync(slug, ct);
        return category != null 
            ? Result<CategoryDto>.Success(mapper.Map<CategoryDto>(category)) 
            : Result<CategoryDto>.Failure($"Slug '{slug}' tidak terdaftar.");
    }

    public async Task<Result<IEnumerable<CategoryTreeDto>>> GetTreeAsync(CancellationToken ct = default)
    {
        // Ambil root categories, sisanya akan di-resolve secara rekursif
        var rootCategories = await unitOfWork.Categories.GetRootCategoriesAsync(ct);
        var result = new List<CategoryTreeDto>();

        foreach (var category in rootCategories)
        {
            result.Add(await BuildTreeAsync(category, ct));
        }

        return Result<IEnumerable<CategoryTreeDto>>.Success(result);
    }

    public async Task<Result<CategoryDto>> CreateAsync(CreateCategoryDto dto, CancellationToken ct = default)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            // Slug manual sementara untuk keperluan issue #14
            Slug = dto.Name.ToLower().Trim().Replace(" ", "-"), 
            Description = dto.Description,
            ParentId = dto.ParentId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await unitOfWork.Categories.AddAsync(category, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result<CategoryDto>.Success(mapper.Map<CategoryDto>(category));
    }

    public async Task<Result<CategoryDto>> UpdateAsync(Guid id, UpdateCategoryDto dto, CancellationToken ct = default)
    {
        var category = await unitOfWork.Categories.GetByIdAsync(id, ct);
        if (category == null) 
            return Result<CategoryDto>.Failure("Data kategori tidak ditemukan.");

        if (category.Name != dto.Name)
        {
            // Jika nama berubah, slug juga harus menyesuaikan
            category.Slug = dto.Name.ToLower().Trim().Replace(" ", "-");
            category.Name = dto.Name;
        }

        category.Description = dto.Description;
        category.ParentId = dto.ParentId;
        category.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.Categories.UpdateAsync(category, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result<CategoryDto>.Success(mapper.Map<CategoryDto>(category));
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var category = await unitOfWork.Categories.GetByIdAsync(id, ct);
        if (category == null)
            return Result<bool>.Failure("Gagal menghapus, kategori memang tidak ada.");

        await unitOfWork.Categories.DeleteAsync(category, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }

    private async Task<CategoryTreeDto> BuildTreeAsync(Category category, CancellationToken ct)
    {
        var children = await unitOfWork.Categories.GetChildrenAsync(category.Id, ct);
        var childDtos = new List<CategoryTreeDto>();

        foreach (var child in children)
        {
            childDtos.Add(await BuildTreeAsync(child, ct));
        }

        return new CategoryTreeDto(
            category.Id,
            category.Name,
            category.Slug,
            category.Description,
            childDtos
        );
    }
}


