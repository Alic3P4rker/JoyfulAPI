namespace Joyful.API.Models;

public record UserCreateDto
{
    public string EmailAddress { get; init; } = String.Empty;
    public string Password { get; init; } = String.Empty;
}