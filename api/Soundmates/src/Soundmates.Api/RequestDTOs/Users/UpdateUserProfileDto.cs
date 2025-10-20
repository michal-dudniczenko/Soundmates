using Soundmates.Application.ResponseDTOs.Dictionaries;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Soundmates.Api.RequestDTOs.Users;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "userType")]
[JsonDerivedType(typeof(UpdateUserProfileArtistDto), "artist")]
[JsonDerivedType(typeof(UpdateUserProfileBandDto), "band")]
public abstract class UpdateUserProfileDto
{
    [Required]
    public required bool IsBand { get; set; }

    [Required]
    [MaxLength(50)]
    public required string Name { get; set; }

    [MaxLength(500)]
    public required string Description { get; set; }

    [Required]
    public required Guid CountryId { get; set; }

    [Required]
    public required Guid CityId { get; set; }

    [Required]
    public required IList<TagDto> Tags { get; set; }

    [Required]
    public required IList<Guid> MusicSamplesOrder { get; set; }

    [Required]
    public required IList<Guid> ProfilePicturesOrder { get; set; }
}
