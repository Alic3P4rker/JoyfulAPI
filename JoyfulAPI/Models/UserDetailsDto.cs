namespace Joyful.API.Models;

public record UserDetailsDto
{
    public Guid Id { get; init; }
    public string EmailAddress { get; init; } = String.Empty;
}