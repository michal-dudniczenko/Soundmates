using System.ComponentModel.DataAnnotations;

namespace Soundmates.Api.RequestDTOs.Matching;

public class UpdateMatchPreferenceDto
{
    [Required]
    public required bool ShowArtists { get; set; }

    [Required]
    public required bool ShowBands { get; set; }

    public required int? MaxDistance { get; set; }

    public required Guid? CountryId { get; set; }

    public required Guid? CityId { get; set; }

    public required int? ArtistMinAge { get; set; }

    public required int? ArtistMaxAge { get; set; }

    public required Guid? ArtistGenderId { get; set; }

    public required int? BandMinMembersCount { get; set; }

    public required int? BandMaxMembersCount { get; set; }

    [Required]
    public required IList<Guid> FilterTagsIds { get; set; }
}
