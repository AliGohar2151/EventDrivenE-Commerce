using ECommerce.Domain.Entities;
using ECommerce.Domain.Events;
using FluentAssertions;
using Xunit;

namespace ECommerce.UnitTests.Domain;

public class PaymentTests
{
    [Fact]
    public void Create_ShouldInitializePendingPaymentAndEmitDomainEvent()
    {
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var payment = Payment.Create(orderId, userId, 150m, "USD");

        payment.OrderId.Should().Be(orderId);
        payment.UserId.Should().Be(userId);
        payment.Amount.Should().Be(150m);
        payment.Status.Should().Be(PaymentStatus.Pending);
        payment.DomainEvents.Should().ContainSingle(e => e is PaymentInitiatedDomainEvent);
    }

    [Fact]
    public void MarkCompleted_ShouldUpdateStatusAndEmitEvent()
    {
        var payment = Payment.Create(Guid.NewGuid(), Guid.NewGuid(), 150m);
        payment.ClearDomainEvents();

        payment.MarkCompleted("TXN-12345");

        payment.Status.Should().Be(PaymentStatus.Completed);
        payment.TransactionId.Should().Be("TXN-12345");
        payment.DomainEvents.Should().ContainSingle(e => e is PaymentCompletedDomainEvent);
    }

    [Fact]
    public void MarkFailed_ShouldUpdateStatusAndRecordReason()
    {
        var payment = Payment.Create(Guid.NewGuid(), Guid.NewGuid(), 150m);
        payment.ClearDomainEvents();

        payment.MarkFailed("Insufficient funds");

        payment.Status.Should().Be(PaymentStatus.Failed);
        payment.FailureReason.Should().Be("Insufficient funds");
        payment.DomainEvents.Should().ContainSingle(e => e is PaymentFailedDomainEvent);
    }
}
