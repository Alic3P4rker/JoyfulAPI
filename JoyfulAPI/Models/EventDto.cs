using Joyful.API.Enums;

namespace Joyful.API.Models;

public record EventDto(
    Guid groupId,
    Guid createdPlannerId,
    string title,
    string description,
    Category category,
    Guid themeId,
    Guid locationId,
    int confirmedAttendeesCount,
    int declinedAttendeesCount,
    int pendingAttendeesCount,
    DateTimeOffset startDateTime,
    DateTimeOffset endDateTime,
    EventStatus eventStatus,
    Priority priority,
    Visibility visibility,
    DateTimeOffset createdAt,
    DateTimeOffset updatedAt
);