namespace Joyful.API.Models;

public record ChatCreateDto(
    string Name,
    List<Guid> ParticipantIds
);