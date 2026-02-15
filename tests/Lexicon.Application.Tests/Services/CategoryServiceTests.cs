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
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly CategoryService _sut;

    public CategoryServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();

        _unitOfWorkMock.Setup(u => u.Categories).Returns(_categoryRepositoryMock.Object);

        _sut = new CategoryService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllCategories()
    {
        // Arrange
        var categories = new List<Category>
        {
            new() { Id = Guid.NewGuid(), Name = "Tech", Slug = "tech" },
            new() { Id = Guid.NewGuid(), Name = "Life", Slug = "life" }
        };

        _categoryRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCategory_WhenFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var category = new Category { Id = id, Name = "Tech", Slug = "tech" };

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        // Act
        var result = await _sut.GetByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
    }

    [Fact]
    public async Task GetBySlugAsync_ShouldReturnCategory_WhenFound()
    {
        // Arrange
        var slug = "tech";
        var category = new Category { Id = Guid.NewGuid(), Name = "Tech", Slug = slug };

        _categoryRepositoryMock.Setup(r => r.GetBySlugAsync(slug, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        // Act
        var result = await _sut.GetBySlugAsync(slug);

        // Assert
        result.Should().NotBeNull();
        result!.Slug.Should().Be(slug);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateCategory()
    {
        // Arrange
        var dto = new CreateCategoryDto("New Tech", "Description", null);

        // Act
        var result = await _sut.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("New Tech");
        result.Slug.Should().Be("new-tech");

        _categoryRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateCategory_WhenFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var category = new Category { Id = id, Name = "Old", Slug = "old" };
        var dto = new UpdateCategoryDto("New Name", "New Desc", null);

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        // Act
        var result = await _sut.UpdateAsync(id, dto);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("New Name");
        result.Slug.Should().Be("new-name");

        _categoryRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var category = new Category { Id = id, Name = "To Delete" };

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        // Act
        var result = await _sut.DeleteAsync(id);

        // Assert
        result.Should().BeTrue();
        _categoryRepositoryMock.Verify(r => r.DeleteAsync(category, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetTreeAsync_ShouldReturnHierarchicalStructure()
    {
        // Arrange
        var rootId = Guid.NewGuid();
        var childId = Guid.NewGuid();

        var rootCategory = new Category { Id = rootId, Name = "Root", Slug = "root", ParentId = null };
        var childCategory = new Category { Id = childId, Name = "Child", Slug = "child", ParentId = rootId };

        _categoryRepositoryMock.Setup(r => r.GetRootCategoriesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { rootCategory });

        _categoryRepositoryMock.Setup(r => r.GetChildrenAsync(rootId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { childCategory });

        _categoryRepositoryMock.Setup(r => r.GetChildrenAsync(childId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<Category>());

        // Act
        var result = await _sut.GetTreeAsync();

        // Assert
        result.Should().HaveCount(1);
        var root = result.First();
        root.Name.Should().Be("Root");
        root.Children.Should().HaveCount(1);
        root.Children.First().Name.Should().Be("Child");
    }
}
