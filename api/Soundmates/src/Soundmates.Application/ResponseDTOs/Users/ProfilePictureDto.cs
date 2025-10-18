namespace Soundmates.Application.ResponseDTOs.Users;

public class ProfilePictureDto
{
    public required Guid Id { get; set; }
    public required string FileUrl { get; set; }
    public required int DisplayOrder { get; set; }
}
