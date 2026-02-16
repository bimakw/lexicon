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
    private readonly AuthorService _authorSvc;
    private readonly Mock<IAuthorRepository> _authorRepo;
    private readonly Mock<IUnitOfWork> _uow;

    public AuthorServiceTests()
    {
        _authorRepo = new Mock<IAuthorRepository>();
        _uow = new Mock<IUnitOfWork>();
        _uow.Setup(u => u.Authors).Returns(_authorRepo.Object);
        _authorSvc = new AuthorService(_uow.Object);
    }

    [Fact]
    public async Task ListPenulisGetAll()
    {
        var penulis = new List<Author>
        {
            new() { Id = Guid.NewGuid(), Name = "Bapak Budi", Email = "budi@kantor.id", CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Andreas Budi", Email = "andreas@kantor.id", CreatedAt = DateTime.UtcNow }
        };
        _authorRepo.Setup(r => r.GetAllAsync(default)).ReturnsAsync(penulis);

        var list = await _authorSvc.GetAllAsync();

        list.Should().HaveCount(2);
        list.Should().Contain(x => x.Name == "Bapak Budi");
    }

    [Fact]
    public async Task CariById()
    {
        var id = Guid.NewGuid();
        _authorRepo.Setup(r => r.GetByIdAsync(id, default))
            .ReturnsAsync(new Author { Id = id, Name = "Pak Darto", Email = "darto@kantor.id", CreatedAt = DateTime.UtcNow });

        var penulis = await _authorSvc.GetByIdAsync(id);

        penulis.Should().NotBeNull();
        penulis!.Name.Should().Be("Pak Darto");
    }

    [Fact]
    public async Task TambahPenulisNew()
    {
        var input = new CreateAuthorDto("Rina Sulis", "rina@kantor.id", "Staff Admin", "https://foto.id/rina.jpg");

        var result = await _authorSvc.CreateAsync(input);

        result.Name.Should().Be("Rina Sulis");
        result.Email.Should().Be("rina@kantor.id");

        _authorRepo.Verify(r => r.AddAsync(It.Is<Author>(a => a.Email == "rina@kantor.id"), default));
        _uow.Verify(u => u.SaveChangesAsync(default));
    }

    [Fact]
    public async Task UpdatePenulis()
    {
        var authorId = Guid.NewGuid();
        var lama = new Author { Id = authorId, Name = "Staff Lama", Email = "lama@kantor.id", CreatedAt = DateTime.UtcNow };
        _authorRepo.Setup(r => r.GetByIdAsync(authorId, default)).ReturnsAsync(lama);

        var dto = new UpdateAuthorDto("Staff Promosi", "promosi@kantor.id", "Naik jabatan", "https://foto.id/promosi.jpg");
        var result = await _authorSvc.UpdateAsync(authorId, dto);

        result!.Name.Should().Be("Staff Promosi");
        _authorRepo.Verify(r => r.UpdateAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task HapusPenulisReturnTrue()
    {
        var authorId = Guid.NewGuid();
        _authorRepo.Setup(r => r.GetByIdAsync(authorId, default))
            .ReturnsAsync(new Author { Id = authorId, Name = "Resign", Email = "keluar@kantor.id", CreatedAt = DateTime.UtcNow });

        var sukses = await _authorSvc.DeleteAsync(authorId);
        sukses.Should().BeTrue();
    }

    [Fact]
    public async Task HapusPenulisReturnFalse()
    {
        _authorRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync((Author?)null);

        var gagal = await _authorSvc.DeleteAsync(Guid.NewGuid());
        gagal.Should().BeFalse();
    }
}
