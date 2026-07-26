using ECommerce.Domain.Entities;
using ECommerce.Domain.Events;
using FluentAssertions;
using Xunit;

namespace ECommerce.UnitTests.Domain;

public class NotificationTests
{
    [Fact]
    public void Create_ShouldInitializePendingNotification()
    {
        var userId = Guid.NewGuid();
        var notification = Notification.Create(
            userId,
            "user@example.com",
            "Subject",
            "Body",
            ECommerce.Domain.Entities.NotificationType.OrderConfirmation,
            ECommerce.Domain.Entities.NotificationChannel.Email);

        notification.UserId.Should().Be(userId);
        notification.Recipient.Should().Be("user@example.com");
        notification.Status.Should().Be(NotificationStatus.Pending);
    }

    [Fact]
    public void MarkSent_ShouldUpdateStatusAndEmitEvent()
    {
        var notification = Notification.Create(Guid.NewGuid(), "user@example.com", "Subject", "Body");
        notification.MarkSent();

        notification.Status.Should().Be(NotificationStatus.Sent);
        notification.DomainEvents.Should().ContainSingle(e => e is NotificationSentDomainEvent);
    }

    [Fact]
    public void MarkFailed_ShouldRecordErrorMessageAndEmitEvent()
    {
        var notification = Notification.Create(Guid.NewGuid(), "user@example.com", "Subject", "Body");
        notification.MarkFailed("SMTP error");

        notification.Status.Should().Be(NotificationStatus.Failed);
        notification.ErrorMessage.Should().Be("SMTP error");
        notification.DomainEvents.Should().ContainSingle(e => e is NotificationFailedDomainEvent);
    }
}
