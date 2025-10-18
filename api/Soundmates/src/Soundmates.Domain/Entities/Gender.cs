namespace Soundmates.Domain.Entities;

public class Gender
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required string Name { get; set; }
}
