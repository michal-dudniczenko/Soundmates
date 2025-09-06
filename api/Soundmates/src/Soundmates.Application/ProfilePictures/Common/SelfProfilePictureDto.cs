namespace Soundmates.Application.ProfilePictures.Common;

public class SelfProfilePictureDto
{
    public required Guid Id { get; init; }
    public required string FileUrl { get; init; }
    public required int DisplayOrder { get; init; }
}
