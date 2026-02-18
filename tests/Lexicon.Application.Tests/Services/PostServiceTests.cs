using FluentAssertions;
using Lexicon.Application.DTOs;
using Lexicon.Application.Services;
using Lexicon.Domain.Common;
using Lexicon.Domain.Entities;
using Lexicon.Domain.Interfaces;
using Moq;
using Xunit;

namespace Lexicon.Application.Tests.Services;

public class PostServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IPostRepository> _postRepositoryMock = new();
    private readonly Mock<ITagRepository> _tagRepositoryMock = new();
    private readonly PostService _service;

    public PostServiceTests()
    {
        _unitOfWorkMock.Setup(u => u.Posts).Returns(_postRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Tags).Returns(_tagRepositoryMock.Object);

        _service = new PostService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Should_ReturnPost_When_PostExistsInDb()
    {
        var postId = Guid.NewGuid();
        var post = new Post
        {
            Id = postId,
            Title = "Implementasi Clean Architecture di .NET",
            Content = "Artikel ini membahas cara memisahkan logic bisnis ke layer yang tepat.",
            Author = new Author { Name = "Indra" },
            Category = new Category { Name = "Software Engineering" }
        };

        _postRepositoryMock.Setup(u => u.GetByIdAsync(postId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(post);
        _tagRepositoryMock.Setup(u => u.GetByPostIdAsync(postId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _service.GetByIdAsync(postId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Title.Should().Be("Implementasi Clean Architecture di .NET");
    }

    [Fact]
    public async Task Should_ReturnFailure_When_PostNotFound()
    {
        var postId = Guid.NewGuid();
        _postRepositoryMock.Setup(u => u.GetByIdAsync(postId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Post?)null);

        var result = await _service.GetByIdAsync(postId);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Post not found");
    }

    [Fact]
    public async Task Should_CreatePost_When_DataIsValid()
    {
        var dto = new CreatePostDto(
            Title: "Tips Menulis Kode yang Bersih",
            Content: "Selalu prioritaskan keterbacaan kode daripada sekadar fungsionalitas semata.",
            Excerpt: "Panduan singkat menulis clean code.",
            FeaturedImage: null,
            AuthorId: Guid.NewGuid(),
            CategoryId: Guid.NewGuid(),
            TagIds: [Guid.NewGuid()]
        );

        _postRepositoryMock.Setup(u => u.GetBySlugAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Post?)null);
        _tagRepositoryMock.Setup(u => u.GetByPostIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _service.CreateAsync(dto);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Title.Should().Be("Tips Menulis Kode yang Bersih");
        _unitOfWorkMock.Verify(u => u.Posts.AddAsync(It.IsAny<Post>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}


