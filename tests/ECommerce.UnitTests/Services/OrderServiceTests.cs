using ECommerce.Application.Services;
using ECommerce.Contracts.Orders;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ECommerce.UnitTests.Services;

public class OrderServiceTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly CartRepository _cartRepository;
    private readonly OrderService _service;

    public OrderServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _cartRepository = new CartRepository();
        _service = new OrderService(_dbContext, _cartRepository);
    }

    [Fact]
    public async Task CreateOrderAsync_ValidStock_ShouldCreateOrderAndReserveStock()
    {
        var userId = Guid.NewGuid();
        var category = Category.Create("Tech", "Tech devices");
        var product = Product.Create("Smartwatch", "SW-01", "Fitness smartwatch", 199m, category.Id);
        var inventory = InventoryItem.Create(product.Id, 10);

        _dbContext.Products.Add(product);
        _dbContext.InventoryItems.Add(inventory);
        await _dbContext.SaveChangesAsync();

        var request = new CreateOrderRequest(
            new ShippingAddressDto("123 Elm St", "Springfield", "IL", "62701", "USA"),
            new List<OrderItemRequest> { new(product.Id, 2) }
        );

        var result = await _service.CreateOrderAsync(userId, request);

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalAmount.Should().Be(398m);
        result.Value.Status.Should().Be("Pending");

        var updatedInventory = await _dbContext.InventoryItems.FirstAsync(i => i.ProductId == product.Id);
        updatedInventory.ReservedQuantity.Should().Be(2);
        updatedInventory.AvailableQuantity.Should().Be(8);
    }

    [Fact]
    public async Task CreateOrderAsync_InsufficientStock_ShouldFailOrderCreation()
    {
        var userId = Guid.NewGuid();
        var category = Category.Create("Tech", "Tech devices");
        var product = Product.Create("Smartwatch", "SW-02", "Fitness smartwatch", 199m, category.Id);
        var inventory = InventoryItem.Create(product.Id, 1);

        _dbContext.Products.Add(product);
        _dbContext.InventoryItems.Add(inventory);
        await _dbContext.SaveChangesAsync();

        var request = new CreateOrderRequest(
            new ShippingAddressDto("123 Elm St", "Springfield", "IL", "62701", "USA"),
            new List<OrderItemRequest> { new(product.Id, 5) }
        );

        var result = await _service.CreateOrderAsync(userId, request);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Order.InsufficientStock");
    }
}
