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
    public async Task GetAllAsync_ShouldReturnAllAuthors()
    {
        var authors = new List<Author> { CreateAuthor("Author 1"), CreateAuthor("Author 2") };
        _authorRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(authors);

        var result = await _authorService.GetAllAsync();

        result.Should().HaveCount(2);
        Assert.Equal("Author 1", result.First().Name);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnAuthor_WhenFound()
    {
        var author = CreateAuthor("Test Author");
        _authorRepoMock.Setup(r => r.GetByIdAsync(author.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        var result = await _authorService.GetByIdAsync(author.Id);

        Assert.NotNull(result);
        result.Email.Should().Be(author.Email);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateAuthor()
    {
        var dto = new CreateAuthorDto("New Author", "new@test.com", "Bio", "https://avatar.url");

        var result = await _authorService.CreateAsync(dto);

        Assert.Equal("New Author", result.Name);
        
        _authorRepoMock.Verify(r => r.AddAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateAuthor_WhenFound()
    {
        var author = CreateAuthor("Old Name");
        var dto = new UpdateAuthorDto("Updated Name", "updated@test.com", "New Bio", "https://new.url");

        _authorRepoMock.Setup(r => r.GetByIdAsync(author.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        var result = await _authorService.UpdateAsync(author.Id, dto);

        Assert.Equal("Updated Name", result!.Name);

        _authorRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenFound()
    {
        var author = CreateAuthor("To Delete");
        _authorRepoMock.Setup(r => r.GetByIdAsync(author.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        var result = await _authorService.DeleteAsync(author.Id);

        Assert.True(result);
        _authorRepoMock.Verify(r => r.DeleteAsync(author, It.IsAny<CancellationToken>()), Times.Once);
    }

    private static Author CreateAuthor(string name)
    {
        return new Author
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = $"{name.Replace(" ", "").ToLower()}@test.com",
            CreatedAt = DateTime.UtcNow
        };
    }
}
