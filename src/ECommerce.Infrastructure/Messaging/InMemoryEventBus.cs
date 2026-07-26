using System.Collections.Concurrent;
using System.Text.Json;
using ECommerce.Application.Abstractions;
using ECommerce.Contracts.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.Messaging;

public class InMemoryEventBus : IEventBus
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InMemoryEventBus> _logger;
    private static readonly ConcurrentBag<IIntegrationEvent> PublishedEvents = new();

    public InMemoryEventBus(IServiceProvider serviceProvider, ILogger<InMemoryEventBus> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class, IIntegrationEvent
    {
        PublishedEvents.Add(@event);
        var eventName = typeof(TEvent).Name;

        var serialized = JsonSerializer.Serialize(@event);
        var deserialized = JsonSerializer.Deserialize<TEvent>(serialized);

        _logger.LogInformation("Event {EventName} [{EventId}] published to EventBus: {Payload}", eventName, @event.Id, serialized);

        if (deserialized is null)
        {
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var handlers = scope.ServiceProvider.GetServices<IIntegrationEventHandler<TEvent>>();

        foreach (var handler in handlers)
        {
            try
            {
                await handler.HandleAsync(deserialized, cancellationToken);
                _logger.LogInformation("Successfully handled {EventName} with handler {HandlerType}", eventName, handler.GetType().Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling event {EventName} with handler {HandlerType}", eventName, handler.GetType().Name);
            }
        }
    }

    public static IReadOnlyCollection<IIntegrationEvent> GetPublishedEvents() => PublishedEvents.ToArray();
    public static void Clear() => PublishedEvents.Clear();
}
