using Joyful.API.Enums;

namespace Joyful.API.Enitites;

public class LocationEntity
{
    public Guid Id { get; set; }
    public Guid PlannerId { get; set; }
    public string Address1 { get; set; }
    public string? Address2 { get; set; }
    public County County { get; set; }
    public string City { get; set; }
    public string PostCode { get; set; }

    public LocationEntity()
    {
        Address1 = String.Empty;
        City = String.Empty;
        PostCode = String.Empty;
    }
}