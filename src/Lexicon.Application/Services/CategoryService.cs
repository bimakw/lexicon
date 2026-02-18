using Lexicon.Application.DTOs;
using Lexicon.Domain.Entities;
using Lexicon.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Lexicon.Application.Services;

<<<<<<< Updated upstream
public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(IUnitOfWork unitOfWork)
=======
public class CategoryService(
    IUnitOfWork unitOfWork, 
    IMapper mapper,
    ILogger<CategoryService> logger) : ICategoryService
{
    public async Task<Result<IEnumerable<CategoryDto>>> GetAllAsync(CancellationToken ct = default)
>>>>>>> Stashed changes
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
<<<<<<< Updated upstream
        var categories = await _unitOfWork.Categories.GetAllAsync(cancellationToken);
        return categories.Select(c => MapToDto(c));
=======
        var category = await unitOfWork.Categories.GetByIdAsync(id, ct);
        
        if (category == null)
            return Result<CategoryDto>.Failure("Kategori tidak ditemukan.");

        return Result<CategoryDto>.Success(mapper.Map<CategoryDto>(category));
>>>>>>> Stashed changes
    }

    public async Task<CategoryDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
<<<<<<< Updated upstream
        var category = await _unitOfWork.Categories.GetByIdAsync(id, cancellationToken);
        return category != null ? MapToDto(category) : null;
=======
        var category = await unitOfWork.Categories.GetBySlugAsync(slug, ct);
        
        if (category == null)
            return Result<CategoryDto>.Failure("Slug kategori tidak terdaftar.");

        return Result<CategoryDto>.Success(mapper.Map<CategoryDto>(category));
>>>>>>> Stashed changes
    }

    public async Task<CategoryDto?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
<<<<<<< Updated upstream
        var category = await _unitOfWork.Categories.GetBySlugAsync(slug, cancellationToken);
        return category != null ? MapToDto(category) : null;
    }

    public async Task<IEnumerable<CategoryTreeDto>> GetTreeAsync(CancellationToken cancellationToken = default)
    {
        var rootCategories = await _unitOfWork.Categories.GetRootCategoriesAsync(cancellationToken);
=======
        // tarik root categories dulu
        var rootCategories = await unitOfWork.Categories.GetRootCategoriesAsync(ct);
>>>>>>> Stashed changes
        var result = new List<CategoryTreeDto>();

        foreach (var category in rootCategories)
        {
            result.Add(await BuildTreeAsync(category, cancellationToken));
        }

        return result;
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        try 
        {
<<<<<<< Updated upstream
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
=======
            var slug = SlugHelper.GenerateSlug(dto.Name);

            // pastikan slug tidak duplikat
            var existing = await unitOfWork.Categories.GetBySlugAsync(slug, ct);
            if (existing != null)
                return Result<CategoryDto>.Failure("Nama atau slug kategori sudah digunakan.");

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Slug = slug,
                Description = dto.Description,
                ParentId = dto.ParentId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await unitOfWork.Categories.AddAsync(category, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return Result<CategoryDto>.Success(mapper.Map<CategoryDto>(category));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error pas nyimpen kategori baru: {CategoryName}", dto.Name);
            return Result<CategoryDto>.Failure("Gagal memproses pembuatan kategori baru.");
        }
>>>>>>> Stashed changes
    }

    public async Task<CategoryDto?> UpdateAsync(Guid id, UpdateCategoryDto dto, CancellationToken cancellationToken = default)
    {
<<<<<<< Updated upstream
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
=======
        try 
        {
            var category = await unitOfWork.Categories.GetByIdAsync(id, ct);
            if (category == null) 
                return Result<CategoryDto>.Failure("Data kategori tidak ditemukan di sistem.");

            if (category.Name != dto.Name)
            {
                var newSlug = SlugHelper.GenerateSlug(dto.Name);
                
                // cek slug baru
                var existing = await unitOfWork.Categories.GetBySlugAsync(newSlug, ct);
                if (existing != null && existing.Id != id)
                    return Result<CategoryDto>.Failure("Nama atau slug tersebut sudah digunakan oleh kategori lain.");

                category.Name = dto.Name;
                category.Slug = newSlug;
            }

            category.Description = dto.Description;
            category.ParentId = dto.ParentId;
            category.UpdatedAt = DateTime.UtcNow;

            await unitOfWork.Categories.UpdateAsync(category, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return Result<CategoryDto>.Success(mapper.Map<CategoryDto>(category));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Gagal update kategori {CategoryId}", id);
            return Result<CategoryDto>.Failure("Gagal memperbarui data kategori karena kendala teknis.");
        }
>>>>>>> Stashed changes
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
<<<<<<< Updated upstream
        var category = await _unitOfWork.Categories.GetByIdAsync(id, cancellationToken);
        if (category == null) return false;

        await _unitOfWork.Categories.DeleteAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
=======
        try 
        {
            var category = await unitOfWork.Categories.GetByIdAsync(id, ct);
            if (category == null)
                return Result<bool>.Failure("Kategori yang ingin dihapus tidak ditemukan.");

            await unitOfWork.Categories.DeleteAsync(category, ct);
            await unitOfWork.SaveChangesAsync(ct);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error pas hapus kategori {CategoryId}", id);
            return Result<bool>.Failure("Gagal menghapus kategori. Pastikan tidak ada data yang terkait.");
        }
>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
    private static string GenerateSlug(string name)
    {
        return name.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("--", "-")
            .Trim('-');
    }
}
=======

>>>>>>> Stashed changes
