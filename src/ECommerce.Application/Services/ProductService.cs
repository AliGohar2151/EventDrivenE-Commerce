using ECommerce.Application.Abstractions;
using ECommerce.Contracts.Products;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Primitives;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Services;

public class ProductService : IProductService
{
    private readonly IApplicationDbContext _context;

    public ProductService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ProductResponse>> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        if (await _context.Products.AnyAsync(p => p.Sku == request.Sku, cancellationToken))
        {
            return Result.Failure<ProductResponse>(Error.Conflict("Product.DuplicateSku", $"Product with SKU '{request.Sku}' already exists."));
        }

        var category = await _context.Categories.FindAsync(new object[] { request.CategoryId }, cancellationToken);
        if (category is null)
        {
            return Result.Failure<ProductResponse>(Error.NotFound("Category.NotFound", "Specified category was not found."));
        }

        if (!Enum.TryParse<ProductStatus>(request.Status, true, out var status))
        {
            status = ProductStatus.Active;
        }

        var variants = request.Variants?.Select(v => new ProductVariant(v.VariantSku, v.Name, v.PriceModifier));
        var product = Product.Create(request.Name, request.Sku, request.Description, request.Price, category.Id, status, variants);

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(product, category.Name));
    }

    public async Task<Result<ProductResponse>> UpdateProductAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (product is null)
        {
            return Result.Failure<ProductResponse>(Error.NotFound("Product.NotFound", "Product was not found."));
        }

        var category = await _context.Categories.FindAsync(new object[] { request.CategoryId }, cancellationToken);
        if (category is null)
        {
            return Result.Failure<ProductResponse>(Error.NotFound("Category.NotFound", "Specified category was not found."));
        }

        if (!Enum.TryParse<ProductStatus>(request.Status, true, out var status))
        {
            status = ProductStatus.Active;
        }

        var variants = request.Variants?.Select(v => new ProductVariant(v.VariantSku, v.Name, v.PriceModifier));
        product.Update(request.Name, request.Description, request.Price, category.Id, status, variants);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToResponse(product, category.Name));
    }

    public async Task<Result> DeleteProductAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (product is null)
        {
            return Result.Failure(Error.NotFound("Product.NotFound", "Product was not found."));
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<ProductResponse>> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (product is null)
        {
            return Result.Failure<ProductResponse>(Error.NotFound("Product.NotFound", "Product was not found."));
        }

        var categoryName = (await _context.Categories.FindAsync(new object[] { product.CategoryId }, cancellationToken))?.Name ?? "Unassigned";

        return Result.Success(MapToResponse(product, categoryName));
    }

    public async Task<Result<PagedListResponse<ProductResponse>>> GetProductsAsync(ProductQueryParameters parameters, CancellationToken cancellationToken = default)
    {
        IQueryable<Product> query = _context.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search.Trim().ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(search) || p.Description.ToLower().Contains(search) || p.Sku.ToLower().Contains(search));
        }

        if (parameters.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == parameters.CategoryId.Value);
        }

        if (parameters.MinPrice.HasValue)
        {
            query = query.Where(p => p.Price >= parameters.MinPrice.Value);
        }

        if (parameters.MaxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= parameters.MaxPrice.Value);
        }

        if (!string.IsNullOrWhiteSpace(parameters.Status) && Enum.TryParse<ProductStatus>(parameters.Status, true, out var status))
        {
            query = query.Where(p => p.Status == status);
        }

        var isDescending = string.Equals(parameters.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);

        query = (parameters.SortBy?.ToLower()) switch
        {
            "price" => isDescending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
            "sku" => isDescending ? query.OrderByDescending(p => p.Sku) : query.OrderBy(p => p.Sku),
            "created" => isDescending ? query.OrderByDescending(p => p.CreatedOnUtc) : query.OrderBy(p => p.CreatedOnUtc),
            _ => isDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name)
        };

        var totalCount = await query.LongCountAsync(cancellationToken);
        var page = parameters.Page < 1 ? 1 : parameters.Page;
        var pageSize = parameters.PageSize < 1 ? 10 : (parameters.PageSize > 100 ? 100 : parameters.PageSize);

        var products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var categoryIds = products.Select(p => p.CategoryId).Distinct();
        var categoryMap = await _context.Categories
            .Where(c => categoryIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c.Name, cancellationToken);

        var productResponses = products
            .Select(p => MapToResponse(p, categoryMap.TryGetValue(p.CategoryId, out var name) ? name : "Unassigned"))
            .ToList();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var pagedResponse = new PagedListResponse<ProductResponse>(
            productResponses,
            page,
            pageSize,
            totalCount,
            totalPages,
            page < totalPages,
            page > 1
        );

        return Result.Success(pagedResponse);
    }

    private static ProductResponse MapToResponse(Product product, string categoryName)
    {
        var variants = product.Variants
            .Select(v => new ProductVariantDto(v.VariantSku, v.Name, v.PriceModifier))
            .ToList();

        return new ProductResponse(
            product.Id,
            product.Name,
            product.Sku,
            product.Description,
            product.Price,
            product.CategoryId,
            categoryName,
            product.Status.ToString(),
            variants,
            product.CreatedOnUtc,
            product.UpdatedOnUtc
        );
    }
}
