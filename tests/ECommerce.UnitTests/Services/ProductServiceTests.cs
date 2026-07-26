using ECommerce.Application.Services;
using ECommerce.Contracts.Products;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ECommerce.UnitTests.Services;

public class ProductServiceTests
{
    private readonly ApplicationDbContext _context;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _service = new ProductService(_context);
    }

    [Fact]
    public async Task GetProductsAsync_WithSearchAndPagination_ShouldReturnFilteredResults()
    {
        var category = Category.Create("Electronics", "Tech products");
        _context.Categories.Add(category);

        _context.Products.Add(Product.Create("iPhone 15", "IP15", "Apple Smartphone", 999m, category.Id));
        _context.Products.Add(Product.Create("Samsung Galaxy", "SG24", "Android Phone", 899m, category.Id));
        _context.Products.Add(Product.Create("Dell XPS", "DX15", "Laptop", 1499m, category.Id));
        await _context.SaveChangesAsync();

        var query = new ProductQueryParameters(Search: "Phone", Page: 1, PageSize: 10);
        var result = await _service.GetProductsAsync(query);

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalCount.Should().Be(2);
        result.Value.Items.Should().OnlyContain(p => p.Name.Contains("Phone") || p.Description.Contains("Phone"));
    }

    [Fact]
    public async Task CreateProduct_DuplicateSku_ShouldReturnConflict()
    {
        var category = Category.Create("Electronics", "Tech products");
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        var request = new CreateProductRequest("Headphones", "HP-001", "Wireless headphones", 199m, category.Id);
        await _service.CreateProductAsync(request);

        var duplicateResult = await _service.CreateProductAsync(request);

        duplicateResult.IsFailure.Should().BeTrue();
        duplicateResult.Error.Code.Should().Be("Product.DuplicateSku");
    }
}
