using Joyful.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Joyful.API.Configurations;

public sealed class MessagesConfiguration : IEntityTypeConfiguration<MessageEntity>
{
    void IEntityTypeConfiguration<MessageEntity>.Configure(EntityTypeBuilder<MessageEntity> builder)
    {
        builder
            .HasKey(m => m.Id);

         builder
            .Property(m => m.Id)
            .ValueGeneratedOnAdd();

        builder
            .HasOne(m => m.Sender)
            .WithMany(u => u.Messages)
            .HasForeignKey(m => m.SenderId);

        builder
            .HasOne(m => m.Chat)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ChatId);
    }
}