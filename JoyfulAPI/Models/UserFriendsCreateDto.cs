namespace Joyful.API.Models;

public record UserFriendsCreateDto(
    Guid UserId,
    Guid FriendId
);