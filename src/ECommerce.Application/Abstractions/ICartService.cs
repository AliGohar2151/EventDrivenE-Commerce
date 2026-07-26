using ECommerce.Contracts.Cart;
using ECommerce.Domain.Primitives;

namespace ECommerce.Application.Abstractions;

public interface ICartService
{
    Task<Result<CartResponse>> GetCartAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<CartResponse>> AddItemToCartAsync(Guid userId, AddItemToCartRequest request, CancellationToken cancellationToken = default);
    Task<Result<CartResponse>> UpdateItemQuantityAsync(Guid userId, Guid productId, string? variantSku, UpdateCartItemQuantityRequest request, CancellationToken cancellationToken = default);
    Task<Result<CartResponse>> RemoveItemFromCartAsync(Guid userId, Guid productId, string? variantSku, CancellationToken cancellationToken = default);
    Task<Result> ClearCartAsync(Guid userId, CancellationToken cancellationToken = default);
}
