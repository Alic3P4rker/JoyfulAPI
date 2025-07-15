namespace Joyful.API.Models;

public record ChatCreateDto(
    string Name,
    Guid creatorId,
    List<Guid> ParticipantIds
);