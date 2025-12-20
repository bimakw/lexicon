using Lexicon.Application.DTOs;
using Lexicon.Domain.Entities;
using Lexicon.Domain.Interfaces;

namespace Lexicon.Application.Services;

public class TagService : ITagService
{
    private readonly IUnitOfWork _unitOfWork;

    public TagService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<TagDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var tags = await _unitOfWork.Tags.GetAllAsync(cancellationToken);
        return tags.Select(MapToDto);
    }

    public async Task<TagDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tag = await _unitOfWork.Tags.GetByIdAsync(id, cancellationToken);
        return tag != null ? MapToDto(tag) : null;
    }

    public async Task<TagDto?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var tag = await _unitOfWork.Tags.GetBySlugAsync(slug, cancellationToken);
        return tag != null ? MapToDto(tag) : null;
    }

    public async Task<TagDto> CreateAsync(CreateTagDto dto, CancellationToken cancellationToken = default)
    {
        var tag = new Tag
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Slug = GenerateSlug(dto.Name),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Tags.AddAsync(tag, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(tag);
    }

    public async Task<TagDto?> UpdateAsync(Guid id, UpdateTagDto dto, CancellationToken cancellationToken = default)
    {
        var tag = await _unitOfWork.Tags.GetByIdAsync(id, cancellationToken);
        if (tag == null) return null;

        tag.Name = dto.Name;
        tag.Slug = GenerateSlug(dto.Name);
        tag.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Tags.UpdateAsync(tag, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(tag);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tag = await _unitOfWork.Tags.GetByIdAsync(id, cancellationToken);
        if (tag == null) return false;

        await _unitOfWork.Tags.DeleteAsync(tag, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static TagDto MapToDto(Tag tag)
    {
        return new TagDto(
            tag.Id,
            tag.Name,
            tag.Slug,
            tag.PostTags.Count,
            tag.CreatedAt
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
