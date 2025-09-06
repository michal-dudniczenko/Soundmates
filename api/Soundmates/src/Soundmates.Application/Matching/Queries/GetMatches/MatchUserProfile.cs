namespace Soundmates.Application.Matching.Queries.GetMatches;

public class MatchUserProfile
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required int BirthYear { get; init; }
    public required string City { get; init; }
    public required string Country { get; init; }
}
