using Joyful.API.Enums;

namespace Joyful.API.Models;

public record EventCreateDto()
{
    public Guid groupId { get; set; }
    public Guid createdPlannerId { get; set; }
    public string title { get; set; } = String.Empty;
    public string description { get; set; } = String.Empty;
    public Category category { get; set; }
    public Guid themeId { get; set; }
    public Guid locationId { get; set; }
    public DateTimeOffset startDateTime { get; set; }
    public DateTimeOffset endDateTime { get; set; }
    public EventStatus eventStatus { get; set; }
    public Visibility visibility { get; set; }
}