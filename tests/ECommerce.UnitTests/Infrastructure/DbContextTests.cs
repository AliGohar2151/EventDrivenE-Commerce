using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ECommerce.UnitTests.Infrastructure;

public class DbContextTests
{
    [Fact]
    public void ModelBuilder_ShouldConfigureAllEntities()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);
        var model = context.Model;

        model.FindEntityType(typeof(User)).Should().NotBeNull();
        model.FindEntityType(typeof(Role)).Should().NotBeNull();
        model.FindEntityType(typeof(Permission)).Should().NotBeNull();
        model.FindEntityType(typeof(Category)).Should().NotBeNull();
        model.FindEntityType(typeof(Product)).Should().NotBeNull();
    }

    [Fact]
    public async Task DbContext_ShouldAddAndSaveProductAndCategory()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);

        var category = Category.Create("Electronics", "Gadgets and devices");
        var product = Product.Create("Smartphone", "SM-001", "High-end smartphone", 699.99m, category.Id);

        context.Categories.Add(category);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var savedProduct = await context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);

        savedProduct.Should().NotBeNull();
        savedProduct!.Name.Should().Be("Smartphone");
        savedProduct.Price.Should().Be(699.99m);
        savedProduct.CategoryId.Should().Be(category.Id);
    }
}
