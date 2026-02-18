using Lexicon.Application.DTOs;
using Lexicon.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lexicon.Api.Controllers;

[ApiController]
[Route("api")]
public class CommentsController(ICommentService commentService) : ControllerBase
{
    [HttpGet("posts/{postId:guid}/comments")]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetPostComments(
        Guid postId,
        [FromQuery] bool? approved = null,
        CancellationToken ct = default)
    {
        var result = await commentService.GetByPostIdAsync(postId, approved, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("comments/pending")]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetPendingComments(CancellationToken ct)
    {
        var result = await commentService.GetPendingAsync(ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("posts/{postId:guid}/comments")]
    public async Task<ActionResult<CommentDto>> CreateComment(Guid postId, CreateCommentDto dto, CancellationToken ct)
    {
        var result = await commentService.CreateAsync(postId, dto, ct);
        if (!result.IsSuccess) return BadRequest(result.Error);

        return Created($"/api/posts/{postId}/comments", result.Value);
    }

    [HttpPut("comments/{id:guid}/approve")]
    public async Task<ActionResult<CommentDto>> ApproveComment(Guid id, CancellationToken ct)
    {
        var result = await commentService.ApproveAsync(id, ct);
        if (!result.IsSuccess)
            return result.Error?.Contains("tidak ditemukan") == true ? NotFound(result.Error) : BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpDelete("comments/{id:guid}")]
    public async Task<IActionResult> DeleteComment(Guid id, CancellationToken ct)
    {
        var result = await commentService.DeleteAsync(id, ct);
        if (!result.IsSuccess)
            return result.Error?.Contains("tidak ditemukan") == true ? NotFound(result.Error) : BadRequest(result.Error);

        return NoContent();
    }
}
