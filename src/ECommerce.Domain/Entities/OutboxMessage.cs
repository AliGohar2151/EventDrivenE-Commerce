using ECommerce.Domain.Primitives;

namespace ECommerce.Domain.Entities;

public class OutboxMessage : Entity<Guid>
{
    private OutboxMessage()
        : base(Guid.Empty)
    {
        Type = string.Empty;
        Content = string.Empty;
    }

    private OutboxMessage(Guid id, string type, string content, DateTime occurredOnUtc)
        : base(id)
    {
        Type = type;
        Content = content;
        OccurredOnUtc = occurredOnUtc;
    }

    public string Type { get; private set; }
    public string Content { get; private set; }
    public DateTime OccurredOnUtc { get; private set; }
    public DateTime? ProcessedOnUtc { get; private set; }
    public string? Error { get; private set; }

    public static OutboxMessage Create(string type, string content)
    {
        return new OutboxMessage(Guid.NewGuid(), type, content, DateTime.UtcNow);
    }

    public void MarkProcessed()
    {
        ProcessedOnUtc = DateTime.UtcNow;
    }

    public void MarkFailed(string error)
    {
        Error = error;
    }
}
