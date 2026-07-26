using ECommerce.Application.Abstractions;
using ECommerce.Contracts.Events;
using ECommerce.Contracts.Orders;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Primitives;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Services;

public class OrderService : IOrderService
{
    private readonly IApplicationDbContext _context;
    private readonly ICartRepository _cartRepository;
    private readonly IEventBus _eventBus;

    public OrderService(IApplicationDbContext context, ICartRepository cartRepository, IEventBus eventBus)
    {
        _context = context;
        _cartRepository = cartRepository;
        _eventBus = eventBus;
    }

    public async Task<Result<OrderResponse>> CreateOrderAsync(Guid userId, CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Items is null || request.Items.Count == 0)
        {
            return Result.Failure<OrderResponse>(Error.Validation("Order.NoItems", "Order must contain at least one item."));
        }

        var orderItems = new List<OrderItem>();
        foreach (var itemReq in request.Items)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == itemReq.ProductId, cancellationToken);
            if (product is null || product.Status == ProductStatus.Discontinued)
            {
                return Result.Failure<OrderResponse>(Error.NotFound("Product.Unavailable", $"Product '{itemReq.ProductId}' is unavailable."));
            }

            var inventory = await _context.InventoryItems.FirstOrDefaultAsync(i => i.ProductId == itemReq.ProductId, cancellationToken);
            if (inventory is null || inventory.AvailableQuantity < itemReq.Quantity)
            {
                var available = inventory?.AvailableQuantity ?? 0;
                return Result.Failure<OrderResponse>(Error.Conflict("Order.InsufficientStock", $"Insufficient stock for product '{product.Name}'. Available: {available}."));
            }

            var reserveResult = inventory.ReserveStock(itemReq.Quantity);
            if (reserveResult.IsFailure)
            {
                return Result.Failure<OrderResponse>(reserveResult.Error);
            }

            var price = product.Price;
            if (!string.IsNullOrWhiteSpace(itemReq.VariantSku))
            {
                var variant = product.Variants.FirstOrDefault(v => v.VariantSku == itemReq.VariantSku);
                if (variant is not null)
                {
                    price += variant.PriceModifier;
                }
            }

            orderItems.Add(OrderItem.Create(product.Id, product.Name, product.Sku, itemReq.VariantSku, price, itemReq.Quantity));
        }

        var shippingAddress = new ShippingAddress(
            request.ShippingAddress.Street,
            request.ShippingAddress.City,
            request.ShippingAddress.State,
            request.ShippingAddress.ZipCode,
            request.ShippingAddress.Country
        );

        var order = Order.Create(userId, shippingAddress, orderItems);
        _context.Orders.Add(order);

        await _context.SaveChangesAsync(cancellationToken);

        // Clear shopping cart after successful order creation
        await _cartRepository.DeleteAsync(userId, cancellationToken);

        // Publish OrderCreatedIntegrationEvent to EventBus
        var integrationItems = order.Items.Select(i => new OrderItemIntegrationDto(
            i.ProductId, i.ProductName, i.ProductSku, i.VariantSku, i.UnitPrice, i.Quantity, i.TotalPrice
        )).ToList();

        await _eventBus.PublishAsync(new OrderCreatedIntegrationEvent(
            order.Id, order.OrderNumber, order.UserId, order.TotalAmount, integrationItems, DateTime.UtcNow
        ), cancellationToken);

        return Result.Success(MapToResponse(order));
    }

    public async Task<Result<OrderResponse>> GetOrderByIdAsync(Guid userId, Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

        if (order is null)
        {
            return Result.Failure<OrderResponse>(Error.NotFound("Order.NotFound", "Order was not found."));
        }

        if (order.UserId != userId)
        {
            return Result.Failure<OrderResponse>(Error.Forbidden("Order.AccessDenied", "You do not have access to view this order."));
        }

        return Result.Success(MapToResponse(order));
    }

    public async Task<Result<IReadOnlyCollection<OrderResponse>>> GetOrdersForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var orders = await _context.Orders
            .Include(o => o.Items)
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedOnUtc)
            .ToListAsync(cancellationToken);

        var responses = orders.Select(MapToResponse).ToList();
        return Result.Success<IReadOnlyCollection<OrderResponse>>(responses.AsReadOnly());
    }

    public async Task<Result<OrderResponse>> UpdateOrderStatusAsync(Guid orderId, UpdateOrderStatusRequest request, CancellationToken cancellationToken = default)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

        if (order is null)
        {
            return Result.Failure<OrderResponse>(Error.NotFound("Order.NotFound", "Order was not found."));
        }

        if (!Enum.TryParse<OrderStatus>(request.Status, true, out var newStatus))
        {
            return Result.Failure<OrderResponse>(Error.Validation("Order.InvalidStatus", $"Invalid order status '{request.Status}'."));
        }

        var oldStatus = order.Status.ToString();
        var transitionResult = order.TransitionToStatus(newStatus, request.Reason);
        if (transitionResult.IsFailure)
        {
            return Result.Failure<OrderResponse>(transitionResult.Error);
        }

        if (newStatus == OrderStatus.Cancelled)
        {
            foreach (var item in order.Items)
            {
                var inventory = await _context.InventoryItems.FirstOrDefaultAsync(i => i.ProductId == item.ProductId, cancellationToken);
                inventory?.ReleaseStock(item.Quantity);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Publish OrderStatusChangedIntegrationEvent to EventBus
        await _eventBus.PublishAsync(new OrderStatusChangedIntegrationEvent(
            order.Id, order.OrderNumber, oldStatus, newStatus.ToString(), DateTime.UtcNow
        ), cancellationToken);

        return Result.Success(MapToResponse(order));
    }

    public async Task<Result> CancelOrderAsync(Guid userId, Guid orderId, string reason, CancellationToken cancellationToken = default)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

        if (order is null)
        {
            return Result.Failure(Error.NotFound("Order.NotFound", "Order was not found."));
        }

        if (order.UserId != userId)
        {
            return Result.Failure(Error.Forbidden("Order.AccessDenied", "You do not have permission to cancel this order."));
        }

        var oldStatus = order.Status.ToString();
        var cancelResult = order.Cancel(reason);
        if (cancelResult.IsFailure)
        {
            return cancelResult;
        }

        foreach (var item in order.Items)
        {
            var inventory = await _context.InventoryItems.FirstOrDefaultAsync(i => i.ProductId == item.ProductId, cancellationToken);
            inventory?.ReleaseStock(item.Quantity);
        }

        await _context.SaveChangesAsync(cancellationToken);

        await _eventBus.PublishAsync(new OrderStatusChangedIntegrationEvent(
            order.Id, order.OrderNumber, oldStatus, OrderStatus.Cancelled.ToString(), DateTime.UtcNow
        ), cancellationToken);

        return Result.Success();
    }

    private static OrderResponse MapToResponse(Order order)
    {
        var items = order.Items
            .Select(i => new OrderItemDto(i.Id, i.ProductId, i.ProductName, i.ProductSku, i.VariantSku, i.UnitPrice, i.Quantity, i.TotalPrice))
            .ToList();

        var address = new ShippingAddressDto(
            order.ShippingAddress.Street,
            order.ShippingAddress.City,
            order.ShippingAddress.State,
            order.ShippingAddress.ZipCode,
            order.ShippingAddress.Country
        );

        return new OrderResponse(
            order.Id,
            order.OrderNumber,
            order.UserId,
            order.Status.ToString(),
            order.TotalAmount,
            address,
            items,
            order.CreatedOnUtc,
            order.UpdatedOnUtc
        );
    }
}
