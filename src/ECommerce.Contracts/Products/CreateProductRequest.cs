namespace ECommerce.Contracts.Products;

public record CreateProductRequest(
    string Name,
    string Sku,
    string Description,
    decimal Price,
    Guid CategoryId,
    string Status = "Active",
    List<ProductVariantDto>? Variants = null
);

public record ProductVariantDto(
    string VariantSku,
    string Name,
    decimal PriceModifier
);
