namespace Soundmates.Domain.Entities;

public class BandMember
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required string Name { get; set; }
    public required int Age { get; set; }
    public required int DisplayOrder { get; set; }

    public Guid BandId { get; set; }
    public Band Band { get; set; } = null!;

    public Guid BandRoleId { get; set; }
    public BandRole BandRole { get; set; } = null!;
}
