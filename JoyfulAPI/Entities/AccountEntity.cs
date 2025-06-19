namespace Joyful.API.Entities;

public class AccountEntity
{
    public Guid Id { get; set; }
    public string? Otp { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsVerified { get; set; }
}