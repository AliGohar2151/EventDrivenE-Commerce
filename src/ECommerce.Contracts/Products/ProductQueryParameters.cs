namespace ECommerce.Contracts.Products;

public record ProductQueryParameters(
    string? Search = null,
    Guid? CategoryId = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    string? Status = null,
    string? SortBy = "name",
    string? SortOrder = "asc",
    int Page = 1,
    int PageSize = 10
);
