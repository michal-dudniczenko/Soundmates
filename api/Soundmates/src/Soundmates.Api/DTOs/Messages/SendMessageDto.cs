namespace Soundmates.Api.DTOs.Messages;

public class SendMessageDto
{
    public required int ReceiverId { get; set; }
    public required string Content { get; set; }
}
