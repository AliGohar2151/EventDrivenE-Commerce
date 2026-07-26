using ECommerce.Application.Abstractions;
using ECommerce.Contracts.Products;
using ECommerce.Infrastructure.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/v1/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpPost]
    [HasPermission("Products.Create")]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        var result = await _productService.CreateProductAsync(request, cancellationToken);
        if (result.IsFailure)
        {
            return Problem(result.Error.Description, statusCode: result.Error.Type switch
            {
                Domain.Primitives.ErrorType.Conflict => StatusCodes.Status409Conflict,
                Domain.Primitives.ErrorType.NotFound => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status400BadRequest
            });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    [HasPermission("Products.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var result = await _productService.UpdateProductAsync(id, request, cancellationToken);
        if (result.IsFailure)
        {
            return Problem(result.Error.Description, statusCode: result.Error.Type switch
            {
                Domain.Primitives.ErrorType.NotFound => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status400BadRequest
            });
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [HasPermission("Products.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _productService.DeleteProductAsync(id, cancellationToken);
        if (result.IsFailure)
        {
            return Problem(result.Error.Description, statusCode: StatusCodes.Status404NotFound);
        }

        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _productService.GetProductByIdAsync(id, cancellationToken);
        if (result.IsFailure)
        {
            return NotFound(result.Error.Description);
        }

        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] ProductQueryParameters parameters, CancellationToken cancellationToken)
    {
        var result = await _productService.GetProductsAsync(parameters, cancellationToken);
        return Ok(result.Value);
    }
}
