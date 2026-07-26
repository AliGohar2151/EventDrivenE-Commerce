using System.Security.Claims;
using ECommerce.Application.Abstractions;
using ECommerce.Contracts.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/v1/cart")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _cartService.GetCartAsync(userId, cancellationToken);
        return Ok(result.Value);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] AddItemToCartRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _cartService.AddItemToCartAsync(userId, request, cancellationToken);
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

    [HttpPut("items/{productId:guid}")]
    public async Task<IActionResult> UpdateItemQuantity(
        Guid productId,
        [FromQuery] string? variantSku,
        [FromBody] UpdateCartItemQuantityRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _cartService.UpdateItemQuantityAsync(userId, productId, variantSku, request, cancellationToken);
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

    [HttpDelete("items/{productId:guid}")]
    public async Task<IActionResult> RemoveItem(
        Guid productId,
        [FromQuery] string? variantSku,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _cartService.RemoveItemFromCartAsync(userId, productId, variantSku, cancellationToken);
        if (result.IsFailure)
        {
            return Problem(result.Error.Description, statusCode: StatusCodes.Status404NotFound);
        }

        return Ok(result.Value);
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        await _cartService.ClearCartAsync(userId, cancellationToken);
        return NoContent();
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}
