using FluentAssertions;
using Lexicon.Application.DTOs;
using Lexicon.Application.Services;
using Lexicon.Domain.Entities;
using Lexicon.Domain.Interfaces;
using Moq;
using Xunit;

namespace Lexicon.Application.Tests.Services;

public class AuthorServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IAuthorRepository> _authorRepoMock;
    private readonly AuthorService _authorService;

    public AuthorServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _authorRepoMock = new Mock<IAuthorRepository>();

        _unitOfWorkMock.Setup(u => u.Authors).Returns(_authorRepoMock.Object);
        _authorService = new AuthorService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task When_GetAll_Should_ReturnDaftarPenulis()
    {
        var authors = new List<Author> { CreateAuthor("Eko Kurniawan"), CreateAuthor("Sandhika Galih") };
        _authorRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(authors);

        var result = await _authorService.GetAllAsync();

        result.Should().HaveCount(2);
        result.First().Name.Should().Be("Eko Kurniawan");
    }

    [Fact]
    public async Task When_GetById_Found_Should_ReturnDataPenulis()
    {
        var author = CreateAuthor("Budi Santoso");
        _authorRepoMock.Setup(r => r.GetByIdAsync(author.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        var result = await _authorService.GetByIdAsync(author.Id);

        result.Should().NotBeNull();
        result!.Email.Should().Be("budi.santoso@contoh.com");
    }

    [Fact]
    public async Task When_Create_WithValidData_Should_SimpanDanReturnDto()
    {
        var dto = new CreateAuthorDto("Rani Maharani", "rani@contoh.com", "Content Creator", "https://foto.url");

        var result = await _authorService.CreateAsync(dto);

        result.Should().NotBeNull();
        result.Name.Should().Be("Rani Maharani");

        _authorRepoMock.Verify(r => r.AddAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task When_Update_Found_Should_UbahDataDanReturnDto()
    {
        var author = CreateAuthor("Nama Asli");
        var dto = new UpdateAuthorDto("Nama Panggung", "panggung@contoh.com", "Artis", "https://baru.url");

        _authorRepoMock.Setup(r => r.GetByIdAsync(author.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        var result = await _authorService.UpdateAsync(author.Id, dto);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Nama Panggung");

        _authorRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task When_Delete_Found_Should_HapusDataDanReturnTrue()
    {
        var author = CreateAuthor("Penulis Tidak Aktif");
        _authorRepoMock.Setup(r => r.GetByIdAsync(author.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        var result = await _authorService.DeleteAsync(author.Id);

        result.Should().BeTrue();
        _authorRepoMock.Verify(r => r.DeleteAsync(author, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task When_Delete_NotFound_Should_ReturnFalse()
    {
        var id = Guid.NewGuid();
        _authorRepoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Author?)null);

        var result = await _authorService.DeleteAsync(id);

        result.Should().BeFalse();
        _authorRepoMock.Verify(r => r.DeleteAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static Author CreateAuthor(string name)
    {
        return new Author
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = $"{name.ToLower().Replace(" ", ".")}@contoh.com",
            CreatedAt = DateTime.UtcNow
        };
    }
}
