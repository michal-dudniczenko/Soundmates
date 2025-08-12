using System.ComponentModel.DataAnnotations;

namespace Soundmates.Domain.Entities;

public class Message
{
    public int Id { get; set; }

    [MaxLength(4000)]
    [RegularExpression(@"\S+", ErrorMessage = "Message content cannot be empty or whitespace.")]
    public required string Content { get; set; }
    public DateTime Timestamp { get; private set; } = DateTime.UtcNow;
    public required int SenderId { get; set; }
    public required int ReceiverId { get; set; }
}
