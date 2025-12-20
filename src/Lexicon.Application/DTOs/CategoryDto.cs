namespace Lexicon.Application.DTOs;

public record CategoryDto(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    Guid? ParentId,
    string? ParentName,
    int PostCount,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CategoryTreeDto(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    IEnumerable<CategoryTreeDto> Children
);

public record CreateCategoryDto(
    string Name,
    string? Description,
    Guid? ParentId
);

public record UpdateCategoryDto(
    string Name,
    string? Description,
    Guid? ParentId
);
