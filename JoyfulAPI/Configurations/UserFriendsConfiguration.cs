using Joyful.API.Models;
using Joyful.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Joyful.API.Configurations;

public sealed class UserFriendsEntityConfiguration : IEntityTypeConfiguration<UserFriendsEntity>
{
    void IEntityTypeConfiguration<UserFriendsEntity>.Configure(EntityTypeBuilder<UserFriendsEntity> builder)
    {
        builder.HasKey(uf => new { uf.UserId, uf.FriendId });

        builder
            .HasOne(uf => uf.User)
            .WithMany(u => u.Friends)
            .HasForeignKey(uf => uf.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(uf => uf.Friend)
            .WithMany()
            .HasForeignKey(uf => uf.FriendId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}