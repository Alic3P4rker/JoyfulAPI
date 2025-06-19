using Joyful.API.Enums;

namespace Joyful.API.Models;

public record PollDto(
    Guid eventId,
    PollType pollType,
    string question,
    string optionsJson,
    PollStatus pollStatus,
    Guid createdbyPlannerId,
    DateTimeOffset createdAt,
    DateTimeOffset votingDeadline,
    bool allowAnonymousVoting,
    bool showResultsLive,
    DateTimeOffset closingAt
);