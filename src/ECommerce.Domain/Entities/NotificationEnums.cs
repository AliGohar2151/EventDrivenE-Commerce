namespace ECommerce.Domain.Entities;

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
