namespace Lexicon.Application.DTOs;

public record CommentDto(
    Guid Id,
    Guid PostId,
    string AuthorName,
    string Email,
    string Content,
    bool IsApproved,
    DateTime CreatedAt
);

public record CreateCommentDto(
    string AuthorName,
    string Email,
    string Content
);
