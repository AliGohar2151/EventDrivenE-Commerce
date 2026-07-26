namespace ECommerce.Contracts.Orders;

public record ShippingAddressDto(
    string Street,
    string City,
    string State,
    string ZipCode,
    string Country
);

public record OrderItemRequest(
    Guid ProductId,
    int Quantity,
    string? VariantSku = null
);

public record CreateOrderRequest(
    ShippingAddressDto ShippingAddress,
    List<OrderItemRequest> Items
);

public record UpdateOrderStatusRequest(
    string Status,
    string? Reason = null
);

public record OrderItemDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    string ProductSku,
    string? VariantSku,
    decimal UnitPrice,
    int Quantity,
    decimal TotalPrice
);

public record OrderResponse(
    Guid Id,
    string OrderNumber,
    Guid UserId,
    string Status,
    decimal TotalAmount,
    ShippingAddressDto ShippingAddress,
    IReadOnlyCollection<OrderItemDto> Items,
    DateTime CreatedOnUtc,
    DateTime? UpdatedOnUtc
);
