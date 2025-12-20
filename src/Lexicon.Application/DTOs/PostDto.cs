using Lexicon.Domain.Common;

namespace Lexicon.Application.DTOs;

public record PostDto(
    Guid Id,
    string Title,
    string Slug,
    string Content,
    string? Excerpt,
    string? FeaturedImage,
    string Status,
    DateTime? PublishedAt,
    Guid AuthorId,
    string? AuthorName,
    Guid? CategoryId,
    string? CategoryName,
    IEnumerable<TagDto> Tags,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record PostListDto(
    Guid Id,
    string Title,
    string Slug,
    string? Excerpt,
    string? FeaturedImage,
    string Status,
    DateTime? PublishedAt,
    string? AuthorName,
    string? CategoryName,
    DateTime CreatedAt
);

public record CreatePostDto(
    string Title,
    string Content,
    string? Excerpt,
    string? FeaturedImage,
    Guid AuthorId,
    Guid? CategoryId,
    IEnumerable<Guid>? TagIds
);

public record UpdatePostDto(
    string Title,
    string Content,
    string? Excerpt,
    string? FeaturedImage,
    Guid? CategoryId,
    IEnumerable<Guid>? TagIds
);

public record PagedResult<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);
