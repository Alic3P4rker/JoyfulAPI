namespace Joyful.API.Models;

public record LoginDto
{
    public string EmailAddress { get; init; } = String.Empty;
    public string Password { get; init; } = String.Empty;
}