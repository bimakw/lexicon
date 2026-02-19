using FluentAssertions;
using Lexicon.Application.DTOs;
using Lexicon.Application.Services;
using Lexicon.Domain.Entities;
using Lexicon.Domain.Interfaces;
using Moq;
using Xunit;

namespace Lexicon.Application.Tests.Services;

public class CommentServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<ICommentRepository> _repoMock = new();
    private readonly CommentService _service;

    public CommentServiceTests()
    {
        _uowMock.Setup(u => u.Comments).Returns(_repoMock.Object);
        _service = new CommentService(_uowMock.Object);
    }

    [Fact]
    public async Task CreateComment_Success()
    {
        var postId = Guid.NewGuid();
        var dto = new CreateCommentDto("Budi", "budi.s@gmail.com", "Artikelnya mantap banget, sangat membantu!");

        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _service.CreateAsync(postId, dto);

        result.IsSuccess.Should().BeTrue();
        result.Value!.AuthorName.Should().Be("Budi");
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Comment>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ApproveComment_Ok()
    {
        var id = Guid.NewGuid();
        var comment = new Comment { Id = id, IsApproved = false };

        _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(comment);
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _service.ApproveAsync(id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.IsApproved.Should().BeTrue();
    }
}
