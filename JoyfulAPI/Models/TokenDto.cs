namespace Joyful.API.Models;

public record TokenDto(
    string AccessToken,
    string RefreshToken
);