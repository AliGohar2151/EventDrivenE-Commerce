using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Sku)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(p => p.Sku)
            .IsUnique();

        builder.Property(p => p.Description)
            .HasMaxLength(2000);

        builder.Property(p => p.Price)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.CategoryId)
            .IsRequired();

        builder.Property(p => p.CreatedOnUtc)
            .IsRequired();

        builder.Property(p => p.UpdatedOnUtc);

        builder.OwnsMany(p => p.Variants, v =>
        {
            v.ToTable("product_variants");
            v.WithOwner().HasForeignKey("ProductId");
            v.HasKey("VariantSku");
            v.Property(x => x.VariantSku).HasMaxLength(100);
            v.Property(x => x.Name).HasMaxLength(200).IsRequired();
            v.Property(x => x.PriceModifier).HasPrecision(18, 2);
        });

        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
