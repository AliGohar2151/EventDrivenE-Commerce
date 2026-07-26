using ECommerce.Domain.Primitives;

namespace ECommerce.Domain.Events;

public record NotificationSentDomainEvent(Guid NotificationId, Guid UserId, string Recipient, DateTime OccurredOnUtc) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
}

public record NotificationFailedDomainEvent(Guid NotificationId, Guid UserId, string ErrorMessage, DateTime OccurredOnUtc) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
}
