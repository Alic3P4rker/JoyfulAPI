using Joyful.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Joyful.API.Configurations;

public sealed class VoteConfiguration : IEntityTypeConfiguration<VoteEntity>
{
    void IEntityTypeConfiguration<VoteEntity>.Configure(EntityTypeBuilder<VoteEntity> builder)
    {
        builder.Property(v => v.ChosenSuggestionsJson)
            .HasColumnType("json");
    }
}