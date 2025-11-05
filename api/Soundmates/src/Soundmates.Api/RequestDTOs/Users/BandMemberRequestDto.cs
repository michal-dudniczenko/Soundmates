using System.ComponentModel.DataAnnotations;
using static Soundmates.Domain.Constants.AppConstants;

namespace Soundmates.Api.RequestDTOs.Users;

public class BandMemberRequestDto
{
    [Required]
    [MaxLength(MaxBandMemberNameLength)]
    public required string Name { get; set; }

    [Required]
    [Range(MinBandMemberAge, MaxBandMemberAge)]
    public required int Age { get; set; }

    [Required]
    public required Guid BandRoleId { get; set; }
}
