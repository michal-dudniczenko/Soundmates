using Soundmates.Application.ResponseDTOs.Dictionaries;
using Soundmates.Application.ResponseDTOs.Users;
using Soundmates.Domain.Entities;
using static Soundmates.Application.Common.UserMediaHelpers;

namespace Soundmates.Application.ResponseDTOs.Mappings;

public static class ArtistMappings
{
    public static OtherUserProfileArtistDto ToOtherUserProfileDto(this Artist artist)
    {
        ArgumentNullException.ThrowIfNull(artist);

        return new OtherUserProfileArtistDto
        {
            Id = artist.User.Id,
            Name = artist.User.Name!,
            Description = artist.User.Description!,
            CountryId = artist.User.CountryId,
            CityId = artist.User.CityId,
            Tags = artist.User.Tags.Select(t => new TagDto
            {
                Id = t.Id,
                Name = t.Name,
                TagCategoryId = t.TagCategoryId
            }).ToList(),
            MusicSamples = artist.User.MusicSamples.OrderBy(ms => ms.DisplayOrder).Select(ms => new MusicSampleDto
            {
                Id = ms.Id,
                FileUrl = GetMusicSampleUrl(ms.FileName)
            }).ToList(),
            ProfilePictures = artist.User.ProfilePictures.OrderBy(pp => pp.DisplayOrder).Select(pp => new ProfilePictureDto
            {
                Id = pp.Id,
                FileUrl = GetProfilePictureUrl(pp.FileName)
            }).ToList(),
            BirthDate = artist.BirthDate
        };
    }

    public static SelfUserProfileArtistDto ToSelfUserProfileDto(this Artist artist)
    {
        ArgumentNullException.ThrowIfNull(artist);

        return new SelfUserProfileArtistDto
        {
            Id = artist.User.Id,
            Email = artist.User.Email,
            Name = artist.User.Name!,
            Description = artist.User.Description!,
            CountryId = artist.User.CountryId,
            CityId = artist.User.CityId,
            Tags = artist.User.Tags.Select(t => new TagDto
            {
                Id = t.Id,
                Name = t.Name,
                TagCategoryId = t.TagCategoryId
            }).ToList(),
            MusicSamples = artist.User.MusicSamples.OrderBy(ms => ms.DisplayOrder).Select(ms => new MusicSampleDto
            {
                Id = ms.Id,
                FileUrl = GetMusicSampleUrl(ms.FileName)
            }).ToList(),
            ProfilePictures = artist.User.ProfilePictures.OrderBy(pp => pp.DisplayOrder).Select(pp => new ProfilePictureDto
            {
                Id = pp.Id,
                FileUrl = GetProfilePictureUrl(pp.FileName)
            }).ToList(),
            BirthDate = artist.BirthDate
        };
    }
}
