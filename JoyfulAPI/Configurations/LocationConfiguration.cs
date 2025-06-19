using Joyful.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Joyful.API.Configurations;

public sealed class LocationConfiguration : IEntityTypeConfiguration<LocationEntity>
{
    void IEntityTypeConfiguration<LocationEntity>.Configure(EntityTypeBuilder<LocationEntity> builder)
    {
        builder.Property(l => l.County)
            .HasConversion<string>();
    }
}