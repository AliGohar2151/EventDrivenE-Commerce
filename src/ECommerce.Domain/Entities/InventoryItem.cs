using ECommerce.Domain.Events;
using ECommerce.Domain.Primitives;

namespace ECommerce.Domain.Entities;

public class InventoryItem : AggregateRoot<Guid>
{
    private InventoryItem(
        Guid id,
        Guid productId,
        int stockQuantity,
        int lowStockThreshold)
        : base(id)
    {
        ProductId = productId;
        StockQuantity = stockQuantity;
        ReservedQuantity = 0;
        LowStockThreshold = lowStockThreshold;
        CreatedOnUtc = DateTime.UtcNow;
        Version = 1;
    }

    public Guid ProductId { get; private set; }
    public int StockQuantity { get; private set; }
    public int ReservedQuantity { get; private set; }
    public int AvailableQuantity => StockQuantity - ReservedQuantity;
    public int LowStockThreshold { get; private set; }
    public bool IsLowStock => AvailableQuantity <= LowStockThreshold;
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime? UpdatedOnUtc { get; private set; }
    public uint Version { get; private set; }

    public static InventoryItem Create(Guid productId, int initialQuantity, int lowStockThreshold = 5)
    {
        if (initialQuantity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(initialQuantity), "Initial quantity cannot be negative.");
        }

        var item = new InventoryItem(Guid.NewGuid(), productId, initialQuantity, lowStockThreshold);
        item.AddDomainEvent(new InventoryItemCreatedDomainEvent(item.Id, item.ProductId, item.StockQuantity, DateTime.UtcNow));

        if (item.IsLowStock)
        {
            item.AddDomainEvent(new LowStockDetectedDomainEvent(item.Id, item.ProductId, item.AvailableQuantity, item.LowStockThreshold, DateTime.UtcNow));
        }

        return item;
    }

    public Result AdjustStock(int quantityChange, string reason)
    {
        var newQuantity = StockQuantity + quantityChange;
        if (newQuantity - ReservedQuantity < 0)
        {
            return Result.Failure(Error.Validation("Inventory.InsufficientStockForAdjustment", "Stock adjustment would result in negative available quantity."));
        }

        StockQuantity = newQuantity;
        UpdatedOnUtc = DateTime.UtcNow;
        Version++;

        AddDomainEvent(new StockAdjustedDomainEvent(Id, ProductId, quantityChange, StockQuantity, reason, DateTime.UtcNow));

        CheckLowStock();

        return Result.Success();
    }

    public Result ReserveStock(int quantity)
    {
        if (quantity <= 0)
        {
            return Result.Failure(Error.Validation("Inventory.InvalidReservationQuantity", "Reservation quantity must be greater than zero."));
        }

        if (quantity > AvailableQuantity)
        {
            return Result.Failure(Error.Conflict("Inventory.InsufficientStock", $"Cannot reserve {quantity} units. Only {AvailableQuantity} units available."));
        }

        ReservedQuantity += quantity;
        UpdatedOnUtc = DateTime.UtcNow;
        Version++;

        AddDomainEvent(new StockReservedDomainEvent(Id, ProductId, quantity, AvailableQuantity, DateTime.UtcNow));

        CheckLowStock();

        return Result.Success();
    }

    public Result ReleaseStock(int quantity)
    {
        if (quantity <= 0)
        {
            return Result.Failure(Error.Validation("Inventory.InvalidReleaseQuantity", "Release quantity must be greater than zero."));
        }

        if (quantity > ReservedQuantity)
        {
            return Result.Failure(Error.Validation("Inventory.ReleaseExceedsReserved", $"Cannot release {quantity} units. Only {ReservedQuantity} units reserved."));
        }

        ReservedQuantity -= quantity;
        UpdatedOnUtc = DateTime.UtcNow;
        Version++;

        AddDomainEvent(new StockReleasedDomainEvent(Id, ProductId, quantity, AvailableQuantity, DateTime.UtcNow));

        return Result.Success();
    }

    public Result CommitStock(int quantity)
    {
        if (quantity <= 0)
        {
            return Result.Failure(Error.Validation("Inventory.InvalidCommitQuantity", "Commit quantity must be greater than zero."));
        }

        if (quantity > ReservedQuantity)
        {
            return Result.Failure(Error.Validation("Inventory.CommitExceedsReserved", $"Cannot commit {quantity} units. Only {ReservedQuantity} units reserved."));
        }

        ReservedQuantity -= quantity;
        StockQuantity -= quantity;
        UpdatedOnUtc = DateTime.UtcNow;
        Version++;

        CheckLowStock();

        return Result.Success();
    }

    private void CheckLowStock()
    {
        if (IsLowStock)
        {
            AddDomainEvent(new LowStockDetectedDomainEvent(Id, ProductId, AvailableQuantity, LowStockThreshold, DateTime.UtcNow));
        }
    }
}
