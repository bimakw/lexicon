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
    public async Task When_GetAll_Should_ReturnSemuaKategori()
    {
        var categories = new List<Category> { CreateCategory("Elektronik"), CreateCategory("Peralatan Rumah") };
        _categoryRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        var result = await _categoryService.GetAllAsync();

        result.Should().HaveCount(2);
        result.First().Name.Should().Be("Elektronik");
    }

    [Fact]
    public async Task When_GetById_Found_Should_ReturnDataKategori()
    {
        var category = CreateCategory("Laptop Gaming");
        _categoryRepoMock.Setup(r => r.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var result = await _categoryService.GetByIdAsync(category.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(category.Id);
    }

    [Fact]
    public async Task When_GetBySlug_Found_Should_ReturnDataCategory()
    {
        var category = CreateCategory("Smartphone Terbaru");
        _categoryRepoMock.Setup(r => r.GetBySlugAsync(category.Slug, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var result = await _categoryService.GetBySlugAsync(category.Slug);

        result.Should().NotBeNull();
        result!.Slug.Should().Be("smartphone-terbaru");
    }

    [Fact]
    public async Task When_Create_WithValidData_Should_SimpanDanReturnDto()
    {
        var dto = new CreateCategoryDto("Kamera Digital", "Untuk fotografi", null);

        var result = await _categoryService.CreateAsync(dto);

        result.Should().NotBeNull();
        result.Name.Should().Be("Kamera Digital");

        _categoryRepoMock.Verify(r => r.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task When_Update_Found_Should_UbahDataDanReturnDto()
    {
        var category = CreateCategory("Nama Lama");
        var dto = new UpdateCategoryDto("Nama Baru", "Deskripsi Baru", null);

        _categoryRepoMock.Setup(r => r.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var result = await _categoryService.UpdateAsync(category.Id, dto);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Nama Baru");

        _categoryRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task When_Delete_Found_Should_HapusDataDanReturnTrue()
    {
        var category = CreateCategory("Kategori Sampah");
        _categoryRepoMock.Setup(r => r.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var result = await _categoryService.DeleteAsync(category.Id);

        result.Should().BeTrue();
        _categoryRepoMock.Verify(r => r.DeleteAsync(category, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task When_GetTree_Should_ReturnStrukturHierarki()
    {
        var root = CreateCategory("Induk Perusahaan");
        var child = CreateCategory("Divisi IT");
        child.ParentId = root.Id;

        _categoryRepoMock.Setup(r => r.GetRootCategoriesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { root });

        _categoryRepoMock.Setup(r => r.GetChildrenAsync(root.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { child });

        _categoryRepoMock.Setup(r => r.GetChildrenAsync(child.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<Category>());

        var result = await _categoryService.GetTreeAsync();

        result.Should().HaveCount(1);
        result.First().Children.Should().HaveCount(1);
        result.First().Children.First().Name.Should().Be("Divisi IT");
    }

    private static Category CreateCategory(string name)
    {
        return new Category
        {
            Id = Guid.NewGuid(),
            Name = name,
            Slug = name.ToLower().Replace(" ", "-"),
            Description = "Deskripsi default",
            CreatedAt = DateTime.UtcNow
        };
    }
}
