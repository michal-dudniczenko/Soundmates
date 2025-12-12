namespace Soundmates.Application.ResponseDTOs.Matching;

public class MatchPreferenceDto
{
    public required bool ShowArtists { get; set; }
    public required bool ShowBands { get; set; }
    public required int? MaxDistance { get; set; }
    public required Guid? CountryId { get; set; }
    public required Guid? CityId { get; set; }
    public required int? ArtistMinAge { get; set; }
    public required int? ArtistMaxAge { get; set; }
    public required Guid? ArtistGenderId { get; set; }
    public required int? BandMinMembersCount { get; set; }
    public required int? BandMaxMembersCount { get; set; }
    public required IList<Guid> FilterTagsIds { get; set; }
}
