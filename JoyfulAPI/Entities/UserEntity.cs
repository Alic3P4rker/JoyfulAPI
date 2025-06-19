namespace Joyful.API.Enitites;

public class UserEntity
{
    private string _emailAddress = String.Empty;
    private string _passwordHash = String.Empty;
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