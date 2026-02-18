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
    private readonly CategoryService _svc;
    private readonly Mock<ICategoryRepository> _catRepo;
    private readonly Mock<IUnitOfWork> _uow;

    public CategoryServiceTests()
    {
        _catRepo = new Mock<ICategoryRepository>();
        _uow = new Mock<IUnitOfWork>();
        _uow.Setup(x => x.Categories).Returns(_catRepo.Object);
        _svc = new CategoryService(_uow.Object);
    }

    private Category BikinKategori(string nama, Guid? parentId = null)
    {
        return new Category
        {
            Id = Guid.NewGuid(),
            Name = nama,
            Slug = nama.ToLower().Replace(" ", "-"),
            ParentId = parentId,
            CreatedAt = DateTime.UtcNow
        };
    }

    [Fact]
    public async Task Should_ReturnAllCategories_When_Called()
    {
        var list = new List<Category>
        {
            BikinKategori("Laporan Bulanan"),
            BikinKategori("Surat Keluar")
        };
        _catRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(list);

        var result = await _svc.GetAllAsync();

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Should_ReturnCategory_When_Found()
    {
        var kat = BikinKategori("Dokumen Teknis");
        _catRepo.Setup(r => r.GetByIdAsync(kat.Id, It.IsAny<CancellationToken>())).ReturnsAsync(kat);

        var result = await _svc.GetByIdAsync(kat.Id);
        result.Should().NotBeNull();
        result!.Name.Should().Be("Dokumen Teknis");
    }

    [Fact]
    public async Task Should_ReturnCategory_When_FoundBySlug()
    {
        var kat = BikinKategori("Panduan Kerja");
        _catRepo.Setup(r => r.GetBySlugAsync("panduan-kerja", default)).ReturnsAsync(kat);

        var result = await _svc.GetBySlugAsync("panduan-kerja");
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_CreateCategory_When_ValidDto()
    {
        var dto = new CreateCategoryDto("Arsip Lama", "Dokumen lama perusahaan", null);

        var result = await _svc.CreateAsync(dto);

        result.Name.Should().Be("Arsip Lama");
        _uow.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Should_UpdateCategory_When_Found()
    {
        var existing = BikinKategori("Nama Salah");
        _catRepo.Setup(r => r.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>())).ReturnsAsync(existing);

        var dto = new UpdateCategoryDto("Nama Benar", "Deskripsi diperbaiki", null);
        var result = await _svc.UpdateAsync(existing.Id, dto);

        result!.Name.Should().Be("Nama Benar");
        _catRepo.Verify(r => r.UpdateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task Should_DeleteCategory_When_Found()
    {
        var kat = BikinKategori("Kategori Usang");
        _catRepo.Setup(r => r.GetByIdAsync(kat.Id, default)).ReturnsAsync(kat);

        var ok = await _svc.DeleteAsync(kat.Id);
        ok.Should().BeTrue();
    }

    [Fact]
    public async Task Should_ReturnHierarchy_When_TreeRequested()
    {
        var main = BikinKategori("Kantor Pusat");
        var child = BikinKategori("Cabang Surabaya", main.Id);

        _catRepo.Setup(r => r.GetRootCategoriesAsync(default)).ReturnsAsync(new[] { main });
        _catRepo.Setup(r => r.GetChildrenAsync(main.Id, default)).ReturnsAsync(new[] { child });
        _catRepo.Setup(r => r.GetChildrenAsync(child.Id, default)).ReturnsAsync(Array.Empty<Category>());

        var tree = await _svc.GetTreeAsync();

        tree.Should().ContainSingle();
        tree.First().Children.Should().ContainSingle()
            .Which.Name.Should().Be("Cabang Surabaya");
    }
}
