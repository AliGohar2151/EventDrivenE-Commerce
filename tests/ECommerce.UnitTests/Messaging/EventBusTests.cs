using ECommerce.Application.Abstractions;
using ECommerce.Contracts.Events;
using ECommerce.Infrastructure.Messaging;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ECommerce.UnitTests.Messaging;

public class EventBusTests
{
    [Fact]
    public async Task PublishAsync_ShouldDispatchToRegisteredIntegrationEventHandler()
    {
        InMemoryEventBus.Clear();
        var services = new ServiceCollection();
        var testHandler = new TestOrderCreatedHandler();
        services.AddSingleton<IIntegrationEventHandler<OrderCreatedIntegrationEvent>>(testHandler);
        var serviceProvider = services.BuildServiceProvider();

        var eventBus = new InMemoryEventBus(serviceProvider, NullLogger<InMemoryEventBus>.Instance);

        var @event = new OrderCreatedIntegrationEvent(
            Guid.NewGuid(),
            "ORD-TEST-001",
            Guid.NewGuid(),
            299.99m,
            new List<OrderItemIntegrationDto>(),
            DateTime.UtcNow
        );

        await eventBus.PublishAsync(@event);

        testHandler.HandledEvents.Should().ContainSingle(e => e.OrderId == @event.OrderId);
        InMemoryEventBus.GetPublishedEvents().Should().ContainSingle(e => e.Id == @event.Id);
    }
}

public class TestOrderCreatedHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
{
    public List<OrderCreatedIntegrationEvent> HandledEvents { get; } = new();

    public Task HandleAsync(OrderCreatedIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        HandledEvents.Add(@event);
        return Task.CompletedTask;
    }
}
