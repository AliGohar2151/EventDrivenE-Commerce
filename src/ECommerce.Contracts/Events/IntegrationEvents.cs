namespace ECommerce.Contracts.Events;

public interface IIntegrationEvent
{
    Guid Id { get; }
    DateTime OccurredOnUtc { get; }
}

public record OrderCreatedIntegrationEvent(
    Guid OrderId,
    string OrderNumber,
    Guid UserId,
    decimal TotalAmount,
    IReadOnlyCollection<OrderItemIntegrationDto> Items,
    DateTime OccurredOnUtc
) : IIntegrationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
}

public record OrderItemIntegrationDto(
    Guid ProductId,
    string ProductName,
    string ProductSku,
    string? VariantSku,
    decimal UnitPrice,
    int Quantity,
    decimal TotalPrice
);

public record OrderStatusChangedIntegrationEvent(
    Guid OrderId,
    string OrderNumber,
    string OldStatus,
    string NewStatus,
    DateTime OccurredOnUtc
) : IIntegrationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
}

public record StockReservedIntegrationEvent(
    Guid InventoryItemId,
    Guid ProductId,
    int ReservedQuantity,
    int AvailableQuantity,
    DateTime OccurredOnUtc
) : IIntegrationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
}
