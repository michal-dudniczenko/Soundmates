namespace Soundmates.Application.ResponseDTOs.Users;

public class OtherUserProfileArtistDto : OtherUserProfileDto
{
    public required DateOnly? BirthDate { get; set; }
}
