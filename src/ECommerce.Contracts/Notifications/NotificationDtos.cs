namespace ECommerce.Contracts.Notifications;

public enum NotificationType
{
    OrderConfirmation = 0,
    PaymentSuccess = 1,
    PaymentFailure = 2,
    ShipmentUpdate = 3,
    DeliveryConfirmation = 4,
    Custom = 5
}

public enum NotificationChannel
{
    Email = 0,
    Sms = 1,
    InApp = 2
}

public record SendNotificationRequest(
    Guid UserId,
    string Recipient,
    string Subject,
    string Body,
    NotificationType Type = NotificationType.Custom,
    NotificationChannel Channel = NotificationChannel.Email
);

public record NotificationResponse(
    Guid Id,
    Guid UserId,
    string Recipient,
    string Subject,
    string Body,
    string Type,
    string Channel,
    string Status,
    string? ErrorMessage,
    DateTime SentAtUtc
);
