using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

internal sealed class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.ToTable("inventory_items");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.ProductId)
            .IsRequired();

        builder.HasIndex(i => i.ProductId)
            .IsUnique();

        builder.Property(i => i.StockQuantity)
            .IsRequired();

        builder.Property(i => i.ReservedQuantity)
            .IsRequired();

        builder.Ignore(i => i.AvailableQuantity);
        builder.Ignore(i => i.IsLowStock);

        builder.Property(i => i.LowStockThreshold)
            .IsRequired();

        builder.Property(i => i.CreatedOnUtc)
            .IsRequired();

        builder.Property(i => i.UpdatedOnUtc);

        builder.Property(i => i.Version)
            .IsConcurrencyToken()
            .IsRequired();

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
