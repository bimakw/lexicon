using Lexicon.Application.DTOs;
using Lexicon.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lexicon.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorsController(IAuthorService authorService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthors(CancellationToken cancellationToken)
    {
        var result = await authorService.GetAllAsync(cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AuthorDto>> GetAuthor(Guid id, CancellationToken cancellationToken)
    {
        var result = await authorService.GetByIdAsync(id, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpPost]
    public async Task<ActionResult<AuthorDto>> CreateAuthor(CreateAuthorDto dto, CancellationToken cancellationToken)
    {
        var result = await authorService.CreateAsync(dto, cancellationToken);
        if (!result.IsSuccess) return BadRequest(result.Error);
        
        return CreatedAtAction(nameof(GetAuthor), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AuthorDto>> UpdateAuthor(Guid id, UpdateAuthorDto dto, CancellationToken cancellationToken)
    {
        var result = await authorService.UpdateAsync(id, dto, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAuthor(Guid id, CancellationToken cancellationToken)
    {
        var result = await authorService.DeleteAsync(id, cancellationToken);
        return result.IsSuccess ? NoContent() : NotFound(result.Error);
    }
}
