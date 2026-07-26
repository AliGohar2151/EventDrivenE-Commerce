using ECommerce.Contracts.Orders;
using ECommerce.Domain.Primitives;

namespace ECommerce.Application.Abstractions;

public interface IOrderService
{
    Task<Result<OrderResponse>> CreateOrderAsync(Guid userId, CreateOrderRequest request, CancellationToken cancellationToken = default);
    Task<Result<OrderResponse>> GetOrderByIdAsync(Guid userId, Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<OrderResponse>>> GetOrdersForUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<OrderResponse>> UpdateOrderStatusAsync(Guid orderId, UpdateOrderStatusRequest request, CancellationToken cancellationToken = default);
    Task<Result> CancelOrderAsync(Guid userId, Guid orderId, string reason, CancellationToken cancellationToken = default);
}
