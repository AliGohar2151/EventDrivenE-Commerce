using ECommerce.Domain.Events;
using ECommerce.Domain.Primitives;

namespace ECommerce.Domain.Entities;

public class Order : AggregateRoot<Guid>
{
    private readonly List<OrderItem> _items = new();

    private Order()
        : base(Guid.Empty)
    {
        OrderNumber = string.Empty;
        ShippingAddress = null!;
    }

    private Order(
        Guid id,
        string orderNumber,
        Guid userId,
        ShippingAddress shippingAddress)
        : base(id)
    {
        OrderNumber = orderNumber;
        UserId = userId;
        ShippingAddress = shippingAddress;
        Status = OrderStatus.Pending;
        CreatedOnUtc = DateTime.UtcNow;
    }

    public string OrderNumber { get; private set; }
    public Guid UserId { get; private set; }
    public OrderStatus Status { get; private set; }
    public ShippingAddress ShippingAddress { get; private set; }
    public decimal TotalAmount => _items.Sum(i => i.TotalPrice);
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime? UpdatedOnUtc { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public static Order Create(
        Guid userId,
        ShippingAddress shippingAddress,
        IEnumerable<OrderItem> items)
    {
        var orderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        var order = new Order(Guid.NewGuid(), orderNumber, userId, shippingAddress);

        order._items.AddRange(items);
        order.AddDomainEvent(new OrderCreatedDomainEvent(order.Id, order.OrderNumber, order.UserId, order.TotalAmount, DateTime.UtcNow));

        return order;
    }

    public Result TransitionToStatus(OrderStatus newStatus, string? reason = null)
    {
        if (!IsValidTransition(Status, newStatus))
        {
            return Result.Failure(Error.Conflict("Order.InvalidStatusTransition", $"Cannot transition order from {Status} to {newStatus}."));
        }

        var oldStatus = Status;
        Status = newStatus;
        UpdatedOnUtc = DateTime.UtcNow;

        AddDomainEvent(new OrderStatusChangedDomainEvent(Id, OrderNumber, oldStatus.ToString(), newStatus.ToString(), DateTime.UtcNow));

        if (newStatus == OrderStatus.Cancelled && !string.IsNullOrWhiteSpace(reason))
        {
            AddDomainEvent(new OrderCancelledDomainEvent(Id, OrderNumber, reason, DateTime.UtcNow));
        }

        return Result.Success();
    }

    public Result Cancel(string reason)
    {
        if (Status == OrderStatus.Delivered || Status == OrderStatus.Cancelled)
        {
            return Result.Failure(Error.Conflict("Order.CannotCancel", $"Order cannot be cancelled in state {Status}."));
        }

        return TransitionToStatus(OrderStatus.Cancelled, reason);
    }

    public static bool IsValidTransition(OrderStatus currentStatus, OrderStatus newStatus)
    {
        if (currentStatus == newStatus)
        {
            return true;
        }

        return currentStatus switch
        {
            OrderStatus.Pending => newStatus is OrderStatus.PaymentProcessing or OrderStatus.Cancelled,
            OrderStatus.PaymentProcessing => newStatus is OrderStatus.Paid or OrderStatus.PaymentFailed or OrderStatus.Cancelled,
            OrderStatus.PaymentFailed => newStatus is OrderStatus.Cancelled or OrderStatus.PaymentProcessing,
            OrderStatus.Paid => newStatus is OrderStatus.Processing or OrderStatus.Cancelled,
            OrderStatus.Processing => newStatus is OrderStatus.Shipped or OrderStatus.Cancelled,
            OrderStatus.Shipped => newStatus is OrderStatus.Delivered,
            OrderStatus.Delivered => false,
            OrderStatus.Cancelled => false,
            _ => false
        };
    }
}
