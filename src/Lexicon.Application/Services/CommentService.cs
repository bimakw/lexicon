using Lexicon.Application.DTOs;
using Lexicon.Domain.Entities;
using Lexicon.Domain.Interfaces;

namespace Lexicon.Application.Services;

public class CommentService : ICommentService
{
    private readonly IUnitOfWork _unitOfWork;

    public CommentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CommentDto>> GetByPostIdAsync(Guid postId, bool? isApproved = null, CancellationToken cancellationToken = default)
    {
        var comments = await _unitOfWork.Comments.GetByPostIdAsync(postId, isApproved, cancellationToken);
        return comments.Select(MapToDto);
    }

    public async Task<IEnumerable<CommentDto>> GetPendingAsync(CancellationToken cancellationToken = default)
    {
        var comments = await _unitOfWork.Comments.GetPendingCommentsAsync(cancellationToken);
        return comments.Select(MapToDto);
    }

    public async Task<CommentDto> CreateAsync(Guid postId, CreateCommentDto dto, CancellationToken cancellationToken = default)
    {
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            PostId = postId,
            AuthorName = dto.AuthorName,
            Email = dto.Email,
            Content = dto.Content,
            IsApproved = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Comments.AddAsync(comment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(comment);
    }

    public async Task<CommentDto?> ApproveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var comment = await _unitOfWork.Comments.GetByIdAsync(id, cancellationToken);
        if (comment == null) return null;

        comment.IsApproved = true;
        comment.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Comments.UpdateAsync(comment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(comment);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var comment = await _unitOfWork.Comments.GetByIdAsync(id, cancellationToken);
        if (comment == null) return false;

        await _unitOfWork.Comments.DeleteAsync(comment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static CommentDto MapToDto(Comment comment)
    {
        return new CommentDto(
            comment.Id,
            comment.PostId,
            comment.AuthorName,
            comment.Email,
            comment.Content,
            comment.IsApproved,
            comment.CreatedAt
        );
    }
}
