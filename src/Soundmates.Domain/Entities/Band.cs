namespace Soundmates.Domain.Entities;

public class Band
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public ICollection<BandMember> Members { get; } = [];

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
