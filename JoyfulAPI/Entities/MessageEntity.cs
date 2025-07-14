using Joyful.API.Enums;
namespace Joyful.API.Entities;

public class MessageEntity
{
    public int Id { get; set; }
    public string Content { get; set; } = null!;
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public MessageStatus Status { get; set; } = MessageStatus.Sent;

    public UserEntity Sender { get; set; } = null!;
    public UserEntity Receiver { get; set; } = null!;
}
