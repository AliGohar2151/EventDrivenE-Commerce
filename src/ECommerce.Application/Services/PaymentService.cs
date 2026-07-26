using ECommerce.Application.Abstractions;
using ECommerce.Contracts.Events;
using ECommerce.Contracts.Payments;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Primitives;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IApplicationDbContext _context;
    private readonly IPaymentProvider _paymentProvider;
    private readonly IEventBus _eventBus;

    public PaymentService(
        IApplicationDbContext context,
        IPaymentProvider paymentProvider,
        IEventBus eventBus)
    {
        _context = context;
        _paymentProvider = paymentProvider;
        _eventBus = eventBus;
    }

    public async Task<Result<PaymentResponse>> ProcessPaymentAsync(Guid userId, ProcessPaymentRequest request, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(request.IdempotencyKey))
        {
            var existingPayment = await _context.Payments
                .FirstOrDefaultAsync(p => p.IdempotencyKey == request.IdempotencyKey, cancellationToken);

            if (existingPayment is not null)
            {
                return Result.Success(MapToResponse(existingPayment));
            }
        }

        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order is null)
        {
            return Result.Failure<PaymentResponse>(Error.NotFound("Order.NotFound", "Order was not found."));
        }

        if (order.UserId != userId)
        {
            return Result.Failure<PaymentResponse>(Error.Forbidden("Payment.Forbidden", "You do not have permission to pay for this order."));
        }

        if (order.Status is OrderStatus.Paid or OrderStatus.Shipped or OrderStatus.Delivered or OrderStatus.Cancelled)
        {
            return Result.Failure<PaymentResponse>(Error.Conflict("Payment.InvalidOrderState", $"Order is in state '{order.Status}' and cannot process payment."));
        }

        var payment = Payment.Create(order.Id, userId, request.Amount, request.Currency, "MockProvider", request.IdempotencyKey);
        _context.Payments.Add(payment);

        order.TransitionToStatus(OrderStatus.PaymentProcessing, "Payment processing initiated.");
        await _context.SaveChangesAsync(cancellationToken);

        await _eventBus.PublishAsync(new PaymentRequestedIntegrationEvent(
            payment.Id, payment.OrderId, payment.UserId, payment.Amount, payment.Currency, DateTime.UtcNow
        ), cancellationToken);

        var providerResult = await _paymentProvider.ProcessPaymentAsync(order.Id, request.Amount, request.Currency, cancellationToken);

        if (providerResult.IsSuccess)
        {
            payment.MarkCompleted(providerResult.TransactionId);
            order.TransitionToStatus(OrderStatus.Paid, "Payment processed successfully.");

            await _context.SaveChangesAsync(cancellationToken);

            await _eventBus.PublishAsync(new PaymentSucceededIntegrationEvent(
                payment.Id, payment.OrderId, payment.UserId, payment.Amount, providerResult.TransactionId, DateTime.UtcNow
            ), cancellationToken);
        }
        else
        {
            var failureReason = providerResult.ErrorMessage ?? "Payment gateway error.";
            payment.MarkFailed(failureReason);
            order.TransitionToStatus(OrderStatus.PaymentFailed, failureReason);

            await _context.SaveChangesAsync(cancellationToken);

            await _eventBus.PublishAsync(new PaymentFailedIntegrationEvent(
                payment.Id, payment.OrderId, payment.UserId, failureReason, DateTime.UtcNow
            ), cancellationToken);
        }

        return Result.Success(MapToResponse(payment));
    }

    public async Task<Result<PaymentResponse>> GetPaymentByOrderIdAsync(Guid userId, Guid orderId, CancellationToken cancellationToken = default)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(p => p.OrderId == orderId, cancellationToken);

        if (payment is null)
        {
            return Result.Failure<PaymentResponse>(Error.NotFound("Payment.NotFound", "Payment for specified order was not found."));
        }

        if (payment.UserId != userId)
        {
            return Result.Failure<PaymentResponse>(Error.Forbidden("Payment.Forbidden", "Access denied."));
        }

        return Result.Success(MapToResponse(payment));
    }

    private static PaymentResponse MapToResponse(Payment payment)
    {
        return new PaymentResponse(
            payment.Id,
            payment.OrderId,
            payment.UserId,
            payment.Amount,
            payment.Currency,
            payment.Status.ToString(),
            payment.TransactionId,
            payment.FailureReason,
            payment.CreatedOnUtc,
            payment.UpdatedOnUtc
        );
    }
}
