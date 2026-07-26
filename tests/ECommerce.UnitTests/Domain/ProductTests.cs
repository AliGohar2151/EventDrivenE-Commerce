using ECommerce.Domain.Entities;
using ECommerce.Domain.Events;
using FluentAssertions;
using Xunit;

namespace ECommerce.UnitTests.Domain;

public class ProductTests
{
    [Fact]
    public void Create_ShouldRecordProductCreatedDomainEvent()
    {
        var categoryId = Guid.NewGuid();
        var product = Product.Create("Laptop", "SKU-123", "High-performance laptop", 1299.99m, categoryId);

        product.Name.Should().Be("Laptop");
        product.Sku.Should().Be("SKU-123");
        product.Price.Should().Be(1299.99m);
        product.Status.Should().Be(ProductStatus.Active);
        product.DomainEvents.Should().ContainSingle(e => e is ProductCreatedDomainEvent);
    }

    [Fact]
    public void Update_ShouldModifyPropertiesAndRecordEvent()
    {
        var categoryId = Guid.NewGuid();
        var product = Product.Create("Laptop", "SKU-123", "High-performance laptop", 1299.99m, categoryId);
        product.ClearDomainEvents();

        product.Update("Gaming Laptop", "Pro gaming laptop", 1499.99m, categoryId, ProductStatus.Active);

        product.Name.Should().Be("Gaming Laptop");
        product.Price.Should().Be(1499.99m);
        product.UpdatedOnUtc.Should().NotBeNull();
        product.DomainEvents.Should().ContainSingle(e => e is ProductUpdatedDomainEvent);
    }
}
