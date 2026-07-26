namespace ECommerce.Contracts.Payments;

public record ProcessPaymentRequest(
    Guid OrderId,
    decimal Amount,
    string Currency = "USD",
    string PaymentMethod = "CreditCard",
    string? IdempotencyKey = null
);

public record PaymentResponse(
    Guid Id,
    Guid OrderId,
    Guid UserId,
    decimal Amount,
    string Currency,
    string Status,
    string TransactionId,
    string? FailureReason,
    DateTime CreatedOnUtc,
    DateTime? UpdatedOnUtc
);
