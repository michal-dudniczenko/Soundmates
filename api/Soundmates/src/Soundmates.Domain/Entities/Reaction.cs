namespace Soundmates.Domain.Entities;

public abstract class Reaction
{
    public int Id { get; set; }
    public DateTime Timestamp { get; private set; } = DateTime.UtcNow;
    public required int GiverId { get; set; }
    public required int ReceiverId { get; set; }
}
