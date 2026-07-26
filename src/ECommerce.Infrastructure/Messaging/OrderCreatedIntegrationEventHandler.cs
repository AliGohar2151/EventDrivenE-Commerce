using ECommerce.Application.Abstractions;
using ECommerce.Contracts.Events;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.Messaging;

public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
{
    private readonly ILogger<OrderCreatedIntegrationEventHandler> _logger;

    public OrderCreatedIntegrationEventHandler(ILogger<OrderCreatedIntegrationEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(OrderCreatedIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processed OrderCreatedIntegrationEvent: Order {OrderNumber} (ID: {OrderId}) for User {UserId} with Total Amount {TotalAmount}",
            @event.OrderNumber, @event.OrderId, @event.UserId, @event.TotalAmount);

        return Task.CompletedTask;
    }
}
