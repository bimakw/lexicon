using Lexicon.Application.DTOs;
using Lexicon.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lexicon.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorService _authorService;

    public AuthorsController(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthors(CancellationToken cancellationToken)
    {
        var authors = await _authorService.GetAllAsync(cancellationToken);
        return Ok(authors);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AuthorDto>> GetAuthor(Guid id, CancellationToken cancellationToken)
    {
        var author = await _authorService.GetByIdAsync(id, cancellationToken);
        if (author == null) return NotFound();
        return Ok(author);
    }

    [HttpPost]
    public async Task<ActionResult<AuthorDto>> CreateAuthor(CreateAuthorDto dto, CancellationToken cancellationToken)
    {
        var author = await _authorService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, author);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AuthorDto>> UpdateAuthor(Guid id, UpdateAuthorDto dto, CancellationToken cancellationToken)
    {
        var author = await _authorService.UpdateAsync(id, dto, cancellationToken);
        if (author == null) return NotFound();
        return Ok(author);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAuthor(Guid id, CancellationToken cancellationToken)
    {
        var result = await _authorService.DeleteAsync(id, cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }
}
