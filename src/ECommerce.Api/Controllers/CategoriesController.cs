using ECommerce.Application.Abstractions;
using ECommerce.Contracts.Products;
using ECommerce.Infrastructure.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/v1/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpPost]
    [HasPermission("Products.Create")]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _categoryService.CreateCategoryAsync(request, cancellationToken);
        if (result.IsFailure)
        {
            return Problem(result.Error.Description, statusCode: StatusCodes.Status409Conflict);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    [HasPermission("Products.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _categoryService.UpdateCategoryAsync(id, request, cancellationToken);
        if (result.IsFailure)
        {
            return Problem(result.Error.Description, statusCode: StatusCodes.Status404NotFound);
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [HasPermission("Products.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _categoryService.DeleteCategoryAsync(id, cancellationToken);
        if (result.IsFailure)
        {
            return Problem(result.Error.Description, statusCode: result.Error.Type switch
            {
                Domain.Primitives.ErrorType.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status404NotFound
            });
        }

        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _categoryService.GetCategoryByIdAsync(id, cancellationToken);
        if (result.IsFailure)
        {
            return NotFound(result.Error.Description);
        }

        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
    {
        var result = await _categoryService.GetCategoriesAsync(cancellationToken);
        return Ok(result.Value);
    }
}
