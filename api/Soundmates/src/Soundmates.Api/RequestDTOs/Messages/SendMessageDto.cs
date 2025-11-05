using System.ComponentModel.DataAnnotations;
using static Soundmates.Domain.Constants.AppConstants;

namespace Soundmates.Api.RequestDTOs.Messages;

public class SendMessageDto
{
    [Required]
    public required Guid ReceiverId { get; set; }

    [Required]
    [MaxLength(MaxMessageContentLength)]
    public required string Content { get; set; }
}
