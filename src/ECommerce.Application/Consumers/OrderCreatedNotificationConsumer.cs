using ECommerce.Application.Abstractions;
using ECommerce.Contracts.Events;
using ECommerce.Contracts.Notifications;

namespace ECommerce.Application.Consumers;

public class OrderCreatedNotificationConsumer : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
{
    private readonly INotificationService _notificationService;

    public OrderCreatedNotificationConsumer(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task HandleAsync(OrderCreatedIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        var subject = $"Order Confirmation - {@event.OrderNumber}";
        var body = $"Thank you for your order! Order {@event.OrderNumber} for {@event.TotalAmount:C} has been created successfully.";

        var request = new SendNotificationRequest(
            @event.UserId,
            $"user_{@event.UserId}@example.com",
            subject,
            body,
            NotificationType.OrderConfirmation,
            NotificationChannel.Email
        );

        await _notificationService.SendNotificationAsync(request, cancellationToken);
    }
}
