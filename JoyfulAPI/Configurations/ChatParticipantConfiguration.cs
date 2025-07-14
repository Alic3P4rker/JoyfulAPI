using Joyful.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Joyful.API.Configurations;

public sealed class ChatParticipantConfiguration : IEntityTypeConfiguration<ChatParticipantEntity>
{
    void IEntityTypeConfiguration<ChatParticipantEntity>.Configure(EntityTypeBuilder<ChatParticipantEntity> builder)
    {
        builder
            .HasKey(cp => new { cp.ChatId, cp.UserId }); 

        builder
            .HasOne(cp => cp.Chat)
            .WithMany(c => c.ChatParticipants)
            .HasForeignKey(cp => cp.ChatId);

        builder
            .HasOne(cp => cp.User)
            .WithMany(u => u.ChatParticipations)
            .HasForeignKey(cp => cp.UserId);
    }
}