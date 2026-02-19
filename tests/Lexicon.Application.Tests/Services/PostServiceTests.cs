using FluentAssertions;
using Lexicon.Application.DTOs;
using Lexicon.Application.Services;
using Lexicon.Domain.Entities;
using Lexicon.Domain.Interfaces;
using Moq;
using Xunit;

namespace Lexicon.Application.Tests.Services;

public class PostServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<IPostRepository> _postRepoMock = new();
    private readonly Mock<ITagRepository> _tagRepoMock = new();
    private readonly PostService _service;

    public PostServiceTests()
    {
        _uowMock.Setup(u => u.Posts).Returns(_postRepoMock.Object);
        _uowMock.Setup(u => u.Tags).Returns(_tagRepoMock.Object);
        _service = new PostService(_uowMock.Object);
    }

    [Fact]
    public async Task GetById_Ok()
    {
        var id = Guid.NewGuid();
        var post = new Post
        {
            Id = id,
            Title = "Microservices vs Monolith: Mana yang Lebih Worth It?",
            Content = "Pembahasan mendalam soal pemilihan arsitektur untuk sistem skala besar.",
            Author = new Author { Name = "Rizky Ramadhan" },
            Category = new Category { Name = "Software Systems" }
        };

        _postRepoMock.Setup(u => u.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(post);
        _tagRepoMock.Setup(u => u.GetByPostIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _service.GetByIdAsync(id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Title.Should().Be("Microservices vs Monolith: Mana yang Lebih Worth It?");
    }

    [Fact]
    public async Task GetById_NotFound()
    {
        var id = Guid.NewGuid();
        _postRepoMock.Setup(u => u.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Post?)null);

        var result = await _service.GetByIdAsync(id);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Post gak ketemu");
    }

    [Fact]
    public async Task CreatePost_Success()
    {
        var dto = new CreatePostDto(
            Title: "Cara Optimasi PostgreSQL biar Gak Lemot",
            Content: "Tips and trick optimasi index dan query di PostgreSQL 16.",
            Excerpt: "Guide singkat optimasi DB.",
            FeaturedImage: null,
            AuthorId: Guid.NewGuid(),
            CategoryId: Guid.NewGuid(),
            TagIds: [Guid.NewGuid()]
        );

        _postRepoMock.Setup(u => u.GetBySlugAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Post?)null);
        _tagRepoMock.Setup(u => u.GetByPostIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _service.CreateAsync(dto);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Title.Should().Be("Cara Optimasi PostgreSQL biar Gak Lemot");
    }
}



