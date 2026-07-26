using ECommerce.Application.Abstractions;

namespace ECommerce.Infrastructure.Payments;

public class MockPaymentProvider : IPaymentProvider
{
    public Task<PaymentProviderResult> ProcessPaymentAsync(Guid orderId, decimal amount, string currency, CancellationToken cancellationToken = default)
    {
        if (amount == 99999m)
        {
            return Task.FromResult(new PaymentProviderResult(false, string.Empty, "Card declined: Insufficient funds."));
        }

        var transactionId = $"TXN-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        return Task.FromResult(new PaymentProviderResult(true, transactionId, null));
    }
}
