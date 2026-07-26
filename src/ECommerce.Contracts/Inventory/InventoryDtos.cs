namespace ECommerce.Contracts.Inventory;

public record AddInventoryItemRequest(
    Guid ProductId,
    int Quantity,
    int LowStockThreshold = 5
);

public record AdjustStockRequest(
    int QuantityChange,
    string Reason
);

public record ReserveStockRequest(
    int Quantity
);

public record ReleaseStockRequest(
    int Quantity
);

public record InventoryItemResponse(
    Guid Id,
    Guid ProductId,
    int StockQuantity,
    int ReservedQuantity,
    int AvailableQuantity,
    int LowStockThreshold,
    bool IsLowStock,
    DateTime CreatedOnUtc,
    DateTime? UpdatedOnUtc
);
