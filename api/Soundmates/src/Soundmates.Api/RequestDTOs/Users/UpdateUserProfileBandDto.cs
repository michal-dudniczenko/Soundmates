using System.ComponentModel.DataAnnotations;

namespace Soundmates.Api.RequestDTOs.Users;

public class UpdateUserProfileBandDto : UpdateUserProfileDto
{
    [Required]
    public required IList<BandMemberRequestDto> BandMembers { get; set; }
}
