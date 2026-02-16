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
    private readonly TagService _tagService;

    public TagServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _tagRepositoryMock = new Mock<ITagRepository>();

        _unitOfWorkMock.Setup(u => u.Tags).Returns(_tagRepositoryMock.Object);
        _tagService = new TagService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllTags()
    {
        var tags = new List<Tag> { CreateTag("Tag 1"), CreateTag("Tag 2") };
        _tagRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(tags);

        var result = await _tagService.GetAllAsync();

        result.Should().HaveCount(2);
        Assert.Equal("Tag 1", result.First().Name);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTag_WhenTagExists()
    {
        var tag = CreateTag("Test Tag");
        _tagRepositoryMock.Setup(r => r.GetByIdAsync(tag.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tag);

        var result = await _tagService.GetByIdAsync(tag.Id);

        result.Should().NotBeNull();
        Assert.Equal(tag.Id, result!.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenTagDoesNotExist()
    {
        var tagId = Guid.NewGuid();
        _tagRepositoryMock.Setup(r => r.GetByIdAsync(tagId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tag?)null);

        var result = await _tagService.GetByIdAsync(tagId);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetBySlugAsync_ShouldReturnTag_WhenTagExists()
    {
        var tag = CreateTag("Test Tag");
        _tagRepositoryMock.Setup(r => r.GetBySlugAsync(tag.Slug, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tag);

        var result = await _tagService.GetBySlugAsync(tag.Slug);

        result.Should().NotBeNull();
        result!.Slug.Should().Be(tag.Slug);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateTagAndReturnDto()
    {
        var dto = new CreateTagDto("New Tag");

        var result = await _tagService.CreateAsync(dto);

        result.Should().NotBeNull();
        Assert.Equal("New Tag", result.Name);

        _tagRepositoryMock.Verify(r => r.AddAsync(It.Is<Tag>(t => t.Name == "New Tag"), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateTag_WhenTagExists()
    {
        var tag = CreateTag("Old Name");
        var dto = new UpdateTagDto("Updated Name");

        _tagRepositoryMock.Setup(r => r.GetByIdAsync(tag.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tag);

        var result = await _tagService.UpdateAsync(tag.Id, dto);

        result.Should().NotBeNull();
        Assert.Equal("Updated Name", result!.Name);

        _tagRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Tag>(t => t.Name == "Updated Name"), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenTagExists()
    {
        var tag = CreateTag("Delete Me");
        _tagRepositoryMock.Setup(r => r.GetByIdAsync(tag.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tag);

        var result = await _tagService.DeleteAsync(tag.Id);

        Assert.True(result);
        _tagRepositoryMock.Verify(r => r.DeleteAsync(tag, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenTagDoesNotExist()
    {
        var tagId = Guid.NewGuid();
        _tagRepositoryMock.Setup(r => r.GetByIdAsync(tagId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tag?)null);

        var result = await _tagService.DeleteAsync(tagId);

        Assert.False(result);
        _tagRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Tag>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static Tag CreateTag(string name)
    {
        return new Tag
        {
            Id = Guid.NewGuid(),
            Name = name,
            Slug = name.ToLower().Replace(" ", "-")
        };
    }
}
