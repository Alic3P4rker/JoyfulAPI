namespace Joyful.API.Entities;

public class VoteEntity
{
    public Guid Id { get; set; }
    public Guid PollId { get; set; }
    public Guid VoterId { get; set; }
    public string ChosenSuggestionsJson { get; set; }
    public DateTimeOffset VotedAt { get; set; }

    public VoteEntity()
    {
        ChosenSuggestionsJson = String.Empty;
    }
}