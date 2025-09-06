using System.ComponentModel.DataAnnotations;

namespace Soundmates.Domain.Entities;

public class Message
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    [MaxLength(4000)]
    public required string Content { get; set; }
    public DateTime Timestamp { get; private set; } = DateTime.UtcNow;
    public required Guid SenderId { get; set; }
    public required Guid ReceiverId { get; set; }
}
