namespace ECommerce.Application.Abstractions;

public record PaymentProviderResult(
    bool IsSuccess,
    string TransactionId,
    string? ErrorMessage
);

public interface IPaymentProvider
{
    Task<PaymentProviderResult> ProcessPaymentAsync(Guid orderId, decimal amount, string currency, CancellationToken cancellationToken = default);
}
