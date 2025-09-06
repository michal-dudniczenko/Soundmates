namespace Soundmates.Api.DTOs.ProfilePictures;

public class SelfProfilePictureDto
{
    public required Guid Id { get; set; }
    public required string FileUrl { get; set; }
    public required int DisplayOrder { get; set; }
}
