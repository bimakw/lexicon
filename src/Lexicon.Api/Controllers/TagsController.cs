using Lexicon.Application.DTOs;
using Lexicon.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lexicon.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly ITagService _tagService;

    public TagsController(ITagService tagService)
    {
        _tagService = tagService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TagDto>>> GetTags(CancellationToken cancellationToken)
    {
        var tags = await _tagService.GetAllAsync(cancellationToken);
        return Ok(tags);
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<TagDto>> GetTag(string slug, CancellationToken cancellationToken)
    {
        var tag = await _tagService.GetBySlugAsync(slug, cancellationToken);
        if (tag == null) return NotFound();
        return Ok(tag);
    }

    [HttpGet("id/{id:guid}")]
    public async Task<ActionResult<TagDto>> GetTagById(Guid id, CancellationToken cancellationToken)
    {
        var tag = await _tagService.GetByIdAsync(id, cancellationToken);
        if (tag == null) return NotFound();
        return Ok(tag);
    }

    [HttpPost]
    public async Task<ActionResult<TagDto>> CreateTag(CreateTagDto dto, CancellationToken cancellationToken)
    {
        var tag = await _tagService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetTagById), new { id = tag.Id }, tag);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TagDto>> UpdateTag(Guid id, UpdateTagDto dto, CancellationToken cancellationToken)
    {
        var tag = await _tagService.UpdateAsync(id, dto, cancellationToken);
        if (tag == null) return NotFound();
        return Ok(tag);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTag(Guid id, CancellationToken cancellationToken)
    {
        var result = await _tagService.DeleteAsync(id, cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }
}
