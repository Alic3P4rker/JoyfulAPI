namespace Joyful.API.Entities;

public class UserFriendsEntity
{
    public Guid UserId { get; set; }
    public Guid FriendId { get; set; }

    public UserEntity User { get; set; } = null!;
    public UserEntity Friend { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}