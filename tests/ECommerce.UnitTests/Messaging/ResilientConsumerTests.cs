using ECommerce.Application.Abstractions;
using ECommerce.Contracts.Events;
using ECommerce.Infrastructure.Messaging;
using ECommerce.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ECommerce.UnitTests.Messaging;

public class ResilientConsumerTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ResilientConsumer _consumer;

    public ResilientConsumerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _consumer = new ResilientConsumer(_dbContext, NullLogger<ResilientConsumer>.Instance);
    }

    [Fact]
    public async Task ConsumeAsync_SuccessfulExecution_ShouldSaveToInbox()
    {
        var handler = new AlwaysSuccessHandler();
        var @event = new PaymentSucceededIntegrationEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 100m, "TXN-1", DateTime.UtcNow);

        await _consumer.ConsumeAsync(@event, handler);

        var inboxMessage = await _dbContext.InboxMessages.FirstOrDefaultAsync(m => m.Id == @event.Id);
        inboxMessage.Should().NotBeNull();
        handler.CallCount.Should().Be(1);
    }

    [Fact]
    public async Task ConsumeAsync_DuplicateEvent_ShouldSkipExecutionViaInboxCheck()
    {
        var handler = new AlwaysSuccessHandler();
        var @event = new PaymentSucceededIntegrationEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 100m, "TXN-1", DateTime.UtcNow);

        await _consumer.ConsumeAsync(@event, handler);
        await _consumer.ConsumeAsync(@event, handler);

        handler.CallCount.Should().Be(1);
    }

    [Fact]
    public async Task ConsumeAsync_ExhaustedRetries_ShouldRouteToDeadLetterStorage()
    {
        var handler = new AlwaysFailingHandler();
        var @event = new PaymentFailedIntegrationEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Card Declined", DateTime.UtcNow);

        await _consumer.ConsumeAsync(@event, handler, maxRetries: 3, initialDelayMs: 1);

        handler.CallCount.Should().Be(3);

        var deadLetter = await _dbContext.DeadLetterMessages.FirstOrDefaultAsync(m => m.EventId == @event.Id);
        deadLetter.Should().NotBeNull();
        deadLetter!.ErrorMessage.Should().Contain("Simulated handler crash");
        deadLetter.RetryCount.Should().Be(3);
    }
}

public class AlwaysSuccessHandler : IIntegrationEventHandler<PaymentSucceededIntegrationEvent>
{
    public int CallCount { get; private set; }

    public Task HandleAsync(PaymentSucceededIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        CallCount++;
        return Task.CompletedTask;
    }
}

public class AlwaysFailingHandler : IIntegrationEventHandler<PaymentFailedIntegrationEvent>
{
    public int CallCount { get; private set; }

    public Task HandleAsync(PaymentFailedIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        CallCount++;
        throw new InvalidOperationException("Simulated handler crash");
    }
}
