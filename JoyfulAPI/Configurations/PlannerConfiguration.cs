using Joyful.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Joyful.API.Configurations;

public sealed class PlannerConfiguration : IEntityTypeConfiguration<PlannerEntity>
{
    void IEntityTypeConfiguration<PlannerEntity>.Configure(EntityTypeBuilder<PlannerEntity> builder)
    {
        builder.Property(p => p.Gender)
            .HasConversion<string>();

        builder.Property(p => p.Role)
            .HasConversion<string>();

        builder.Property(p => p.Status)
            .HasConversion<string>();

        builder
            .HasMany<PlannerGroupEntity>()
            .WithOne()
            .HasForeignKey(p => p.PlannerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}