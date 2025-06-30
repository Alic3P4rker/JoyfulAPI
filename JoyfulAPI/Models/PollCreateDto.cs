using Joyful.API.Enums;

namespace Joyful.API.Models;

public record PollCreateDto
{
    public Guid EventId { get; set; }
    public PollType PollType { get; set; }
    public string Question { get; set; } = String.Empty;
    public string OptionsJson { get; set; } = String.Empty;
    public PollStatus PollStatus { get; set; }
    public Guid CreatedByPlannerId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? VotingDeadline { get; set; }
    public bool AllowAnonymousVoting { get; set; }
    public bool ShowResultsLive { get; set; }
    public DateTimeOffset? ClosingAt { get; set; }  
}