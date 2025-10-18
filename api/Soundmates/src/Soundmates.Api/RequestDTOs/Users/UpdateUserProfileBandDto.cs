using Soundmates.Application.ResponseDTOs.Users;
using System.ComponentModel.DataAnnotations;

namespace Soundmates.Api.RequestDTOs.Users;

public class UpdateUserProfileBandDto : UpdateUserProfileDto
{
    [Required]
    public required IList<BandMemberDto> BandMembers { get; set; }
}
