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
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<ITagRepository> _tagRepo = new();
    private readonly TagService _tagService;

    public TagServiceTests()
    {
        _uow.Setup(u => u.Tags).Returns(_tagRepo.Object);
        _tagService = new TagService(_uow.Object);
    }

    [Fact]
    public async Task Should_ReturnAllTags_When_Called()
    {
        _tagRepo.Setup(r => r.GetAllAsync(default)).ReturnsAsync(new List<Tag>
        {
            new() { Id = Guid.NewGuid(), Name = "Berita Politik", Slug = "berita-politik" },
            new() { Id = Guid.NewGuid(), Name = "Review Hp", Slug = "review-hp" }
        });

        var hasil = await _tagService.GetAllAsync();
        hasil.Should().HaveCount(2);
        hasil.First().Name.Should().Be("Berita Politik");
    }

    [Fact]
    public async Task Should_ReturnTag_When_Found()
    {
        var tagId = Guid.NewGuid();
        var tag = new Tag { Id = tagId, Name = "Tutorial Masak", Slug = "tutorial-masak" };
        _tagRepo.Setup(r => r.GetByIdAsync(tagId, default)).ReturnsAsync(tag);

        var hasil = await _tagService.GetByIdAsync(tagId);

        hasil.Should().NotBeNull();
        hasil!.Name.Should().Be("Tutorial Masak");
    }

    [Fact]
    public async Task Should_ReturnNull_When_NotFound()
    {
        var randomId = Guid.NewGuid();
        _tagRepo.Setup(r => r.GetByIdAsync(randomId, default)).ReturnsAsync((Tag?)null);

        var hasil = await _tagService.GetByIdAsync(randomId);
        hasil.Should().BeNull();
    }

    [Fact]
    public async Task Should_ReturnTag_When_FoundBySlug()
    {
        var tag = new Tag { Id = Guid.NewGuid(), Name = "Tips Freelance", Slug = "tips-freelance" };
        _tagRepo.Setup(r => r.GetBySlugAsync("tips-freelance", default)).ReturnsAsync(tag);

        var hasil = await _tagService.GetBySlugAsync("tips-freelance");
        hasil.Should().NotBeNull();
        hasil!.Slug.Should().Be("tips-freelance");
    }

    [Fact]
    public async Task Should_CreateTag_When_ValidDto()
    {
        var input = new CreateTagDto("Lowongan Kerja");
        var hasil = await _tagService.CreateAsync(input);

        hasil.Name.Should().Be("Lowongan Kerja");

        _tagRepo.Verify(r => r.AddAsync(
            It.Is<Tag>(t => t.Name == "Lowongan Kerja"),
            It.IsAny<CancellationToken>()), Times.Once);

        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_UpdateTag_When_Found()
    {
        var existing = new Tag { Id = Guid.NewGuid(), Name = "Nama Lama", Slug = "nama-lama" };
        _tagRepo.Setup(r => r.GetByIdAsync(existing.Id, default)).ReturnsAsync(existing);

        var hasil = await _tagService.UpdateAsync(existing.Id, new UpdateTagDto("Nama Baru"));

        hasil.Should().NotBeNull();
        hasil!.Name.Should().Be("Nama Baru");
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task Should_DeleteTag_When_Found()
    {
        var tag = new Tag { Id = Guid.NewGuid(), Name = "Hapus Ini", Slug = "hapus-ini" };
        _tagRepo.Setup(r => r.GetByIdAsync(tag.Id, default)).ReturnsAsync(tag);

        var berhasil = await _tagService.DeleteAsync(tag.Id);

        berhasil.Should().BeTrue();
        _tagRepo.Verify(r => r.DeleteAsync(tag, It.IsAny<CancellationToken>()), Times.Once);
    }
}
