namespace Soundmates.Domain.Entities;

public class Match
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public DateTime Timestamp { get; private set; } = DateTime.UtcNow;

    public Guid User1Id { get; set; }
    public User User1 { get; set; } = null!;

    public Guid User2Id { get; set; }
    public User User2 { get; set; } = null!;
}
