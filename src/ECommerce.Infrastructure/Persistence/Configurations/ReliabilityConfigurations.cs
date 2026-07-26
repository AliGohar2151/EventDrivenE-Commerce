using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Type).HasMaxLength(250).IsRequired();
        builder.Property(m => m.Content).IsRequired();
        builder.Property(m => m.OccurredOnUtc).IsRequired();
        builder.Property(m => m.ProcessedOnUtc);
        builder.Property(m => m.Error);
    }
}

internal sealed class InboxMessageConfiguration : IEntityTypeConfiguration<InboxMessage>
{
    public void Configure(EntityTypeBuilder<InboxMessage> builder)
    {
        builder.ToTable("inbox_messages");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.HandlerName).HasMaxLength(250).IsRequired();
        builder.Property(m => m.ProcessedOnUtc).IsRequired();
    }
}

internal sealed class DeadLetterMessageConfiguration : IEntityTypeConfiguration<DeadLetterMessage>
{
    public void Configure(EntityTypeBuilder<DeadLetterMessage> builder)
    {
        builder.ToTable("dead_letter_messages");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.EventId).IsRequired();
        builder.Property(m => m.EventType).HasMaxLength(250).IsRequired();
        builder.Property(m => m.Content).IsRequired();
        builder.Property(m => m.ErrorMessage).HasMaxLength(1000).IsRequired();
        builder.Property(m => m.StackTrace);
        builder.Property(m => m.RetryCount).IsRequired();
        builder.Property(m => m.FailedAtUtc).IsRequired();
    }
}
