namespace Joyful.API.Entities;

public class ThemeEntity
{
    public Guid Id { get; set; }
    public Guid PlannerId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public ThemeEntity()
    {
        Name = String.Empty;
        Description = String.Empty;
    }
}