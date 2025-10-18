using System.ComponentModel.DataAnnotations;

namespace Soundmates.Api.RequestDTOs.Messages;

public class SendMessageDto
{
    public required Guid ReceiverId { get; set; }

    [Required]
    [MaxLength(4000)]
    public required string Content { get; set; }
}
