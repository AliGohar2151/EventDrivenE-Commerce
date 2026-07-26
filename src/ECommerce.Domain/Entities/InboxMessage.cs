using ECommerce.Domain.Primitives;

namespace ECommerce.Domain.Entities;

public class InboxMessage : Entity<Guid>
{
    private InboxMessage()
        : base(Guid.Empty)
    {
        HandlerName = string.Empty;
    }

    private InboxMessage(Guid id, string handlerName, DateTime processedOnUtc)
        : base(id)
    {
        HandlerName = handlerName;
        ProcessedOnUtc = processedOnUtc;
    }

    public string HandlerName { get; private set; }
    public DateTime ProcessedOnUtc { get; private set; }

    public static InboxMessage Create(Guid messageId, string handlerName)
    {
        return new InboxMessage(messageId, handlerName, DateTime.UtcNow);
    }
}
