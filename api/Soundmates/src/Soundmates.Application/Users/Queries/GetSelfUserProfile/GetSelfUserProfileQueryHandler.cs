using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ResponseDTOs.Dictionaries;
using Soundmates.Application.ResponseDTOs.Mappings;
using Soundmates.Application.ResponseDTOs.Users;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;
using static Soundmates.Application.Common.UserMediaHelpers;

namespace Soundmates.Application.Users.Queries.GetSelfUserProfile;

public class GetSelfUserProfileQueryHandler(
    IArtistRepository _artistRepository,
    IBandRepository _bandRepository,
    IAuthService _authService
) : IRequestHandler<GetSelfUserProfileQuery, Result<SelfUserProfileDto>>
{
    public async Task<Result<SelfUserProfileDto>> Handle(GetSelfUserProfileQuery request, CancellationToken cancellationToken)
    {
        var authorizedUser = await _authService.GetAuthorizedUserAsync(subClaim: request.SubClaim, checkForFirstLogin: false);

        if (authorizedUser is null)
        {
            return Result<SelfUserProfileDto>.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "Invalid access token.");
        }

        SelfUserProfileDto userProfile;

        if (authorizedUser.IsBand is null)
        {
            userProfile = new SelfUserProfileDto
            {
                Id = authorizedUser.Id,
                IsBand = authorizedUser.IsBand,
                Email = authorizedUser.Email,
                Name = authorizedUser.Name,
                Description = authorizedUser.Description,
                CountryId = authorizedUser.CountryId,
                CityId = authorizedUser.CityId,
                IsFirstLogin = authorizedUser.IsFirstLogin,
                Tags = authorizedUser.Tags.Select(t => new TagDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    TagCategoryId = t.TagCategoryId
                }).ToList(),
                MusicSamples = authorizedUser.MusicSamples.OrderBy(ms => ms.DisplayOrder).Select(ms => new MusicSampleDto
                {
                    Id = ms.Id,
                    FileUrl = GetMusicSampleUrl(ms.FileName)
                }).ToList(),
                ProfilePictures = authorizedUser.ProfilePictures.OrderBy(pp => pp.DisplayOrder).Select(pp => new ProfilePictureDto
                {
                    Id = pp.Id,
                    FileUrl = GetProfilePictureUrl(pp.FileName)
                }).ToList(),
            };
        } 
        else if ((bool)authorizedUser.IsBand)
        {
            var band = await _bandRepository.GetByUserIdAsync(authorizedUser.Id);
            if (band is null)
            {
                return Result<SelfUserProfileDto>.Failure(
                    errorType: ErrorType.NotFound,
                    errorMessage: $"Band with userId = {authorizedUser.Id} not found.");
            }

            userProfile = band.ToSelfUserProfileDto();
        } 
        else
        {
            var artist = await _artistRepository.GetByUserIdAsync(authorizedUser.Id);
            if (artist is null)
            {
                return Result<SelfUserProfileDto>.Failure(
                    errorType: ErrorType.NotFound,
                    errorMessage: $"Artist with userId = {authorizedUser.Id} not found.");
            }

            userProfile = artist.ToSelfUserProfileDto();
        }
        
        return Result<SelfUserProfileDto>.Success(userProfile);
    }
}
