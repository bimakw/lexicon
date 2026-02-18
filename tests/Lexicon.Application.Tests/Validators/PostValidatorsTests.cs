using FluentAssertions;
using Lexicon.Application.DTOs;
using Lexicon.Application.Validators;
using Xunit;

namespace Lexicon.Application.Tests.Validators;

public class PostValidatorsTests
{
    private readonly CreatePostDtoValidator _createValidator = new();
    private readonly UpdatePostDtoValidator _updateValidator = new();

    [Fact]
    public void CreatePostDto_ShouldPass_WithValidData()
    {
        // Arrange
        var dto = new CreatePostDto(
            Title: "Valid Title",
            Content: "Valid Content",
            Excerpt: null,
            FeaturedImage: null,
            AuthorId: Guid.NewGuid(),
            CategoryId: Guid.NewGuid(),
            TagIds: null
        );

        // Act
        var result = _createValidator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void CreatePostDto_ShouldFail_WhenTitleIsEmpty()
    {
        // Arrange
        var dto = new CreatePostDto(
            Title: "",
            Content: "Valid Content",
            Excerpt: null,
            FeaturedImage: null,
            AuthorId: Guid.NewGuid(),
            CategoryId: Guid.NewGuid(),
            TagIds: null
        );

        // Act
        var result = _createValidator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Fact]
    public void CreatePostDto_ShouldFail_WhenAuthorIdIsEmpty()
    {
        // Arrange
        var dto = new CreatePostDto(
            Title: "Valid Title",
            Content: "Valid Content",
            Excerpt: null,
            FeaturedImage: null,
            AuthorId: Guid.Empty,
            CategoryId: Guid.NewGuid(),
            TagIds: null
        );

        // Act
        var result = _createValidator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "AuthorId");
    }
}
