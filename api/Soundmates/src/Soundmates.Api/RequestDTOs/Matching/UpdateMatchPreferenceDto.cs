using System.ComponentModel.DataAnnotations;

namespace Soundmates.Api.RequestDTOs.Matching;

public class UpdateMatchPreferenceDto
{
    [Required]
    public required bool ShowArtists { get; set; }

    [Required]
    public required bool ShowBands { get; set; }

    [Required]
    public required int? MaxDistance { get; set; }

    [Required]
    public required Guid? CountryId { get; set; }

    [Required]
    public required Guid? CityId { get; set; }

    [Required]
    public required int? ArtistMinAge { get; set; }

    [Required]
    public required int? ArtistMaxAge { get; set; }

    [Required]
    public required Guid? ArtistGenderId { get; set; }

    [Required]
    public required int? BandMinMembersCount { get; set; }

    [Required]
    public required int? BandMaxMembersCount { get; set; }

    [Required]
    public required IList<Guid> FilterTags { get; set; }
}
