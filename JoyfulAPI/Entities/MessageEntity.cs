using Joyful.API.Enums;
namespace Joyful.API.Entities;

public class MessageEntity
{
    public int Id { get; set; }
    public string Content { get; set; } = null!;
    public Guid SenderId { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public MessageStatus Status { get; set; } = MessageStatus.Sent;
    public ChatEntity Chat { get; set; } = null!;
    public int ChatId { get; set; }

    public UserEntity Sender { get; set; } = null!;
}
