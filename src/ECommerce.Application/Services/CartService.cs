using ECommerce.Application.Abstractions;
using ECommerce.Contracts.Cart;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Primitives;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IApplicationDbContext _dbContext;

    public CartService(ICartRepository cartRepository, IApplicationDbContext dbContext)
    {
        _cartRepository = cartRepository;
        _dbContext = dbContext;
    }

    public async Task<Result<CartResponse>> GetCartAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken) ?? Cart.Create(userId);
        return Result.Success(MapToResponse(cart));
    }

    public async Task<Result<CartResponse>> AddItemToCartAsync(Guid userId, AddItemToCartRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Quantity <= 0)
        {
            return Result.Failure<CartResponse>(Error.Validation("Cart.InvalidQuantity", "Quantity must be greater than zero."));
        }

        var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);
        if (product is null || product.Status == ProductStatus.Discontinued)
        {
            return Result.Failure<CartResponse>(Error.NotFound("Product.Unavailable", "Product is unavailable or was not found."));
        }

        var inventory = await _dbContext.InventoryItems.FirstOrDefaultAsync(i => i.ProductId == request.ProductId, cancellationToken);
        if (inventory is null || inventory.AvailableQuantity < request.Quantity)
        {
            var available = inventory?.AvailableQuantity ?? 0;
            return Result.Failure<CartResponse>(Error.Conflict("Cart.InsufficientStock", $"Cannot add {request.Quantity} items to cart. Only {available} available in stock."));
        }

        var price = product.Price;
        if (!string.IsNullOrWhiteSpace(request.VariantSku))
        {
            var variant = product.Variants.FirstOrDefault(v => v.VariantSku == request.VariantSku);
            if (variant is not null)
            {
                price += variant.PriceModifier;
            }
        }

        var cart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken) ?? Cart.Create(userId);
        cart.AddOrUpdateItem(product.Id, product.Name, product.Sku, request.VariantSku, price, request.Quantity);

        await _cartRepository.SaveAsync(cart, cancellationToken);

        return Result.Success(MapToResponse(cart));
    }

    public async Task<Result<CartResponse>> UpdateItemQuantityAsync(Guid userId, Guid productId, string? variantSku, UpdateCartItemQuantityRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Quantity <= 0)
        {
            return Result.Failure<CartResponse>(Error.Validation("Cart.InvalidQuantity", "Quantity must be greater than zero."));
        }

        var cart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken);
        if (cart is null)
        {
            return Result.Failure<CartResponse>(Error.NotFound("Cart.NotFound", "Shopping cart is empty."));
        }

        var inventory = await _dbContext.InventoryItems.FirstOrDefaultAsync(i => i.ProductId == productId, cancellationToken);
        if (inventory is null || inventory.AvailableQuantity < request.Quantity)
        {
            var available = inventory?.AvailableQuantity ?? 0;
            return Result.Failure<CartResponse>(Error.Conflict("Cart.InsufficientStock", $"Cannot update quantity to {request.Quantity}. Only {available} available in stock."));
        }

        var updated = cart.UpdateItemQuantity(productId, variantSku, request.Quantity);
        if (!updated)
        {
            return Result.Failure<CartResponse>(Error.NotFound("Cart.ItemNotFound", "Item was not found in cart."));
        }

        await _cartRepository.SaveAsync(cart, cancellationToken);

        return Result.Success(MapToResponse(cart));
    }

    public async Task<Result<CartResponse>> RemoveItemFromCartAsync(Guid userId, Guid productId, string? variantSku, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken);
        if (cart is null)
        {
            return Result.Failure<CartResponse>(Error.NotFound("Cart.NotFound", "Shopping cart is empty."));
        }

        var removed = cart.RemoveItem(productId, variantSku);
        if (!removed)
        {
            return Result.Failure<CartResponse>(Error.NotFound("Cart.ItemNotFound", "Item was not found in cart."));
        }

        await _cartRepository.SaveAsync(cart, cancellationToken);

        return Result.Success(MapToResponse(cart));
    }

    public async Task<Result> ClearCartAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await _cartRepository.DeleteAsync(userId, cancellationToken);
        return Result.Success();
    }

    private static CartResponse MapToResponse(Cart cart)
    {
        var items = cart.Items
            .Select(i => new CartItemDto(i.ProductId, i.ProductName, i.ProductSku, i.VariantSku, i.UnitPrice, i.Quantity, i.TotalPrice))
            .ToList();

        return new CartResponse(
            cart.UserId,
            items,
            cart.TotalItemCount,
            cart.GrandTotalAmount,
            cart.UpdatedOnUtc
        );
    }
}
