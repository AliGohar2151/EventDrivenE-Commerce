using ECommerce.Domain.Primitives;

namespace ECommerce.Domain.Events;

public record ProductCreatedDomainEvent(Guid ProductId, string Name, string Sku, decimal Price, DateTime OccurredOnUtc) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
}

public record ProductUpdatedDomainEvent(Guid ProductId, string Name, decimal Price, string Status, DateTime OccurredOnUtc) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
}
