namespace Joyful.API.Models;

public record MessageCreateDto
(
    int ChatId,
    string Content,
    Guid SenderId
);