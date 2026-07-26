using ECommerce.Domain.Entities;
using ECommerce.Domain.Events;
using FluentAssertions;
using Xunit;

namespace ECommerce.UnitTests.Domain;

public class CartTests
{
    [Fact]
    public void AddOrUpdateItem_NewItem_ShouldAddToCartAndCalculateTotals()
    {
        var userId = Guid.NewGuid();
        var cart = Cart.Create(userId);

        var productId = Guid.NewGuid();
        cart.AddOrUpdateItem(productId, "Keyboard", "KB-01", null, 49.99m, 2);

        cart.TotalItemCount.Should().Be(2);
        cart.GrandTotalAmount.Should().Be(99.98m);
        cart.Items.Should().ContainSingle();
        cart.DomainEvents.Should().ContainSingle(e => e is CartItemAddedDomainEvent);
    }

    [Fact]
    public void AddOrUpdateItem_ExistingItem_ShouldUpdateQuantityAndTotals()
    {
        var userId = Guid.NewGuid();
        var cart = Cart.Create(userId);
        var productId = Guid.NewGuid();

        cart.AddOrUpdateItem(productId, "Keyboard", "KB-01", null, 49.99m, 2);
        cart.AddOrUpdateItem(productId, "Keyboard", "KB-01", null, 49.99m, 3);

        cart.TotalItemCount.Should().Be(5);
        cart.GrandTotalAmount.Should().Be(249.95m);
    }

    [Fact]
    public void Clear_ShouldRemoveAllItemsAndEmitEvent()
    {
        var userId = Guid.NewGuid();
        var cart = Cart.Create(userId);
        cart.AddOrUpdateItem(Guid.NewGuid(), "Keyboard", "KB-01", null, 49.99m, 2);
        cart.ClearDomainEvents();

        cart.Clear();

        cart.Items.Should().BeEmpty();
        cart.TotalItemCount.Should().Be(0);
        cart.GrandTotalAmount.Should().Be(0);
        cart.DomainEvents.Should().ContainSingle(e => e is CartClearedDomainEvent);
    }
}
