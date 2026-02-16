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
    private readonly Mock<ITagRepository> _tagRepoMock;
    private readonly TagService _tagService;

    public TagServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _tagRepoMock = new Mock<ITagRepository>();

        _unitOfWorkMock.Setup(u => u.Tags).Returns(_tagRepoMock.Object);
        _tagService = new TagService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task When_GetAll_Should_ReturnDaftarTagLengkap()
    {
        var tags = new List<Tag> { CreateTag("Berita Teknologi"), CreateTag("Info Kesehatan") };
        _tagRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(tags);

        var result = await _tagService.GetAllAsync();

        result.Should().HaveCount(2);
        result.First().Name.Should().Be("Berita Teknologi");
    }

    [Fact]
    public async Task When_GetById_Found_Should_ReturnDataTag()
    {
        var tag = CreateTag("Tutorial Masak");
        _tagRepoMock.Setup(r => r.GetByIdAsync(tag.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tag);

        var result = await _tagService.GetByIdAsync(tag.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(tag.Id);
        result.Name.Should().Be("Tutorial Masak");
    }

    [Fact]
    public async Task When_GetById_NotFound_Should_ReturnNull()
    {
        var tagId = Guid.NewGuid();
        _tagRepoMock.Setup(r => r.GetByIdAsync(tagId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tag?)null);

        var result = await _tagService.GetByIdAsync(tagId);

        result.Should().BeNull();
    }

    [Fact]
    public async Task When_GetBySlug_Found_Should_ReturnDataTag()
    {
        var tag = CreateTag("Tips Keuangan");
        _tagRepoMock.Setup(r => r.GetBySlugAsync(tag.Slug, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tag);

        var result = await _tagService.GetBySlugAsync(tag.Slug);

        result.Should().NotBeNull();
        result!.Slug.Should().Be("tips-keuangan");
    }

    [Fact]
    public async Task When_Create_WithValidData_Should_SimpanDanReturnDto()
    {
        var dto = new CreateTagDto("Loker Jakarta");

        var result = await _tagService.CreateAsync(dto);

        result.Should().NotBeNull();
        result.Name.Should().Be("Loker Jakarta");
        result.Slug.Should().Be("loker-jakarta");

        _tagRepoMock.Verify(r => r.AddAsync(It.Is<Tag>(t => t.Name == "Loker Jakarta"), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task When_Update_Found_Should_UbahDataDanReturnDto()
    {
        var tag = CreateTag("Nama Lama");
        var dto = new UpdateTagDto("Nama Baru");

        _tagRepoMock.Setup(r => r.GetByIdAsync(tag.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tag);

        var result = await _tagService.UpdateAsync(tag.Id, dto);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Nama Baru");

        _tagRepoMock.Verify(r => r.UpdateAsync(It.Is<Tag>(t => t.Name == "Nama Baru"), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task When_Delete_Found_Should_HapusDataDanReturnTrue()
    {
        var tag = CreateTag("Tag Sampah");
        _tagRepoMock.Setup(r => r.GetByIdAsync(tag.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tag);

        var result = await _tagService.DeleteAsync(tag.Id);

        result.Should().BeTrue();
        _tagRepoMock.Verify(r => r.DeleteAsync(tag, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task When_Delete_NotFound_Should_ReturnFalse()
    {
        var tagId = Guid.NewGuid();
        _tagRepoMock.Setup(r => r.GetByIdAsync(tagId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tag?)null);

        var result = await _tagService.DeleteAsync(tagId);

        result.Should().BeFalse();
        _tagRepoMock.Verify(r => r.DeleteAsync(It.IsAny<Tag>(), It.IsAny<CancellationToken>()), Times.Never);
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
