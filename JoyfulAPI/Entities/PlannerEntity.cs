using Joyful.API.Enums;

namespace Joyful.API.Entities;

public class PlannerEntity
{
    //Planner Information 
    public Guid Id { get; set; }
    public string? DisplayName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Gender Gender { get; set; }
    public DateOnly Dob { get; set; }
    public string EmailAddress { get; set; }
    public string? TelephoneNumber { get; set; }

    //Account Information
    public Status Status { get; set; }
    public Role Role { get; set; }

    public PlannerEntity()
    {
        FirstName = String.Empty;
        LastName = String.Empty;
        EmailAddress = String.Empty;
    }
}