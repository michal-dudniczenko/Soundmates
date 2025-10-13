namespace Soundmates.Domain.Entities;

public class BandMember
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required string Name { get; set; }
    public int Age { get; set; }

    public Guid BandId { get; set; }
    public Guid BandRoleId { get; set; }
}
