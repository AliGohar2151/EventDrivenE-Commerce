using ECommerce.Contracts.Products;
using ECommerce.Domain.Primitives;

namespace ECommerce.Application.Abstractions;

public interface ICategoryService
{
    Task<Result<CategoryResponse>> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default);
    Task<Result<CategoryResponse>> UpdateCategoryAsync(Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<CategoryResponse>> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<CategoryResponse>>> GetCategoriesAsync(CancellationToken cancellationToken = default);
}
