using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ResponseDTOs.Dictionaries;
using Soundmates.Application.ResponseDTOs.Matching;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;

namespace Soundmates.Application.Matching.Queries.GetMatchPreference;

public class GetMatchPreferenceQueryHandler(
    IMatchPreferenceRepository _matchPreferenceRepository,
    IAuthService _authService
) : IRequestHandler<GetMatchPreferenceQuery, Result<MatchPreferenceDto>>
{
    public async Task<Result<MatchPreferenceDto>> Handle(GetMatchPreferenceQuery request, CancellationToken cancellationToken)
    {
        var authorizedUser = await _authService.GetAuthorizedUserAsync(subClaim: request.SubClaim, checkForFirstLogin: true);

        if (authorizedUser is null)
        {
            return Result<MatchPreferenceDto>.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "Invalid access token.");
        }

        var matchPreference = await _matchPreferenceRepository.GetUserMatchPreferenceAsync(authorizedUser.Id);

        if (matchPreference is null)
        {
            return Result<MatchPreferenceDto>.Failure(
                errorType: ErrorType.InternalServerError,
                errorMessage: $"Could not get match preference for user with id: {authorizedUser.Id}");
        }

        var matchPreferenceDto = new MatchPreferenceDto
        {
            ShowArtists = matchPreference.ShowArtists,
            ShowBands = matchPreference.ShowBands,
            MaxDistance = matchPreference.MaxDistance,
            CountryId = matchPreference.CountryId,
            CityId = matchPreference.CityId,
            ArtistMinAge = matchPreference.ArtistMinAge,
            ArtistMaxAge = matchPreference.ArtistMaxAge,
            ArtistGenderId = matchPreference.ArtistGenderId,
            BandMinMembersCount = matchPreference.BandMinMembersCount,
            BandMaxMembersCount = matchPreference.BandMaxMembersCount,
            FilterTags = matchPreference.Tags.Select(t => new TagDto
            {
                Id = t.Id,
                Name = t.Name,
                TagCategoryId = t.TagCategoryId
            }).ToList()
        };

        return Result<MatchPreferenceDto>.Success(matchPreferenceDto);
    }
}
