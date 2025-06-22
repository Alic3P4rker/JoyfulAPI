using Joyful.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Joyful.API.Configurations;

public class PlannerGroupConfiguration : IEntityTypeConfiguration<PlannerGroupEntity>
{
    void IEntityTypeConfiguration<PlannerGroupEntity>.Configure(EntityTypeBuilder<PlannerGroupEntity> builder)
    {
        builder
            .HasKey(p => new { p.PlannerId, p.GroupId });
    }
}