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
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<ITagRepository> _repoMock = new();
    private readonly TagService _service;

    public TagServiceTests()
    {
        _uowMock.Setup(u => u.Tags).Returns(_repoMock.Object);
        _service = new TagService(_uowMock.Object);
    }

    [Fact]
    public async Task GetById_Ok()
    {
        var id = Guid.NewGuid();
        var tag = new Tag { Id = id, Name = "Microservices" };

        _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tag);

        var result = await _service.GetByIdAsync(id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Microservices");
    }

    [Fact]
    public async Task CreateTag_Success()
    {
        var dto = new CreateTagDto("Clean Code");

        _repoMock.Setup(r => r.GetBySlugAsync("clean-code", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tag?)null);
            
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _service.CreateAsync(dto);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Clean Code");
    }

    [Fact]
    public async Task CreateTag_Gagal_Duplikat()
    {
        var dto = new CreateTagDto("Backend");
        var existing = new Tag { Name = "Backend", Slug = "backend" };

        _repoMock.Setup(r => r.GetBySlugAsync("backend", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var result = await _service.CreateAsync(dto);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Nama/Slug tag udah dipake");
    }
}
