using ECommerce.Application.Abstractions;
using ECommerce.Contracts.Products;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Primitives;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IApplicationDbContext _context;

    public CategoryService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CategoryResponse>> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        if (await _context.Categories.AnyAsync(c => c.Name == request.Name, cancellationToken))
        {
            return Result.Failure<CategoryResponse>(Error.Conflict("Category.DuplicateName", $"Category '{request.Name}' already exists."));
        }

        var category = Category.Create(request.Name, request.Description);
        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(new CategoryResponse(category.Id, category.Name, category.Description));
    }

    public async Task<Result<CategoryResponse>> UpdateCategoryAsync(Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories.FindAsync(new object[] { id }, cancellationToken);
        if (category is null)
        {
            return Result.Failure<CategoryResponse>(Error.NotFound("Category.NotFound", "Category was not found."));
        }

        category.Update(request.Name, request.Description);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(new CategoryResponse(category.Id, category.Name, category.Description));
    }

    public async Task<Result> DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories.FindAsync(new object[] { id }, cancellationToken);
        if (category is null)
        {
            return Result.Failure(Error.NotFound("Category.NotFound", "Category was not found."));
        }

        if (await _context.Products.AnyAsync(p => p.CategoryId == id, cancellationToken))
        {
            return Result.Failure(Error.Conflict("Category.HasAssociatedProducts", "Cannot delete category that contains products."));
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<CategoryResponse>> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories.FindAsync(new object[] { id }, cancellationToken);
        if (category is null)
        {
            return Result.Failure<CategoryResponse>(Error.NotFound("Category.NotFound", "Category was not found."));
        }

        return Result.Success(new CategoryResponse(category.Id, category.Name, category.Description));
    }

    public async Task<Result<IReadOnlyCollection<CategoryResponse>>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _context.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CategoryResponse(c.Id, c.Name, c.Description))
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyCollection<CategoryResponse>>(categories.AsReadOnly());
    }
}
