using ECommerce.Application.Services;
using ECommerce.Contracts.Cart;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ECommerce.UnitTests.Services;

public class CartServiceTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly CartRepository _cartRepository;
    private readonly CartService _service;

    public CartServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _cartRepository = new CartRepository();
        _service = new CartService(_cartRepository, _dbContext);
    }

    [Fact]
    public async Task AddItemToCartAsync_ValidStock_ShouldAddToCart()
    {
        var userId = Guid.NewGuid();
        var category = Category.Create("Electronics", "Tech");
        var product = Product.Create("Monitor", "MON-01", "4K Monitor", 399.99m, category.Id);
        var inventory = InventoryItem.Create(product.Id, 10);

        _dbContext.Products.Add(product);
        _dbContext.InventoryItems.Add(inventory);
        await _dbContext.SaveChangesAsync();

        var request = new AddItemToCartRequest(product.Id, 2);
        var result = await _service.AddItemToCartAsync(userId, request);

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalItemCount.Should().Be(2);
        result.Value.GrandTotalAmount.Should().Be(799.98m);
    }

    [Fact]
    public async Task AddItemToCartAsync_ExceedingAvailableStock_ShouldReturnConflict()
    {
        var userId = Guid.NewGuid();
        var category = Category.Create("Electronics", "Tech");
        var product = Product.Create("Monitor", "MON-02", "4K Monitor", 399.99m, category.Id);
        var inventory = InventoryItem.Create(product.Id, 1);

        _dbContext.Products.Add(product);
        _dbContext.InventoryItems.Add(inventory);
        await _dbContext.SaveChangesAsync();

        var request = new AddItemToCartRequest(product.Id, 5);
        var result = await _service.AddItemToCartAsync(userId, request);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Cart.InsufficientStock");
    }
}
