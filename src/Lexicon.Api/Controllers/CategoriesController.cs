using Lexicon.Application.DTOs;
using Lexicon.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lexicon.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories(CancellationToken cancellationToken)
    {
        var result = await categoryService.GetAllAsync(cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("tree")]
    public async Task<ActionResult<IEnumerable<CategoryTreeDto>>> GetCategoryTree(CancellationToken cancellationToken)
    {
        var result = await categoryService.GetTreeAsync(cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<CategoryDto>> GetCategory(string slug, CancellationToken cancellationToken)
    {
        var result = await categoryService.GetBySlugAsync(slug, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpGet("id/{id:guid}")]
    public async Task<ActionResult<CategoryDto>> GetCategoryById(Guid id, CancellationToken cancellationToken)
    {
        var result = await categoryService.GetByIdAsync(id, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto dto, CancellationToken cancellationToken)
    {
        var result = await categoryService.CreateAsync(dto, cancellationToken);
        if (!result.IsSuccess) return BadRequest(result.Error);
        
        return CreatedAtAction(nameof(GetCategoryById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CategoryDto>> UpdateCategory(Guid id, UpdateCategoryDto dto, CancellationToken cancellationToken)
    {
        var result = await categoryService.UpdateAsync(id, dto, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCategory(Guid id, CancellationToken cancellationToken)
    {
        var result = await categoryService.DeleteAsync(id, cancellationToken);
        return result.IsSuccess ? NoContent() : NotFound(result.Error);
    }
}
