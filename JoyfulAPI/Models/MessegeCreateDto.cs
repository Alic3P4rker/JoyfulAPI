namespace Joyful.API.Models;

public record MessageCreateDto
(
    string Content,
    Guid SenderId,
    Guid ReceiverId
);