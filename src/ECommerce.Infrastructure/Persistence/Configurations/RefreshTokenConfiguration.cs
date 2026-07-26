using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

internal sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");

        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.Token)
            .HasMaxLength(500)
            .IsRequired();

        builder.HasIndex(rt => rt.Token)
            .IsUnique();

        builder.Property(rt => rt.UserId)
            .IsRequired();

        builder.Property(rt => rt.ExpiresOnUtc)
            .IsRequired();

        builder.Property(rt => rt.CreatedOnUtc)
            .IsRequired();

        builder.Property(rt => rt.ReplacedByToken)
            .HasMaxLength(500);

        builder.HasOne<User>()
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
