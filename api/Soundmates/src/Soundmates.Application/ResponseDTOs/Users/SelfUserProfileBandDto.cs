namespace Soundmates.Application.ResponseDTOs.Users;

public class SelfUserProfileBandDto : SelfUserProfileDto
{
    public required IList<SelfUserProfileBandDto> BandMembers { get; set; }
}
