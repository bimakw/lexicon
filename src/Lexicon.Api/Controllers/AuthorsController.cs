using Lexicon.Application.DTOs;
using Lexicon.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lexicon.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorsController(IAuthorService authorService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthors(CancellationToken ct)
    {
        var result = await authorService.GetAllAsync(ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AuthorDto>> GetAuthor(Guid id, CancellationToken ct)
    {
        var result = await authorService.GetByIdAsync(id, ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpPost]
    public async Task<ActionResult<AuthorDto>> CreateAuthor(CreateAuthorDto dto, CancellationToken ct)
    {
        var result = await authorService.CreateAsync(dto, ct);
        if (!result.IsSuccess) return BadRequest(result.Error);
        
        return CreatedAtAction(nameof(GetAuthor), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AuthorDto>> UpdateAuthor(Guid id, UpdateAuthorDto dto, CancellationToken ct)
    {
        var result = await authorService.UpdateAsync(id, dto, ct);
        if (!result.IsSuccess) 
            return result.Error?.Contains("tidak ditemukan") == true ? NotFound(result.Error) : BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAuthor(Guid id, CancellationToken ct)
    {
        var result = await authorService.DeleteAsync(id, ct);
        if (!result.IsSuccess)
            return result.Error?.Contains("tidak ditemukan") == true ? NotFound(result.Error) : BadRequest(result.Error);

        return NoContent();
    }
}
