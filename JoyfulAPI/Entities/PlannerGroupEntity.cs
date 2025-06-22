namespace Joyful.API.Entities;

public class PlannerGroupEntity
{
    public Guid PlannerId { get; set; }
    public Guid GroupId { get; set; }
    public GroupEntity Group { get; set; }
}