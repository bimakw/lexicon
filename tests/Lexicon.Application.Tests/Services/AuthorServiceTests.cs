using AutoMapper;
using FluentAssertions;
using Lexicon.Application.DTOs;
using Lexicon.Application.Services;
using Lexicon.Domain.Common;
using Lexicon.Domain.Entities;
using Lexicon.Domain.Interfaces;
using Moq;
using Xunit;

namespace Lexicon.Application.Tests.Services;

public class AuthorServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IAuthorRepository> _authorRepoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly AuthorService _service;

    public AuthorServiceTests()
    {
        _unitOfWorkMock.Setup(u => u.Authors).Returns(_authorRepoMock.Object);
        _service = new AuthorService(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Should_ReturnAuthor_When_IdIsValid()
    {
        var authorId = Guid.NewGuid();
        var author = new Author { Id = authorId, Name = "Ahmad" };
        var dto = new AuthorDto(authorId, "Ahmad", "ahmad@lexicon.com", "Content Creator", null, 0, DateTime.UtcNow);

        _authorRepoMock.Setup(r => r.GetByIdAsync(authorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);
        _mapperMock.Setup(m => m.Map<AuthorDto>(author)).Returns(dto);

        var result = await _service.GetByIdAsync(authorId);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Ahmad");
    }

    [Fact]
    public async Task Should_Fail_When_AuthorDoesNotExist()
    {
        var authorId = Guid.NewGuid();
        _authorRepoMock.Setup(r => r.GetByIdAsync(authorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Author?)null);

        var result = await _service.GetByIdAsync(authorId);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Author not found");
    }

    [Fact]
    public async Task Should_SuccessfullyRegister_When_AuthorDataComplete()
    {
        var dto = new CreateAuthorDto("Indra S", "indra@lexicon.com", "Principal Engineer", null);
        var authorDto = new AuthorDto(Guid.NewGuid(), "Indra S", "indra@lexicon.com", "Principal Engineer", null, 0, DateTime.UtcNow);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map<AuthorDto>(It.IsAny<Author>())).Returns(authorDto);

        var result = await _service.CreateAsync(dto);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Indra S");
        _authorRepoMock.Verify(r => r.AddAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}


