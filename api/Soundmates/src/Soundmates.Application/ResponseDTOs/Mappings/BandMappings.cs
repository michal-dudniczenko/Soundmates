using Soundmates.Application.ResponseDTOs.Dictionaries;
using Soundmates.Application.ResponseDTOs.Users;
using Soundmates.Domain.Entities;
using static Soundmates.Application.Common.UserMediaHelpers;

namespace Soundmates.Application.ResponseDTOs.Mappings;

public static class BandMappings
{
    public static OtherUserProfileBandDto ToOtherUserProfileDto(this Band band)
    {
        ArgumentNullException.ThrowIfNull(band);

        return new OtherUserProfileBandDto
        {
            Id = band.User.Id,
            IsBand = band.User.IsBand,
            Name = band.User.Name!,
            Description = band.User.Description!,
            CountryId = band.User.CountryId,
            CityId = band.User.CityId,
            Tags = band.User.Tags.Select(t => new TagDto
            {
                Id = t.Id,
                Name = t.Name,
                TagCategoryId = t.TagCategoryId
            }).ToList(),
            MusicSamples = band.User.MusicSamples.OrderBy(ms => ms.DisplayOrder).Select(ms => new MusicSampleDto
            {
                Id = ms.Id,
                FileUrl = GetMusicSampleUrl(ms.FileName)
            }).ToList(),
            ProfilePictures = band.User.ProfilePictures.OrderBy(pp => pp.DisplayOrder).Select(pp => new ProfilePictureDto
            {
                Id = pp.Id,
                FileUrl = GetProfilePictureUrl(pp.FileName)
            }).ToList(),
            BandMembers = band.Members.OrderBy(m => m.DisplayOrder).Select(bm => new BandMemberDto
            {
                Id = bm.Id,
                Name = bm.Name,
                Age = bm.Age,
                BandRoleId = bm.BandRoleId
            }).ToList()
        };
    }

    public static SelfUserProfileBandDto ToSelfUserProfileDto(this Band band)
    {
        ArgumentNullException.ThrowIfNull(band);

        return new SelfUserProfileBandDto
        {
            Id = band.User.Id,
            IsBand = band.User.IsBand,
            Email = band.User.Email,
            Name = band.User.Name!,
            Description = band.User.Description!,
            CountryId = band.User.CountryId,
            CityId = band.User.CityId,
            IsFirstLogin = band.User.IsFirstLogin,
            Tags = band.User.Tags.Select(t => new TagDto
            {
                Id = t.Id,
                Name = t.Name,
                TagCategoryId = t.TagCategoryId
            }).ToList(),
            MusicSamples = band.User.MusicSamples.OrderBy(ms => ms.DisplayOrder).Select(ms => new MusicSampleDto
            {
                Id = ms.Id,
                FileUrl = GetMusicSampleUrl(ms.FileName)
            }).ToList(),
            ProfilePictures = band.User.ProfilePictures.OrderBy(pp => pp.DisplayOrder).Select(pp => new ProfilePictureDto
            {
                Id = pp.Id,
                FileUrl = GetProfilePictureUrl(pp.FileName)
            }).ToList(),
            BandMembers = band.Members.OrderBy(m => m.DisplayOrder).Select(bm => new BandMemberDto
            {
                Id = bm.Id,
                Name = bm.Name,
                Age = bm.Age,
                BandRoleId = bm.BandRoleId
            }).ToList()
        };
    }
}
