using Lexicon.Application.DTOs;
using Lexicon.Domain.Entities;
using Lexicon.Domain.Interfaces;

namespace Lexicon.Application.Services;

public class AuthorService : IAuthorService
{
    private readonly IUnitOfWork _unitOfWork;

    public AuthorService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<AuthorDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var authors = await _unitOfWork.Authors.GetAllAsync(cancellationToken);
        return authors.Select(MapToDto);
    }

    public async Task<AuthorDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var author = await _unitOfWork.Authors.GetByIdAsync(id, cancellationToken);
        return author != null ? MapToDto(author) : null;
    }

    public async Task<AuthorDto> CreateAsync(CreateAuthorDto dto, CancellationToken cancellationToken = default)
    {
        var author = new Author
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Email = dto.Email,
            Bio = dto.Bio,
            AvatarUrl = dto.AvatarUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Authors.AddAsync(author, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(author);
    }

    public async Task<AuthorDto?> UpdateAsync(Guid id, UpdateAuthorDto dto, CancellationToken cancellationToken = default)
    {
        var author = await _unitOfWork.Authors.GetByIdAsync(id, cancellationToken);
        if (author == null) return null;

        author.Name = dto.Name;
        author.Email = dto.Email;
        author.Bio = dto.Bio;
        author.AvatarUrl = dto.AvatarUrl;
        author.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Authors.UpdateAsync(author, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(author);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var author = await _unitOfWork.Authors.GetByIdAsync(id, cancellationToken);
        if (author == null) return false;

        await _unitOfWork.Authors.DeleteAsync(author, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static AuthorDto MapToDto(Author author)
    {
        return new AuthorDto(
            author.Id,
            author.Name,
            author.Email,
            author.Bio,
            author.AvatarUrl,
            author.Posts.Count,
            author.CreatedAt,
            author.UpdatedAt
        );
    }
}
