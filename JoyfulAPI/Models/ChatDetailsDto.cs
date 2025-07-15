namespace Joyful.API.Models;

public record ChatDetailsDto()
{
    public int Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public Guid CreatedById { get; set; } = Guid.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public UserDetailsDto CreatedBy { get; set; } = new UserDetailsDto();
    public IEnumerable<UserFriendsDetailsDto> Participants { get; set; } = new List<UserFriendsDetailsDto>();
    public IEnumerable<MessageDetailsDto> Messages { get; set; } = new List<MessageDetailsDto>();
}