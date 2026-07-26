using ECommerce.Application.Abstractions;
using ECommerce.Contracts.Notifications;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Primitives;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Services;

public class NotificationService : INotificationService
{
    private readonly IApplicationDbContext _context;
    private readonly INotificationProvider _provider;

    public NotificationService(IApplicationDbContext context, INotificationProvider provider)
    {
        _context = context;
        _provider = provider;
    }

    public async Task<Result<NotificationResponse>> SendNotificationAsync(SendNotificationRequest request, CancellationToken cancellationToken = default)
    {
        var domainType = (Domain.Entities.NotificationType)request.Type;
        var domainChannel = (Domain.Entities.NotificationChannel)request.Channel;

        var notification = Notification.Create(
            request.UserId,
            request.Recipient,
            request.Subject,
            request.Body,
            domainType,
            domainChannel
        );

        _context.Notifications.Add(notification);

        var sendResult = await _provider.SendAsync(request.Recipient, request.Subject, request.Body, request.Channel, cancellationToken);
        if (sendResult.IsSuccess)
        {
            notification.MarkSent();
        }
        else
        {
            notification.MarkFailed(sendResult.ErrorMessage ?? "Notification provider error.");
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(notification));
    }

    public async Task<Result<IReadOnlyCollection<NotificationResponse>>> GetUserNotificationsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var notifications = await _context.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.SentAtUtc)
            .ToListAsync(cancellationToken);

        var responses = notifications.Select(MapToResponse).ToList();
        return Result.Success<IReadOnlyCollection<NotificationResponse>>(responses.AsReadOnly());
    }

    private static NotificationResponse MapToResponse(Notification notification)
    {
        return new NotificationResponse(
            notification.Id,
            notification.UserId,
            notification.Recipient,
            notification.Subject,
            notification.Body,
            notification.Type.ToString(),
            notification.Channel.ToString(),
            notification.Status.ToString(),
            notification.ErrorMessage,
            notification.SentAtUtc
        );
    }
}
