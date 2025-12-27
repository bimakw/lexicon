using Lexicon.Application.DTOs;
using Lexicon.Application.Services;
using Lexicon.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lexicon.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostService _postService;

    public PostsController(IPostService postService)
    {
        _postService = postService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<PostListDto>>> GetPosts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? status = null,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] Guid? authorId = null,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        PostStatus? postStatus = null;
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<PostStatus>(status, true, out var parsed))
            postStatus = parsed;

        var result = await _postService.GetPagedAsync(page, pageSize, postStatus, categoryId, authorId, search, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{slug}")]
    [AllowAnonymous]
    public async Task<ActionResult<PostDto>> GetPost(string slug, CancellationToken cancellationToken)
    {
        var post = await _postService.GetBySlugAsync(slug, cancellationToken);
        if (post == null) return NotFound();
        return Ok(post);
    }

    [HttpGet("id/{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<PostDto>> GetPostById(Guid id, CancellationToken cancellationToken)
    {
        var post = await _postService.GetByIdAsync(id, cancellationToken);
        if (post == null) return NotFound();
        return Ok(post);
    }

    [HttpPost]
    [Authorize(Policy = "CanCreatePosts")]
    public async Task<ActionResult<PostDto>> CreatePost(CreatePostDto dto, CancellationToken cancellationToken)
    {
        var post = await _postService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetPostById), new { id = post.Id }, post);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "CanUpdatePosts")]
    public async Task<ActionResult<PostDto>> UpdatePost(Guid id, UpdatePostDto dto, CancellationToken cancellationToken)
    {
        var post = await _postService.UpdateAsync(id, dto, cancellationToken);
        if (post == null) return NotFound();
        return Ok(post);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "CanDeletePosts")]
    public async Task<IActionResult> DeletePost(Guid id, CancellationToken cancellationToken)
    {
        var result = await _postService.DeleteAsync(id, cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpPost("{id:guid}/publish")]
    [Authorize(Policy = "CanPublishPosts")]
    public async Task<ActionResult<PostDto>> PublishPost(Guid id, CancellationToken cancellationToken)
    {
        var post = await _postService.PublishAsync(id, cancellationToken);
        if (post == null) return NotFound();
        return Ok(post);
    }

    [HttpPost("{id:guid}/unpublish")]
    [Authorize(Policy = "CanPublishPosts")]
    public async Task<ActionResult<PostDto>> UnpublishPost(Guid id, CancellationToken cancellationToken)
    {
        var post = await _postService.UnpublishAsync(id, cancellationToken);
        if (post == null) return NotFound();
        return Ok(post);
    }
}
