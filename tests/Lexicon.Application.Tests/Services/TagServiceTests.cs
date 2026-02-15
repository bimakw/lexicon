using FluentAssertions;
using Lexicon.Application.DTOs;
using Lexicon.Application.Services;
using Lexicon.Domain.Entities;
using Lexicon.Domain.Interfaces;
using Moq;
using Xunit;

namespace Lexicon.Application.Tests.Services;

public class TagServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITagRepository> _tagRepositoryMock;
    private readonly TagService _sut;

    public TagServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _tagRepositoryMock = new Mock<ITagRepository>();

        _unitOfWorkMock.Setup(u => u.Tags).Returns(_tagRepositoryMock.Object);

        _sut = new TagService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllTags()
    {
        // Arrange
        var tags = new List<Tag>
        {
            new() { Id = Guid.NewGuid(), Name = "Tag 1", Slug = "tag-1" },
            new() { Id = Guid.NewGuid(), Name = "Tag 2", Slug = "tag-2" }
        };

        _tagRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(tags);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.First().Name.Should().Be("Tag 1");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTag_WhenTagExists()
    {
        // Arrange
        var tagId = Guid.NewGuid();
        var tag = new Tag { Id = tagId, Name = "Test Tag", Slug = "test-tag" };

        _tagRepositoryMock.Setup(r => r.GetByIdAsync(tagId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tag);

        // Act
        var result = await _sut.GetByIdAsync(tagId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(tagId);
        result.Name.Should().Be("Test Tag");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenTagDoesNotExist()
    {
        // Arrange
        var tagId = Guid.NewGuid();

        _tagRepositoryMock.Setup(r => r.GetByIdAsync(tagId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tag?)null);

        // Act
        var result = await _sut.GetByIdAsync(tagId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetBySlugAsync_ShouldReturnTag_WhenTagExists()
    {
        // Arrange
        var slug = "test-tag";
        var tag = new Tag { Id = Guid.NewGuid(), Name = "Test Tag", Slug = slug };

        _tagRepositoryMock.Setup(r => r.GetBySlugAsync(slug, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tag);

        // Act
        var result = await _sut.GetBySlugAsync(slug);

        // Assert
        result.Should().NotBeNull();
        result!.Slug.Should().Be(slug);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateTagAndReturnDto()
    {
        // Arrange
        var dto = new CreateTagDto("New Tag");

        // Act
        var result = await _sut.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("New Tag");
        result.Slug.Should().Be("new-tag");

        _tagRepositoryMock.Verify(r => r.AddAsync(It.Is<Tag>(t => t.Name == "New Tag" && t.Slug == "new-tag"), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateTag_WhenTagExists()
    {
        // Arrange
        var tagId = Guid.NewGuid();
        var existingTag = new Tag { Id = tagId, Name = "Old Name", Slug = "old-name" };
        var dto = new UpdateTagDto("Updated Name");

        _tagRepositoryMock.Setup(r => r.GetByIdAsync(tagId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTag);

        // Act
        var result = await _sut.UpdateAsync(tagId, dto);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Updated Name");
        result.Slug.Should().Be("updated-name");

        _tagRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Tag>(t => t.Name == "Updated Name"), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenTagDoesNotExist()
    {
        // Arrange
        var tagId = Guid.NewGuid();
        var dto = new UpdateTagDto("Updated Name");

        _tagRepositoryMock.Setup(r => r.GetByIdAsync(tagId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tag?)null);

        // Act
        var result = await _sut.UpdateAsync(tagId, dto);

        // Assert
        result.Should().BeNull();
        _tagRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Tag>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenTagExists()
    {
        // Arrange
        var tagId = Guid.NewGuid();
        var existingTag = new Tag { Id = tagId, Name = "Tag To Delete" };

        _tagRepositoryMock.Setup(r => r.GetByIdAsync(tagId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTag);

        // Act
        var result = await _sut.DeleteAsync(tagId);

        // Assert
        result.Should().BeTrue();
        _tagRepositoryMock.Verify(r => r.DeleteAsync(existingTag, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenTagDoesNotExist()
    {
        // Arrange
        var tagId = Guid.NewGuid();

        _tagRepositoryMock.Setup(r => r.GetByIdAsync(tagId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tag?)null);

        // Act
        var result = await _sut.DeleteAsync(tagId);

        // Assert
        result.Should().BeFalse();
        _tagRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Tag>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
