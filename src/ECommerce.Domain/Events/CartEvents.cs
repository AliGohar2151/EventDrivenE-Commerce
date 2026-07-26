using ECommerce.Domain.Primitives;

namespace ECommerce.Domain.Events;

public record CartItemAddedDomainEvent(Guid UserId, Guid ProductId, int Quantity, decimal UnitPrice, DateTime OccurredOnUtc) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
}

public record CartClearedDomainEvent(Guid UserId, DateTime OccurredOnUtc) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
}
