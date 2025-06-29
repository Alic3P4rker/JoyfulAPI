using Joyful.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Joyful.API.Configurations;

public sealed class ThemeConfiguration : IEntityTypeConfiguration<ThemeEntity>
{
    void IEntityTypeConfiguration<ThemeEntity>.Configure(EntityTypeBuilder<ThemeEntity> builder)
    {
        builder
            .HasMany<EventEntity>()
            .WithOne()
            .HasForeignKey(e => e.ThemeId);        
    }
}