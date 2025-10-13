namespace Soundmates.Domain.Entities;

public abstract class Reaction
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public DateTime Timestamp { get; private set; } = DateTime.UtcNow;

    public required Guid GiverId { get; set; }
    public required Guid ReceiverId { get; set; }
}
