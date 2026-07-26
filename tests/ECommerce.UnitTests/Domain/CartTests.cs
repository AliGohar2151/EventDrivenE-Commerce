using ECommerce.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace ECommerce.UnitTests.Domain;

public class CartTests
{
    [Fact]
    public void AddOrUpdateItem_ShouldCalculateGrandTotalAndItemCountCorrectly()
    {
        var userId = Guid.NewGuid();
        var cart = Cart.Create(userId);

        var pId1 = Guid.NewGuid();
        var pId2 = Guid.NewGuid();

        cart.AddOrUpdateItem(pId1, "Laptop", "SKU-1", null, 1000m, 2);
        cart.AddOrUpdateItem(pId2, "Mouse", "SKU-2", null, 25m, 3);

        cart.TotalItemCount.Should().Be(5);
        cart.GrandTotalAmount.Should().Be(2075m);
    }

    [Fact]
    public void Clear_ShouldEmptyItemsAndResetTotals()
    {
        var cart = Cart.Create(Guid.NewGuid());
        cart.AddOrUpdateItem(Guid.NewGuid(), "Item", "SKU-X", null, 50m, 2);

        cart.Clear();

        cart.Items.Should().BeEmpty();
        cart.TotalItemCount.Should().Be(0);
        cart.GrandTotalAmount.Should().Be(0m);
    }
}
