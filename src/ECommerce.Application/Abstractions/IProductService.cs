using ECommerce.Contracts.Products;
using ECommerce.Domain.Primitives;

namespace ECommerce.Application.Abstractions;

public interface IProductService
{
    Task<Result<ProductResponse>> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken = default);
    Task<Result<ProductResponse>> UpdateProductAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteProductAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<ProductResponse>> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<PagedListResponse<ProductResponse>>> GetProductsAsync(ProductQueryParameters parameters, CancellationToken cancellationToken = default);
}
