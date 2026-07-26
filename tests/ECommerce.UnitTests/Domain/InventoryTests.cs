using ECommerce.Domain.Entities;
using ECommerce.Domain.Events;
using FluentAssertions;
using Xunit;

namespace ECommerce.UnitTests.Domain;

public class InventoryTests
{
    [Fact]
    public void Create_ShouldInitializeStockAndAvailableQuantity()
    {
        var productId = Guid.NewGuid();
        var item = InventoryItem.Create(productId, 50, 5);

        item.ProductId.Should().Be(productId);
        item.StockQuantity.Should().Be(50);
        item.ReservedQuantity.Should().Be(0);
        item.AvailableQuantity.Should().Be(50);
        item.IsLowStock.Should().BeFalse();
    }

    [Fact]
    public void ReserveStock_InsufficientQuantity_ShouldReturnFailureResult()
    {
        var item = InventoryItem.Create(Guid.NewGuid(), 10, 2);

        var result = item.ReserveStock(15);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Inventory.InsufficientStock");
        item.ReservedQuantity.Should().Be(0);
    }

    [Fact]
    public void ReserveStock_CrossingLowStockThreshold_ShouldEmitLowStockEvent()
    {
        var item = InventoryItem.Create(Guid.NewGuid(), 10, 5);
        item.ClearDomainEvents();

        var result = item.ReserveStock(6);

        result.IsSuccess.Should().BeTrue();
        item.AvailableQuantity.Should().Be(4);
        item.IsLowStock.Should().BeTrue();
        item.DomainEvents.Should().ContainSingle(e => e is LowStockDetectedDomainEvent);
    }
}
