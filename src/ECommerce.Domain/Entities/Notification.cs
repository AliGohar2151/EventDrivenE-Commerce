using ECommerce.Domain.Events;
using ECommerce.Domain.Primitives;

namespace ECommerce.Domain.Entities;

public class Notification : AggregateRoot<Guid>
{
    private Notification()
        : base(Guid.Empty)
    {
        Recipient = string.Empty;
        Subject = string.Empty;
        Body = string.Empty;
    }

    private Notification(
        Guid id,
        Guid userId,
        string recipient,
        string subject,
        string body,
        NotificationType type,
        NotificationChannel channel)
        : base(id)
    {
        UserId = userId;
        Recipient = recipient;
        Subject = subject;
        Body = body;
        Type = type;
        Channel = channel;
        Status = NotificationStatus.Pending;
        SentAtUtc = DateTime.UtcNow;
    }

    public Guid UserId { get; private set; }
    public string Recipient { get; private set; }
    public string Subject { get; private set; }
    public string Body { get; private set; }
    public NotificationType Type { get; private set; }
    public NotificationChannel Channel { get; private set; }
    public NotificationStatus Status { get; private set; }
    public string? ErrorMessage { get; private set; }
    public DateTime SentAtUtc { get; private set; }

    public static Notification Create(
        Guid userId,
        string recipient,
        string subject,
        string body,
        NotificationType type = NotificationType.Custom,
        NotificationChannel channel = NotificationChannel.Email)
    {
        if (string.IsNullOrWhiteSpace(recipient))
        {
            throw new ArgumentException("Recipient cannot be empty.", nameof(recipient));
        }

        return new Notification(Guid.NewGuid(), userId, recipient, subject, body, type, channel);
    }

    public void MarkSent()
    {
        Status = NotificationStatus.Sent;
        AddDomainEvent(new NotificationSentDomainEvent(Id, UserId, Recipient, DateTime.UtcNow));
    }

    public void MarkFailed(string errorMessage)
    {
        Status = NotificationStatus.Failed;
        ErrorMessage = errorMessage;
        AddDomainEvent(new NotificationFailedDomainEvent(Id, UserId, errorMessage, DateTime.UtcNow));
    }
}
