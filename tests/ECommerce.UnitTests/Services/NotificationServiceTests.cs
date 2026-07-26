using ECommerce.Application.Consumers;
using ECommerce.Application.Services;
using ECommerce.Contracts.Events;
using ECommerce.Contracts.Notifications;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Notifications;
using ECommerce.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ECommerce.UnitTests.Services;

public class NotificationServiceTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly MockNotificationProvider _provider;
    private readonly NotificationService _service;

    public NotificationServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _provider = new MockNotificationProvider(NullLogger<MockNotificationProvider>.Instance);
        _service = new NotificationService(_dbContext, _provider);
    }

    [Fact]
    public async Task SendNotificationAsync_ShouldPersistAndMarkSent()
    {
        var userId = Guid.NewGuid();
        var request = new SendNotificationRequest(
            userId,
            "test@example.com",
            "Welcome",
            "Welcome to our store",
            ECommerce.Contracts.Notifications.NotificationType.Custom,
            ECommerce.Contracts.Notifications.NotificationChannel.Email);

        var result = await _service.SendNotificationAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be("Sent");

        var persisted = await _dbContext.Notifications.FirstOrDefaultAsync(n => n.Id == result.Value.Id);
        persisted.Should().NotBeNull();
        persisted!.Status.Should().Be(NotificationStatus.Sent);
    }

    [Fact]
    public async Task OrderCreatedNotificationConsumer_ShouldSendOrderConfirmation()
    {
        var consumer = new OrderCreatedNotificationConsumer(_service);
        var userId = Guid.NewGuid();
        var @event = new OrderCreatedIntegrationEvent(Guid.NewGuid(), "ORD-999", userId, 199.99m, new List<OrderItemIntegrationDto>(), DateTime.UtcNow);

        await consumer.HandleAsync(@event);

        var userNotifications = await _dbContext.Notifications.Where(n => n.UserId == userId).ToListAsync();
        userNotifications.Should().ContainSingle(n => n.Subject.Contains("ORD-999"));
    }
}
