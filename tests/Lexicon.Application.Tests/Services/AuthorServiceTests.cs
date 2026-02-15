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
    private readonly Mock<IAuthorRepository> _authorRepositoryMock;
    private readonly AuthorService _sut;

    public AuthorServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _authorRepositoryMock = new Mock<IAuthorRepository>();

        _unitOfWorkMock.Setup(u => u.Authors).Returns(_authorRepositoryMock.Object);

        _sut = new AuthorService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllAuthors()
    {
        // Arrange
        var authors = new List<Author>
        {
            new() { Id = Guid.NewGuid(), Name = "Author 1", Email = "a1@test.com" },
            new() { Id = Guid.NewGuid(), Name = "Author 2", Email = "a2@test.com" }
        };

        _authorRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(authors);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.First().Name.Should().Be("Author 1");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnAuthor_WhenFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var author = new Author { Id = id, Name = "Test Author", Email = "test@test.com" };

        _authorRepositoryMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        // Act
        var result = await _sut.GetByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
        result.Email.Should().Be("test@test.com");
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateAuthor()
    {
        // Arrange
        var dto = new CreateAuthorDto("New Author", "new@test.com", "Bio", "https://avatar.url");

        // Act
        var result = await _sut.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("New Author");
        result.Email.Should().Be("new@test.com");

        _authorRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateAuthor_WhenFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var author = new Author { Id = id, Name = "Old Name", Email = "old@test.com" };
        var dto = new UpdateAuthorDto("Updated Name", "updated@test.com", "New Bio", "https://new.url");

        _authorRepositoryMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        // Act
        var result = await _sut.UpdateAsync(id, dto);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Updated Name");
        result.Email.Should().Be("updated@test.com");

        _authorRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var author = new Author { Id = id, Name = "To Delete" };

        _authorRepositoryMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        // Act
        var result = await _sut.DeleteAsync(id);

        // Assert
        result.Should().BeTrue();
        _authorRepositoryMock.Verify(r => r.DeleteAsync(author, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();

        _authorRepositoryMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Author?)null);

        // Act
        var result = await _sut.DeleteAsync(id);

        // Assert
        result.Should().BeFalse();
        _authorRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
