using Joyful.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Joyful.API.Configurations;

public sealed class PollConfiguration : IEntityTypeConfiguration<PollEntity>
{
    void IEntityTypeConfiguration<PollEntity>.Configure(EntityTypeBuilder<PollEntity> builder)
    {
        builder.Property(p => p.PollType)
            .HasConversion<string>();

        builder.Property(p => p.PollStatus)
            .HasConversion<string>();

        builder.Property(p => p.OptionsJson)
            .HasColumnType("json");

        builder
            .HasMany<VoteEntity>()
            .WithOne()
            .HasForeignKey(v => v.PollId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}