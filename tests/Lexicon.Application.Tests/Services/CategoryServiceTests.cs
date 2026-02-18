using AutoMapper;
using FluentAssertions;
using Lexicon.Application.DTOs;
using Lexicon.Application.Services;
using Lexicon.Domain.Entities;
using Lexicon.Domain.Interfaces;
using Moq;
using Xunit;

namespace Lexicon.Application.Tests.Services;

public class CategoryServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<ICategoryRepository> _repoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
        _uowMock.Setup(u => u.Categories).Returns(_repoMock.Object);
        _service = new CategoryService(_uowMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetById_Ok()
    {
        var id = Guid.NewGuid();
        var category = new Category { Id = id, Name = "Data Science" };
        var dto = new CategoryDto(id, "Data Science", "data-science", "Seputar AI dan mesin learning", null, null, 0, DateTime.UtcNow, DateTime.UtcNow);

        _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        _mapperMock.Setup(m => m.Map<CategoryDto>(category)).Returns(dto);

        var result = await _service.GetByIdAsync(id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Data Science");
    }

    [Fact]
    public async Task GetById_NotFound()
    {
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var result = await _service.GetByIdAsync(id);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Kategori gak ketemu");
    }

    [Fact]
    public async Task CreateCategory_Success()
    {
        var dto = new CreateCategoryDto("Frontend Architecture", "Best practice bikin UI yang clean", null);
        var categoryDto = new CategoryDto(Guid.NewGuid(), "Frontend Architecture", "frontend-architecture", "Best practice bikin UI yang clean", null, null, 0, DateTime.UtcNow, DateTime.UtcNow);

        _repoMock.Setup(r => r.GetBySlugAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);
        _mapperMock.Setup(m => m.Map<CategoryDto>(It.IsAny<Category>())).Returns(categoryDto);

        var result = await _service.CreateAsync(dto);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Frontend Architecture");
    }
}



