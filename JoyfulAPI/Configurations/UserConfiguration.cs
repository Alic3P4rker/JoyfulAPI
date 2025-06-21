using Joyful.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Joyful.API.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    void IEntityTypeConfiguration<UserEntity>.Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder
            .HasOne<AccountEntity>()
            .WithOne()
            .HasForeignKey<AccountEntity>(a => a.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}