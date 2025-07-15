using Joyful.API.Enums;

namespace Joyful.API.Models;

public record EventDetailsDto()
{
    public Guid groupId { get; set; }
    public Guid createdPlannerId { get; set; }
    public string title { get; set; } = String.Empty;
    public string description { get; set; } = String.Empty;
    public Category category { get; set; }
    public Guid themeId { get; set; }
    public Guid locationId { get; set; }
    public int confirmedAttendeesCount { get; set; }
    public int declinedAttendeesCount { get; set; }
    public int pendingAttendeesCount { get; set; }
    public DateTimeOffset startDateTime { get; set; }
    public DateTimeOffset endDateTime { get; set; }
    public EventStatus eventStatus { get; set; }
    public Priority priority { get; set; }
    public Visibility visibility { get; set; }
    public DateTimeOffset createdAt { get; set; }
    public DateTimeOffset updatedAt { get; set; }
}