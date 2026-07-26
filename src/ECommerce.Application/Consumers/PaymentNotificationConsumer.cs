using ECommerce.Application.Abstractions;
using ECommerce.Contracts.Events;
using ECommerce.Contracts.Notifications;

namespace ECommerce.Application.Consumers;

public class PaymentNotificationConsumer :
    IIntegrationEventHandler<PaymentSucceededIntegrationEvent>,
    IIntegrationEventHandler<PaymentFailedIntegrationEvent>
{
    private readonly INotificationService _notificationService;

    public PaymentNotificationConsumer(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task HandleAsync(PaymentSucceededIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        var subject = "Payment Succeeded";
        var body = $"Payment for Order ID {@event.OrderId} was successful. Transaction ID: {@event.TransactionId}. Amount: {@event.Amount:C}.";

        var request = new SendNotificationRequest(
            @event.UserId,
            $"user_{@event.UserId}@example.com",
            subject,
            body,
            NotificationType.PaymentSuccess,
            NotificationChannel.Email
        );

        await _notificationService.SendNotificationAsync(request, cancellationToken);
    }

    public async Task HandleAsync(PaymentFailedIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        var subject = "Payment Failed";
        var body = $"Payment for Order ID {@event.OrderId} failed. Reason: {@event.FailureReason}.";

        var request = new SendNotificationRequest(
            @event.UserId,
            $"user_{@event.UserId}@example.com",
            subject,
            body,
            NotificationType.PaymentFailure,
            NotificationChannel.Email
        );

        await _notificationService.SendNotificationAsync(request, cancellationToken);
    }
}
