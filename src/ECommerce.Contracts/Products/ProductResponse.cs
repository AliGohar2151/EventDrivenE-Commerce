namespace ECommerce.Contracts.Products;

public record ProductResponse(
    Guid Id,
    string Name,
    string Sku,
    string Description,
    decimal Price,
    Guid CategoryId,
    string CategoryName,
    string Status,
    IReadOnlyCollection<ProductVariantDto> Variants,
    DateTime CreatedOnUtc,
    DateTime? UpdatedOnUtc
);
