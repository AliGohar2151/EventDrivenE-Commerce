using ECommerce.Application.Abstractions;
using ECommerce.Contracts.Inventory;
using ECommerce.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/v1/inventory")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpPost]
    [HasPermission("Products.Create")]
    public async Task<IActionResult> Add([FromBody] AddInventoryItemRequest request, CancellationToken cancellationToken)
    {
        var result = await _inventoryService.AddInventoryItemAsync(request, cancellationToken);
        if (result.IsFailure)
        {
            return Problem(result.Error.Description, statusCode: result.Error.Type switch
            {
                Domain.Primitives.ErrorType.Conflict => StatusCodes.Status409Conflict,
                Domain.Primitives.ErrorType.NotFound => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status400BadRequest
            });
        }

        return CreatedAtAction(nameof(GetByProductId), new { productId = result.Value.ProductId }, result.Value);
    }

    [HttpPost("{productId:guid}/adjust")]
    [HasPermission("Products.Update")]
    public async Task<IActionResult> AdjustStock(Guid productId, [FromBody] AdjustStockRequest request, CancellationToken cancellationToken)
    {
        var result = await _inventoryService.AdjustStockAsync(productId, request, cancellationToken);
        if (result.IsFailure)
        {
            return Problem(result.Error.Description, statusCode: result.Error.Type switch
            {
                Domain.Primitives.ErrorType.NotFound => StatusCodes.Status404NotFound,
                Domain.Primitives.ErrorType.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status400BadRequest
            });
        }

        return Ok(result.Value);
    }

    [HttpPost("{productId:guid}/reserve")]
    [Authorize]
    public async Task<IActionResult> ReserveStock(Guid productId, [FromBody] ReserveStockRequest request, CancellationToken cancellationToken)
    {
        var result = await _inventoryService.ReserveStockAsync(productId, request, cancellationToken);
        if (result.IsFailure)
        {
            return Problem(result.Error.Description, statusCode: result.Error.Type switch
            {
                Domain.Primitives.ErrorType.Conflict => StatusCodes.Status409Conflict,
                Domain.Primitives.ErrorType.NotFound => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status400BadRequest
            });
        }

        return Ok(result.Value);
    }

    [HttpPost("{productId:guid}/release")]
    [Authorize]
    public async Task<IActionResult> ReleaseStock(Guid productId, [FromBody] ReleaseStockRequest request, CancellationToken cancellationToken)
    {
        var result = await _inventoryService.ReleaseStockAsync(productId, request, cancellationToken);
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

    [HttpGet("{productId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetByProductId(Guid productId, CancellationToken cancellationToken)
    {
        var result = await _inventoryService.GetInventoryByProductIdAsync(productId, cancellationToken);
        if (result.IsFailure)
        {
            return NotFound(result.Error.Description);
        }

        return Ok(result.Value);
    }

    [HttpGet("low-stock")]
    [HasPermission("Products.Read")]
    public async Task<IActionResult> GetLowStockItems(CancellationToken cancellationToken)
    {
        var result = await _inventoryService.GetLowStockItemsAsync(cancellationToken);
        return Ok(result.Value);
    }
}
