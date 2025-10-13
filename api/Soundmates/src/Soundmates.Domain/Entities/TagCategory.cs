using Soundmates.Domain.Enums;

namespace Soundmates.Domain.Entities;

public class TagCategory
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required string Name { get; set; }
    public UserType UserType { get; set; }

    public ICollection<Tag> Tags { get; set; } = [];
}
