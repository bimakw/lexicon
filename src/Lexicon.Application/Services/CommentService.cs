using Lexicon.Application.DTOs;
using Lexicon.Domain.Entities;
using Lexicon.Domain.Interfaces;
using Lexicon.Domain.Common;
using Microsoft.Extensions.Logging;

namespace Lexicon.Application.Services;

public class CommentService(
    IUnitOfWork unitOfWork,
    ILogger<CommentService> logger) : ICommentService
{
    public async Task<Result<IEnumerable<CommentDto>>> GetByPostIdAsync(Guid postId, bool? isApproved = null, CancellationToken ct = default)
    {
        var comments = await unitOfWork.Comments.GetByPostIdAsync(postId, isApproved, ct);
        return Result<IEnumerable<CommentDto>>.Success(comments.Select(MapToDto));
    }

    public async Task<Result<IEnumerable<CommentDto>>> GetPendingAsync(CancellationToken ct = default)
    {
        var comments = await unitOfWork.Comments.GetPendingCommentsAsync(ct);
        return Result<IEnumerable<CommentDto>>.Success(comments.Select(MapToDto));
    }

    public async Task<Result<CommentDto>> CreateAsync(Guid postId, CreateCommentDto dto, CancellationToken ct = default)
    {
        try 
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

            await unitOfWork.Comments.AddAsync(comment, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return Result<CommentDto>.Success(MapToDto(comment));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Gagal simpan komen buat post {PostId} dari {Email}", postId, dto.Email);
            return Result<CommentDto>.Failure("Gagal memproses penambahan komentar baru.");
        }
    }

    public async Task<Result<CommentDto>> ApproveAsync(Guid id, CancellationToken ct = default)
    {
        try 
        {
            var comment = await unitOfWork.Comments.GetByIdAsync(id, ct);
            if (comment == null)
                return Result<CommentDto>.Failure("Komentar tidak ditemukan.");

            // approve comment & update timestamp
            comment.IsApproved = true;
            comment.UpdatedAt = DateTime.UtcNow;

            await unitOfWork.Comments.UpdateAsync(comment, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return Result<CommentDto>.Success(MapToDto(comment));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error pas approve komen {CommentId}", id);
            return Result<CommentDto>.Failure("Gagal menyetujui komentar karena kendala teknis.");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        try 
        {
            var comment = await unitOfWork.Comments.GetByIdAsync(id, ct);
            if (comment == null)
                return Result<bool>.Failure("Komentar yang ingin dihapus tidak ditemukan.");

            await unitOfWork.Comments.DeleteAsync(comment, ct);
            await unitOfWork.SaveChangesAsync(ct);
            
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Gagal hapus komen {CommentId}", id);
            return Result<bool>.Failure("Gagal menghapus komentar. Silakan coba lagi.");
        }
    }

    private static CommentDto MapToDto(Comment comment) => new(
        comment.Id,
        comment.PostId,
        comment.AuthorName,
        comment.Email,
        comment.Content,
        comment.IsApproved,
        comment.CreatedAt
    );
}
