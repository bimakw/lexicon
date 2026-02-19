using Lexicon.Application.DTOs;
using Lexicon.Domain.Entities;
using Lexicon.Domain.Interfaces;
using Lexicon.Domain.Common;
using Microsoft.Extensions.Logging;
using Lexicon.Application.Helpers;

namespace Lexicon.Application.Services;

public class TagService(
    IUnitOfWork unitOfWork,
    ILogger<TagService> logger) : ITagService
{
    public async Task<Result<IEnumerable<TagDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var tags = await unitOfWork.Tags.GetAllAsync(ct);
        return Result<IEnumerable<TagDto>>.Success(tags.Select(MapToDto));
    }

    public async Task<Result<TagDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var tag = await unitOfWork.Tags.GetByIdAsync(id, ct);
        
        if (tag == null)
            return Result<TagDto>.Failure("Tag tidak ditemukan.");

        return Result<TagDto>.Success(MapToDto(tag));
    }

    public async Task<Result<TagDto>> GetBySlugAsync(string slug, CancellationToken ct = default)
    {
        var tag = await unitOfWork.Tags.GetBySlugAsync(slug, ct);
        
        if (tag == null)
            return Result<TagDto>.Failure("Tag tidak ditemukan.");

        return Result<TagDto>.Success(MapToDto(tag));
    }

    public async Task<Result<TagDto>> CreateAsync(CreateTagDto dto, CancellationToken ct = default)
    {
        try 
        {
            var slug = SlugHelper.GenerateSlug(dto.Name);

            // cek slug duplikat
            var existing = await unitOfWork.Tags.GetBySlugAsync(slug, ct);
            if (existing != null)
                return Result<TagDto>.Failure("Nama atau slug tag sudah digunakan.");

            var tag = new Tag
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Slug = slug,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await unitOfWork.Tags.AddAsync(tag, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return Result<TagDto>.Success(MapToDto(tag));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Gagal bikin tag baru: {TagName}", dto.Name);
            return Result<TagDto>.Failure("Gagal memproses pembuatan tag baru.");
        }
    }

    public async Task<Result<TagDto>> UpdateAsync(Guid id, UpdateTagDto dto, CancellationToken ct = default)
    {
        try 
        {
            var tag = await unitOfWork.Tags.GetByIdAsync(id, ct);
            if (tag == null) 
                return Result<TagDto>.Failure("Tag yang akan diperbarui tidak ditemukan.");

            if (tag.Name != dto.Name)
            {
                var newSlug = SlugHelper.GenerateSlug(dto.Name);
                
                // pastikan tidak duplikat dengan tag lain
                var existing = await unitOfWork.Tags.GetBySlugAsync(newSlug, ct);
                if (existing != null && existing.Id != id)
                    return Result<TagDto>.Failure("Nama atau slug tersebut sudah digunakan oleh tag lain.");

                tag.Name = dto.Name;
                tag.Slug = newSlug;
            }

            tag.UpdatedAt = DateTime.UtcNow;

            await unitOfWork.Tags.UpdateAsync(tag, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return Result<TagDto>.Success(MapToDto(tag));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error pas update tag {TagId}", id);
            return Result<TagDto>.Failure("Gagal memperbarui data tag karena kendala teknis.");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        try 
        {
            var tag = await unitOfWork.Tags.GetByIdAsync(id, ct);
            if (tag == null)
                return Result<bool>.Failure("Tag yang ingin dihapus tidak ditemukan.");

            await unitOfWork.Tags.DeleteAsync(tag, ct);
            await unitOfWork.SaveChangesAsync(ct);
            
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Gagal hapus tag {TagId}", id);
            return Result<bool>.Failure("Gagal menghapus tag. Silakan coba lagi nanti.");
        }
    }

    private static TagDto MapToDto(Tag tag) => new(
        tag.Id,
        tag.Name,
        tag.Slug,
        tag.PostTags?.Count ?? 0,
        tag.CreatedAt
    );
}
