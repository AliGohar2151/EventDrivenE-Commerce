using ECommerce.Domain.Events;
using ECommerce.Domain.Primitives;

namespace ECommerce.Domain.Entities;

public class Product : AggregateRoot<Guid>
{
    private readonly List<ProductVariant> _variants = new();

    private Product(
        Guid id,
        string name,
        string sku,
        string description,
        decimal price,
        Guid categoryId,
        ProductStatus status)
        : base(id)
    {
        Name = name;
        Sku = sku;
        Description = description;
        Price = price;
        CategoryId = categoryId;
        Status = status;
        CreatedOnUtc = DateTime.UtcNow;
    }

    public string Name { get; private set; }
    public string Sku { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public Guid CategoryId { get; private set; }
    public ProductStatus Status { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime? UpdatedOnUtc { get; private set; }

    public IReadOnlyCollection<ProductVariant> Variants => _variants.AsReadOnly();

    public static Product Create(
        string name,
        string sku,
        string description,
        decimal price,
        Guid categoryId,
        ProductStatus status = ProductStatus.Active,
        IEnumerable<ProductVariant>? variants = null)
    {
        var product = new Product(Guid.NewGuid(), name, sku, description, price, categoryId, status);

        if (variants is not null)
        {
            product._variants.AddRange(variants);
        }

        product.AddDomainEvent(new ProductCreatedDomainEvent(product.Id, product.Name, product.Sku, product.Price, DateTime.UtcNow));

        return product;
    }

    public void Update(
        string name,
        string description,
        decimal price,
        Guid categoryId,
        ProductStatus status,
        IEnumerable<ProductVariant>? variants = null)
    {
        Name = name;
        Description = description;
        Price = price;
        CategoryId = categoryId;
        Status = status;
        UpdatedOnUtc = DateTime.UtcNow;

        _variants.Clear();
        if (variants is not null)
        {
            _variants.AddRange(variants);
        }

        AddDomainEvent(new ProductUpdatedDomainEvent(Id, Name, Price, Status.ToString(), DateTime.UtcNow));
    }
}
