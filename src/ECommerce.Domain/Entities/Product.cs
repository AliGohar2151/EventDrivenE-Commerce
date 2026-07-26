using ECommerce.Domain.Primitives;

namespace ECommerce.Domain.Entities;

public class Product : Entity<Guid>
{
    private Product(Guid id, string name, string sku, decimal price, Guid categoryId)
        : base(id)
    {
        Name = name;
        Sku = sku;
        Price = price;
        CategoryId = categoryId;
        CreatedOnUtc = DateTime.UtcNow;
    }

    public string Name { get; private set; }
    public string Sku { get; private set; }
    public decimal Price { get; private set; }
    public Guid CategoryId { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }

    public static Product Create(string name, string sku, decimal price, Guid categoryId)
    {
        return new Product(Guid.NewGuid(), name, sku, price, categoryId);
    }
}
