namespace Soundmates.Domain.Entities;

public class Match
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public DateTime Timestamp { get; private set; } = DateTime.UtcNow;

    public required Guid User1Id { get; set; }
    public required Guid User2Id { get; set; }
}
