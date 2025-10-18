namespace Soundmates.Domain.Entities;

public class Tag
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required string Name { get; set; }

    public Guid TagCategoryId { get; set; }
    public TagCategory TagCategory { get; set; } = null!;
}
