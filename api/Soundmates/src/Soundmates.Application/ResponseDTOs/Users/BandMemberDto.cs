namespace Soundmates.Application.ResponseDTOs.Users;

public class BandMemberDto
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required int Age { get; set; }
    public required Guid BandRoleId { get; set; }
}
