using ECommerce.Domain.Primitives;

namespace ECommerce.Domain.Entities;

public class CartItem : ValueObject
{
    public CartItem(
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

        ProductId = productId;
        ProductName = productName;
        ProductSku = productSku;
        VariantSku = variantSku;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public Guid ProductId { get; }
    public string ProductName { get; }
    public string ProductSku { get; }
    public string? VariantSku { get; }
    public decimal UnitPrice { get; }
    public int Quantity { get; private set; }
    public decimal TotalPrice => UnitPrice * Quantity;

    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero.", nameof(newQuantity));
        }

        Quantity = newQuantity;
    }

    public override IEnumerable<object?> GetAtomicValues()
    {
        yield return ProductId;
        yield return VariantSku;
        yield return UnitPrice;
        yield return Quantity;
    }
}
