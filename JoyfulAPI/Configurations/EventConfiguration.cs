using Joyful.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Joyful.API.Configurations;

public sealed class EventConfiguration : IEntityTypeConfiguration<EventEntity>
{
    void IEntityTypeConfiguration<EventEntity>.Configure(EntityTypeBuilder<EventEntity> builder)
    {
        builder.Property(p => p.Category)
            .HasConversion<string>();

        builder.Property(p => p.EventStatus)
            .HasConversion<string>();

        builder.Property(p => p.Priority)
            .HasConversion<string>();

        builder.Property(p => p.EventVisibity)
            .HasConversion<string>();

        builder
            .HasMany<PollEntity>()
            .WithOne()
            .HasForeignKey(p => p.EventId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}