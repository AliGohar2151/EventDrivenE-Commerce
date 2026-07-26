using System.Text.Json;
using ECommerce.Application.Abstractions;
using ECommerce.Contracts.Events;
using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.Messaging;

public class ResilientConsumer
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<ResilientConsumer> _logger;

    public ResilientConsumer(IApplicationDbContext context, ILogger<ResilientConsumer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task ConsumeAsync<TEvent>(
        TEvent @event,
        IIntegrationEventHandler<TEvent> handler,
        int maxRetries = 3,
        int initialDelayMs = 100,
        CancellationToken cancellationToken = default)
        where TEvent : class, IIntegrationEvent
    {
        var handlerName = handler.GetType().FullName ?? handler.GetType().Name;

        var existingInboxMessage = await _context.InboxMessages
            .FirstOrDefaultAsync(m => m.Id == @event.Id && m.HandlerName == handlerName, cancellationToken);

        if (existingInboxMessage is not null)
        {
            _logger.LogInformation("Duplicate event {EventId} detected for handler {HandlerName}. Skipping execution.", @event.Id, handlerName);
            return;
        }

        int attempt = 0;
        var random = new Random();

        while (true)
        {
            attempt++;
            try
            {
                _logger.LogInformation("Executing event {EventId} (Attempt {Attempt}/{MaxRetries}) with CorrelationId {CorrelationId}",
                    @event.Id, attempt, maxRetries, @event.CorrelationId);

                await handler.HandleAsync(@event, cancellationToken);

                _context.InboxMessages.Add(InboxMessage.Create(@event.Id, handlerName));
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully processed event {EventId} on attempt {Attempt}", @event.Id, attempt);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed attempt {Attempt}/{MaxRetries} for event {EventId}: {ErrorMessage}",
                    attempt, maxRetries, @event.Id, ex.Message);

                if (attempt >= maxRetries)
                {
                    var serializedContent = JsonSerializer.Serialize(@event);
                    var deadLetter = DeadLetterMessage.Create(
                        @event.Id,
                        typeof(TEvent).Name,
                        serializedContent,
                        ex.Message,
                        ex.StackTrace,
                        attempt
                    );

                    _context.DeadLetterMessages.Add(deadLetter);
                    await _context.SaveChangesAsync(cancellationToken);

                    _logger.LogError(ex, "Max retry limit reached ({MaxRetries}) for event {EventId}. Message routed to Dead Letter Storage.", maxRetries, @event.Id);
                    return;
                }

                var backoffMs = initialDelayMs * Math.Pow(2, attempt - 1);
                var jitterMs = random.Next(0, 50);
                var totalDelayMs = (int)(backoffMs + jitterMs);

                await Task.Delay(totalDelayMs, cancellationToken);
            }
        }
    }
}
