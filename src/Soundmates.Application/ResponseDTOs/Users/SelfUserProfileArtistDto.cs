namespace Soundmates.Application.ResponseDTOs.Users;

public class SelfUserProfileArtistDto : SelfUserProfileDto
{
    public required DateOnly BirthDate { get; set; }
    public required Guid GenderId { get; set; }
}
