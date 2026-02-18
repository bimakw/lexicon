using Lexicon.Application.DTOs;
using Lexicon.Domain.Common;
using Lexicon.Domain.Entities;
using Lexicon.Domain.Interfaces;

namespace Lexicon.Application.Services;

public class AuthorService(IUnitOfWork unitOfWork) : IAuthorService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<IEnumerable<AuthorDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var authors = await _unitOfWork.Authors.GetAllAsync(cancellationToken);
        return Result<IEnumerable<AuthorDto>>.Success(authors.Select(MapToDto));
    }

    public async Task<Result<AuthorDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var author = await _unitOfWork.Authors.GetByIdAsync(id, cancellationToken);
        return author != null 
            ? Result<AuthorDto>.Success(MapToDto(author)) 
            : Result<AuthorDto>.Failure("Author not found");
    }

    public async Task<Result<AuthorDto>> CreateAsync(CreateAuthorDto dto, CancellationToken cancellationToken = default)
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

        return Result<AuthorDto>.Success(MapToDto(author));
    }

    public async Task<Result<AuthorDto>> UpdateAsync(Guid id, UpdateAuthorDto dto, CancellationToken cancellationToken = default)
    {
        var author = await _unitOfWork.Authors.GetByIdAsync(id, cancellationToken);
        if (author == null) 
            return Result<AuthorDto>.Failure("Author not found");

        author.Name = dto.Name;
        author.Email = dto.Email;
        author.Bio = dto.Bio;
        author.AvatarUrl = dto.AvatarUrl;
        author.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Authors.UpdateAsync(author, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<AuthorDto>.Success(MapToDto(author));
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var author = await _unitOfWork.Authors.GetByIdAsync(id, cancellationToken);
        if (author == null)
            return Result<bool>.Failure("Author not found");

        await _unitOfWork.Authors.DeleteAsync(author, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
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
