using System.Collections.Concurrent;
using ECommerce.Application.Abstractions;
using ECommerce.Domain.Entities;

namespace ECommerce.Infrastructure.Persistence;

public class CartRepository : ICartRepository
{
    private static readonly ConcurrentDictionary<Guid, Cart> Carts = new();

    public Task<Cart?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        Carts.TryGetValue(userId, out var cart);
        return Task.FromResult(cart);
    }

    public Task SaveAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        Carts[cart.UserId] = cart;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        Carts.TryRemove(userId, out _);
        return Task.CompletedTask;
    }
}
