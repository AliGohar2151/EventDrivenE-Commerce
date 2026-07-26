using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Abstractions;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<Permission> Permissions { get; }
    DbSet<Category> Categories { get; }
    DbSet<Product> Products { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<UserRole> UserRoles { get; }
    DbSet<RolePermission> RolePermissions { get; }
    DbSet<InventoryItem> InventoryItems { get; }
    DbSet<Order> Orders { get; }
    DbSet<Payment> Payments { get; }
    DbSet<OutboxMessage> OutboxMessages { get; }
    DbSet<InboxMessage> InboxMessages { get; }
    DbSet<DeadLetterMessage> DeadLetterMessages { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
