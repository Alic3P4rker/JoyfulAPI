using Joyful.API.Enums;

namespace Joyful.API.Entities;

public class GroupEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string NameNormalised { get; set; }
    public Guid GroupLeaderId { get; set; }
    public Status Status { get; set; }
    public string? AccessCode { get; set; }
    public bool IsPrivate { get; set; }
    public bool IsOrganisation { get; set; }
    public bool RequiresApproval { get; set; }

    public GroupEntity()
    {
        Name = String.Empty;
        NameNormalised = String.Empty;
    }

}