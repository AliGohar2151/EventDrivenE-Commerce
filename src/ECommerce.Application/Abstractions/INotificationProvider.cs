using ECommerce.Contracts.Notifications;

namespace ECommerce.Application.Abstractions;

public record NotificationProviderResult(bool IsSuccess, string? ErrorMessage);

public interface INotificationProvider
{
    Task<NotificationProviderResult> SendAsync(
        string recipient,
        string subject,
        string body,
        NotificationChannel channel,
        CancellationToken cancellationToken = default);
}
