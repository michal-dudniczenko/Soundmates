using System.ComponentModel.DataAnnotations;

namespace Soundmates.Api.RequestDTOs.Matching;

public class SwipeDto
{
    [Required]
    public required Guid ReceiverId { get; set; }
}
