using ECommerce.Application.Abstractions;
using ECommerce.Contracts.Inventory;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Primitives;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Services;

public class InventoryService : IInventoryService
{
    private readonly IApplicationDbContext _context;

    public InventoryService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<InventoryItemResponse>> AddInventoryItemAsync(AddInventoryItemRequest request, CancellationToken cancellationToken = default)
    {
        var productExists = await _context.Products.AnyAsync(p => p.Id == request.ProductId, cancellationToken);
        if (!productExists)
        {
            return Result.Failure<InventoryItemResponse>(Error.NotFound("Product.NotFound", "Product was not found."));
        }

        var existingItem = await _context.InventoryItems.FirstOrDefaultAsync(i => i.ProductId == request.ProductId, cancellationToken);
        if (existingItem is not null)
        {
            return Result.Failure<InventoryItemResponse>(Error.Conflict("Inventory.AlreadyExists", "Inventory record already exists for this product."));
        }

        var inventoryItem = InventoryItem.Create(request.ProductId, request.Quantity, request.LowStockThreshold);
        _context.InventoryItems.Add(inventoryItem);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(inventoryItem));
    }

    public async Task<Result<InventoryItemResponse>> AdjustStockAsync(Guid productId, AdjustStockRequest request, CancellationToken cancellationToken = default)
    {
        var item = await _context.InventoryItems.FirstOrDefaultAsync(i => i.ProductId == productId, cancellationToken);
        if (item is null)
        {
            return Result.Failure<InventoryItemResponse>(Error.NotFound("Inventory.NotFound", "Inventory item was not found."));
        }

        var result = item.AdjustStock(request.QuantityChange, request.Reason);
        if (result.IsFailure)
        {
            return Result.Failure<InventoryItemResponse>(result.Error);
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Failure<InventoryItemResponse>(Error.Conflict("Inventory.ConcurrencyConflict", "The inventory item was updated by another transaction. Please retry."));
        }

        return Result.Success(MapToResponse(item));
    }

    public async Task<Result<InventoryItemResponse>> ReserveStockAsync(Guid productId, ReserveStockRequest request, CancellationToken cancellationToken = default)
    {
        var item = await _context.InventoryItems.FirstOrDefaultAsync(i => i.ProductId == productId, cancellationToken);
        if (item is null)
        {
            return Result.Failure<InventoryItemResponse>(Error.NotFound("Inventory.NotFound", "Inventory item was not found."));
        }

        var result = item.ReserveStock(request.Quantity);
        if (result.IsFailure)
        {
            return Result.Failure<InventoryItemResponse>(result.Error);
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Failure<InventoryItemResponse>(Error.Conflict("Inventory.ConcurrencyConflict", "The inventory item was modified concurrently. Please retry reservation."));
        }

        return Result.Success(MapToResponse(item));
    }

    public async Task<Result<InventoryItemResponse>> ReleaseStockAsync(Guid productId, ReleaseStockRequest request, CancellationToken cancellationToken = default)
    {
        var item = await _context.InventoryItems.FirstOrDefaultAsync(i => i.ProductId == productId, cancellationToken);
        if (item is null)
        {
            return Result.Failure<InventoryItemResponse>(Error.NotFound("Inventory.NotFound", "Inventory item was not found."));
        }

        var result = item.ReleaseStock(request.Quantity);
        if (result.IsFailure)
        {
            return Result.Failure<InventoryItemResponse>(result.Error);
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Failure<InventoryItemResponse>(Error.Conflict("Inventory.ConcurrencyConflict", "The inventory item was modified concurrently. Please retry."));
        }

        return Result.Success(MapToResponse(item));
    }

    public async Task<Result<InventoryItemResponse>> GetInventoryByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var item = await _context.InventoryItems.FirstOrDefaultAsync(i => i.ProductId == productId, cancellationToken);
        if (item is null)
        {
            return Result.Failure<InventoryItemResponse>(Error.NotFound("Inventory.NotFound", "Inventory item was not found."));
        }

        return Result.Success(MapToResponse(item));
    }

    public async Task<Result<IReadOnlyCollection<InventoryItemResponse>>> GetLowStockItemsAsync(CancellationToken cancellationToken = default)
    {
        var items = await _context.InventoryItems
            .AsNoTracking()
            .Where(i => (i.StockQuantity - i.ReservedQuantity) <= i.LowStockThreshold)
            .ToListAsync(cancellationToken);

        var responses = items.Select(MapToResponse).ToList();
        return Result.Success<IReadOnlyCollection<InventoryItemResponse>>(responses.AsReadOnly());
    }

    private static InventoryItemResponse MapToResponse(InventoryItem item)
    {
        return new InventoryItemResponse(
            item.Id,
            item.ProductId,
            item.StockQuantity,
            item.ReservedQuantity,
            item.AvailableQuantity,
            item.LowStockThreshold,
            item.IsLowStock,
            item.CreatedOnUtc,
            item.UpdatedOnUtc
        );
    }
}
