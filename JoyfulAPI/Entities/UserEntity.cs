namespace Joyful.API.Entities;

public class UserEntity
{
    private string _emailAddress = String.Empty;
    private string _passwordHash = String.Empty;

    public Guid id { get; set; }
    public string EmailAddress
    {
        get => _emailAddress;
        set => _emailAddress = value;
    }
    public string Password
    {
        get => _passwordHash;
        set => _passwordHash = value;
    }
}