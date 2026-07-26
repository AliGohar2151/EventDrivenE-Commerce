namespace ECommerce.Contracts.Products;

public record PagedListResponse<T>(
    IReadOnlyCollection<T> Items,
    int Page,
    int PageSize,
    long TotalCount,
    int TotalPages,
    bool HasNextPage,
    bool HasPreviousPage
);
