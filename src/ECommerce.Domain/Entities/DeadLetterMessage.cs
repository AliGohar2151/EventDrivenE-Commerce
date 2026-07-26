using ECommerce.Domain.Primitives;

namespace ECommerce.Domain.Entities;

public class DeadLetterMessage : Entity<Guid>
{
    private DeadLetterMessage()
        : base(Guid.Empty)
    {
        EventType = string.Empty;
        Content = string.Empty;
        ErrorMessage = string.Empty;
    }

    private DeadLetterMessage(
        Guid id,
        Guid eventId,
        string eventType,
        string content,
        string errorMessage,
        string? stackTrace,
        int retryCount)
        : base(id)
    {
        EventId = eventId;
        EventType = eventType;
        Content = content;
        ErrorMessage = errorMessage;
        StackTrace = stackTrace;
        RetryCount = retryCount;
        FailedAtUtc = DateTime.UtcNow;
    }

    public Guid EventId { get; private set; }
    public string EventType { get; private set; }
    public string Content { get; private set; }
    public string ErrorMessage { get; private set; }
    public string? StackTrace { get; private set; }
    public int RetryCount { get; private set; }
    public DateTime FailedAtUtc { get; private set; }

    public static DeadLetterMessage Create(
        Guid eventId,
        string eventType,
        string content,
        string errorMessage,
        string? stackTrace,
        int retryCount)
    {
        return new DeadLetterMessage(
            Guid.NewGuid(),
            eventId,
            eventType,
            content,
            errorMessage,
            stackTrace,
            retryCount);
    }
}
