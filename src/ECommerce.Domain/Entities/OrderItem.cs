using ECommerce.Domain.Primitives;

namespace ECommerce.Domain.Entities;

public class OrderItem : Entity<Guid>
{
    private OrderItem()
        : base(Guid.Empty)
    {
        ProductName = string.Empty;
        ProductSku = string.Empty;
    }

    private OrderItem(
        Guid id,
        Guid productId,
        string productName,
        string productSku,
        string? variantSku,
        decimal unitPrice,
        int quantity)
        : base(id)
    {
        ProductId = productId;
        ProductName = productName;
        ProductSku = productSku;
        VariantSku = variantSku;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; }
    public string ProductSku { get; private set; }
    public string? VariantSku { get; private set; }
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public decimal TotalPrice => UnitPrice * Quantity;

    public static OrderItem Create(
        Guid productId,
        string productName,
        string productSku,
        string? variantSku,
        decimal unitPrice,
        int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        }

        return new OrderItem(Guid.NewGuid(), productId, productName, productSku, variantSku, unitPrice, quantity);
    }
}
