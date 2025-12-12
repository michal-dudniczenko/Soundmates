namespace Soundmates.Application.ResponseDTOs.Users;

public class OtherUserProfileBandDto : OtherUserProfileDto
{
    public required IList<BandMemberDto> BandMembers { get; set; }
}
