namespace Joyful.API.Models;

public record OptionResultDto
{
    public string OptionText { get; set; } = String.Empty;
    public int VoteCount { get; set; }
}