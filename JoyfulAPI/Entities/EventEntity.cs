using Joyful.API.Enums;

namespace Joyful.API.Enitites;

public class EventEntity
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public Guid CreatedPlannerId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Category Category { get; set; }
    public Guid? ThemeId { get; set; }
    public Guid? LocationId { get; set; }
    public int ConfirmedAttendeesCount { get; set; }
    public int DeclinedAttendeesCount { get; set; }
    public int PendingAttendeesCount { get; set; }
    public DateTimeOffset StartDateTime { get; set; }
    public DateTimeOffset EndDateTime { get; set; }
    public EventStatus EventStatus { get; set; }
    public Priority Priority { get; set; }
    public Visibility EventVisibity { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public EventEntity()
    {
        Title = String.Empty;
        Description = String.Empty;
    }

}