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

public class CategoryServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ICategoryRepository> _categoryRepoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
        _unitOfWorkMock.Setup(u => u.Categories).Returns(_categoryRepoMock.Object);
        _service = new CategoryService(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Should_ReturnCategory_When_IdentifierExists()
    {
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Teknologi" };
        var dto = new CategoryDto(categoryId, "Teknologi", "teknologi", "Seputar dunia teknologi", null, null, 0, DateTime.UtcNow, DateTime.UtcNow);

        _categoryRepoMock.Setup(r => r.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        _mapperMock.Setup(m => m.Map<CategoryDto>(category)).Returns(dto);

        var result = await _service.GetByIdAsync(categoryId);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Teknologi");
    }

    [Fact]
    public async Task Should_Fail_When_CategoryNotFound()
    {
        var categoryId = Guid.NewGuid();
        _categoryRepoMock.Setup(r => r.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var result = await _service.GetByIdAsync(categoryId);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Kategori tidak ditemukan.");
    }

    [Fact]
    public async Task Should_SuccessfullyCreate_When_DataValid()
    {
        var dto = new CreateCategoryDto("Programming", "Tutorial pemrograman mobile dan web", null);
        var categoryDto = new CategoryDto(Guid.NewGuid(), "Programming", "programming", "Tutorial pemrograman mobile dan web", null, null, 0, DateTime.UtcNow, DateTime.UtcNow);

        _categoryRepoMock.Setup(r => r.GetBySlugAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);
        _mapperMock.Setup(m => m.Map<CategoryDto>(It.IsAny<Category>())).Returns(categoryDto);

        var result = await _service.CreateAsync(dto);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Programming");
        _categoryRepoMock.Verify(r => r.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}


