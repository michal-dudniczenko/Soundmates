namespace Soundmates.Api.DTOs.Users;

public class OtherUserProfileDto
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required int BirthYear { get; set; }
    public required string City { get; set; }
    public required string Country { get; set; }
}
