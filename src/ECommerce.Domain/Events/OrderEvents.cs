using ECommerce.Domain.Primitives;

namespace ECommerce.Domain.Events;

public record OrderCreatedDomainEvent(Guid OrderId, string OrderNumber, Guid UserId, decimal TotalAmount, DateTime OccurredOnUtc) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
}

public record OrderStatusChangedDomainEvent(Guid OrderId, string OrderNumber, string OldStatus, string NewStatus, DateTime OccurredOnUtc) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
}

public record OrderCancelledDomainEvent(Guid OrderId, string OrderNumber, string Reason, DateTime OccurredOnUtc) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
}
