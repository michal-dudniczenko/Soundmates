using System.ComponentModel.DataAnnotations;

namespace Soundmates.Api.RequestDTOs.Users;

public class BandMemberRequestDto
{
    [Required]
    public required string Name { get; set; }

    [Required]
    public required int Age { get; set; }

    [Required]
    public required Guid BandRoleId { get; set; }
}
