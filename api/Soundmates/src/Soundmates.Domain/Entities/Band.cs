namespace Soundmates.Domain.Entities;

public class Band : UserBase
{
    public ICollection<BandMember> Members { get; set; } = [];
}
