﻿namespace Soundmates.Api.DTOs.Messages;

public class MessageDto
{
    public required string Content { get; set; }
    public required DateTime Timestamp { get; set; }
    public required Guid SenderId { get; set; }
    public required Guid ReceiverId { get; set; }
}
