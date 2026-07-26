using ECommerce.Contracts.Events;

namespace ECommerce.Application.Abstractions;

public interface IIntegrationEventHandler<in TEvent>
    where TEvent : class, IIntegrationEvent
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}
