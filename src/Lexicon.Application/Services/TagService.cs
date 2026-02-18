using Lexicon.Application.DTOs;
using Lexicon.Domain.Common;
using Lexicon.Domain.Entities;
using Lexicon.Domain.Interfaces;

namespace Lexicon.Application.Services;

public class TagService(IUnitOfWork unitOfWork) : ITagService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<IEnumerable<TagDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var tags = await _unitOfWork.Tags.GetAllAsync(cancellationToken);
        return Result<IEnumerable<TagDto>>.Success(tags.Select(MapToDto));
    }

    public async Task<Result<TagDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tag = await _unitOfWork.Tags.GetByIdAsync(id, cancellationToken);
        return tag != null 
            ? Result<TagDto>.Success(MapToDto(tag)) 
            : Result<TagDto>.Failure("Tag not found");
    }

    public async Task<Result<TagDto>> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var tag = await _unitOfWork.Tags.GetBySlugAsync(slug, cancellationToken);
        return tag != null 
            ? Result<TagDto>.Success(MapToDto(tag)) 
            : Result<TagDto>.Failure("Tag not found");
    }

    public async Task<Result<TagDto>> CreateAsync(CreateTagDto dto, CancellationToken cancellationToken = default)
    {
        var tag = new Tag
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Slug = dto.Name.ToLower().Replace(" ", "-"), // Basic slug for #14
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Tags.AddAsync(tag, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<TagDto>.Success(MapToDto(tag));
    }

    public async Task<Result<TagDto>> UpdateAsync(Guid id, UpdateTagDto dto, CancellationToken cancellationToken = default)
    {
        var tag = await _unitOfWork.Tags.GetByIdAsync(id, cancellationToken);
        if (tag == null) 
            return Result<TagDto>.Failure("Tag not found");

        if (tag.Name != dto.Name)
        {
            tag.Slug = dto.Name.ToLower().Replace(" ", "-"); // Basic slug for #14
            tag.Name = dto.Name;
        }

        tag.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Tags.UpdateAsync(tag, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<TagDto>.Success(MapToDto(tag));
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tag = await _unitOfWork.Tags.GetByIdAsync(id, cancellationToken);
        if (tag == null)
            return Result<bool>.Failure("Tag not found");

        await _unitOfWork.Tags.DeleteAsync(tag, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
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
}
