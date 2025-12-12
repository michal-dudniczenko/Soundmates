namespace Soundmates.Domain.Entities;

public class TagCategory
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required string Name { get; set; }
    public required bool IsForBand { get; set; }

    public ICollection<Tag> Tags { get; set; } = [];
}
