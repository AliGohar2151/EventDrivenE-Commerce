using ECommerce.Domain.Primitives;

namespace ECommerce.Domain.Events;

public record PaymentInitiatedDomainEvent(Guid PaymentId, Guid OrderId, decimal Amount, DateTime OccurredOnUtc) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
}

public record PaymentCompletedDomainEvent(Guid PaymentId, Guid OrderId, string TransactionId, DateTime OccurredOnUtc) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
}

public record PaymentFailedDomainEvent(Guid PaymentId, Guid OrderId, string FailureReason, DateTime OccurredOnUtc) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
}
