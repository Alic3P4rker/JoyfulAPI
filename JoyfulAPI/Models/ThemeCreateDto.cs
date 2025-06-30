namespace Joyful.API.Models;

public record ThemeCreateDto(
    Guid plannerId,
    string name,
    string description
);