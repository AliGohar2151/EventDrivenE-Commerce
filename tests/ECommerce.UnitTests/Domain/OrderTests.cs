using ECommerce.Domain.Entities;
using ECommerce.Domain.Events;
using FluentAssertions;
using Xunit;

namespace ECommerce.UnitTests.Domain;

public class OrderTests
{
    [Fact]
    public void Create_ShouldInitializeInPendingStatusAndEmitEvent()
    {
        var userId = Guid.NewGuid();
        var address = new ShippingAddress("123 Main St", "City", "State", "12345", "Country");
        var items = new[] { OrderItem.Create(Guid.NewGuid(), "Laptop", "SKU-1", null, 999.99m, 1) };

        var order = Order.Create(userId, address, items);

        order.Status.Should().Be(OrderStatus.Pending);
        order.TotalAmount.Should().Be(999.99m);
        order.DomainEvents.Should().ContainSingle(e => e is OrderCreatedDomainEvent);
    }

    [Theory]
    [InlineData(OrderStatus.Pending, OrderStatus.PaymentProcessing, true)]
    [InlineData(OrderStatus.PaymentProcessing, OrderStatus.Paid, true)]
    [InlineData(OrderStatus.Paid, OrderStatus.Processing, true)]
    [InlineData(OrderStatus.Processing, OrderStatus.Shipped, true)]
    [InlineData(OrderStatus.Shipped, OrderStatus.Delivered, true)]
    [InlineData(OrderStatus.Pending, OrderStatus.Delivered, false)]
    [InlineData(OrderStatus.Delivered, OrderStatus.Cancelled, false)]
    public void StateMachine_ShouldValidateTransitions(OrderStatus currentStatus, OrderStatus targetStatus, bool expectedValid)
    {
        var userId = Guid.NewGuid();
        var address = new ShippingAddress("123 Main St", "City", "State", "12345", "Country");
        var items = new[] { OrderItem.Create(Guid.NewGuid(), "Mouse", "SKU-2", null, 19.99m, 1) };
        var order = Order.Create(userId, address, items);

        if (currentStatus != OrderStatus.Pending)
        {
            if (currentStatus == OrderStatus.PaymentProcessing) order.TransitionToStatus(OrderStatus.PaymentProcessing);
            else if (currentStatus == OrderStatus.Paid) { order.TransitionToStatus(OrderStatus.PaymentProcessing); order.TransitionToStatus(OrderStatus.Paid); }
            else if (currentStatus == OrderStatus.Processing) { order.TransitionToStatus(OrderStatus.PaymentProcessing); order.TransitionToStatus(OrderStatus.Paid); order.TransitionToStatus(OrderStatus.Processing); }
            else if (currentStatus == OrderStatus.Shipped) { order.TransitionToStatus(OrderStatus.PaymentProcessing); order.TransitionToStatus(OrderStatus.Paid); order.TransitionToStatus(OrderStatus.Processing); order.TransitionToStatus(OrderStatus.Shipped); }
            else if (currentStatus == OrderStatus.Delivered) { order.TransitionToStatus(OrderStatus.PaymentProcessing); order.TransitionToStatus(OrderStatus.Paid); order.TransitionToStatus(OrderStatus.Processing); order.TransitionToStatus(OrderStatus.Shipped); order.TransitionToStatus(OrderStatus.Delivered); }
        }

        var result = order.TransitionToStatus(targetStatus);

        result.IsSuccess.Should().Be(expectedValid);
        if (expectedValid)
        {
            order.Status.Should().Be(targetStatus);
        }
    }
}
