namespace Soundmates.Application.ResponseDTOs.Users;

public class SelfUserProfileBandDto : SelfUserProfileDto
{
    public required IList<BandMemberDto> BandMembers { get; set; }
}
