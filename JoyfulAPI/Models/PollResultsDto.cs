namespace Joyful.API.Models;

public record PollResultsDto
{
    public Guid PollId { get; set; }
    public string Question { get; set; } = String.Empty;
    public int TotalVotes { get; set; }
    public List<OptionResultDto> OptionResults { get; set; } = new List<OptionResultDto>();

}
