namespace Joyful.API.Models;

public record VoteDto (
    Guid pollId,
    Guid voterId,
    string chosenSuggestionsJson,
    DateTimeOffset votedAt
);