using System.ComponentModel.DataAnnotations;

namespace Soundmates.Api.RequestDTOs.Users;

public class BandMemberRequestDto
{
    [Required]
    [MaxLength(50)]
    public required string Name { get; set; }

    [Required]
    [Range(0, 100)]
    public required int Age { get; set; }

    [Required]
    public required Guid BandRoleId { get; set; }
}
