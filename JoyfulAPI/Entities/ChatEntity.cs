using Joyful.API.Enums;
namespace Joyful.API.Entities;

public class ChatEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid CreatedById { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public UserEntity CreatedBy { get; set; } = null!;
    public ICollection<ChatParticipantEntity> ChatParticipants { get; set; } = new List<ChatParticipantEntity>();
    public ICollection<MessageEntity> Messages { get; set; } = new List<MessageEntity>();
}