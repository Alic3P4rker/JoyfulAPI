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
    }
}