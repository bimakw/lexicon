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
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<IAuthorRepository> _repoMock = new();
    private readonly AuthorService _service;

    public AuthorServiceTests()
    {
        _uowMock.Setup(u => u.Authors).Returns(_repoMock.Object);
        _service = new AuthorService(_uowMock.Object);
    }

    [Fact]
    public async Task GetById_Ok()
    {
        var id = Guid.NewGuid();
        var author = new Author { Id = id, Name = "Rizky Ramadhan", Email = "rizky.rmd@gmail.com" };

        _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        var result = await _service.GetByIdAsync(id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Rizky Ramadhan");
    }

    [Fact]
    public async Task GetById_Notfound()
    {
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Author?)null);

        var result = await _service.GetByIdAsync(id);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Author gak ketemu");
    }

    [Fact]
    public async Task CreateAuthor_Success()
    {
        var dto = new CreateAuthorDto("Andi Wijaya", "andi.wijaya@outlook.com", "Senior Backend Engineer", null);

        _repoMock.Setup(r => r.GetByEmailAsync(dto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Author?)null);
            
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _service.CreateAsync(dto);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Andi Wijaya");
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact] 
    public async Task CreateAuthor_Gagal_EmailDuplikat()
    {
        var dto = new CreateAuthorDto("Budi Santoso", "budi.s@gmail.com", null, null);
        var existing = new Author { Email = dto.Email };

        _repoMock.Setup(r => r.GetByEmailAsync(dto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var result = await _service.CreateAsync(dto);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Email udah dipake orang lain");
    }
}



