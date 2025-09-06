namespace Soundmates.Application.Messages.Common;

public class MessageDto
{
    public required string Content { get; init; }
    public required DateTime Timestamp { get; init; }
    public required int SenderId { get; init; }
    public required int ReceiverId { get; init; }
}
