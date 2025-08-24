namespace Soundmates.Api.DTOs.Messages;

public class MessageDto
{
    public required string Content { get; set; }
    public required DateTime Timestamp { get; set; }
    public required int SenderId { get; set; }
    public required int ReceiverId { get; set; }
}
