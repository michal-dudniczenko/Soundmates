using MediatR;
using Soundmates.Application.Common;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;

namespace Soundmates.Application.Matching.Commands.UpdateMatchPreference;

public class UpdateMatchPreferenceCommandHandler(
    IMatchPreferenceRepository _matchPreferenceRepository,
    IAuthService _authService
) : IRequestHandler<UpdateMatchPreferenceCommand, Result>
{
    public async Task<Result> Handle(UpdateMatchPreferenceCommand request, CancellationToken cancellationToken)
    {
        var authorizedUser = await _authService.GetAuthorizedUserAsync(subClaim: request.SubClaim, checkForFirstLogin: true);

        if (authorizedUser is null)
        {
            return Result.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "Invalid access token.");
        }

        var userMatchPreference = new UserMatchPreference
        {
            ShowArtists = request.ShowArtists,
            ShowBands = request.ShowBands,
            MaxDistance = request.MaxDistance,
            CountryId = request.CountryId,
            CityId = request.CityId,
            ArtistMinAge = request.ArtistMinAge,
            ArtistMaxAge = request.ArtistMaxAge,
            ArtistGenderId = request.ArtistGenderId,
            BandMinMembersCount = request.BandMinMembersCount,
            BandMaxMembersCount = request.BandMaxMembersCount,
            UserId = authorizedUser.Id
        };

        await _matchPreferenceRepository.AddUpdateAsync(userMatchPreference, request.FilterTagsIds);

        return Result.Success();
    }
}
