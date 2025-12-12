namespace Soundmates.Domain.Entities;

public abstract class Reaction
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public DateTime Timestamp { get; private set; } = DateTime.UtcNow;

    public Guid GiverId { get; set; }
    public User Giver { get; set; } = null!;

    public Guid ReceiverId { get; set; }
    public User Receiver { get; set; } = null!;
}
