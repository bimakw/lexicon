namespace Lexicon.Application.DTOs;

public record MediaDto(
    Guid Id,
    string FileName,
    string FilePath,
    string ContentType,
    long Size,
    string? AltText,
    DateTime CreatedAt
);

public record UploadMediaDto(
    string FileName,
    string ContentType,
    long Size,
    string? AltText
);
