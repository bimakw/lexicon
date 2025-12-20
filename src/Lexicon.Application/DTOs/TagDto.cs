namespace Lexicon.Application.DTOs;

public record TagDto(
    Guid Id,
    string Name,
    string Slug,
    int PostCount,
    DateTime CreatedAt
);

public record CreateTagDto(string Name);

public record UpdateTagDto(string Name);
