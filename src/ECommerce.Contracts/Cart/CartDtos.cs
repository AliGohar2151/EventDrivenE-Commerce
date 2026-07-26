namespace ECommerce.Contracts.Cart;

public record AddItemToCartRequest(
    Guid ProductId,
    int Quantity,
    string? VariantSku = null
);

public record UpdateCartItemQuantityRequest(
    int Quantity
);

public record CartItemDto(
    Guid ProductId,
    string ProductName,
    string ProductSku,
    string? VariantSku,
    decimal UnitPrice,
    int Quantity,
    decimal TotalPrice
);

public record CartResponse(
    Guid UserId,
    IReadOnlyCollection<CartItemDto> Items,
    int TotalItemCount,
    decimal GrandTotalAmount,
    DateTime UpdatedOnUtc
);
