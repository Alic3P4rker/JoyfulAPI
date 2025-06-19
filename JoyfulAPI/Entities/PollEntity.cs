using Joyful.API.Enums;

namespace Joyful.API.Entities;

public class PollEntity
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public PollType PollType { get; set; }
    public string Question { get; set; }
    public string OptionsJson { get; set; }
    public PollStatus PollStatus { get; set; }
    public Guid CreatedByPlannerId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? VotingDeadline { get; set; }
    public bool AllowAnonymousVoting { get; set; }
    public bool ShowResultsLive { get; set; }
    public DateTimeOffset? ClosingAt { get; set; }    

    public PollEntity()
    {
        Question = String.Empty;
        OptionsJson = String.Empty;
    }
}