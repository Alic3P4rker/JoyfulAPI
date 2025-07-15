namespace Joyful.API.Entities;

public class ChatParticipantEntity
{
    public int ChatId { get; set; }
    public Guid UserId { get; set; }

    public ChatEntity Chat { get; set; } = null!;
    public UserEntity User { get; set; } = null!;

    public DateTime JoinedAt = DateTime.UtcNow;
}