using ECommerce.Contracts.Payments;
using ECommerce.Domain.Primitives;

namespace ECommerce.Application.Abstractions;

public interface IPaymentService
{
    Task<Result<PaymentResponse>> ProcessPaymentAsync(Guid userId, ProcessPaymentRequest request, CancellationToken cancellationToken = default);
    Task<Result<PaymentResponse>> GetPaymentByOrderIdAsync(Guid userId, Guid orderId, CancellationToken cancellationToken = default);
}
