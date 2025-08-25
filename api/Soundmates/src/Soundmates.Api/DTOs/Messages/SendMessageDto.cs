using System.ComponentModel.DataAnnotations;

namespace Soundmates.Api.DTOs.Messages;

public class SendMessageDto
{
    public required int ReceiverId { get; set; }

    [Required]
    [MaxLength(4000)]
    public required string Content { get; set; }
}
