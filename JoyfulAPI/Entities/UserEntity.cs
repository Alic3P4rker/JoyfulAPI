namespace Joyful.API.Entities;

public class UserEntity
{
    private string _emailAddress = String.Empty;
    private string _passwordHash = String.Empty;

    public Guid Id { get; set; }
    public string EmailAddress
    {
        get => _emailAddress;
        set => _emailAddress = value;
    }
    public string PasswordHash
    {
        get => _passwordHash;
        set => _passwordHash = value;
    }

    public ICollection<ChatParticipantEntity> ChatParticipations { get; set; } = new List<ChatParticipantEntity>();
    public ICollection<UserFriendsEntity> Friends { get; set; } = new List<UserFriendsEntity>();
    public ICollection<MessageEntity> Messages { get; set; } = new List<MessageEntity>();
}