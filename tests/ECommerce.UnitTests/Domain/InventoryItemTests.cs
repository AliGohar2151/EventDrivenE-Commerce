using ECommerce.Domain.Entities;
using ECommerce.Domain.Events;
using FluentAssertions;
using Xunit;

namespace ECommerce.UnitTests.Domain;

public class InventoryItemTests
{
    [Fact]
    public void Create_ValidParameters_ShouldInitializeCorrectly()
    {
        var productId = Guid.NewGuid();
        var item = InventoryItem.Create(productId, 50, 10);

        item.ProductId.Should().Be(productId);
        item.StockQuantity.Should().Be(50);
        item.ReservedQuantity.Should().Be(0);
        item.AvailableQuantity.Should().Be(50);
        item.IsLowStock.Should().BeFalse();
        item.DomainEvents.Should().ContainSingle(e => e is InventoryItemCreatedDomainEvent);
    }

    [Fact]
    public void ReserveStock_WithinAvailable_ShouldIncreaseReservedQuantity()
    {
        var item = InventoryItem.Create(Guid.NewGuid(), 10, 2);
        item.ClearDomainEvents();

        var result = item.ReserveStock(7);

        result.IsSuccess.Should().BeTrue();
        item.ReservedQuantity.Should().Be(7);
        item.AvailableQuantity.Should().Be(3);
        item.DomainEvents.Should().ContainSingle(e => e is StockReservedDomainEvent);
    }

    [Fact]
    public void ReserveStock_ExceedingAvailable_ShouldPreventOverselling()
    {
        var item = InventoryItem.Create(Guid.NewGuid(), 10, 2);

        var result = item.ReserveStock(12);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Inventory.InsufficientStock");
        item.ReservedQuantity.Should().Be(0);
        item.AvailableQuantity.Should().Be(10);
    }

    [Fact]
    public void LowStockDetection_WhenAvailableReachesThreshold_ShouldEmitEvent()
    {
        var item = InventoryItem.Create(Guid.NewGuid(), 10, 5);
        item.ClearDomainEvents();

        item.ReserveStock(6); // Available = 4 (<= threshold 5)

        item.IsLowStock.Should().BeTrue();
        item.DomainEvents.Should().Contain(e => e is LowStockDetectedDomainEvent);
    }
}
