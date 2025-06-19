using Joyful.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Joyful.API.Configurations;

public sealed class GroupConfiguration : IEntityTypeConfiguration<GroupEntity>
{
    void IEntityTypeConfiguration<GroupEntity>.Configure(EntityTypeBuilder<GroupEntity> builder)
    {
        builder.Property(g => g.Status)
            .HasConversion<string>();
    }
}