namespace Joyful.API.Models;

public record VoteDto
{
    public List<string> ChosenSuggestions { get; set; } = null!;
    public DateTimeOffset VotedAt { get; set; }
}