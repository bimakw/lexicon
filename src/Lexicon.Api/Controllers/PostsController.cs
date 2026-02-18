using Lexicon.Application.DTOs;
using Lexicon.Application.Services;
using Lexicon.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lexicon.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController(IPostService postService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<PostListDto>>> GetPosts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? status = null,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] Guid? authorId = null,
        [FromQuery] string? search = null,
        CancellationToken ct = default)
    {
        PostStatus? postStatus = null;
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<PostStatus>(status, true, out var parsed))
            postStatus = parsed;

        var result = await postService.GetPagedAsync(page, pageSize, postStatus, categoryId, authorId, search, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{slug}")]
    [AllowAnonymous]
    public async Task<ActionResult<PostDto>> GetPost(string slug, CancellationToken ct)
    {
        var result = await postService.GetBySlugAsync(slug, ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpGet("id/{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<PostDto>> GetPostById(Guid id, CancellationToken ct)
    {
        var result = await postService.GetByIdAsync(id, ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpPost]
    [Authorize(Policy = "CanCreatePosts")]
    public async Task<ActionResult<PostDto>> CreatePost(CreatePostDto dto, CancellationToken ct)
    {
        var result = await postService.CreateAsync(dto, ct);
        if (!result.IsSuccess) return BadRequest(result.Error);

        return CreatedAtAction(nameof(GetPostById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "CanUpdatePosts")]
    public async Task<ActionResult<PostDto>> UpdatePost(Guid id, UpdatePostDto dto, CancellationToken ct)
    {
        var result = await postService.UpdateAsync(id, dto, ct);
        if (!result.IsSuccess)
            return result.Error?.Contains("tidak ditemukan") == true ? NotFound(result.Error) : BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "CanDeletePosts")]
    public async Task<IActionResult> DeletePost(Guid id, CancellationToken ct)
    {
        var result = await postService.DeleteAsync(id, ct);
        if (!result.IsSuccess)
            return result.Error?.Contains("tidak ditemukan") == true ? NotFound(result.Error) : BadRequest(result.Error);

        return NoContent();
    }

    [HttpPost("{id:guid}/publish")]
    [Authorize(Policy = "CanPublishPosts")]
    public async Task<ActionResult<PostDto>> PublishPost(Guid id, CancellationToken ct)
    {
        var result = await postService.PublishAsync(id, ct);
        if (!result.IsSuccess)
            return result.Error?.Contains("tidak ditemukan") == true ? NotFound(result.Error) : BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPost("{id:guid}/unpublish")]
    [Authorize(Policy = "CanPublishPosts")]
    public async Task<ActionResult<PostDto>> UnpublishPost(Guid id, CancellationToken ct)
    {
        var result = await postService.UnpublishAsync(id, ct);
        if (!result.IsSuccess)
            return result.Error?.Contains("tidak ditemukan") == true ? NotFound(result.Error) : BadRequest(result.Error);

        return Ok(result.Value);
    }
}
