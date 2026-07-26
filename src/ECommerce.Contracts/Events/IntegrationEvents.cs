namespace ECommerce.Contracts.Events;

public interface IIntegrationEvent
{
    Guid Id { get; }
    string CorrelationId { get; }
    DateTime OccurredOnUtc { get; }
}

public record OrderCreatedIntegrationEvent(
    Guid OrderId,
    string OrderNumber,
    Guid UserId,
    decimal TotalAmount,
    IReadOnlyCollection<OrderItemIntegrationDto> Items,
    DateTime OccurredOnUtc,
    string CorrelationId = ""
) : IIntegrationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public string CorrelationId { get; } = string.IsNullOrWhiteSpace(CorrelationId) ? Guid.NewGuid().ToString() : CorrelationId;
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
    DateTime OccurredOnUtc,
    string CorrelationId = ""
) : IIntegrationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public string CorrelationId { get; } = string.IsNullOrWhiteSpace(CorrelationId) ? Guid.NewGuid().ToString() : CorrelationId;
}

public record StockReservedIntegrationEvent(
    Guid InventoryItemId,
    Guid ProductId,
    int ReservedQuantity,
    int AvailableQuantity,
    DateTime OccurredOnUtc,
    string CorrelationId = ""
) : IIntegrationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public string CorrelationId { get; } = string.IsNullOrWhiteSpace(CorrelationId) ? Guid.NewGuid().ToString() : CorrelationId;
}

public record PaymentRequestedIntegrationEvent(
    Guid PaymentId,
    Guid OrderId,
    Guid UserId,
    decimal Amount,
    string Currency,
    DateTime OccurredOnUtc,
    string CorrelationId = ""
) : IIntegrationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public string CorrelationId { get; } = string.IsNullOrWhiteSpace(CorrelationId) ? Guid.NewGuid().ToString() : CorrelationId;
}

public record PaymentSucceededIntegrationEvent(
    Guid PaymentId,
    Guid OrderId,
    Guid UserId,
    decimal Amount,
    string TransactionId,
    DateTime OccurredOnUtc,
    string CorrelationId = ""
) : IIntegrationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public string CorrelationId { get; } = string.IsNullOrWhiteSpace(CorrelationId) ? Guid.NewGuid().ToString() : CorrelationId;
}

public record PaymentFailedIntegrationEvent(
    Guid PaymentId,
    Guid OrderId,
    Guid UserId,
    string FailureReason,
    DateTime OccurredOnUtc,
    string CorrelationId = ""
) : IIntegrationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public string CorrelationId { get; } = string.IsNullOrWhiteSpace(CorrelationId) ? Guid.NewGuid().ToString() : CorrelationId;
}
