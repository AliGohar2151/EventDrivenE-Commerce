using ECommerce.Domain.Events;
using ECommerce.Domain.Primitives;

namespace ECommerce.Domain.Entities;

public class Cart : AggregateRoot<Guid>
{
    private readonly List<CartItem> _items = new();

    private Cart(Guid userId)
        : base(userId)
    {
        UserId = userId;
        UpdatedOnUtc = DateTime.UtcNow;
    }

    public Guid UserId { get; private set; }
    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();
    public int TotalItemCount => _items.Sum(i => i.Quantity);
    public decimal GrandTotalAmount => _items.Sum(i => i.TotalPrice);
    public DateTime UpdatedOnUtc { get; private set; }

    public static Cart Create(Guid userId)
    {
        return new Cart(userId);
    }

    public void AddOrUpdateItem(
        Guid productId,
        string productName,
        string productSku,
        string? variantSku,
        decimal unitPrice,
        int quantity)
    {
        var existingItem = _items.FirstOrDefault(i => i.ProductId == productId && i.VariantSku == variantSku);
        if (existingItem is not null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            var newItem = new CartItem(productId, productName, productSku, variantSku, unitPrice, quantity);
            _items.Add(newItem);
        }

        UpdatedOnUtc = DateTime.UtcNow;
        AddDomainEvent(new CartItemAddedDomainEvent(UserId, productId, quantity, unitPrice, DateTime.UtcNow));
    }

    public bool UpdateItemQuantity(Guid productId, string? variantSku, int quantity)
    {
        var existingItem = _items.FirstOrDefault(i => i.ProductId == productId && i.VariantSku == variantSku);
        if (existingItem is null)
        {
            return false;
        }

        existingItem.UpdateQuantity(quantity);
        UpdatedOnUtc = DateTime.UtcNow;
        return true;
    }

    public bool RemoveItem(Guid productId, string? variantSku)
    {
        var existingItem = _items.FirstOrDefault(i => i.ProductId == productId && i.VariantSku == variantSku);
        if (existingItem is null)
        {
            return false;
        }

        _items.Remove(existingItem);
        UpdatedOnUtc = DateTime.UtcNow;
        return true;
    }

    public void Clear()
    {
        _items.Clear();
        UpdatedOnUtc = DateTime.UtcNow;
        AddDomainEvent(new CartClearedDomainEvent(UserId, DateTime.UtcNow));
    }
}
