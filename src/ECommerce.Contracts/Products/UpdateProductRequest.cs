namespace ECommerce.Contracts.Products;

public record UpdateProductRequest(
    string Name,
    string Description,
    decimal Price,
    Guid CategoryId,
    string Status,
    List<ProductVariantDto>? Variants = null
);
