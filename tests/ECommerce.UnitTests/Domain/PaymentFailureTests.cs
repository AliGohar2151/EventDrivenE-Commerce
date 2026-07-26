using ECommerce.Domain.Entities;
using ECommerce.Domain.Events;
using FluentAssertions;
using Xunit;

namespace ECommerce.UnitTests.Domain;

public class PaymentFailureTests
{
    [Fact]
    public void Create_WithZeroAmount_ShouldThrowArgumentException()
    {
        var act = () => Payment.Create(Guid.NewGuid(), Guid.NewGuid(), 0m);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*amount*");
    }

    [Fact]
    public void Create_WithNegativeAmount_ShouldThrowArgumentException()
    {
        var act = () => Payment.Create(Guid.NewGuid(), Guid.NewGuid(), -50m);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*amount*");
    }

    [Fact]
    public void MarkFailed_ShouldSetStatusAndFailureReason()
    {
        var payment = Payment.Create(Guid.NewGuid(), Guid.NewGuid(), 100m);

        payment.MarkFailed("Card declined");

        payment.Status.Should().Be(PaymentStatus.Failed);
        payment.FailureReason.Should().Be("Card declined");
        payment.DomainEvents.Should().ContainSingle(e => e is PaymentFailedDomainEvent);
    }

    [Fact]
    public void MarkCompleted_ShouldSetStatusAndTransactionId()
    {
        var payment = Payment.Create(Guid.NewGuid(), Guid.NewGuid(), 100m);

        payment.MarkCompleted("TXN-ABC-123");

        payment.Status.Should().Be(PaymentStatus.Completed);
        payment.TransactionId.Should().Be("TXN-ABC-123");
        payment.DomainEvents.Should().ContainSingle(e => e is PaymentCompletedDomainEvent);
    }

    [Fact]
    public void Create_WithExplicitIdempotencyKey_ShouldUseProvidedKey()
    {
        var orderId = Guid.NewGuid();
        var payment = Payment.Create(orderId, Guid.NewGuid(), 199m, idempotencyKey: "IDEM-CUSTOM-KEY");

        payment.IdempotencyKey.Should().Be("IDEM-CUSTOM-KEY");
    }
}
