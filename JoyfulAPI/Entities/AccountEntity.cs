namespace Joyful.API.Enitites;

public class AccountEntity
{
    public Guid Id { get; set; }
    public string? Otp { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}