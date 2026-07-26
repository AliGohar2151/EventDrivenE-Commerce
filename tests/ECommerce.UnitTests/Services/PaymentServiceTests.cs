using ECommerce.Application.Services;
using ECommerce.Contracts.Payments;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Messaging;
using ECommerce.Infrastructure.Payments;
using ECommerce.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ECommerce.UnitTests.Services;

public class PaymentServiceTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly MockPaymentProvider _paymentProvider;
    private readonly InMemoryEventBus _eventBus;
    private readonly PaymentService _service;

    public PaymentServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _paymentProvider = new MockPaymentProvider();
        _eventBus = new InMemoryEventBus(new ServiceCollection().BuildServiceProvider(), NullLogger<InMemoryEventBus>.Instance);
        _service = new PaymentService(_dbContext, _paymentProvider, _eventBus);
    }

    [Fact]
    public async Task ProcessPaymentAsync_SuccessfulProvider_ShouldMarkOrderPaidAndPaymentCompleted()
    {
        var userId = Guid.NewGuid();
        var address = new ShippingAddress("123 Main", "City", "ST", "12345", "Country");
        var item = OrderItem.Create(Guid.NewGuid(), "Laptop", "SKU-1", null, 1200m, 1);
        var order = Order.Create(userId, address, new[] { item });

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        var request = new ProcessPaymentRequest(order.Id, 1200m, "USD", "CreditCard", "IDEM-KEY-001");
        var result = await _service.ProcessPaymentAsync(userId, request);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be("Completed");
        result.Value.TransactionId.Should().NotBeNullOrEmpty();

        var updatedOrder = await _dbContext.Orders.FirstAsync(o => o.Id == order.Id);
        updatedOrder.Status.Should().Be(OrderStatus.Paid);
    }

    [Fact]
    public async Task ProcessPaymentAsync_DuplicateIdempotencyKey_ShouldReturnExistingPaymentWithoutRecharging()
    {
        var userId = Guid.NewGuid();
        var address = new ShippingAddress("123 Main", "City", "ST", "12345", "Country");
        var item = OrderItem.Create(Guid.NewGuid(), "Laptop", "SKU-1", null, 1200m, 1);
        var order = Order.Create(userId, address, new[] { item });

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        var request = new ProcessPaymentRequest(order.Id, 1200m, "USD", "CreditCard", "IDEM-KEY-DUP");
        var firstResult = await _service.ProcessPaymentAsync(userId, request);
        var duplicateResult = await _service.ProcessPaymentAsync(userId, request);

        firstResult.IsSuccess.Should().BeTrue();
        duplicateResult.IsSuccess.Should().BeTrue();
        duplicateResult.Value.Id.Should().Be(firstResult.Value.Id);
        duplicateResult.Value.TransactionId.Should().Be(firstResult.Value.TransactionId);
    }
}
