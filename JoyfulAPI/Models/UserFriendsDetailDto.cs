namespace Joyful.API.Models;

public record UserFriendsDetailsDto()
{
    public Guid UserId { get; init; } = Guid.Empty;
    public Guid FriendId { get; init; } = Guid.Empty;
}