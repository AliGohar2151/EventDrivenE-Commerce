using ECommerce.Contracts.Inventory;
using ECommerce.Domain.Primitives;

namespace ECommerce.Application.Abstractions;

public interface IInventoryService
{
    Task<Result<InventoryItemResponse>> AddInventoryItemAsync(AddInventoryItemRequest request, CancellationToken cancellationToken = default);
    Task<Result<InventoryItemResponse>> AdjustStockAsync(Guid productId, AdjustStockRequest request, CancellationToken cancellationToken = default);
    Task<Result<InventoryItemResponse>> ReserveStockAsync(Guid productId, ReserveStockRequest request, CancellationToken cancellationToken = default);
    Task<Result<InventoryItemResponse>> ReleaseStockAsync(Guid productId, ReleaseStockRequest request, CancellationToken cancellationToken = default);
    Task<Result<InventoryItemResponse>> GetInventoryByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<InventoryItemResponse>>> GetLowStockItemsAsync(CancellationToken cancellationToken = default);
}
