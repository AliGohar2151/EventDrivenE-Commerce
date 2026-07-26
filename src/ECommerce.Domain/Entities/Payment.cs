using ECommerce.Domain.Events;
using ECommerce.Domain.Primitives;

namespace ECommerce.Domain.Entities;

public class Payment : AggregateRoot<Guid>
{
    private Payment()
        : base(Guid.Empty)
    {
        Currency = "USD";
        PaymentProvider = "MockProvider";
        TransactionId = string.Empty;
        IdempotencyKey = string.Empty;
    }

    private Payment(
        Guid id,
        Guid orderId,
        Guid userId,
        decimal amount,
        string currency,
        string paymentProvider,
        string idempotencyKey)
        : base(id)
    {
        OrderId = orderId;
        UserId = userId;
        Amount = amount;
        Currency = currency;
        PaymentProvider = paymentProvider;
        IdempotencyKey = idempotencyKey;
        Status = PaymentStatus.Pending;
        TransactionId = string.Empty;
        CreatedOnUtc = DateTime.UtcNow;
    }

    public Guid OrderId { get; private set; }
    public Guid UserId { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string PaymentProvider { get; private set; }
    public string TransactionId { get; private set; }
    public string IdempotencyKey { get; private set; }
    public string? FailureReason { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime? UpdatedOnUtc { get; private set; }

    public static Payment Create(
        Guid orderId,
        Guid userId,
        decimal amount,
        string currency = "USD",
        string paymentProvider = "MockProvider",
        string? idempotencyKey = null)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Payment amount must be greater than zero.", nameof(amount));
        }

        var key = idempotencyKey ?? $"IDEM-{orderId}-{Guid.NewGuid()}";
        var payment = new Payment(Guid.NewGuid(), orderId, userId, amount, currency, paymentProvider, key);
        payment.AddDomainEvent(new PaymentInitiatedDomainEvent(payment.Id, payment.OrderId, payment.Amount, DateTime.UtcNow));

        return payment;
    }

    public void MarkCompleted(string transactionId)
    {
        Status = PaymentStatus.Completed;
        TransactionId = transactionId;
        UpdatedOnUtc = DateTime.UtcNow;
        AddDomainEvent(new PaymentCompletedDomainEvent(Id, OrderId, transactionId, DateTime.UtcNow));
    }

    public void MarkFailed(string failureReason)
    {
        Status = PaymentStatus.Failed;
        FailureReason = failureReason;
        UpdatedOnUtc = DateTime.UtcNow;
        AddDomainEvent(new PaymentFailedDomainEvent(Id, OrderId, failureReason, DateTime.UtcNow));
    }
}
