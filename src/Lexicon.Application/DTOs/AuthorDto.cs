namespace Lexicon.Application.DTOs;

public record AuthorDto(
    Guid Id,
    string Name,
    string Email,
    string? Bio,
    string? AvatarUrl,
    int PostCount,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreateAuthorDto(
    string Name,
    string Email,
    string? Bio,
    string? AvatarUrl
);

public record UpdateAuthorDto(
    string Name,
    string Email,
    string? Bio,
    string? AvatarUrl
);
