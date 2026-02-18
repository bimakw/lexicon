using Lexicon.Application.DTOs;
using Lexicon.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lexicon.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController(ITagService tagService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TagDto>>> GetTags(CancellationToken ct)
    {
        var result = await tagService.GetAllAsync(ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<TagDto>> GetTag(string slug, CancellationToken ct)
    {
        var result = await tagService.GetBySlugAsync(slug, ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpGet("id/{id:guid}")]
    public async Task<ActionResult<TagDto>> GetTagById(Guid id, CancellationToken ct)
    {
        var result = await tagService.GetByIdAsync(id, ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpPost]
    public async Task<ActionResult<TagDto>> CreateTag(CreateTagDto dto, CancellationToken ct)
    {
        var result = await tagService.CreateAsync(dto, ct);
        if (!result.IsSuccess) return BadRequest(result.Error);

        return CreatedAtAction(nameof(GetTagById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TagDto>> UpdateTag(Guid id, UpdateTagDto dto, CancellationToken ct)
    {
        var result = await tagService.UpdateAsync(id, dto, ct);
        if (!result.IsSuccess)
            return result.Error?.Contains("tidak ditemukan") == true ? NotFound(result.Error) : BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTag(Guid id, CancellationToken ct)
    {
        var result = await tagService.DeleteAsync(id, ct);
        if (!result.IsSuccess)
            return result.Error?.Contains("tidak ditemukan") == true ? NotFound(result.Error) : BadRequest(result.Error);

        return NoContent();
    }
}
