using ECommerce.Domain.Primitives;

namespace ECommerce.Domain.Events;

public record InventoryItemCreatedDomainEvent(Guid InventoryItemId, Guid ProductId, int StockQuantity, DateTime OccurredOnUtc) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
}

public record StockAdjustedDomainEvent(Guid InventoryItemId, Guid ProductId, int QuantityChange, int NewStockQuantity, string Reason, DateTime OccurredOnUtc) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
}

public record StockReservedDomainEvent(Guid InventoryItemId, Guid ProductId, int ReservedQuantity, int RemainingAvailable, DateTime OccurredOnUtc) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
}

public record StockReleasedDomainEvent(Guid InventoryItemId, Guid ProductId, int ReleasedQuantity, int NewAvailableQuantity, DateTime OccurredOnUtc) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
}

public record LowStockDetectedDomainEvent(Guid InventoryItemId, Guid ProductId, int AvailableQuantity, int Threshold, DateTime OccurredOnUtc) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
}
