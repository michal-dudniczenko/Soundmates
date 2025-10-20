using Soundmates.Application.ResponseDTOs.Dictionaries;
using System.Text.Json.Serialization;

namespace Soundmates.Application.ResponseDTOs.Users;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "userType")]
[JsonDerivedType(typeof(SelfUserProfileArtistDto), "artist")]
[JsonDerivedType(typeof(SelfUserProfileBandDto), "band")]
public class SelfUserProfileDto
{
    public required Guid Id { get; set; }
    public required bool? IsBand { get; set; }
    public required string Email { get; set; }
    public required string? Name { get; set; }
    public required string? Description { get; set; }
    public required Guid? CountryId { get; set; }
    public required Guid? CityId { get; set; }
    public required bool IsFirstLogin { get; set; }
    public required IList<TagDto> Tags { get; set; }
    public required IList<MusicSampleDto> MusicSamples { get; set; }
    public required IList<ProfilePictureDto> ProfilePictures { get; set; }
}
