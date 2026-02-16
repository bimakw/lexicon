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
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICategoryRepository> _categoryRepoMock;
    private readonly CategoryService _categoryService;

    public CategoryServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _categoryRepoMock = new Mock<ICategoryRepository>();

        _unitOfWorkMock.Setup(u => u.Categories).Returns(_categoryRepoMock.Object);
        _categoryService = new CategoryService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllCategories()
    {
        var categories = new List<Category> { CreateCategory("Tech"), CreateCategory("Life") };
        _categoryRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        var result = await _categoryService.GetAllAsync();

        result.Should().HaveCount(2);
        Assert.Equal("Tech", result.First().Name);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCategory_WhenFound()
    {
        var category = CreateCategory("Tech");
        _categoryRepoMock.Setup(r => r.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var result = await _categoryService.GetByIdAsync(category.Id);

        Assert.NotNull(result);
        result.Id.Should().Be(category.Id);
    }

    [Fact]
    public async Task GetBySlugAsync_ShouldReturnCategory_WhenFound()
    {
        var category = CreateCategory("Tech");
        _categoryRepoMock.Setup(r => r.GetBySlugAsync(category.Slug, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var result = await _categoryService.GetBySlugAsync(category.Slug);

        Assert.NotNull(result);
        Assert.Equal(category.Slug, result.Slug);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateCategory()
    {
        var dto = new CreateCategoryDto("New Tech", "Description", null);

        var result = await _categoryService.CreateAsync(dto);

        result.Name.Should().Be("New Tech");
        _categoryRepoMock.Verify(r => r.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateCategory_WhenFound()
    {
        var category = CreateCategory("Old");
        var dto = new UpdateCategoryDto("New Name", "New Desc", null);

        _categoryRepoMock.Setup(r => r.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var result = await _categoryService.UpdateAsync(category.Id, dto);

        Assert.Equal("New Name", result!.Name);

        _categoryRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenFound()
    {
        var category = CreateCategory("To Delete");
        _categoryRepoMock.Setup(r => r.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var result = await _categoryService.DeleteAsync(category.Id);

        Assert.True(result);
        _categoryRepoMock.Verify(r => r.DeleteAsync(category, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetTreeAsync_ShouldReturnHierarchicalStructure()
    {
        var root = CreateCategory("Root");
        var child = CreateCategory("Child");
        child.ParentId = root.Id;

        _categoryRepoMock.Setup(r => r.GetRootCategoriesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { root });

        _categoryRepoMock.Setup(r => r.GetChildrenAsync(root.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { child });

        _categoryRepoMock.Setup(r => r.GetChildrenAsync(child.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<Category>());

        var result = await _categoryService.GetTreeAsync();

        result.Should().HaveCount(1);
        Assert.Equal("Child", result.First().Children.First().Name);
    }

    private static Category CreateCategory(string name)
    {
        return new Category
        {
            Id = Guid.NewGuid(),
            Name = name,
            Slug = name.ToLower(),
            Description = "Desc",
            CreatedAt = DateTime.UtcNow
        };
    }
}
