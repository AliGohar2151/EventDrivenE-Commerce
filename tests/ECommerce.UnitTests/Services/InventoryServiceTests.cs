using ECommerce.Application.Services;
using ECommerce.Contracts.Inventory;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ECommerce.UnitTests.Services;

public class InventoryServiceTests
{
    private readonly ApplicationDbContext _context;
    private readonly InventoryService _service;

    public InventoryServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _service = new InventoryService(_context);
    }

    [Fact]
    public async Task ReserveStock_ValidQuantity_ShouldUpdateAvailableStock()
    {
        var category = Category.Create("Electronics", "Tech");
        var product = Product.Create("Tablet", "TAB-01", "Tablet device", 499m, category.Id);
        var inventory = InventoryItem.Create(product.Id, 20);

        _context.Products.Add(product);
        _context.InventoryItems.Add(inventory);
        await _context.SaveChangesAsync();

        var request = new ReserveStockRequest(5);
        var result = await _service.ReserveStockAsync(product.Id, request);

        result.IsSuccess.Should().BeTrue();
        result.Value.AvailableQuantity.Should().Be(15);
        result.Value.ReservedQuantity.Should().Be(5);
    }

    [Fact]
    public async Task ReserveStock_ExceedingAvailableStock_ShouldReturnInsufficientStockError()
    {
        var category = Category.Create("Electronics", "Tech");
        var product = Product.Create("Tablet", "TAB-02", "Tablet device", 499m, category.Id);
        var inventory = InventoryItem.Create(product.Id, 5);

        _context.Products.Add(product);
        _context.InventoryItems.Add(inventory);
        await _context.SaveChangesAsync();

        var request = new ReserveStockRequest(10);
        var result = await _service.ReserveStockAsync(product.Id, request);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Inventory.InsufficientStock");
    }
}
