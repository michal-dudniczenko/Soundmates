namespace Soundmates.Domain.Entities;

public class Match
{
    public int Id { get; set; }
    public DateTime Timestamp { get; private set; } = DateTime.UtcNow;
    public required int User1Id { get; set; }
    public required int User2Id { get; set; }
}
