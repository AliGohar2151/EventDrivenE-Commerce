using ECommerce.Application.Abstractions;
using ECommerce.Contracts.Cart;
using ECommerce.Contracts.Orders;
using ECommerce.Contracts.Payments;
using ECommerce.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ECommerce.IntegrationTests;

public class E2EWorkflowTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public E2EWorkflowTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task E2E_CompleteOrderAndPaymentFlow_ShouldSucceedAndDeliverNotification()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        var authService = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();
        var cartService = scope.ServiceProvider.GetRequiredService<ICartService>();
        var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
        var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

        var category = Category.Create("Electronics", "Tech products");
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();

        var product = Product.Create("Mechanical Keyboard", "KB-88", "RGB Gaming Keyboard", 120m, category.Id);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        var inventoryItem = InventoryItem.Create(product.Id, 25);
        dbContext.InventoryItems.Add(inventoryItem);
        await dbContext.SaveChangesAsync();

        var userId = Guid.NewGuid();
        var registerRequest = new Contracts.Authentication.RegisterUserRequest($"e2e_user_{userId}@example.com", "Password123!", "E2E", "Tester");
        var registerResult = await authService.RegisterAsync(registerRequest);
        registerResult.IsSuccess.Should().BeTrue();
        var loggedInUserId = registerResult.Value.User.Id;

        var addItemReq = new AddItemToCartRequest(product.Id, 2);
        var cartResult = await cartService.AddItemToCartAsync(loggedInUserId, addItemReq);
        cartResult.IsSuccess.Should().BeTrue();
        cartResult.Value.TotalItemCount.Should().Be(2);

        var createOrderReq = new CreateOrderRequest(
            new ShippingAddressDto("100 Tech Way", "Silicon Valley", "CA", "94025", "USA"),
            new List<OrderItemRequest> { new(product.Id, 2) }
        );
        var orderResult = await orderService.CreateOrderAsync(loggedInUserId, createOrderReq);
        orderResult.IsSuccess.Should().BeTrue();
        orderResult.Value.Status.Should().Be("Pending");
        var orderId = orderResult.Value.Id;

        var updatedInventory = await dbContext.InventoryItems.FirstAsync(i => i.ProductId == product.Id);
        updatedInventory.ReservedQuantity.Should().Be(2);
        updatedInventory.AvailableQuantity.Should().Be(23);

        var processPaymentReq = new ProcessPaymentRequest(orderId, 240m, "USD", "CreditCard", $"IDEM-E2E-{orderId}");
        var paymentResult = await paymentService.ProcessPaymentAsync(loggedInUserId, processPaymentReq);
        paymentResult.IsSuccess.Should().BeTrue();
        paymentResult.Value.Status.Should().Be("Completed");

        var paidOrderResult = await orderService.GetOrderByIdAsync(loggedInUserId, orderId);
        paidOrderResult.Value.Status.Should().Be("Paid");

        var notificationsResult = await notificationService.GetUserNotificationsAsync(loggedInUserId);
        notificationsResult.IsSuccess.Should().BeTrue();
        notificationsResult.Value.Should().NotBeEmpty();
    }
}
