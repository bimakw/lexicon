using Lexicon.Application.DTOs;
using Lexicon.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lexicon.Api.Controllers;

[ApiController]
[Route("api")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpGet("posts/{postId:guid}/comments")]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetPostComments(
        Guid postId,
        [FromQuery] bool? approved = null,
        CancellationToken cancellationToken = default)
    {
        var comments = await _commentService.GetByPostIdAsync(postId, approved, cancellationToken);
        return Ok(comments);
    }

    [HttpGet("comments/pending")]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetPendingComments(CancellationToken cancellationToken)
    {
        var comments = await _commentService.GetPendingAsync(cancellationToken);
        return Ok(comments);
    }

    [HttpPost("posts/{postId:guid}/comments")]
    public async Task<ActionResult<CommentDto>> CreateComment(Guid postId, CreateCommentDto dto, CancellationToken cancellationToken)
    {
        var comment = await _commentService.CreateAsync(postId, dto, cancellationToken);
        return Created($"/api/posts/{postId}/comments", comment);
    }

    [HttpPut("comments/{id:guid}/approve")]
    public async Task<ActionResult<CommentDto>> ApproveComment(Guid id, CancellationToken cancellationToken)
    {
        var comment = await _commentService.ApproveAsync(id, cancellationToken);
        if (comment == null) return NotFound();
        return Ok(comment);
    }

    [HttpDelete("comments/{id:guid}")]
    public async Task<IActionResult> DeleteComment(Guid id, CancellationToken cancellationToken)
    {
        var result = await _commentService.DeleteAsync(id, cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }
}
