using ECommerce.Contracts.Notifications;
using ECommerce.Domain.Primitives;

namespace ECommerce.Application.Abstractions;

public interface INotificationService
{
    Task<Result<NotificationResponse>> SendNotificationAsync(SendNotificationRequest request, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<NotificationResponse>>> GetUserNotificationsAsync(Guid userId, CancellationToken cancellationToken = default);
}
